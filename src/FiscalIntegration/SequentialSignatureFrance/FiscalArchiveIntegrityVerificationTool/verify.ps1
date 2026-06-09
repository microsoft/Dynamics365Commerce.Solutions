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
                    $valid = Verify-Data `
                        $recalculatedDataToSignBytes `
                        $node.Signature `
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
# MIIncQYJKoZIhvcNAQcCoIInYjCCJ14CAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCCOCdGI10BPOhR9
# VO7pR7ijNvw6WfwWEFI3M4lGIJ2HE6CCDMkwggYEMIID7KADAgECAhMzAAACHPrN
# xZvoL37EAAAAAAIcMA0GCSqGSIb3DQEBCwUAMFcxCzAJBgNVBAYTAlVTMR4wHAYD
# VQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBD
# b2RlIFNpZ25pbmcgUENBIDIwMjQwHhcNMjYwNDE2MTg1OTQxWhcNMjcwNDE1MTg1
# OTQxWjB0MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UE
# BxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMR4wHAYD
# VQQDExVNaWNyb3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IB
# DwAwggEKAoIBAQDVsZfgOKmM31HPfoWOoNEiw0SlCiIxUMC0I9NMWbucKOw/e9lP
# oAoehQVu6SG65V4EPzrYsnBnFPNoi4/HoOdjhz1qkrEt4I6tEcxXU6oOeY9zGveC
# /3iBeuhLYxM3M/PkcUoebF+Nednm8OkdSPoDu8imViHPQq/8CQUu0WRR4rE+dMRf
# rpVqfmNi2qWCX94T4MsepijGVkwE//tJg0ryAiYdHT34LSnlG/RSBZmQRGWZ5g8j
# qnKjRParSqMft1gvjuUTVgtWNZfgcLFSK5Wa0myrq8OPcgTGGsRgun+tnSS+IxDT
# xVsAPH1OzvPjwomguByhUe/OcvUN0D5Wmp7xAgMBAAGjggGqMIIBpjAOBgNVHQ8B
# Af8EBAMCB4AwHwYDVR0lBBgwFgYKKwYBBAGCN0wIAQYIKwYBBQUHAwMwHQYDVR0O
# BBYEFNoH7a2YDjOSwpkp6DHcmUS7J+0yMFQGA1UdEQRNMEukSTBHMS0wKwYDVQQL
# EyRNaWNyb3NvZnQgSXJlbGFuZCBPcGVyYXRpb25zIExpbWl0ZWQxFjAUBgNVBAUT
# DTIzMDAxMis1MDc1NjkwHwYDVR0jBBgwFoAUf1k/VCHarU/vBeXmo9ctBpQSCDEw
# YAYDVR0fBFkwVzBVoFOgUYZPaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9w
# cy9jcmwvTWljcm9zb2Z0JTIwQ29kZSUyMFNpZ25pbmclMjBQQ0ElMjAyMDI0LmNy
# bDBtBggrBgEFBQcBAQRhMF8wXQYIKwYBBQUHMAKGUWh0dHA6Ly93d3cubWljcm9z
# b2Z0LmNvbS9wa2lvcHMvY2VydHMvTWljcm9zb2Z0JTIwQ29kZSUyMFNpZ25pbmcl
# MjBQQ0ElMjAyMDI0LmNydDAMBgNVHRMBAf8EAjAAMA0GCSqGSIb3DQEBCwUAA4IC
# AQAUnEqhaRXe0T3hIJjvdQErEkrA/7bByjn6t5IArODkkRjzkYwtKMc2yYj2quaN
# rLutWw2YZcngKPy1b71YyDJQTy4NDRwaSh9Tw5thrk3NmcPrAHia5vtcBJ1CgtKK
# 7mQbIcQ22d/N3813ayCDDFewu1+jsZmX+r/aTEqaOM4TVxVtRSkuCy8nAXKuChOK
# Li/zA4XuH8iEYqIsj2YoNaeSxVmeGiERXpKdo3dDmYi0kO5w2D8VS4c3+9h6gElY
# BaAAg/dYErBg27qT3vv0zRDJhJufvCNylA8S7/+8H5E/PV5cng6na9VV/w9OV3qu
# uND6zdGa2EX38Glp50F9AIQk3p2xXmcvorDeM4XJ7UlWYBi6g80J1SSOQnInCYFE
# msfUNn3+1AaTJKSJL83quKArTac2pKhu0Yzzzrzo6HrsRiQKzpnRBb1/dMa6P3hz
# 75XbMRBctNsFhZC07WCmjExdLg2eHW5uV0TY8D5+6wozJf7vF3+WHkYPO85Z+BC6
# U4FkNbYNycZ9cE4j1tXRdyDCfml6c0HWPHjNVDObrv9lKt3qUqFpX38VCqVCyNOO
# 1UcXfQiVjJw32U2WUKZjt/neJKHEBsm9kFsLuWzkQ53+qcaSaytmsCnk2gOglrlD
# 5d3kKyvvAw+rzm0lT8K38P6PLxfZQHhu4W8dV7Av8N2ZmDCCBr0wggSloAMCAQIC
# EzMAAAA5O7Y3Gb8GHWcAAAAAADkwDQYJKoZIhvcNAQEMBQAwgYgxCzAJBgNVBAYT
# AlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYD
# VQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xMjAwBgNVBAMTKU1pY3Jvc29mdCBS
# b290IENlcnRpZmljYXRlIEF1dGhvcml0eSAyMDExMB4XDTI0MDgwODIwNTQxOFoX
# DTM2MDMyMjIyMTMwNFowVzELMAkGA1UEBhMCVVMxHjAcBgNVBAoTFU1pY3Jvc29m
# dCBDb3Jwb3JhdGlvbjEoMCYGA1UEAxMfTWljcm9zb2Z0IENvZGUgU2lnbmluZyBQ
# Q0EgMjAyNDCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBANgBnB7jOMeq
# lRYHNa265v4IY9fH8TKhemHfPINe1gpLaV3dhg324WwH06LcHbpnsBukCDNitryo
# 0dtS/EW6I/yEL/bLSY8hKpbfQuWusBPr9qazYcDxCW/qnjb5JsI1s8bNOg3bVATv
# QVL4tcf03aTycsz8QeCdM0l/yHRObJ9QqazM1r6VPEOJ7LL+uEEb73w6QCuhs89a
# 1uv1zerOYMnsneRRwCbpyW11IcggU0cRKDDq1pjVJzIbIF6+oiXXbReOsgeI8zu1
# FyQfK0fVkaya8SmVHQ/tOf23mZ4W9k0Ri22QW9p3UgSC5OUDktKxxcCmGL6tXLfO
# GSWHIIV4YrTJTT6PNty5REojHJuZHArkF9VnHTERWoTjAzfI3kP+5b4alUdhgAZ7
# ttOu1bVnXfHaqPYl2rPs20ji03LOVWsh/radgE17es5hL+t6lV0eVHrVhsssROWJ
# uz2MXMCt7iw7lFPG9LXKGjsmonn2gotGdHIuEg5JnJMJVmixd5LRlkmgYRZKzhxS
# CwyoGIq0PhaA7Y+VPct5pCHkijcIIDm0nlkK+0KyepolcqGm0T/GYQRMhHJlGOOm
# VQop36wUVUYklUy++vDWeEgEo4s7hxN6mIbf2MSIQ/iIfMZgJxC69oukMUXCrOC3
# SkE/xIkgpfl22MM1itkZ35nNXkMolU1lAgMBAAGjggFOMIIBSjAOBgNVHQ8BAf8E
# BAMCAYYwEAYJKwYBBAGCNxUBBAMCAQAwHQYDVR0OBBYEFH9ZP1Qh2q1P7wXl5qPX
# LQaUEggxMBkGCSsGAQQBgjcUAgQMHgoAUwB1AGIAQwBBMA8GA1UdEwEB/wQFMAMB
# Af8wHwYDVR0jBBgwFoAUci06AjGQQ7kUBU7h6qfHMdEjiTQwWgYDVR0fBFMwUTBP
# oE2gS4ZJaHR0cDovL2NybC5taWNyb3NvZnQuY29tL3BraS9jcmwvcHJvZHVjdHMv
# TWljUm9vQ2VyQXV0MjAxMV8yMDExXzAzXzIyLmNybDBeBggrBgEFBQcBAQRSMFAw
# TgYIKwYBBQUHMAKGQmh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMv
# TWljUm9vQ2VyQXV0MjAxMV8yMDExXzAzXzIyLmNydDANBgkqhkiG9w0BAQwFAAOC
# AgEAFJQfOChP7onn6fLIMKrSlN1WYKwDFgAddymOUO3FrM8d7B/W/iQ6DxXsDn7D
# 5W4wMwYeLystcEqfkjz4NURRgazyMu5yRzQh4LqjA4tStTcJh1opExo7nn5PuPBY
# nbu0+THSuVHTe0VTTPVhily/piFrDo3axQ9P4C+Ol5yet+2gTfekICS5xS+cYfSI
# vgn0JksVBVMYVI5QFu/qhnLhsEFEUzG8fvv0hjgkO+lkpV9ty6GkN4vdnd7ya6Q6
# aR9y34aiM1qmxaxBi6OUnyNl6fkuun/diTFnYDLTppOkr/mg5WSfCiDVMNCxtj4w
# PKC5OmHm1DQIt/MNokbbH3UGsFP1QbzsLocuSqLCvH09Io3fDPTmscR9Y75G4qX7
# RTX8AdBPo0I6OEojf39zuFZt0qOHm65YWQE69cZM2ueE1MB05dNNgHK9gTE7zKvK
# /fg8B2qjW88MT/WF5V5uvZGtqa9FSL2RazArA+rDPuf6JGYz4HpgMZHB4S6szWSK
# YBv0VisCzfxgeU+dquXW9bd0auYlOB58DPcOYKdc3Se94g+xL4pcEhbB54JOgAkw
# YTu/9dLeH2pDqeJZAABVDWRQCaXfO5LgyKwKCLYXpigrZYCjUSBcr+Ve8PFWMhVT
# Ql0v4q8J/AUmQN5W4n101cY2L4A7GTQG1h32HHAvfQESWP0xghn+MIIZ+gIBATBu
# MFcxCzAJBgNVBAYTAlVTMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24x
# KDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25pbmcgUENBIDIwMjQCEzMAAAIc
# +s3Fm+gvfsQAAAAAAhwwDQYJYIZIAWUDBAIBBQCgga4wGQYJKoZIhvcNAQkDMQwG
# CisGAQQBgjcCAQQwHAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwLwYJKoZI
# hvcNAQkEMSIEIHnVPZTIssygv/lyrp5dAweFdMDJR7ent1MXm/528v4iMEIGCisG
# AQQBgjcCAQwxNDAyoBSAEgBNAGkAYwByAG8AcwBvAGYAdKEagBhodHRwOi8vd3d3
# Lm1pY3Jvc29mdC5jb20wDQYJKoZIhvcNAQEBBQAEggEAvIcLiKuaAAr+KQbRI70a
# ydR5h+hcgdbiREM/qlB+WXrykmVpAERBWJ76xfdAA5UzBsQPa7RgUktC7F1Ibppx
# 1QT4uSLvSDI+T9YjW9QYig6ZKQEsK2zYONI7ew7sFAp5oKPVWGCtEKIqnSGcDk72
# psxDxwEvIO70OG0WrPr3WawLWnAgnJjOq83SN+vRFOVKYGiRi662Mj4jVAEoTPJd
# 40tUoNGfXKVRcKKVMaRMOhc82XxEgeb1MlfpHlLVVdN2sTPexH2E2iLU/YZOMJEu
# FAw77wTC4f+q46ymHxWtWRY8W3cQ52GN0Kyns1l7vw/0gY3UtFOpAj3S5w/jav6T
# BaGCF7AwghesBgorBgEEAYI3AwMBMYIXnDCCF5gGCSqGSIb3DQEHAqCCF4kwgheF
# AgEDMQ8wDQYJYIZIAWUDBAIBBQAwggFaBgsqhkiG9w0BCRABBKCCAUkEggFFMIIB
# QQIBAQYKKwYBBAGEWQoDATAxMA0GCWCGSAFlAwQCAQUABCDEECvuRrMJEQ46Dvxn
# JSxkM4zzQEoIc947Ab69uYFWPgIGahB2EwSyGBMyMDI2MDYwOTEwMzAwMy42MDda
# MASAAgH0oIHZpIHWMIHTMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3Rv
# bjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0
# aW9uMS0wKwYDVQQLEyRNaWNyb3NvZnQgSXJlbGFuZCBPcGVyYXRpb25zIExpbWl0
# ZWQxJzAlBgNVBAsTHm5TaGllbGQgVFNTIEVTTjozMjFBLTA1RTAtRDk0NzElMCMG
# A1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2VydmljZaCCEf4wggcoMIIFEKAD
# AgECAhMzAAACGqmgHQagD0OqAAEAAAIaMA0GCSqGSIb3DQEBCwUAMHwxCzAJBgNV
# BAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4w
# HAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29m
# dCBUaW1lLVN0YW1wIFBDQSAyMDEwMB4XDTI1MDgxNDE4NDgyOFoXDTI2MTExMzE4
# NDgyOFowgdMxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYD
# VQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xLTAr
# BgNVBAsTJE1pY3Jvc29mdCBJcmVsYW5kIE9wZXJhdGlvbnMgTGltaXRlZDEnMCUG
# A1UECxMeblNoaWVsZCBUU1MgRVNOOjMyMUEtMDVFMC1EOTQ3MSUwIwYDVQQDExxN
# aWNyb3NvZnQgVGltZS1TdGFtcCBTZXJ2aWNlMIICIjANBgkqhkiG9w0BAQEFAAOC
# Ag8AMIICCgKCAgEAmYEAwSTz79q2V3ZWzQ5Ev7RKgadQtMBy7+V3XQ8R0NL8R9mu
# pxcqJQ/KPeZGJTER+9Qq/t7HOQfBbDy6e0TepvBFV/RY3w+LOPMKn0Uoh2/8IvdS
# bJ8qAWRVoz2S9VrJzZpB8/f5rQcRETgX/t8N66D2JlEXv4fZQB7XzcJMXr1puhuX
# bOt9RYEyN1Q3Z7YjRkhfBsRc+SD/C9F4iwZqfQgo82GG4wguIhjJU7+XMfrv4vxA
# FNVg3mn1PoMWGZWio+e14+PGYPVLKlad+0IhdHK5AgPyXKkqAhEZpYhYYVEItHOO
# vqrwukxVAJXMvWA3GatWkRZn33WDJVtghCW6XPLi1cDKiGE5UcXZSV4OjQIUB8vp
# 2LUMRXud5I49FIBcE9nT00z8A+EekrPM+OAk07aDfwZbdmZ56j7ub5fNDLf8yIb8
# QxZ8Mr4RwWy/czBuV5rkWQQ+msjJ5AKtYZxJdnaZehUgUNArU/u36SH1eXKMQGRX
# r/xeKFGI8vvv5Jl1knZ8UqEQr9PxDbis7OXp2WSMK5lLGdYVH8VownYF3sbOiRkx
# 5Q5GaEyTehOQp2SfdbsJZlg0SXmHphGnoW1/gQ/5P6BgSq4PAWIZaDJj6AvLLCdb
# URgR5apNQQed2zYUgUbjACA/TomA8Ll7Arrv2oZGiUO5Vdi4xxtA3BRTQTUCAwEA
# AaOCAUkwggFFMB0GA1UdDgQWBBTwqyIJ3QMoPasDcGdGovbaY8IlNjAfBgNVHSME
# GDAWgBSfpxVdAF5iXYP05dJlpxtTNRnpcjBfBgNVHR8EWDBWMFSgUqBQhk5odHRw
# Oi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpb3BzL2NybC9NaWNyb3NvZnQlMjBUaW1l
# LVN0YW1wJTIwUENBJTIwMjAxMCgxKS5jcmwwbAYIKwYBBQUHAQEEYDBeMFwGCCsG
# AQUFBzAChlBodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpb3BzL2NlcnRzL01p
# Y3Jvc29mdCUyMFRpbWUtU3RhbXAlMjBQQ0ElMjAyMDEwKDEpLmNydDAMBgNVHRMB
# Af8EAjAAMBYGA1UdJQEB/wQMMAoGCCsGAQUFBwMIMA4GA1UdDwEB/wQEAwIHgDAN
# BgkqhkiG9w0BAQsFAAOCAgEA1a72WFq7B6bJT3VOJ21nnToPJ9O/q51bw1bhPfQy
# 67uy+f8x8akipzNL2k5b6mtxuPbZGpBqpBKguDwQmxVpX8cGmafeo3wGr4a8Yk6S
# y09tEh/Nwwlsyq7BRrJNn6bGOB8iG4OTy+pmMUh7FejNPRgvgeo/OPytm4NNrMMg
# 98UVlrZxGNOYsifpRJFg5jE/Yu6lqFa1lTm9cHuPYxWa2oEwC0sEAsTFb69iKpN0
# sO19xBZCr0h5ClU9Pgo6ekiJb7QJoDzrDoPQHwbNA87Cto7TLuphj0m9l/I70gLj
# Eq53SHjuURzwpmNxdm18Qg+rlkaMC6Y2KukOfJ7oCSu9vcNGQM+inl9gsNgirZ6y
# Jk9VsXEsoTtoR7fMNU6Py6ufJQGMTmq6ZCq2eIGOXWMBb79ZF6tiKTa4qami3US0
# mTY41J129XmAglVy+ujSZkHu2lHJDRHs7FjnIXZVUE5pl6yUIl23jG50fRTLQcSt
# dwY/LvJUgEHCIzjvlLTqLt6JVR5bcs5aN4Dh0YPG95B9iDMZrq4rli5SnGNWev5L
# LsDY1fbrK6uVpD+psvSLsNpht27QcHRsYdAMALXM+HNsz2LZ8xiOfwt6rOsVWXoi
# HV86/TeMy5TZFUl7qB59INoMSJgDRladVXeT9fwOuirFIoqgjKGk3vO2bELrYMN0
# QVwwggdxMIIFWaADAgECAhMzAAAAFcXna54Cm0mZAAAAAAAVMA0GCSqGSIb3DQEB
# CwUAMIGIMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UE
# BxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMTIwMAYD
# VQQDEylNaWNyb3NvZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHkgMjAxMDAe
# Fw0yMTA5MzAxODIyMjVaFw0zMDA5MzAxODMyMjVaMHwxCzAJBgNVBAYTAlVTMRMw
# EQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVN
# aWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0
# YW1wIFBDQSAyMDEwMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA5OGm
# TOe0ciELeaLL1yR5vQ7VgtP97pwHB9KpbE51yMo1V/YBf2xK4OK9uT4XYDP/XE/H
# ZveVU3Fa4n5KWv64NmeFRiMMtY0Tz3cywBAY6GB9alKDRLemjkZrBxTzxXb1hlDc
# wUTIcVxRMTegCjhuje3XD9gmU3w5YQJ6xKr9cmmvHaus9ja+NSZk2pg7uhp7M62A
# W36MEBydUv626GIl3GoPz130/o5Tz9bshVZN7928jaTjkY+yOSxRnOlwaQ3KNi1w
# jjHINSi947SHJMPgyY9+tVSP3PoFVZhtaDuaRr3tpK56KTesy+uDRedGbsoy1cCG
# MFxPLOJiss254o2I5JasAUq7vnGpF1tnYN74kpEeHT39IM9zfUGaRnXNxF803RKJ
# 1v2lIH1+/NmeRd+2ci/bfV+AutuqfjbsNkz2K26oElHovwUDo9Fzpk03dJQcNIIP
# 8BDyt0cY7afomXw/TNuvXsLz1dhzPUNOwTM5TI4CvEJoLhDqhFFG4tG9ahhaYQFz
# ymeiXtcodgLiMxhy16cg8ML6EgrXY28MyTZki1ugpoMhXV8wdJGUlNi5UPkLiWHz
# NgY1GIRH29wb0f2y1BzFa/ZcUlFdEtsluq9QBXpsxREdcu+N+VLEhReTwDwV2xo3
# xwgVGD94q0W29R6HXtqPnhZyacaue7e3PmriLq0CAwEAAaOCAd0wggHZMBIGCSsG
# AQQBgjcVAQQFAgMBAAEwIwYJKwYBBAGCNxUCBBYEFCqnUv5kxJq+gpE8RjUpzxD/
# LwTuMB0GA1UdDgQWBBSfpxVdAF5iXYP05dJlpxtTNRnpcjBcBgNVHSAEVTBTMFEG
# DCsGAQQBgjdMg30BATBBMD8GCCsGAQUFBwIBFjNodHRwOi8vd3d3Lm1pY3Jvc29m
# dC5jb20vcGtpb3BzL0RvY3MvUmVwb3NpdG9yeS5odG0wEwYDVR0lBAwwCgYIKwYB
# BQUHAwgwGQYJKwYBBAGCNxQCBAweCgBTAHUAYgBDAEEwCwYDVR0PBAQDAgGGMA8G
# A1UdEwEB/wQFMAMBAf8wHwYDVR0jBBgwFoAU1fZWy4/oolxiaNE9lJBb186aGMQw
# VgYDVR0fBE8wTTBLoEmgR4ZFaHR0cDovL2NybC5taWNyb3NvZnQuY29tL3BraS9j
# cmwvcHJvZHVjdHMvTWljUm9vQ2VyQXV0XzIwMTAtMDYtMjMuY3JsMFoGCCsGAQUF
# BwEBBE4wTDBKBggrBgEFBQcwAoY+aHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3Br
# aS9jZXJ0cy9NaWNSb29DZXJBdXRfMjAxMC0wNi0yMy5jcnQwDQYJKoZIhvcNAQEL
# BQADggIBAJ1VffwqreEsH2cBMSRb4Z5yS/ypb+pcFLY+TkdkeLEGk5c9MTO1OdfC
# cTY/2mRsfNB1OW27DzHkwo/7bNGhlBgi7ulmZzpTTd2YurYeeNg2LpypglYAA7AF
# vonoaeC6Ce5732pvvinLbtg/SHUB2RjebYIM9W0jVOR4U3UkV7ndn/OOPcbzaN9l
# 9qRWqveVtihVJ9AkvUCgvxm2EhIRXT0n4ECWOKz3+SmJw7wXsFSFQrP8DJ6LGYnn
# 8AtqgcKBGUIZUnWKNsIdw2FzLixre24/LAl4FOmRsqlb30mjdAy87JGA0j3mSj5m
# O0+7hvoyGtmW9I/2kQH2zsZ0/fZMcm8Qq3UwxTSwethQ/gpY3UA8x1RtnWN0SCyx
# TkctwRQEcb9k+SS+c23Kjgm9swFXSVRk2XPXfx5bRAGOWhmRaw2fpCjcZxkoJLo4
# S5pu+yFUa2pFEUep8beuyOiJXk+d0tBMdrVXVAmxaQFEfnyhYWxz/gq77EFmPWn9
# y8FBSX5+k77L+DvktxW/tM4+pTFRhLy/AsGConsXHRWJjXD+57XQKBqJC4822rpM
# +Zv/Cuk0+CQ1ZyvgDbjmjJnW4SLq8CdCPSWU5nR0W2rRnj7tfqAxM328y+l7vzhw
# RNGQ8cirOoo6CGJ/2XBjU02N7oJtpQUQwXEGahC0HVUzWLOhcGbyoYIDWTCCAkEC
# AQEwggEBoYHZpIHWMIHTMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3Rv
# bjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0
# aW9uMS0wKwYDVQQLEyRNaWNyb3NvZnQgSXJlbGFuZCBPcGVyYXRpb25zIExpbWl0
# ZWQxJzAlBgNVBAsTHm5TaGllbGQgVFNTIEVTTjozMjFBLTA1RTAtRDk0NzElMCMG
# A1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2VydmljZaIjCgEBMAcGBSsOAwIa
# AxUA8YrutmKpSrubCaAYsU4pt1Ft8DaggYMwgYCkfjB8MQswCQYDVQQGEwJVUzET
# MBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMV
# TWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGltZS1T
# dGFtcCBQQ0EgMjAxMDANBgkqhkiG9w0BAQsFAAIFAO3SBUUwIhgPMjAyNjA2MDkw
# MzIxNDFaGA8yMDI2MDYxMDAzMjE0MVowdzA9BgorBgEEAYRZCgQBMS8wLTAKAgUA
# 7dIFRQIBADAKAgEAAgIu0AIB/zAHAgEAAgISpTAKAgUA7dNWxQIBADA2BgorBgEE
# AYRZCgQCMSgwJjAMBgorBgEEAYRZCgMCoAowCAIBAAIDB6EgoQowCAIBAAIDAYag
# MA0GCSqGSIb3DQEBCwUAA4IBAQCmitL1Lb7mQ2B1klroK/5wzWjBamcugVsi2BcP
# 7wWgGul691vDF77bPPulvKL//RYLcSK0WC0G49Nx/fJJ1DGxi7J26RBCxhNM8YSV
# YMBJnwHaoYtYoS5fs5y1C6afDpIVwlss2suI1roRwC80KPwWeBCSWvd1kr3WLKvI
# YufECK0vJGJI9C9wrV6coKOgTbgEuY9HmI2nOmKiYQ3DZikEukWkbCO37+/qWxNI
# afM39SR9B7KbP1x3qFDtp4kiWYocL6kgOIxtcE5BRA29Utb43CzMXXfSF5DNpZHm
# qbPzZlJq7wEAziTCV5q53wCM9/60WLpKKU1hS92qPmLBFrM0MYIEDTCCBAkCAQEw
# gZMwfDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcT
# B1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UE
# AxMdTWljcm9zb2Z0IFRpbWUtU3RhbXAgUENBIDIwMTACEzMAAAIaqaAdBqAPQ6oA
# AQAAAhowDQYJYIZIAWUDBAIBBQCgggFKMBoGCSqGSIb3DQEJAzENBgsqhkiG9w0B
# CRABBDAvBgkqhkiG9w0BCQQxIgQgC+/SLyyngFXEYOyauQvqjj1kC2IUWTDGxKAS
# /IF+CY0wgfoGCyqGSIb3DQEJEAIvMYHqMIHnMIHkMIG9BCCdeiHHrbtpKcwB20do
# VU89WHIOH8S7w37uaHcDmemK+zCBmDCBgKR+MHwxCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1w
# IFBDQSAyMDEwAhMzAAACGqmgHQagD0OqAAEAAAIaMCIEINnpPC7N6lgBAY6kGtPv
# TIE9vJGsILbO1qP5YKd+5LPbMA0GCSqGSIb3DQEBCwUABIICAHRRidKditMxg1QF
# /DxaVEDZeq+QZbmtg2BSnvZIgwcRaA8SlaXzHEDnceX5Ek4k9dher350M3BHRA9d
# ffomhGMIBX9nQsEQdf7ocU7iyeQeUVWDN0oRnYiroldupD4rUbHfP25RDOlCvJNP
# uQifXmjq6f4IdAUY9JxlIHxaMDxGaUq9OR/1dJQnb0TpqGxkMlQXNkTIxGOcngID
# qcRPn4fxzsNbp37/MJ6lbXdrZRw5nslFcNiDbJttSsN4kxbnB5OtUK/zkLs82JSQ
# ohSOzx5rP4aLUKBxu20m0Wx1SF8M+thq+T5BfBLLeRhuPzMk+VY/C7osYrpqRjit
# YPEoYfTMjsrNcyzjLyKvTmJOX898WesMIvsBqopo3JRwFv5LeMiHBCp4zneJBcKx
# lj5h0shMQ1Zr9ABhV5gmu0I79WeYI5f9s7uNNwtL6sb1JtoQ/AN2w1QpqFd3mDwh
# Ce8jdk858Y4mN/+RrjiSG8rYW5OeFB/N2Cdx+CB3UrtdwMybv2pYqHXiOerIBgc9
# wl8vHq2ROQtPlOQRyqnkKxPUNgrfxHcNqJtdM2Uhnl7JgqJ1q6xtMUR4j8nFCQkL
# 4zcsKoodIKnadskqsj/VJMsGZrod/I0hglTnSA2XaO7Jh92sfuqrVbF47DHPABjL
# Z3v8WNns/P8JDUAIZAFM0tQ77ip8
# SIG # End signature block
