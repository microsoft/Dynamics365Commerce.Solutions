[CmdletBinding(PositionalBinding = $false)]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Archive,

    [Parameter(Mandatory = $false)]
    [string[]]$LegacyHashAlgorithms,

    [Parameter(Mandatory = $false)]
    [switch]$SaveTransformedXml
)

Add-Type -AssemblyName System.IO
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.Security

#region Main

<#
.DESCRIPTION
    The main function.
#>
function main {
    param()

    try {
        Initialize-Labels

        if (-not $(Test-Path $Archive)) {
            Write-Host ($Labels.FileNotFound -f $Archive)
            return
        }

        $private:archiveItem = Get-Item $Archive
        $script:CertificatesLocation = $private:archiveItem.DirectoryName
        $private:reportBytes, $private:signatureBytes = Get-ReportAndSignatureBytes $private:archiveItem
        $private:reportStream = New-Object System.IO.MemoryStream $private:reportBytes, $false
        $private:xml = New-Object System.Xml.XmlDocument
        $private:xml.Load($private:reportStream)
        $private:archivePeriod = $private:xml.ArchivePeriod

        Validate-ArchiveFile `
            $private:reportBytes `
            $private:signatureBytes `
            $private:archivePeriod.PeriodGrandTotal.HashAlgorithm `
            $private:archivePeriod.PeriodGrandTotal.CertificateThumbprint

        $private:xml = Apply-Transformations $private:reportStream

        $private:nodes = $private:xml.SelectNodes("//*[
            self::PeriodGrandTotal or
            self::Shift or
            self::Receipt or
            self::ReceiptCopy or
            self::AuditEvent]"
        )

        $private:nodes | Validate-DataIntegrity -totalCount $private:nodes.Count
        Write-Host
    }
    catch {
        Write-Host $_ -ForegroundColor "Red"
        exit -1
    }
}

<#
.DESCRIPTION
    Gets byte arrays for the report file and the signature.
#>
function Get-ReportAndSignatureBytes {
    param (
        $archiveItem
    )
    try {
        $mode = [System.IO.FileMode]::Open
        $access = [System.IO.FileAccess]::Read
        $sharing = [IO.FileShare]::Read
        [System.IO.MemoryStream]$reportMemoryStream = New-Object System.IO.MemoryStream
        [Byte[]]$reportBytes = [System.Byte[]]::CreateInstance([System.Byte], 0)
        [Byte[]]$signatureBytes = [System.Byte[]]::CreateInstance([System.Byte], 0)

        if ($archiveItem.Extension -eq ".xml") {
            $reportFileStream = New-Object System.IO.FileStream $archiveItem.FullName, $mode, $access, $sharing
            $reportFileStream.CopyTo($reportMemoryStream)
            $reportBytes = $reportMemoryStream.ToArray()

            $signatureFilePath = Join-Path -Path $archiveItem.Directory "$($archiveItem.BaseName).sign"
            if (Test-Path $signatureFilePath) {
                $signatureFileStream = New-Object System.IO.FileStream $signatureFilePath, $mode, $access, $sharing
                $signatureStreamReader = New-Object System.IO.StreamReader $signatureFileStream
                $signatureText = $signatureStreamReader.ReadToEnd()
                $signatureBytes = [System.Text.Encoding]::ASCII.GetBytes($signatureText)
            }
            else {
                $reason = $Labels.SignatureFileNotFound -f $signatureFilePath
                Write-Warning ($Labels.UnableValidateArchive -f $reason)
            }
        }
        else {
            throw ($Labels.UnsupportedFileType -f $archiveItem)
        }

        @($reportBytes, $signatureBytes)
    }
    finally {
        if ($reportFileStream) { $reportFileStream.Close() }
        if ($signatureFileStream) { $signatureFileStream.Close() }
    }
}

<#
.DESCRIPTION
    Applies the required XSL transformations and returns the transformed XML document.
#>
function Apply-Transformations {
    param (
        [System.IO.Stream]$originalReportStream
    )

    try {
        Write-Progress -Activity $Labels.Initialization
        if ($originalReportStream.CanSeek) {
            $originalReportStream.Position = 0;
        }

        $originalReportStreamXmlReader = [System.Xml.XmlReader]::Create($originalReportStream)
        $transformedReportStream = New-Object System.IO.MemoryStream
        $transformedReportStreamXmlWriter = [System.Xml.XmlWriter]::Create($transformedReportStream)
        $xslInputElement = New-Object System.Xml.Xsl.XslCompiledTransform
        $xslInputElement.Load((Join-Path $PSScriptRoot "sort.xsl"))
        $xslInputElement.Transform($originalReportStreamXmlReader, $transformedReportStreamXmlWriter)
        $transformedReportStream.Position = 0
        $transformedXml = New-Object System.Xml.XmlDocument
        $transformedXml.Load($transformedReportStream)
        if ($SaveTransformedXml) {
            $transformedXmlFileName = [System.IO.Path]::Combine(
                [System.IO.Path]::GetDirectoryName($Archive),
                [System.IO.Path]::GetFileNameWithoutExtension($Archive) + "_transformed.xml")
            $transformedXml.Save($transformedXmlFileName)
            Write-Verbose $transformedXmlFileName
        }
        $transformedXml
    }
    finally {
        Write-Progress -Activity $Labels.Initialization -Completed
        if ($transformedReportStream) { $transformedReportStream.Close() };
    }
}

<#
.DESCRIPTION
    Validates the archive file.
#>
function Validate-ArchiveFile {
    param (
        [System.Byte[]]$reportBytes,
        [System.Byte[]]$signatureBytes,
        [string]$hashAlgorithm,
        [string]$thumbprint
    )
    Write-Verbose $Labels.ValidatingArchive
    try {
        if (-not $reportBytes.Length) {
            Write-Verbose $Labels.ReportEmpty
            return
        }
        if (-not $signatureBytes.Length) {
            Write-Verbose $Labels.SignatureFileEmpty
            return
        }
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$certificate = Find-X509Certificate $thumbprint
        if ($null -eq $certificate) {
            throw ($Labels.CertificateNotFound -f $thumbprint)
        }
        $signature = [System.Text.Encoding]::UTF8.GetString($signatureBytes)
        $valid = Verify-Data $reportBytes $signature $hashAlgorithm $thumbprint
        if ($valid) {
            Write-Host $Labels.ArchiveValidationPassed -ForegroundColor "Green"
        }
        else {
            Write-Host $Labels.ArchiveValidationFailed -ForegroundColor "Red"
        }
    }
    catch {
        Write-Warning ($Labels.UnableValidateArchive -f $_)
    }
}

<#
.DESCRIPTION
    Validates the data integrity of a node.
