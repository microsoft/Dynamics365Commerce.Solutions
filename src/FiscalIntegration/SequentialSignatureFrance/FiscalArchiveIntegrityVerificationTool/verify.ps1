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
    $rsaSignaturePadding = [System.Security.Cryptography.RSASignaturePadding]::Pkcs1
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
# MIIoQwYJKoZIhvcNAQcCoIIoNDCCKDACAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCABOVBQ2DkwtaVM
# GF+OU3vn9wR+6WcWtlCISILalhkIAKCCDXYwggX0MIID3KADAgECAhMzAAADrzBA
# DkyjTQVBAAAAAAOvMA0GCSqGSIb3DQEBCwUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTEwHhcNMjMxMTE2MTkwOTAwWhcNMjQxMTE0MTkwOTAwWjB0MQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMR4wHAYDVQQDExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
# AQDOS8s1ra6f0YGtg0OhEaQa/t3Q+q1MEHhWJhqQVuO5amYXQpy8MDPNoJYk+FWA
# hePP5LxwcSge5aen+f5Q6WNPd6EDxGzotvVpNi5ve0H97S3F7C/axDfKxyNh21MG
# 0W8Sb0vxi/vorcLHOL9i+t2D6yvvDzLlEefUCbQV/zGCBjXGlYJcUj6RAzXyeNAN
# xSpKXAGd7Fh+ocGHPPphcD9LQTOJgG7Y7aYztHqBLJiQQ4eAgZNU4ac6+8LnEGAL
# go1ydC5BJEuJQjYKbNTy959HrKSu7LO3Ws0w8jw6pYdC1IMpdTkk2puTgY2PDNzB
# tLM4evG7FYer3WX+8t1UMYNTAgMBAAGjggFzMIIBbzAfBgNVHSUEGDAWBgorBgEE
# AYI3TAgBBggrBgEFBQcDAzAdBgNVHQ4EFgQURxxxNPIEPGSO8kqz+bgCAQWGXsEw
# RQYDVR0RBD4wPKQ6MDgxHjAcBgNVBAsTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEW
# MBQGA1UEBRMNMjMwMDEyKzUwMTgyNjAfBgNVHSMEGDAWgBRIbmTlUAXTgqoXNzci
# tW2oynUClTBUBgNVHR8ETTBLMEmgR6BFhkNodHRwOi8vd3d3Lm1pY3Jvc29mdC5j
# b20vcGtpb3BzL2NybC9NaWNDb2RTaWdQQ0EyMDExXzIwMTEtMDctMDguY3JsMGEG
# CCsGAQUFBwEBBFUwUzBRBggrBgEFBQcwAoZFaHR0cDovL3d3dy5taWNyb3NvZnQu
# Y29tL3BraW9wcy9jZXJ0cy9NaWNDb2RTaWdQQ0EyMDExXzIwMTEtMDctMDguY3J0
# MAwGA1UdEwEB/wQCMAAwDQYJKoZIhvcNAQELBQADggIBAISxFt/zR2frTFPB45Yd
# mhZpB2nNJoOoi+qlgcTlnO4QwlYN1w/vYwbDy/oFJolD5r6FMJd0RGcgEM8q9TgQ
# 2OC7gQEmhweVJ7yuKJlQBH7P7Pg5RiqgV3cSonJ+OM4kFHbP3gPLiyzssSQdRuPY
# 1mIWoGg9i7Y4ZC8ST7WhpSyc0pns2XsUe1XsIjaUcGu7zd7gg97eCUiLRdVklPmp
# XobH9CEAWakRUGNICYN2AgjhRTC4j3KJfqMkU04R6Toyh4/Toswm1uoDcGr5laYn
# TfcX3u5WnJqJLhuPe8Uj9kGAOcyo0O1mNwDa+LhFEzB6CB32+wfJMumfr6degvLT
# e8x55urQLeTjimBQgS49BSUkhFN7ois3cZyNpnrMca5AZaC7pLI72vuqSsSlLalG
# OcZmPHZGYJqZ0BacN274OZ80Q8B11iNokns9Od348bMb5Z4fihxaBWebl8kWEi2O
# PvQImOAeq3nt7UWJBzJYLAGEpfasaA3ZQgIcEXdD+uwo6ymMzDY6UamFOfYqYWXk
# ntxDGu7ngD2ugKUuccYKJJRiiz+LAUcj90BVcSHRLQop9N8zoALr/1sJuwPrVAtx
# HNEgSW+AKBqIxYWM4Ev32l6agSUAezLMbq5f3d8x9qzT031jMDT+sUAoCw0M5wVt
# CUQcqINPuYjbS1WgJyZIiEkBMIIHejCCBWKgAwIBAgIKYQ6Q0gAAAAAAAzANBgkq
# hkiG9w0BAQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24x
# EDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlv
# bjEyMDAGA1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5
# IDIwMTEwHhcNMTEwNzA4MjA1OTA5WhcNMjYwNzA4MjEwOTA5WjB+MQswCQYDVQQG
# EwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwG
# A1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSgwJgYDVQQDEx9NaWNyb3NvZnQg
# Q29kZSBTaWduaW5nIFBDQSAyMDExMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIIC
# CgKCAgEAq/D6chAcLq3YbqqCEE00uvK2WCGfQhsqa+laUKq4BjgaBEm6f8MMHt03
# a8YS2AvwOMKZBrDIOdUBFDFC04kNeWSHfpRgJGyvnkmc6Whe0t+bU7IKLMOv2akr
# rnoJr9eWWcpgGgXpZnboMlImEi/nqwhQz7NEt13YxC4Ddato88tt8zpcoRb0Rrrg
# OGSsbmQ1eKagYw8t00CT+OPeBw3VXHmlSSnnDb6gE3e+lD3v++MrWhAfTVYoonpy
# 4BI6t0le2O3tQ5GD2Xuye4Yb2T6xjF3oiU+EGvKhL1nkkDstrjNYxbc+/jLTswM9
# sbKvkjh+0p2ALPVOVpEhNSXDOW5kf1O6nA+tGSOEy/S6A4aN91/w0FK/jJSHvMAh
# dCVfGCi2zCcoOCWYOUo2z3yxkq4cI6epZuxhH2rhKEmdX4jiJV3TIUs+UsS1Vz8k
# A/DRelsv1SPjcF0PUUZ3s/gA4bysAoJf28AVs70b1FVL5zmhD+kjSbwYuER8ReTB
# w3J64HLnJN+/RpnF78IcV9uDjexNSTCnq47f7Fufr/zdsGbiwZeBe+3W7UvnSSmn
# Eyimp31ngOaKYnhfsi+E11ecXL93KCjx7W3DKI8sj0A3T8HhhUSJxAlMxdSlQy90
# lfdu+HggWCwTXWCVmj5PM4TasIgX3p5O9JawvEagbJjS4NaIjAsCAwEAAaOCAe0w
# ggHpMBAGCSsGAQQBgjcVAQQDAgEAMB0GA1UdDgQWBBRIbmTlUAXTgqoXNzcitW2o
# ynUClTAZBgkrBgEEAYI3FAIEDB4KAFMAdQBiAEMAQTALBgNVHQ8EBAMCAYYwDwYD
# VR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBRyLToCMZBDuRQFTuHqp8cx0SOJNDBa
# BgNVHR8EUzBRME+gTaBLhklodHRwOi8vY3JsLm1pY3Jvc29mdC5jb20vcGtpL2Ny
# bC9wcm9kdWN0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFfMDNfMjIuY3JsMF4GCCsG
# AQUFBwEBBFIwUDBOBggrBgEFBQcwAoZCaHR0cDovL3d3dy5taWNyb3NvZnQuY29t
# L3BraS9jZXJ0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFfMDNfMjIuY3J0MIGfBgNV
# HSAEgZcwgZQwgZEGCSsGAQQBgjcuAzCBgzA/BggrBgEFBQcCARYzaHR0cDovL3d3
# dy5taWNyb3NvZnQuY29tL3BraW9wcy9kb2NzL3ByaW1hcnljcHMuaHRtMEAGCCsG
# AQUFBwICMDQeMiAdAEwAZQBnAGEAbABfAHAAbwBsAGkAYwB5AF8AcwB0AGEAdABl
# AG0AZQBuAHQALiAdMA0GCSqGSIb3DQEBCwUAA4ICAQBn8oalmOBUeRou09h0ZyKb
# C5YR4WOSmUKWfdJ5DJDBZV8uLD74w3LRbYP+vj/oCso7v0epo/Np22O/IjWll11l
# hJB9i0ZQVdgMknzSGksc8zxCi1LQsP1r4z4HLimb5j0bpdS1HXeUOeLpZMlEPXh6
# I/MTfaaQdION9MsmAkYqwooQu6SpBQyb7Wj6aC6VoCo/KmtYSWMfCWluWpiW5IP0
# wI/zRive/DvQvTXvbiWu5a8n7dDd8w6vmSiXmE0OPQvyCInWH8MyGOLwxS3OW560
# STkKxgrCxq2u5bLZ2xWIUUVYODJxJxp/sfQn+N4sOiBpmLJZiWhub6e3dMNABQam
# ASooPoI/E01mC8CzTfXhj38cbxV9Rad25UAqZaPDXVJihsMdYzaXht/a8/jyFqGa
# J+HNpZfQ7l1jQeNbB5yHPgZ3BtEGsXUfFL5hYbXw3MYbBL7fQccOKO7eZS/sl/ah
# XJbYANahRr1Z85elCUtIEJmAH9AAKcWxm6U/RXceNcbSoqKfenoi+kiVH6v7RyOA
# 9Z74v2u3S5fi63V4GuzqN5l5GEv/1rMjaHXmr/r8i+sLgOppO6/8MO0ETI7f33Vt
# Y5E90Z1WTk+/gFcioXgRMiF670EKsT/7qMykXcGhiJtXcVZOSEXAQsmbdlsKgEhr
# /Xmfwb1tbWrJUnMTDXpQzTGCGiMwghofAgEBMIGVMH4xCzAJBgNVBAYTAlVTMRMw
# EQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVN
# aWNyb3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNp
# Z25pbmcgUENBIDIwMTECEzMAAAOvMEAOTKNNBUEAAAAAA68wDQYJYIZIAWUDBAIB
# BQCgga4wGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcCAQQwHAYKKwYBBAGCNwIBCzEO
# MAwGCisGAQQBgjcCARUwLwYJKoZIhvcNAQkEMSIEIOS1wUPlAk3dF72Oj+a11WnC
# VyoUpIJXnWJtbyhvxqlYMEIGCisGAQQBgjcCAQwxNDAyoBSAEgBNAGkAYwByAG8A
# cwBvAGYAdKEagBhodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20wDQYJKoZIhvcNAQEB
# BQAEggEApcSc5/aRJ4ooLt26Tkesb4cLAELZIOs9Dipr5HotTWdyZ/YRpEcYhwnE
# 8QLtaWaJijAVWx8Zw9uiPBmIlNe7lTrJR4/t9lJfiG52DWGnt10yeb+rk2VdZddp
# Ytx085B/zjrpTkHkSjh8mGXb2P8PxSUApwdCbgTexptKniQ3KKCsx7qTl03wqezk
# H5ZRX4xjkRb8B2iN/d+Qjup0jZTsZccPSz6R/XSLjn+Tw9dBYlLN/v948nsZSIGu
# SgOcR4Ayp4l6vLIVqY6IDvrIhGIy42Q69OYpe5ElhU9l1I5teaev1SsFbeScFIa3
# jphnm5uck3KSKv2ZVhxGwK5tlbOiaaGCF60wghepBgorBgEEAYI3AwMBMYIXmTCC
# F5UGCSqGSIb3DQEHAqCCF4YwgheCAgEDMQ8wDQYJYIZIAWUDBAIBBQAwggFaBgsq
# hkiG9w0BCRABBKCCAUkEggFFMIIBQQIBAQYKKwYBBAGEWQoDATAxMA0GCWCGSAFl
# AwQCAQUABCAe05ZUJJMrFrmb2YaS6DyIsq3XcfAtW82JvWvhImewCAIGZusoPQR/
# GBMyMDI0MTAxMDEwMTU0Mi40ODdaMASAAgH0oIHZpIHWMIHTMQswCQYDVQQGEwJV
# UzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UE
# ChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMS0wKwYDVQQLEyRNaWNyb3NvZnQgSXJl
# bGFuZCBPcGVyYXRpb25zIExpbWl0ZWQxJzAlBgNVBAsTHm5TaGllbGQgVFNTIEVT
# TjozMjFBLTA1RTAtRDk0NzElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAg
# U2VydmljZaCCEfswggcoMIIFEKADAgECAhMzAAAB+KOhJgwMQEj+AAEAAAH4MA0G
# CSqGSIb3DQEBCwUAMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9u
# MRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRp
# b24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwMB4XDTI0
# MDcyNTE4MzEwOFoXDTI1MTAyMjE4MzEwOFowgdMxCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xLTArBgNVBAsTJE1pY3Jvc29mdCBJcmVsYW5kIE9w
# ZXJhdGlvbnMgTGltaXRlZDEnMCUGA1UECxMeblNoaWVsZCBUU1MgRVNOOjMyMUEt
# MDVFMC1EOTQ3MSUwIwYDVQQDExxNaWNyb3NvZnQgVGltZS1TdGFtcCBTZXJ2aWNl
# MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAxR23pXYnD2BuODdeXs2C
# u/T5kKI+bAw8cbtN50Cm/FArjXyL4RTqMe6laQ/CqeMTxgckvZr1JrW0Mi4F15rx
# /VveGhKBmob45DmOcV5xyx7h9Tk59NAl5PNMAWKAIWf270SWAAWxQbpVIhhPWCnV
# V3otVvahEad8pMmoSXrT5Z7Nk1RnB70A2bq9Hk8wIeC3vBuxEX2E8X50IgAHsyaR
# 9roFq3ErzUEHlS8YnSq33ui5uBcrFOcFOCZILuVFVTgEqSrX4UiX0etqi7jUtKyp
# gIflaZcV5cI5XI/eCxY8wDNmBprhYMNlYxdmQ9aLRDcTKWtddWpnJtyl5e3gHuYo
# j8xuDQ0XZNy7ESRwJIK03+rTZqfaYyM4XSK1s0aa+mO69vo/NmJ4R/f1+KucBPJ4
# yUdbqJWM3xMvBwLYycvigI/WK4kgPog0UBNczaQwDVXpcU+TMcOvWP8HBWmWJQIm
# TZInAFivXqUaBbo3wAfPNbsQpvNNGu/12pg0F8O/CdRfgPHfOhIWQ0D8ALCY+Lsi
# wbzcejbrVl4N9fn2wOg2sDa8RfNoD614I0pFjy/lq1NsBo9V4GZBikzX7ZjWCRgd
# 1FCBXGpfpDikHjQ05YOkAakdWDT2bGSaUZJGVYtepIpPTAs1gd/vUogcdiL51o7s
# huHIlB6QSUiQ24XYhRbbQCECAwEAAaOCAUkwggFFMB0GA1UdDgQWBBS9zsZzz57Q
# lT5nrt/oitLv1OQ7tjAfBgNVHSMEGDAWgBSfpxVdAF5iXYP05dJlpxtTNRnpcjBf
# BgNVHR8EWDBWMFSgUqBQhk5odHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpb3Bz
# L2NybC9NaWNyb3NvZnQlMjBUaW1lLVN0YW1wJTIwUENBJTIwMjAxMCgxKS5jcmww
# bAYIKwYBBQUHAQEEYDBeMFwGCCsGAQUFBzAChlBodHRwOi8vd3d3Lm1pY3Jvc29m
# dC5jb20vcGtpb3BzL2NlcnRzL01pY3Jvc29mdCUyMFRpbWUtU3RhbXAlMjBQQ0El
# MjAyMDEwKDEpLmNydDAMBgNVHRMBAf8EAjAAMBYGA1UdJQEB/wQMMAoGCCsGAQUF
# BwMIMA4GA1UdDwEB/wQEAwIHgDANBgkqhkiG9w0BAQsFAAOCAgEAYfk8GzzpEVnG
# l7y6oXoytCb42Hx6TOA0+dkaBI36ftDE9tLubUa/xMbHB5rcNiRhFHZ93RefdPpc
# 4+FF0DAl5lP8xKAO+293RWPKDFOFIxgtZY08t8D9cSQpgGUzyw3lETZebNLEA17A
# /CTpA2F9uh8j84KygeEbj+bidWDiEfayoH2A5/5ywJJxIuLzFVHacvWxSCKoF9hl
# SrZSG5fXWS3namf4tt690UT6AGyWLFWe895coFPxm/m0UIMjjp9VRFH7nb3Ng2Q4
# gPS9E5ZTMZ6nAlmUicDj0NXAs2wQuQrnYnbRAJ/DQW35qLo7Daw9AsItqjFhbMcG
# 68gDc4j74L2KYe/2goBHLwzSn5UDftS1HZI0ZRsqmNHI0TZvvUWX9ajm6SfLBTEt
# oTo6gLOX0UD/9rrhGjdkiCw4SwU5osClgqgiNMK5ndk2gxFlDXHCyLp5qB6BoPpc
# 82RhO0yCzoP9gv7zv2EocAWEsqE5+0Wmu5uarmfvcziLfU1SY240OZW8ld4sS8fn
# ybn/jDMmFAhazV1zH0QERWEsfLSpwkOXaImWNFJ5lmcnf1VTm6cmfasScYtElpjq
# Z9GooCmk1XFApORPs/PO43IcFmPRwagt00iQSw+rBeIH00KQq+FJT/62SB70g9g/
# R8TS6k6b/wt2UWhqrW+Q8lw6Xzgex/YwggdxMIIFWaADAgECAhMzAAAAFcXna54C
# m0mZAAAAAAAVMA0GCSqGSIb3DQEBCwUAMIGIMQswCQYDVQQGEwJVUzETMBEGA1UE
# CBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9z
# b2Z0IENvcnBvcmF0aW9uMTIwMAYDVQQDEylNaWNyb3NvZnQgUm9vdCBDZXJ0aWZp
# Y2F0ZSBBdXRob3JpdHkgMjAxMDAeFw0yMTA5MzAxODIyMjVaFw0zMDA5MzAxODMy
# MjVaMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQH
# EwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNV
# BAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwMIICIjANBgkqhkiG9w0B
# AQEFAAOCAg8AMIICCgKCAgEA5OGmTOe0ciELeaLL1yR5vQ7VgtP97pwHB9KpbE51
# yMo1V/YBf2xK4OK9uT4XYDP/XE/HZveVU3Fa4n5KWv64NmeFRiMMtY0Tz3cywBAY
# 6GB9alKDRLemjkZrBxTzxXb1hlDcwUTIcVxRMTegCjhuje3XD9gmU3w5YQJ6xKr9
# cmmvHaus9ja+NSZk2pg7uhp7M62AW36MEBydUv626GIl3GoPz130/o5Tz9bshVZN
# 7928jaTjkY+yOSxRnOlwaQ3KNi1wjjHINSi947SHJMPgyY9+tVSP3PoFVZhtaDua
# Rr3tpK56KTesy+uDRedGbsoy1cCGMFxPLOJiss254o2I5JasAUq7vnGpF1tnYN74
# kpEeHT39IM9zfUGaRnXNxF803RKJ1v2lIH1+/NmeRd+2ci/bfV+AutuqfjbsNkz2
# K26oElHovwUDo9Fzpk03dJQcNIIP8BDyt0cY7afomXw/TNuvXsLz1dhzPUNOwTM5
# TI4CvEJoLhDqhFFG4tG9ahhaYQFzymeiXtcodgLiMxhy16cg8ML6EgrXY28MyTZk
# i1ugpoMhXV8wdJGUlNi5UPkLiWHzNgY1GIRH29wb0f2y1BzFa/ZcUlFdEtsluq9Q
# BXpsxREdcu+N+VLEhReTwDwV2xo3xwgVGD94q0W29R6HXtqPnhZyacaue7e3Pmri
# Lq0CAwEAAaOCAd0wggHZMBIGCSsGAQQBgjcVAQQFAgMBAAEwIwYJKwYBBAGCNxUC
# BBYEFCqnUv5kxJq+gpE8RjUpzxD/LwTuMB0GA1UdDgQWBBSfpxVdAF5iXYP05dJl
# pxtTNRnpcjBcBgNVHSAEVTBTMFEGDCsGAQQBgjdMg30BATBBMD8GCCsGAQUFBwIB
# FjNodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpb3BzL0RvY3MvUmVwb3NpdG9y
# eS5odG0wEwYDVR0lBAwwCgYIKwYBBQUHAwgwGQYJKwYBBAGCNxQCBAweCgBTAHUA
# YgBDAEEwCwYDVR0PBAQDAgGGMA8GA1UdEwEB/wQFMAMBAf8wHwYDVR0jBBgwFoAU
# 1fZWy4/oolxiaNE9lJBb186aGMQwVgYDVR0fBE8wTTBLoEmgR4ZFaHR0cDovL2Ny
# bC5taWNyb3NvZnQuY29tL3BraS9jcmwvcHJvZHVjdHMvTWljUm9vQ2VyQXV0XzIw
# MTAtMDYtMjMuY3JsMFoGCCsGAQUFBwEBBE4wTDBKBggrBgEFBQcwAoY+aHR0cDov
# L3d3dy5taWNyb3NvZnQuY29tL3BraS9jZXJ0cy9NaWNSb29DZXJBdXRfMjAxMC0w
# Ni0yMy5jcnQwDQYJKoZIhvcNAQELBQADggIBAJ1VffwqreEsH2cBMSRb4Z5yS/yp
# b+pcFLY+TkdkeLEGk5c9MTO1OdfCcTY/2mRsfNB1OW27DzHkwo/7bNGhlBgi7ulm
# ZzpTTd2YurYeeNg2LpypglYAA7AFvonoaeC6Ce5732pvvinLbtg/SHUB2RjebYIM
# 9W0jVOR4U3UkV7ndn/OOPcbzaN9l9qRWqveVtihVJ9AkvUCgvxm2EhIRXT0n4ECW
# OKz3+SmJw7wXsFSFQrP8DJ6LGYnn8AtqgcKBGUIZUnWKNsIdw2FzLixre24/LAl4
# FOmRsqlb30mjdAy87JGA0j3mSj5mO0+7hvoyGtmW9I/2kQH2zsZ0/fZMcm8Qq3Uw
# xTSwethQ/gpY3UA8x1RtnWN0SCyxTkctwRQEcb9k+SS+c23Kjgm9swFXSVRk2XPX
# fx5bRAGOWhmRaw2fpCjcZxkoJLo4S5pu+yFUa2pFEUep8beuyOiJXk+d0tBMdrVX
# VAmxaQFEfnyhYWxz/gq77EFmPWn9y8FBSX5+k77L+DvktxW/tM4+pTFRhLy/AsGC
# onsXHRWJjXD+57XQKBqJC4822rpM+Zv/Cuk0+CQ1ZyvgDbjmjJnW4SLq8CdCPSWU
# 5nR0W2rRnj7tfqAxM328y+l7vzhwRNGQ8cirOoo6CGJ/2XBjU02N7oJtpQUQwXEG
# ahC0HVUzWLOhcGbyoYIDVjCCAj4CAQEwggEBoYHZpIHWMIHTMQswCQYDVQQGEwJV
# UzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UE
# ChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMS0wKwYDVQQLEyRNaWNyb3NvZnQgSXJl
# bGFuZCBPcGVyYXRpb25zIExpbWl0ZWQxJzAlBgNVBAsTHm5TaGllbGQgVFNTIEVT
# TjozMjFBLTA1RTAtRDk0NzElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAg
# U2VydmljZaIjCgEBMAcGBSsOAwIaAxUAtkQt/ebWSQ5DnG+aKRzPELCFE9GggYMw
# gYCkfjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UE
# BxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYD
# VQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDANBgkqhkiG9w0BAQsF
# AAIFAOqx/aswIhgPMjAyNDEwMTAwNzE1NTVaGA8yMDI0MTAxMTA3MTU1NVowdDA6
# BgorBgEEAYRZCgQBMSwwKjAKAgUA6rH9qwIBADAHAgEAAgIVGzAHAgEAAgIT3DAK
# AgUA6rNPKwIBADA2BgorBgEEAYRZCgQCMSgwJjAMBgorBgEEAYRZCgMCoAowCAIB
# AAIDB6EgoQowCAIBAAIDAYagMA0GCSqGSIb3DQEBCwUAA4IBAQCgKHh65v8QHms4
# gWjWKv2Z0d6fS4dK5xXZylqCqcRR1XtLzkkK5NGNOEzawXqD2LWfDTwWBPw9L3hZ
# M+W+Y8F5YF1hMMD4esNfLDfjOg3IMyBTcUUxwKzJVWAH+2IDlEt9TGsbB0aj2aXj
# xlprBehfbDJA0HraHrwMjffkkMWkLPHvvEemxYE1ZrWxWuNN6+vdz/pRJk9ScjuB
# c2vyMkMGMazUipNU/wpeXfJj/Ct3SK/SgL9/0ROR770xFFnzR4ndBboL0WfyHWLm
# SD2f7K+FUD9Wn85ZpqI49jl6kAv7PLlApYz7wCT79oVp9ltCuI5PJpRuULFmu2AX
# 7Nl8jpQdMYIEDTCCBAkCAQEwgZMwfDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldh
# c2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBD
# b3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUtU3RhbXAgUENBIDIw
# MTACEzMAAAH4o6EmDAxASP4AAQAAAfgwDQYJYIZIAWUDBAIBBQCgggFKMBoGCSqG
# SIb3DQEJAzENBgsqhkiG9w0BCRABBDAvBgkqhkiG9w0BCQQxIgQgZYWSPa6i6cXY
# dHWqvUacoZrvVjtA7+DuO3UDB6986g8wgfoGCyqGSIb3DQEJEAIvMYHqMIHnMIHk
# MIG9BCDvzDPyXw1UkAUFYt8bR4UdjM90Qv5xnVaiKD3I0Zz3WjCBmDCBgKR+MHwx
# CzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRt
# b25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMTHU1p
# Y3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwAhMzAAAB+KOhJgwMQEj+AAEAAAH4
# MCIEICNIk49lRTpS1KGaQQgbtjc2QLC6tMLSOuc8+79jH/zhMA0GCSqGSIb3DQEB
# CwUABIICAEqbz8+VmXSkmmJlm1np7n4uI7Y1BdgqMl3Q7WqX6/fQf3PT5V2DTzPV
# 5tB7dOXyvre3V2elDBREwH2M4kzC3iHHYZOP3pRza6lm+an9H4bLRtMrdCOmvflu
# 7HIHeCuCSwRcKKM78/PM9Ot2BwgbtiHs1Rl6xv74y4BUtxzupzclUWNXpLvk+jPJ
# cjX2jjmifAWjUgRxLxxJ141Xm75iOwBw+xNkjmoaVgg5e9jY3ET9qwLBzRxDp8we
# FLI8IIx5BsrL1B+byQI0liPSdilqHxdkDH8zwtnmHkRjyYN33JdJEcwdi1gD+4au
# w+kdJB3LPxxDjfa4yEbYVO4ITYWQIvQ+BDWV4lwdnPjjb2kdUC2Qyh0zFIdZyPOa
# pHAHkBdPsVMxtjFGoxtoaKawzHIgLI/xOSzJjyQ5zygvtwMkO3XZhVzJJRPSunyH
# 8/h8VDdRNJMr1XdmgA7yoXaVXeHBaxCtpUI5SivFPEcwkH4o+5XAQ8sEqxiF0j5b
# JgGDRiq9wd4jo7rB+cL3I1FIU2I2xICwZ6JIgF5GOUGZzYehN273ucrCcQy73i9M
# s4CgEBuVRcclipFVuFAMxrlvT/WdqrmQiNUUcNG6VFCameQm0i/lxPctYhrjwmr0
# fp2scvR9EmPI66VysTuE8DlxQUxyW1QwIx6IGIat3R8hlWdkDHfU
# SIG # End signature block