#>
function Validate-DataIntegrity {
    [CmdletBinding()]
    param (
        [Parameter(ValueFromPipeline)]
        [System.Xml.XmlElement]$node,

        [Parameter()]
        [int]$totalCount
    )

    begin {
        $i = 0
        $percentComplete = 0
        $validCount = 0
        $activity = $Labels.ValidationActivity
    }

    process {
        $valid = $false
        try {
            $nodeName = $node.Name
            $idProperty = $node.Attributes.Item(0).Name
            $idValue = $node.Attributes.Item(0).Value
            $registerNumber = $node.RegisterNumber
            $nodeHeader = "$nodeName $idProperty=$idValue"
            if ($registerNumber) {
                $nodeHeader += ", RegisterNumber=$registerNumber"
            }

            Write-Verbose ($Labels.ValidatingNode -f $nodeHeader)
            $recalculatedDataToSign = Calculate-DataToSign $node
            $recalculatedDataToSignBytes = [System.Text.Encoding]::UTF8.GetBytes($recalculatedDataToSign)
            if ($recalculatedDataToSign -eq $node.DataToSign) {
                $hashAlgorithms = @($node.HashAlgorithm)
                $legacy = -not $node.DataToSignFormatVersion
                if ($legacy) {
                    $valid = $false
                    if (-not $hashAlgorithms -and $LegacyHashAlgorithms) {
                        $hashAlgorithms = $LegacyHashAlgorithms
                    }
                }

                foreach ($hashAlgorithm in $hashAlgorithms) {
                    $signature = ConvertFrom-LegacyBase64UrlString ($node.Signature)
                    $valid = Verify-Data `
                        $recalculatedDataToSignBytes `
                        $signature `
                        $hashAlgorithm `
                        $node.CertificateThumbprint
                    if ($valid) {
                        break
                    }
                }
            }
            if ($valid) {
                Write-Verbose ($Labels.NodeValidationPassed -f $nodeHeader)
            }
            else {
                if ($recalculatedDataToSign -eq $node.DataToSign) {
                    $reason = $Labels.SignatureInvalid
                }
                else {
                    $reason = $Labels.DataIntegrityViolated
                }
                Write-Host  ($Labels.NodeValidationFailed -f $nodeHeader, $reason) -ForegroundColor "Red"
                if ($recalculatedDataToSign -ne $node.DataToSign) {
                    $originalMetadata = ConvertTo-Metadata $node $node.DataToSign
                    $recalculatedMetadata = ConvertTo-Metadata $node $recalculatedDataToSign
                    $diffHashTables = Compare-HashTable -Expected $originalMetadata -Actual $recalculatedMetadata
                    if ($diffHashTables) {
                        Write-Host $Labels.SeeExpectedAndActual
                        $diffHashTables | `
                            ForEach-Object { [PSCustomObject]$_ } | `
                            Format-Table -Property $Labels.AttributeHeader, $Labels.ExpectedValueHeader, $Labels.ActualValueHeader | `
                            Out-String
                    }
                }
            }
        }
        catch {
            Write-Warning ($Labels.UnableToValidateNode -f $nodeHeader, $_)
        }
        finally {
            if ($valid) {
                $validCount++
            }
            $i++
            $percentComplete = [int]($i / $totalCount * 100)
            Write-Progress -Activity $activity -Status ($Labels.PercentComplete -f $percentComplete) -PercentComplete $percentComplete
        }
    }

    end {
        Write-Progress -Activity $activity -Completed
        $invalidCount = $totalCount - $validCount
        $percentIncorrect = if ($totalCount) { [int]($invalidCount / $totalCount * 100) } else { 0 }
        Write-Host ($Labels.ValidationCompleted -f $invalidCount, $totalCount, $percentIncorrect)
    }
}

#endregion

<#
.DESCRIPTION
    Searches for an X.509 certificate in the specified path with the given thumbprint.
#>
function Find-X509Certificate {
    param (
        [string]$thumbprint
    )

    if (-not $script:CertificatesCache) {
        [hashtable]$script:CertificatesCache = @{}
    }

    if ($script:CertificatesCache.ContainsKey($thumbprint)) {
        $script:CertificatesCache[$thumbprint]
        return
    }

    Write-Verbose ($Labels.SearchingCertificate -f $thumbprint, $script:CertificatesLocation)
    $certificateItems = Get-ChildItem -Path "$script:CertificatesLocation\*" -Include *.cer, *.crt, *.pem, *.pfx
    foreach ($certificateItem in $certificateItems) {
        $certificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 $certificateItem.FullName
        if ($certificate.Thumbprint -eq $thumbprint) {
            Write-Verbose ($Labels.CertificateFound -f $certificate.GetName())
            $script:CertificatesCache[$thumbprint] = $certificate
            $certificate
            break
        }
    }
}

<#
.DESCRIPTION
    Validates the data against the specified signature.
#>
function Verify-Data {
    param (
        [byte[]]$bytes,
        [string]$signature,
        [string]$hashAlgorithm,
        [string]$thumbprint
    )

    if (-not $hashAlgorithm -or -not $thumbprint) {
        throw $Labels.HashOrThumbprintMissing
    }

    $certificate = Find-X509Certificate $thumbprint
    if ($null -eq $certificate) {
        throw ($Labels.CertificateNotFound -f $thumbprint)
    }

    $signatureBytes = ConvertFrom-Base64UrlString $signature
    [System.Security.Cryptography.RSA]$rsa = [System.Security.Cryptography.X509Certificates.RSACertificateExtensions]::GetRSAPublicKey($certificate)
    $rsaSignaturePadding = [System.Security.Cryptography.RSASignaturePadding]::Pss
    $rsa.VerifyData($bytes, $signatureBytes, $hashAlgorithm.ToUpper(), $rsaSignaturePadding)
}

<#
.DESCRIPTION
    Converts the specified base64url string into a byte array.
#>
function ConvertFrom-Base64UrlString {
    param (
        [string]$base64url
    )
    $base64 = $base64url
    if ($base64url.Length % 4 -ne 0) {
        $numberOfPadingCharachers = 4 - ($base64url.Length % 4)
        $lengthWithPadding = $base64url.Length + $numberOfPadingCharachers
        $base64 = $base64url.PadRight($lengthWithPadding, '=')
    }
    $base64 = $base64.Replace('_', '/').Replace('-', '+')
    $bytes = [System.Convert]::FromBase64String($base64)
    $bytes
}

<#
.DESCRIPTION
    Converts a legacy base64Url string to a base64 string.
#>
function ConvertFrom-LegacyBase64UrlString {
    param (
        [string]$legacyBase64UrlString
    )

    if (-not $legacyBase64UrlString) {
        return $legacyBase64UrlString
    }

    if ($legacyBase64UrlString.Length -lt 2) {
        return $legacyBase64UrlString
    }

    [int] $numPadChars = [int][char]$legacyBase64UrlString[$legacyBase64UrlString.Length - 1] - [int][char]'0';

    if ($numPadChars -lt 0 -or $numPadChars -gt 10) {
        return $legacyBase64UrlString
    }

    $suffix = New-Object string('=', $numPadChars)

    $signature = $legacyBase64UrlString.Remove($legacyBase64UrlString.Length - 1);

    $signature = $signature + $suffix

    $signature
}

<#
.DESCRIPTION
    Compares two hash tables and produces an object containing properties that are different.
#>
function Compare-HashTable {
    param (
        [alias('Expected')][hashtable]$expectedTable,
        [alias('Actual')][hashtable]$actualTable
    )

    <# Ensures a string is no longer than the allowed max length. #>
    function FitString ([string]$string) {
        $maxLength = 30
        if ($string.Length -gt $maxLength) {
            $string.Substring(0, $maxLength / 2) + "..." + $string.Substring($string.Length - $maxLength / 2)
        }
        else {
            $string
        }
    }

    foreach ($key in $expectedTable.Keys) {
        $expected = FitString $expectedTable[$key]
        $actual = FitString $actualTable[$key]
        if ($expected -ne $actual) {
            @{ $Labels.AttributeHeader = $key; $Labels.ExpectedValueHeader = $expected; $Labels.ActualValueHeader = $actual }
        }
    }
}

<#
.DESCRIPTION
    Calculates the data to sign for a node.
#>
function Calculate-DataToSign {
    param (
        [System.Xml.XmlElement]$node
    )


    $invalidFormatMessage = ($Labels.DataToSignFormatVersionInvalid -f $node.DataToSignFormatVersion)

    switch ($node.Name) {
        "PeriodGrandTotal" {
            if (-not $node.DataToSignFormatVersion) {
                throw $Labels.DataToSignFormatVersionMissing
            }
            switch ($node.DataToSignFormatVersion) {
                "2.1_4.0" { Calculate-PeriodGrandTotalDataToSign-2140 $node }
                default { throw $invalidFormatMessage }
            }
        }

        "Shift" {
            switch ($node.DataToSignFormatVersion) {
                "2.1_4.0" { Calculate-ShiftDataToSign-2140 $node }
                "" { Calculate-ShiftDataToSign-Legacy $node }
                default { throw $invalidFormatMessage }
            }
        }

        "Receipt" {
            switch ($node.DataToSignFormatVersion) {
                "2.1_4.0" { Calculate-ReceiptDataToSign-2140 $node }
                "" { Calculate-ReceiptDataToSign-Legacy $node }
                default { throw $invalidFormatMessage }
            }
        }

        "ReceiptCopy" {
            switch ($node.DataToSignFormatVersion) {
                "2.1_4.0" { Calculate-ReceiptCopyDataToSign-2140 $node }
                "" { Calculate-ReceiptCopyDataToSign-Legacy $node }
                default { throw $invalidFormatMessage }
            }
        }

        "AuditEvent" {
            switch ($node.DataToSignFormatVersion) {
                "2.1_4.0" { Calculate-AuditEventDataToSign-2140 $node }
                "" { Calculate-AuditEventDataToSign-Legacy $node }
                default { throw $invalidFormatMessage }
            }
        }

        default {
            throw ($Labels.NonSupportedNode -f $node.Name)
        }
    }
}

<#
.DESCRIPTION
    Converts the specified data to sign to a metadata object
    based on the node type and the data to sign format version.
#>
function ConvertTo-Metadata {
    param (
        [System.Xml.XmlElement]$node,
        [string]$dataToSign
    )

    $invalidFormatMessage = ($Labels.DataToSignFormatVersionInvalid -f $node.DataToSignFormatVersion)

    switch ($node.Name) {
        "PeriodGrandTotal" {
            if (-not $node.DataToSignFormatVersion) {
                throw $Labels.DataToSignFormatVersionMissing
            }

            switch ($node.DataToSignFormatVersion) {
                "2.1_4.0" { ConvertTo-PeriodGrandTotalMetadata-2140 $dataToSign }
                default { throw $invalidFormatMessage }
            }
        }

        "Shift" {
            switch ($node.DataToSignFormatVersion) {
                "2.1_4.0" { ConvertTo-ShiftMetadata-2140 $dataToSign }
                "" { ConvertTo-ShiftMetadata-Legacy $dataToSign }
                default { throw $invalidFormatMessage }
            }
        }

        "Receipt" {
            switch ($node.DataToSignFormatVersion) {
                "2.1_4.0" { ConvertTo-ReceiptMetadata-2140 $dataToSign }
                "" { ConvertTo-ReceiptMetadata-Legacy $dataToSign }
                default { throw $invalidFormatMessage }
            }
        }

        "ReceiptCopy" {
            switch ($node.DataToSignFormatVersion) {
                "2.1_4.0" { ConvertTo-ReceiptCopyMetadata-2140 $dataToSign }
                "" { ConvertTo-ReceiptCopyMetadata-Legacy $dataToSign }
                default { throw $invalidFormatMessage }
            }
        }

        "AuditEvent" {
            switch ($node.DataToSignFormatVersion) {
                "2.1_4.0" { ConvertTo-AuditEventMetadata-2140 $dataToSign }
                "" { ConvertTo-AuditEventMetadata-Legacy $dataToSign }
                default { throw $invalidFormatMessage }
            }
        }

        default {
            throw ($Labels.NonSupportedNode -f $node.Name)
        }
    }
}

#region PeriodGrandTotal

<#
.DESCRIPTION
    Calculates the data to sign for the PeriodGrandTotal node.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function Calculate-PeriodGrandTotalDataToSign-2140 {
    param (
        [System.Xml.XmlElement]$periodGrandTotal
    )
    $metadata = ConvertTo-PeriodGrandTotalMetadata-2140 $periodGrandTotal.DataToSign
    $part1 = ConvertTo-PeriodGrandTotalTotalAmountsByTaxRateString-2140 $periodGrandTotal.SelectSingleNode('PeriodGrandTotalLines')
    $part2 = $periodGrandTotal.GrandTotal
    $part3 = $periodGrandTotal.PerpetualGrandTotalAbsoluteValue
    $part4 = $metadata.SignDateTime
    $part5 = [System.String]::Join("|", $periodGrandTotal.FromDate, $periodGrandTotal.ToDate)
    $part6 = $periodGrandTotal.SequentialNumber
    $part7 = $metadata.IsFirstSigned
    $part8 = $metadata.PreviousSignature
    $result = [System.String]::Join(",", $part1, $part2, $part3, $part4, $part5, $part6, $part7)
    if ($part8) {
        $result += "," + $part8
    }
    $result
}

<#
.DESCRIPTION
    Converts PeriodGrandTotalLine elements into the string representation of the total amounts by VAT rate breakdown.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function ConvertTo-PeriodGrandTotalTotalAmountsByTaxRateString-2140 {
    param (
        [System.Xml.XmlElement]$periodGrandTotalLines
    )
    if ($null -eq $periodGrandTotalLines -or $periodGrandTotalLines.ChildNodes.Count -eq 0) {
        "0:0"
        return
    }
    $items = $periodGrandTotalLines.ChildNodes | ForEach-Object { "$($_.TaxRate):$($_.TotalInclTax)" }
    $result = [System.String]::Join("|", $items)
    $result
}

<#
.DESCRIPTION
    Converts the DataToSign element of the PeriodGrandTotal node into a metadata object.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function ConvertTo-PeriodGrandTotalMetadata-2140 {
    param (
        [string]$dataToSign
    )
    $values = $dataToSign.Split(",")
    @{
        PeriodGrandTotalLines            = $values[0]
        GrandTotal                       = $values[1]
        PerpetualGrandTotalAbsoluteValue = $values[2]
        SignDateTime                     = $values[3]
        Period                           = $values[4]
        SequentialNumber                 = $values[5]
        IsFirstSigned                    = $values[6]
        PreviousSignature                = $values[7]
    }
}

#endregion

#region Shift

<#
.DESCRIPTION
    Calculates the data to sign for the Shift node.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function Calculate-ShiftDataToSign-2140 {
    param (
        [System.Xml.XmlElement]$shift
    )
    $metadata = ConvertTo-ShiftMetadata-2140 $shift.DataToSign
    $previousShift = $shift.PreviousSibling
    $perviousSignature = if ($previousShift) {
        $previousShift.Signature
    }
    else {
        $metadata.PreviousSignature
    }

    $part1 = ConvertTo-ShiftTotalAmountsByTaxRateString-2140 $shift.SelectSingleNode('ShiftLines')
    $part2 = $shift.GrandTotal
    $part3 = $shift.PerpetualGrandTotalAbsoluteValue
    $part4 = $shift.Date
    $part5 = $shift.Date.Substring(0, 8)
    $part6 = $shift.SequentialNumber
    $part7 = if ($shift.SequentialNumber -eq "1") { "Y" } else { "N" }
    $part8 = if ($part7 -eq "N") { $perviousSignature } else { "" }
    $result = [System.String]::Join(",", $part1, $part2, $part3, $part4, $part5, $part6, $part7, $part8)
    $result
}

<#
.DESCRIPTION
    Converts ShiftLine elements into the string representation of the total amounts by VAT rate breakdown.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function ConvertTo-ShiftTotalAmountsByTaxRateString-2140 {
    param (
        [System.Xml.XmlElement]$shiftLines
    )
    if ($null -eq $shiftLines -or $shiftLines.ChildNodes.Count -eq 0) {
        "0:0"
        return
    }
    $items = $shiftLines.ChildNodes | ForEach-Object { "$($_.TaxRate):$($_.TotalInclTax)" }
    $result = [System.String]::Join("|", $items)
    $result
}

<#
.DESCRIPTION
    Converts the DataToSign element of the Shift node into a metadata object.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function ConvertTo-ShiftMetadata-2140 {
    param (
        [string]$dataToSign
    )
    $values = $dataToSign.Split(",")
    @{
        ShiftLines                       = $values[0]
        GrandTotal                       = $values[1]
        PerpetualGrandTotalAbsoluteValue = $values[2]
        RegistrationTime                 = $values[3]
        Period                           = $values[4]
        SequentialNumber                 = $values[5]
        IsFirstSigned                    = $values[6]
        PreviousSignature                = $values[7]
    }
}

<#
.DESCRIPTION
    Calculates the data to sign for the Shift node.

.NOTES
    The old version of receipt copy.
#>
function Calculate-ShiftDataToSign-Legacy {
    param (
        [System.Xml.XmlElement]$shift
    )
    $metadata = ConvertTo-ShiftMetadata-Legacy $shift.DataToSign
    $previousShift = $shift.PreviousSibling
    $perviousSignature = if ($previousShift) {
        $previousShift.Signature
    }
    else {
        $metadata.PreviousSignature
    }

    $perviousSignature = if ($metadata.IsFirstSigned -eq "N") { $perviousSignature } else { "" }
    $result = [System.String]::Join(",", $metadata.ShiftData, $metadata.IsFirstSigned, $perviousSignature)
    $result
}

<#
.DESCRIPTION
    Converts the DataToSign element of the Shift node into a metadata object.

.NOTES
    The old version of receipt copy.
#>
function ConvertTo-ShiftMetadata-Legacy {
    param (
        [string]$dataToSign
    )

    $values = $dataToSign.Split(",")
    $shiftData = [string]::Join(",", ($values |  Select-Object -First ($values.Count - 2)))
    @{
        ShiftData         = $shiftData
        IsFirstSigned     = $values[$values.Count - 2]
        PreviousSignature = $values[$values.Count - 1]
    }
}

#endregion

#region Receipt

<#
.DESCRIPTION
    Calculates the data to sign for the Receipt node.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function Calculate-ReceiptDataToSign-2140 {
    param (
        [System.Xml.XmlElement]$receipt
    )
    $metadata = ConvertTo-ReceiptMetadata-2140 $receipt.DataToSign
    $previousReceipt = $receipt.PreviousSibling
    $perviousSignature = if ($previousReceipt) {
        $previousReceipt.Signature
    }
    else {
        $metadata.PreviousSignature
    }

    $part1 = ConvertTo-ReceiptTotalAmountsByTaxRateString-2140 $receipt.SelectSingleNode('FooterLines')
    $part2 = $receipt.Total.InclTax
    $part3 = $receipt.Date
    $part4 = $receipt.RegisterNumber
    $part5 = $receipt.SequentialNumber
    $part6 = $metadata.TransactionType
    $part7 = if ($receipt.SequentialNumber -eq "1") { "Y" } else { "N" }
    $part8 = if ($part7 -eq "N") { $perviousSignature } else { "" }
    $result = [System.String]::Join(",", $part1, $part2, $part3, $part4, $part5, $part6, $part7, $part8)
    $result
}

<#
.DESCRIPTION
    Converts ReceiptLine elements into the string representation of the total amounts by tax rate breakdown.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function ConvertTo-ReceiptTotalAmountsByTaxRateString-2140 {
    param (
        [System.Xml.XmlElement]$footerLines
    )
    if ($null -eq $footerLines -or $footerLines.ChildNodes.Count -eq 0) {
        "0:0"
        return
    }
    $items = $footerLines.ChildNodes | ForEach-Object { "$($_.TaxRate):$($_.TotalInclTax)" }
    $result = [System.String]::Join("|", $items)
    $result
}

<#
.DESCRIPTION
    Converts the DataToSign element of the Receipt node into a metadata object.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function ConvertTo-ReceiptMetadata-2140 {
    param (
        [string]$dataToSign
    )
    $values = $dataToSign.Split(",")
    @{
        TotalAmountsByTaxRate = $values[0]
        TotalAmount           = $values[1]
        RegistrationTime      = $values[2]
        RegisterNumber        = $values[3]
        SequentialNumber      = $values[4]
        TransactionType       = $values[5]
        IsFirstSigned         = $values[6]
        PreviousSignature     = $values[7]
    }
}

<#
.DESCRIPTION
    Calculates the data to sign for the Receipt node.

.NOTES
    The old version of receipt copy.
#>
function Calculate-ReceiptDataToSign-Legacy {
    param (
        [System.Xml.XmlElement]$receipt
    )
    $metadata = ConvertTo-ReceiptMetadata-Legacy $receipt.DataToSign
    $previousReceipt = $receipt.PreviousSibling
    $perviousSignature = if ($previousReceipt) {
        $previousReceipt.Signature
    }
    else {
        $metadata.PreviousSignature
    }

    $part1 = $metadata.TotalAmountsByTaxRate
    $part2 = $metadata.TotalAmount
    $part3 = $metadata.RegistrationTime
    $part4 = $metadata.RegisterNumber
    $part5 = $metadata.SequentialNumber
    $part6 = $metadata.TransactionType
    $part7 = $metadata.IsFirstSigned
    $part8 = if ($part7 -eq "N") { $perviousSignature } else { "" }
    $part9 = $metadata.LineCount
    $result = [System.String]::Join(",", $part1, $part2, $part3, $part4, $part5, $part6, $part7, $part8, $part9)
    $result
}

<#
.DESCRIPTION
    Converts the DataToSign element of the Receipt node into a metadata object.

.NOTES
    The old version of receipt copy.
#>
function ConvertTo-ReceiptMetadata-Legacy {
    param (
        [string]$dataToSign
    )
    $values = $dataToSign.Split(",")
    @{
        TotalAmountsByTaxRate = $values[0]
        TotalAmount           = $values[1]
        RegistrationTime      = $values[2]
        RegisterNumber        = $values[3]
        SequentialNumber      = $values[4]
        TransactionType       = $values[5]
        IsFirstSigned         = $values[6]
        PreviousSignature     = $values[7]
        LineCount             = $values[8]
    }
}

#endregion

#region ReceiptCopy

<#
.DESCRIPTION
    Calculates the data to sign for the ReceiptCopy node.

.NOTES
    The old version of receipt copy.
#>
function Calculate-ReceiptCopyDataToSign-Legacy {
    param (
        [System.Xml.XmlElement]$receiptCopy
    )
    $metadata = ConvertTo-ReceiptCopyMetadata-Legacy $receiptCopy.DataToSign
    $previousReceiptCopy = $receiptCopy.PreviousSibling
    $perviousSignature = if ($previousReceiptCopy) {
        $previousReceiptCopy.Signature
    }
    else {
        $metadata.PreviousSignature
    }
    $part1 = $metadata.ReceiptId
    $part2 = $metadata.TransactionType
    $part3 = $metadata.CopyNumber
    $part4 = $metadata.OperatorCode
    $part5 = $metadata.RegistrationTime
    $part6 = $metadata.SequentialNumber
    $part7 = $metadata.IsFirstSigned
    $part8 = if ($part7 -eq "N") { $perviousSignature } else { "" }
    $result = [System.String]::Join(",", $part1, $part2, $part3, $part4, $part5, $part6, $part7, $part8)
    $result
}

<#
.DESCRIPTION
    Converts the DataToSign element of the ReceiptCopy node into a metadata object.

.NOTES
    The old version of receipt copy.
#>
function ConvertTo-ReceiptCopyMetadata-Legacy {
    param (
        [string]$dataToSign
    )
    $values = $dataToSign.Split(",")
    @{
        ReceiptId         = $values[0]
        TransactionType   = $values[1]
        CopyNumber        = $values[2]
        OperatorCode      = $values[3]
        RegistrationTime  = $values[4]
        SequentialNumber  = $values[5]
        IsFirstSigned     = $values[6]
        PreviousSignature = $values[7]
    }
}

<#
.DESCRIPTION
    Calculates the data to sign for the ReceiptCopy node.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function Calculate-ReceiptCopyDataToSign-2140 {
    param (
        [System.Xml.XmlElement]$receiptCopy
    )
    $metadata = ConvertTo-ReceiptCopyMetadata-2140 $receiptCopy.DataToSign
    $previousReceiptCopy = $receiptCopy.PreviousSibling
    $perviousSignature = if ($previousReceiptCopy) {
        $previousReceiptCopy.Signature
    }
    else {
        $metadata.PreviousSignature
    }
    $part1 = $receiptCopy.RegisterNumber
    $part2 = $metadata.SequentialNumber
    $part3 = $metadata.TransactionType
    $part4 = $receiptCopy.CopyNumber
    $part5 = $receiptCopy.OperatorCode
    $part6 = $receiptCopy.Date
    $part7 = $metadata.RefRegisterNumber
    $part8 = $metadata.RefSequentialNumber
    $part9 = if ($receiptCopy.SequentialNumber -eq "1") { "Y" } else { "N" }
    $part10 = if ($part9 -eq "N") { $perviousSignature } else { "" }
    $result = [System.String]::Join(",", $part1, $part2, $part3, $part4, $part5, $part6, $part7, $part8, $part9, $part10)
    $result
}

<#
.DESCRIPTION
    Converts the DataToSign element of the ReceiptCopy node into a metadata object.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function ConvertTo-ReceiptCopyMetadata-2140 {
    param (
        [string]$dataToSign
    )
    $values = $dataToSign.Split(",")
    @{
        RegisterNumber      = $values[0]
        SequentialNumber    = $values[1]
        TransactionType     = $values[2]
        CopyNumber          = $values[3]
        OperatorCode        = $values[4]
        RegistrationTime    = $values[5]
        RefRegisterNumber   = $values[6]
        RefSequentialNumber = $values[7]
        IsFirstSigned       = $values[8]
        PreviousSignature   = $values[9]
    }
}

#endregion

#region AuditEvent

<#
.DESCRIPTION
    Converts the DataToSign element of the AuditEvent node into a metadata object.

.NOTES
    Old version of audit event
#>
function ConvertTo-AuditEventMetadata-Legacy {
    param (
        [string]$dataToSign
    )
    $values = $dataToSign.Split(",")
    @{
        SequentialNumber  = $values[0]
        Code              = $values[1]
        Message           = $values[2]
        RegistrationTime  = $values[3]
        OperatorCode      = $values[4]
        RegisterNumber    = $values[5]
        IsFirstSigned     = $values[6]
        PreviousSignature = $values[7]
    }
}

<#
.DESCRIPTION
    Calculates the data to sign for the AuditEvent node.

.NOTES
    The old version of audit event
#>
function Calculate-AuditEventDataToSign-Legacy {
    param (
        [System.Xml.XmlElement]$auditEvent
    )
    $previousAuditEvent = $auditEvent.PreviousSibling
    $metadata = ConvertTo-AuditEventMetadata-Legacy $auditEvent.DataToSign
    $perviousSignature = if ($previousAuditEvent) {
        $previousAuditEvent.Signature
    }
    else {
        $metadata.PreviousSignature
    }
    $part1 = $metadata.SequentialNumber
    $part2 = $metadata.Code
    $part3 = $metadata.Message
    $part4 = $metadata.RegistrationTime
    $part5 = $metadata.OperatorCode
    $part6 = $auditEvent.RegisterNumber
    $part7 = $metadata.IsFirstSigned
    $part8 = if ($part7 -eq "N") { $perviousSignature } else { "" }
    $result = [System.String]::Join(",", $part1, $part2, $part3, $part4, $part5, $part6, $part7, $part8)
    $result
}


<#
.DESCRIPTION
    Calculates the data to sign for the AuditEvent node.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function Calculate-AuditEventDataToSign-2140 {
    param (
        [System.Xml.XmlElement]$auditEvent
    )
    $previousAuditEvent = $auditEvent.PreviousSibling
    $metadata = ConvertTo-AuditEventMetadata-2140 $auditEvent.DataToSign
    $perviousSignature = if ($previousAuditEvent) {
        $previousAuditEvent.Signature
    }
    else {
        $metadata.PreviousSignature
    }
    $part1 = $auditEvent.SequentialNumber
    $part2 = $auditEvent.Code
    $part3 = $metadata.Message
    $part4 = $auditEvent.Date
    $part5 = $auditEvent.OperatorCode
    $part6 = $auditEvent.RegisterNumber
    $part7 = if ($auditEvent.SequentialNumber -eq "1") { "Y" } else { "N" }
    $part8 = if ($part7 -eq "N") { $perviousSignature } else { "" }
    $result = [System.String]::Join(",", $part1, $part2, $part3, $part4, $part5, $part6, $part7, $part8)
    $result
}

<#
.DESCRIPTION
    Converts the DataToSign element of the AuditEvent node into a metadata object.

.NOTES
    NF525 V2.1. Data dictionaries R13 and R19 V4.0.
#>
function ConvertTo-AuditEventMetadata-2140 {
    param (
        [string]$dataToSign
    )
    $values = $dataToSign.Split(",")
    @{
        SequentialNumber  = $values[0]
        Code              = $values[1]
        Message           = $values[2]
        RegistrationTime  = $values[3]
        OperatorCode      = $values[4]
        RegisterNumber    = $values[5]
        IsFirstSigned     = $values[6]
        PreviousSignature = $values[7]
    }
}

#endregion

#region Labels

<#
.DESCRIPTION
    Initializes the labels object from the resource file which corresponds to the current culture.
#>
function Initialize-Labels {
    $resourcesDirectoryName = "resources"
    $resourcesFileName = "resources.resjson"
    $defaultCultureName = "en-US"
    $script:Labels = @{}
    $currentCultureName = (Get-Culture).Name
    $labelsPath = [IO.Path]::Combine($PSScriptRoot, $resourcesDirectoryName, $currentCultureName, $resourcesFileName)
    if (-not (Test-Path $labelsPath)) {
        $labelsPath = [IO.Path]::Combine($PSScriptRoot, $resourcesDirectoryName, $defaultCultureName, $resourcesFileName)
    }
    $script:Labels = Get-Content $labelsPath | ConvertFrom-Json
}

#endregion

main

# SIG # Begin signature block
# MIIoVQYJKoZIhvcNAQcCoIIoRjCCKEICAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCChUjriZqzREy58
# qjbD/WbXDVjtxt+xAhAVks0YJaADT6CCDYUwggYDMIID66ADAgECAhMzAAAEA73V
# lV0POxitAAAAAAQDMA0GCSqGSIb3DQEBCwUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTEwHhcNMjQwOTEyMjAxMTEzWhcNMjUwOTExMjAxMTEzWjB0MQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMR4wHAYDVQQDExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
# AQCfdGddwIOnbRYUyg03O3iz19XXZPmuhEmW/5uyEN+8mgxl+HJGeLGBR8YButGV
# LVK38RxcVcPYyFGQXcKcxgih4w4y4zJi3GvawLYHlsNExQwz+v0jgY/aejBS2EJY
# oUhLVE+UzRihV8ooxoftsmKLb2xb7BoFS6UAo3Zz4afnOdqI7FGoi7g4vx/0MIdi
# kwTn5N56TdIv3mwfkZCFmrsKpN0zR8HD8WYsvH3xKkG7u/xdqmhPPqMmnI2jOFw/
# /n2aL8W7i1Pasja8PnRXH/QaVH0M1nanL+LI9TsMb/enWfXOW65Gne5cqMN9Uofv
# ENtdwwEmJ3bZrcI9u4LZAkujAgMBAAGjggGCMIIBfjAfBgNVHSUEGDAWBgorBgEE
# AYI3TAgBBggrBgEFBQcDAzAdBgNVHQ4EFgQU6m4qAkpz4641iK2irF8eWsSBcBkw
# VAYDVR0RBE0wS6RJMEcxLTArBgNVBAsTJE1pY3Jvc29mdCBJcmVsYW5kIE9wZXJh
# dGlvbnMgTGltaXRlZDEWMBQGA1UEBRMNMjMwMDEyKzUwMjkyNjAfBgNVHSMEGDAW
# gBRIbmTlUAXTgqoXNzcitW2oynUClTBUBgNVHR8ETTBLMEmgR6BFhkNodHRwOi8v
# d3d3Lm1pY3Jvc29mdC5jb20vcGtpb3BzL2NybC9NaWNDb2RTaWdQQ0EyMDExXzIw
# MTEtMDctMDguY3JsMGEGCCsGAQUFBwEBBFUwUzBRBggrBgEFBQcwAoZFaHR0cDov
# L3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9jZXJ0cy9NaWNDb2RTaWdQQ0EyMDEx
# XzIwMTEtMDctMDguY3J0MAwGA1UdEwEB/wQCMAAwDQYJKoZIhvcNAQELBQADggIB
# AFFo/6E4LX51IqFuoKvUsi80QytGI5ASQ9zsPpBa0z78hutiJd6w154JkcIx/f7r
# EBK4NhD4DIFNfRiVdI7EacEs7OAS6QHF7Nt+eFRNOTtgHb9PExRy4EI/jnMwzQJV
# NokTxu2WgHr/fBsWs6G9AcIgvHjWNN3qRSrhsgEdqHc0bRDUf8UILAdEZOMBvKLC
# rmf+kJPEvPldgK7hFO/L9kmcVe67BnKejDKO73Sa56AJOhM7CkeATrJFxO9GLXos
# oKvrwBvynxAg18W+pagTAkJefzneuWSmniTurPCUE2JnvW7DalvONDOtG01sIVAB
# +ahO2wcUPa2Zm9AiDVBWTMz9XUoKMcvngi2oqbsDLhbK+pYrRUgRpNt0y1sxZsXO
# raGRF8lM2cWvtEkV5UL+TQM1ppv5unDHkW8JS+QnfPbB8dZVRyRmMQ4aY/tx5x5+
# sX6semJ//FbiclSMxSI+zINu1jYerdUwuCi+P6p7SmQmClhDM+6Q+btE2FtpsU0W
# +r6RdYFf/P+nK6j2otl9Nvr3tWLu+WXmz8MGM+18ynJ+lYbSmFWcAj7SYziAfT0s
# IwlQRFkyC71tsIZUhBHtxPliGUu362lIO0Lpe0DOrg8lspnEWOkHnCT5JEnWCbzu
# iVt8RX1IV07uIveNZuOBWLVCzWJjEGa+HhaEtavjy6i7MIIHejCCBWKgAwIBAgIK
# YQ6Q0gAAAAAAAzANBgkqhkiG9w0BAQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNV
# BAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jv
# c29mdCBDb3Jwb3JhdGlvbjEyMDAGA1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlm
# aWNhdGUgQXV0aG9yaXR5IDIwMTEwHhcNMTEwNzA4MjA1OTA5WhcNMjYwNzA4MjEw
# OTA5WjB+MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UE
# BxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSgwJgYD
# VQQDEx9NaWNyb3NvZnQgQ29kZSBTaWduaW5nIFBDQSAyMDExMIICIjANBgkqhkiG
# 9w0BAQEFAAOCAg8AMIICCgKCAgEAq/D6chAcLq3YbqqCEE00uvK2WCGfQhsqa+la
# UKq4BjgaBEm6f8MMHt03a8YS2AvwOMKZBrDIOdUBFDFC04kNeWSHfpRgJGyvnkmc
# 6Whe0t+bU7IKLMOv2akrrnoJr9eWWcpgGgXpZnboMlImEi/nqwhQz7NEt13YxC4D
# dato88tt8zpcoRb0RrrgOGSsbmQ1eKagYw8t00CT+OPeBw3VXHmlSSnnDb6gE3e+
# lD3v++MrWhAfTVYoonpy4BI6t0le2O3tQ5GD2Xuye4Yb2T6xjF3oiU+EGvKhL1nk
# kDstrjNYxbc+/jLTswM9sbKvkjh+0p2ALPVOVpEhNSXDOW5kf1O6nA+tGSOEy/S6
# A4aN91/w0FK/jJSHvMAhdCVfGCi2zCcoOCWYOUo2z3yxkq4cI6epZuxhH2rhKEmd
# X4jiJV3TIUs+UsS1Vz8kA/DRelsv1SPjcF0PUUZ3s/gA4bysAoJf28AVs70b1FVL
# 5zmhD+kjSbwYuER8ReTBw3J64HLnJN+/RpnF78IcV9uDjexNSTCnq47f7Fufr/zd
# sGbiwZeBe+3W7UvnSSmnEyimp31ngOaKYnhfsi+E11ecXL93KCjx7W3DKI8sj0A3
# T8HhhUSJxAlMxdSlQy90lfdu+HggWCwTXWCVmj5PM4TasIgX3p5O9JawvEagbJjS
# 4NaIjAsCAwEAAaOCAe0wggHpMBAGCSsGAQQBgjcVAQQDAgEAMB0GA1UdDgQWBBRI
# bmTlUAXTgqoXNzcitW2oynUClTAZBgkrBgEEAYI3FAIEDB4KAFMAdQBiAEMAQTAL
# BgNVHQ8EBAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBRyLToCMZBD
# uRQFTuHqp8cx0SOJNDBaBgNVHR8EUzBRME+gTaBLhklodHRwOi8vY3JsLm1pY3Jv
# c29mdC5jb20vcGtpL2NybC9wcm9kdWN0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFf
# MDNfMjIuY3JsMF4GCCsGAQUFBwEBBFIwUDBOBggrBgEFBQcwAoZCaHR0cDovL3d3
# dy5taWNyb3NvZnQuY29tL3BraS9jZXJ0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFf
# MDNfMjIuY3J0MIGfBgNVHSAEgZcwgZQwgZEGCSsGAQQBgjcuAzCBgzA/BggrBgEF
# BQcCARYzaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9kb2NzL3ByaW1h
# cnljcHMuaHRtMEAGCCsGAQUFBwICMDQeMiAdAEwAZQBnAGEAbABfAHAAbwBsAGkA
# YwB5AF8AcwB0AGEAdABlAG0AZQBuAHQALiAdMA0GCSqGSIb3DQEBCwUAA4ICAQBn
# 8oalmOBUeRou09h0ZyKbC5YR4WOSmUKWfdJ5DJDBZV8uLD74w3LRbYP+vj/oCso7
# v0epo/Np22O/IjWll11lhJB9i0ZQVdgMknzSGksc8zxCi1LQsP1r4z4HLimb5j0b
# pdS1HXeUOeLpZMlEPXh6I/MTfaaQdION9MsmAkYqwooQu6SpBQyb7Wj6aC6VoCo/
# KmtYSWMfCWluWpiW5IP0wI/zRive/DvQvTXvbiWu5a8n7dDd8w6vmSiXmE0OPQvy
# CInWH8MyGOLwxS3OW560STkKxgrCxq2u5bLZ2xWIUUVYODJxJxp/sfQn+N4sOiBp
# mLJZiWhub6e3dMNABQamASooPoI/E01mC8CzTfXhj38cbxV9Rad25UAqZaPDXVJi
# hsMdYzaXht/a8/jyFqGaJ+HNpZfQ7l1jQeNbB5yHPgZ3BtEGsXUfFL5hYbXw3MYb
# BL7fQccOKO7eZS/sl/ahXJbYANahRr1Z85elCUtIEJmAH9AAKcWxm6U/RXceNcbS
# oqKfenoi+kiVH6v7RyOA9Z74v2u3S5fi63V4GuzqN5l5GEv/1rMjaHXmr/r8i+sL
# gOppO6/8MO0ETI7f33VtY5E90Z1WTk+/gFcioXgRMiF670EKsT/7qMykXcGhiJtX
# cVZOSEXAQsmbdlsKgEhr/Xmfwb1tbWrJUnMTDXpQzTGCGiYwghoiAgEBMIGVMH4x
# CzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRt
# b25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01p
# Y3Jvc29mdCBDb2RlIFNpZ25pbmcgUENBIDIwMTECEzMAAAQDvdWVXQ87GK0AAAAA
# BAMwDQYJYIZIAWUDBAIBBQCgga4wGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcCAQQw
# HAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwLwYJKoZIhvcNAQkEMSIEICYO
# cUbyYg4rbdUK5Useds6XVVCeAtUzIJ4GWmi3dlu+MEIGCisGAQQBgjcCAQwxNDAy
# oBSAEgBNAGkAYwByAG8AcwBvAGYAdKEagBhodHRwOi8vd3d3Lm1pY3Jvc29mdC5j
# b20wDQYJKoZIhvcNAQEBBQAEggEAeWg4BIhl0691/OgdT7qrp19+RaVosQrTdGRg
# 0rdw0MZwJJJ02HTbilDYxB7UcQl/Shm5LsPj3uZwgUWfIAfraeakhHF3HkQLg6BP
# Ctp+am7jkxQlx7NXH/BqmrN+1omYBl7jWpcKKdDFogNYYheybQPa9/EMIfChFJLn
# pXs92PRl0Exf3jmJsXs9RJuRtK9T4/WbiYhpedhd0u/qqeKUwoCqx1cDVBqwWpSd
# UOkT+IETRd1UCVMqmFFQrZNdSeAsZ8YvGpiNjZ8OagqOavfczwyeOYP1TWhh8v1i
# Doh9ie2i7I7kDDBz5zaL7qpYSFMlqBemIPAa0i65cn0eeyb4lqGCF7AwghesBgor
# BgEEAYI3AwMBMYIXnDCCF5gGCSqGSIb3DQEHAqCCF4kwgheFAgEDMQ8wDQYJYIZI
# AWUDBAIBBQAwggFaBgsqhkiG9w0BCRABBKCCAUkEggFFMIIBQQIBAQYKKwYBBAGE
# WQoDATAxMA0GCWCGSAFlAwQCAQUABCClHc9DR4iUyDd0yqRcP9xrYUvuCi4gZemj
# JOI36gDgsgIGaBLPHET2GBMyMDI1MDUxODEwMTQzOS4zNDlaMASAAgH0oIHZpIHW
# MIHTMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMH
# UmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMS0wKwYDVQQL
# EyRNaWNyb3NvZnQgSXJlbGFuZCBPcGVyYXRpb25zIExpbWl0ZWQxJzAlBgNVBAsT
# Hm5TaGllbGQgVFNTIEVTTjo2NTFBLTA1RTAtRDk0NzElMCMGA1UEAxMcTWljcm9z
# b2Z0IFRpbWUtU3RhbXAgU2VydmljZaCCEf4wggcoMIIFEKADAgECAhMzAAAB9ZkJ
# lLzxxlCMAAEAAAH1MA0GCSqGSIb3DQEBCwUAMHwxCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1w
# IFBDQSAyMDEwMB4XDTI0MDcyNTE4MzEwMVoXDTI1MTAyMjE4MzEwMVowgdMxCzAJ
# BgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25k
# MR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xLTArBgNVBAsTJE1pY3Jv
# c29mdCBJcmVsYW5kIE9wZXJhdGlvbnMgTGltaXRlZDEnMCUGA1UECxMeblNoaWVs
# ZCBUU1MgRVNOOjY1MUEtMDVFMC1EOTQ3MSUwIwYDVQQDExxNaWNyb3NvZnQgVGlt
# ZS1TdGFtcCBTZXJ2aWNlMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA
# zO90cFQTWd/WP84IT7JMIW1fQL61sdfgmhlfT0nvYEb2kvkNF073ZwjveuSWot38
# 7LjE0TCiG93e6I0HzIFQBnbxGP/WPBUirFq7WE5RAsuhNfYUL+PIb9jJq3CwWxIC
# fw5t/pTyIOHjKvo1lQOTWZypir/psZwEE7y2uWAPbZJTFrKen5R73x2Hbxy4eW1D
# cmXjym2wFWv10sBH40ajJfe+OkwcTdoYrY3KkpN/RQSjeycK0bhjo0CGYIYa+ZMA
# ao0SNR/R1J1Y6sLkiCJO3aQrbS1Sz7l+/qJgy8fyEZMND5Ms7C0sEaOvoBHiWSpT
# M4vc0xDLCmc6PGv03CtWu2KiyqrL8BAB1EYyOShI3IT79arDIDrL+de91FfjmSbB
# Y5j+HvS0l3dXkjP3Hon8b74lWwikF0rzErF0n3khVAusx7Sm1oGG+06hz9XAy3Wo
# u+T6Se6oa5LDiQgPTfWR/j9FNk8Ju06oSfTh6c03V0ulla0Iwy+HzUl+WmYxFLU0
# PiaXsmgudNwVqn51zr+Bi3XPJ85wWuy6GGT7nBDmXNzTNkzK98DBQjTOabQXUZ88
# 4Yb9DFNcigmeVTYkyUXZ6hscd8Nyq45A3D3bk+nXnsogK1Z7zZj6XbGft7xgOYvv
# eU6p0+frthbF7MXv+i5qcD9HfFmOq4VYHevVesYb6P0CAwEAAaOCAUkwggFFMB0G
# A1UdDgQWBBRV4Hxb9Uo0oHDwJZJe22ixe2B1ATAfBgNVHSMEGDAWgBSfpxVdAF5i
# XYP05dJlpxtTNRnpcjBfBgNVHR8EWDBWMFSgUqBQhk5odHRwOi8vd3d3Lm1pY3Jv
# c29mdC5jb20vcGtpb3BzL2NybC9NaWNyb3NvZnQlMjBUaW1lLVN0YW1wJTIwUENB
# JTIwMjAxMCgxKS5jcmwwbAYIKwYBBQUHAQEEYDBeMFwGCCsGAQUFBzAChlBodHRw
# Oi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpb3BzL2NlcnRzL01pY3Jvc29mdCUyMFRp
# bWUtU3RhbXAlMjBQQ0ElMjAyMDEwKDEpLmNydDAMBgNVHRMBAf8EAjAAMBYGA1Ud
# JQEB/wQMMAoGCCsGAQUFBwMIMA4GA1UdDwEB/wQEAwIHgDANBgkqhkiG9w0BAQsF
# AAOCAgEAcwxmVPaA9xHffuom0TOSp2hspuf1G0cHW/KXHAuhnpW8/Svlq5j9aKI/
# 8/G6fGIQMr0zlpau8jy83I4zclGdJjl5S02SxDlUKawtWvgf7ida06PgjeQM1eX4
# Lut4bbPfT0FEp77G76hhysXxTJNHv5y+fwThUeiiclihZwqcZMpa46m+oV6igTU6
# I0EnneotMqFs0Q3zHgVVr4WXjnG2Bcnkip42edyg/9iXczqTBrEkvTz0UlltpFGa
# QnLzq+No8VEgq0UG7W1ELZGhmmxFmHABwTT6sPJFV68DfLoC0iB9Qbb9VZ8mvbTV
# 5JtISBklTuVAlEkzXi9LIjNmx+kndBfKP8dxG/xbRXptQDQDaCsS6ogLkwLgH6zS
# s+ul9WmzI0F8zImbhnZhUziIHheFo4H+ZoojPYcgTK6/3bkSbOabmQFf95B8B6e5
# WqXbS5s9OdMdUlW1gTI1r5u+WAwH2KG7dxneoTbf/jYl3TUtP7AHpyck2c0nun/Q
# 0Cycpa9QUH/Dy01k6tQomNXGjivg2/BGcgZJ0Hw8C6KVelEJ31xLoE21m9+NEgSK
# CRoFE1Lkma31SyIaynbdYEb8sOlZynMdm8yPldDwuF54vJiEArjrcDNXe6BobZUi
# TWSKvv1DJadR1SUCO/Od21GgU+hZqu+dKgjKAYdeTIvi9R2rtLYwggdxMIIFWaAD
# AgECAhMzAAAAFcXna54Cm0mZAAAAAAAVMA0GCSqGSIb3DQEBCwUAMIGIMQswCQYD
# VQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEe
# MBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMTIwMAYDVQQDEylNaWNyb3Nv
# ZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHkgMjAxMDAeFw0yMTA5MzAxODIy
# MjVaFw0zMDA5MzAxODMyMjVaMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNo
# aW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29y
# cG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEw
# MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA5OGmTOe0ciELeaLL1yR5
# vQ7VgtP97pwHB9KpbE51yMo1V/YBf2xK4OK9uT4XYDP/XE/HZveVU3Fa4n5KWv64
# NmeFRiMMtY0Tz3cywBAY6GB9alKDRLemjkZrBxTzxXb1hlDcwUTIcVxRMTegCjhu
# je3XD9gmU3w5YQJ6xKr9cmmvHaus9ja+NSZk2pg7uhp7M62AW36MEBydUv626GIl
# 3GoPz130/o5Tz9bshVZN7928jaTjkY+yOSxRnOlwaQ3KNi1wjjHINSi947SHJMPg
# yY9+tVSP3PoFVZhtaDuaRr3tpK56KTesy+uDRedGbsoy1cCGMFxPLOJiss254o2I
# 5JasAUq7vnGpF1tnYN74kpEeHT39IM9zfUGaRnXNxF803RKJ1v2lIH1+/NmeRd+2
# ci/bfV+AutuqfjbsNkz2K26oElHovwUDo9Fzpk03dJQcNIIP8BDyt0cY7afomXw/
# TNuvXsLz1dhzPUNOwTM5TI4CvEJoLhDqhFFG4tG9ahhaYQFzymeiXtcodgLiMxhy
# 16cg8ML6EgrXY28MyTZki1ugpoMhXV8wdJGUlNi5UPkLiWHzNgY1GIRH29wb0f2y
# 1BzFa/ZcUlFdEtsluq9QBXpsxREdcu+N+VLEhReTwDwV2xo3xwgVGD94q0W29R6H
# XtqPnhZyacaue7e3PmriLq0CAwEAAaOCAd0wggHZMBIGCSsGAQQBgjcVAQQFAgMB
# AAEwIwYJKwYBBAGCNxUCBBYEFCqnUv5kxJq+gpE8RjUpzxD/LwTuMB0GA1UdDgQW
# BBSfpxVdAF5iXYP05dJlpxtTNRnpcjBcBgNVHSAEVTBTMFEGDCsGAQQBgjdMg30B
# ATBBMD8GCCsGAQUFBwIBFjNodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpb3Bz
# L0RvY3MvUmVwb3NpdG9yeS5odG0wEwYDVR0lBAwwCgYIKwYBBQUHAwgwGQYJKwYB
# BAGCNxQCBAweCgBTAHUAYgBDAEEwCwYDVR0PBAQDAgGGMA8GA1UdEwEB/wQFMAMB
# Af8wHwYDVR0jBBgwFoAU1fZWy4/oolxiaNE9lJBb186aGMQwVgYDVR0fBE8wTTBL
# oEmgR4ZFaHR0cDovL2NybC5taWNyb3NvZnQuY29tL3BraS9jcmwvcHJvZHVjdHMv
# TWljUm9vQ2VyQXV0XzIwMTAtMDYtMjMuY3JsMFoGCCsGAQUFBwEBBE4wTDBKBggr
# BgEFBQcwAoY+aHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraS9jZXJ0cy9NaWNS
# b29DZXJBdXRfMjAxMC0wNi0yMy5jcnQwDQYJKoZIhvcNAQELBQADggIBAJ1Vffwq
# reEsH2cBMSRb4Z5yS/ypb+pcFLY+TkdkeLEGk5c9MTO1OdfCcTY/2mRsfNB1OW27
# DzHkwo/7bNGhlBgi7ulmZzpTTd2YurYeeNg2LpypglYAA7AFvonoaeC6Ce5732pv
# vinLbtg/SHUB2RjebYIM9W0jVOR4U3UkV7ndn/OOPcbzaN9l9qRWqveVtihVJ9Ak
# vUCgvxm2EhIRXT0n4ECWOKz3+SmJw7wXsFSFQrP8DJ6LGYnn8AtqgcKBGUIZUnWK
# NsIdw2FzLixre24/LAl4FOmRsqlb30mjdAy87JGA0j3mSj5mO0+7hvoyGtmW9I/2
# kQH2zsZ0/fZMcm8Qq3UwxTSwethQ/gpY3UA8x1RtnWN0SCyxTkctwRQEcb9k+SS+
# c23Kjgm9swFXSVRk2XPXfx5bRAGOWhmRaw2fpCjcZxkoJLo4S5pu+yFUa2pFEUep
# 8beuyOiJXk+d0tBMdrVXVAmxaQFEfnyhYWxz/gq77EFmPWn9y8FBSX5+k77L+Dvk
# txW/tM4+pTFRhLy/AsGConsXHRWJjXD+57XQKBqJC4822rpM+Zv/Cuk0+CQ1Zyvg
# DbjmjJnW4SLq8CdCPSWU5nR0W2rRnj7tfqAxM328y+l7vzhwRNGQ8cirOoo6CGJ/
# 2XBjU02N7oJtpQUQwXEGahC0HVUzWLOhcGbyoYIDWTCCAkECAQEwggEBoYHZpIHW
# MIHTMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMH
# UmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMS0wKwYDVQQL
# EyRNaWNyb3NvZnQgSXJlbGFuZCBPcGVyYXRpb25zIExpbWl0ZWQxJzAlBgNVBAsT
# Hm5TaGllbGQgVFNTIEVTTjo2NTFBLTA1RTAtRDk0NzElMCMGA1UEAxMcTWljcm9z
# b2Z0IFRpbWUtU3RhbXAgU2VydmljZaIjCgEBMAcGBSsOAwIaAxUAJsAKu48NbR5Y
# Rg3WSBQCyjzdkvaggYMwgYCkfjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2Fz
# aGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENv
# cnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAx
# MDANBgkqhkiG9w0BAQsFAAIFAOvTtf0wIhgPMjAyNTA1MTgwMTI3MjVaGA8yMDI1
# MDUxOTAxMjcyNVowdzA9BgorBgEEAYRZCgQBMS8wLTAKAgUA69O1/QIBADAKAgEA
# AgJX2QIB/zAHAgEAAgIShzAKAgUA69UHfQIBADA2BgorBgEEAYRZCgQCMSgwJjAM
# BgorBgEEAYRZCgMCoAowCAIBAAIDB6EgoQowCAIBAAIDAYagMA0GCSqGSIb3DQEB
# CwUAA4IBAQA/pf0FnrifX3mDtVqKU1dEvKwjUUBNDMjyksnHxbOoB+WYWnrFNURA
# RnZWzn+aCnm9xrVgZHi00Ap9QfeD2rzepxOtQnR99ffYmBpWU3Vt56u/HpEiwkO9
# vnRDWO0QIy1oZKwF76LtcsL+SaZCUxCOCQb8GqUNmx2gXTekDCX12NxlEdPUlHi/
# hfN9E5E7k2E99j+RWRJZB9+s1rzUdqHgicta0o02F9rKokq8D9/jfkwhixjK7xXY
# CwmRlWkUFg8WJswQAZ6axY+YulB6RD7YsFrEV98iuukSZ1vL/wwIoFuzyArb6gyp
# v9y+PsOA4IbKQusBsnaZ+ZzGQ8P3GysTMYIEDTCCBAkCAQEwgZMwfDELMAkGA1UE
# BhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAc
# BgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0
# IFRpbWUtU3RhbXAgUENBIDIwMTACEzMAAAH1mQmUvPHGUIwAAQAAAfUwDQYJYIZI
# AWUDBAIBBQCgggFKMBoGCSqGSIb3DQEJAzENBgsqhkiG9w0BCRABBDAvBgkqhkiG
# 9w0BCQQxIgQg2Ox7H8H9mNNP0wrYjnADywx7ErAE+505ALNkR5P+qXwwgfoGCyqG
# SIb3DQEJEAIvMYHqMIHnMIHkMIG9BCDB1vLSFwh09ISu4kdEv4/tg9eR1Yk8w5x7
# j5GThqaPNTCBmDCBgKR+MHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5n
# dG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9y
# YXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwAhMz
# AAAB9ZkJlLzxxlCMAAEAAAH1MCIEILdWGaT1L98dmFMjYT09FHtDHFIvQ9P1tyR2
# +J+7HJZTMA0GCSqGSIb3DQEBCwUABIICACn9E+apQPhyY7J2QwO64qeOIPSxwlhB
# FSQFZQDr1KsQ8rRtiAiuOqnP908N1Q8uYztp8uk5Gchky0vw8jTVKJlbq51evCeT
# Pp+DTMI1Xngp0y0xxHQskCKPjXlOm4+d57kwWZDxC01WrFyNTr60Xu8S9R8UvBAv
# PK9Dxw9pC6ID/dhBsATeohVu8Y/46NDrNCV5HrZ68R675NEtO8HQPT1Or+2a4Bcq
# aB/n5pL81FNf/tF5MFLfqrSUBp1VkjkN6uDaoMGw+JGmmXNza7anqx8TPle3olxP
# ElDjcE3WTM9C3A79LJj65lltlmmUrmV48DmuAJZccCAnGQpWjlj93oIR5GMMYQJA
# ePP5otkqeSgc0FdfMg82HgoH2RcophR+hLboeas0VHtpKXUQZIACpcW6TS+3Fly5
# uVzipU81NDAwPcK1asyfeu8lR/Qg/aIdlg5WimEKFqAuFRiBT+BypzQrAyp26i7e
# Imi5/eFJj5poD+uaBLDkYAKX8hUjxgsGwqKKfUIyUqSdO/SglRLWlLKDKg74W9rz
# jvSUGUDm+shIysU1zd+l3JqGiyvol3lLYx4s67KewVXigtIWzWObX2s0h5P/Xoeb
# PMsasjssoPvTHHVi6s/4aKSIkXEGSQyjASU+UomUsfQSXfilWeAxsy7qjmjk5x91
# WKTB2oXwwxtQ
# SIG # End signature block
