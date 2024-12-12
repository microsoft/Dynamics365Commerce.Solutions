<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

    <xsl:output method="xml" indent="yes" />

    <xsl:key name="shifts-by-register-number" match="Shift" use="RegisterNumber"/>
    <xsl:key name="receipts-by-register-number" match="Receipt" use="RegisterNumber"/>
    <xsl:key name="receipt-copies-by-register-number" match="ReceiptCopy" use="RegisterNumber"/>
    <xsl:key name="audit-events-by-register-number" match="AuditEvent" use="RegisterNumber"/>

    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>


    <xsl:template match="Shifts">
        <xsl:apply-templates select="Shift[generate-id() = generate-id(key('shifts-by-register-number', RegisterNumber)[1])]" mode="group"/>
    </xsl:template>

    <xsl:template match="Receipts">
        <xsl:apply-templates select="Receipt[generate-id() = generate-id(key('receipts-by-register-number', RegisterNumber)[1])]" mode="group"/>
    </xsl:template>

    <xsl:template match="ReceiptCopies">
        <xsl:apply-templates select="ReceiptCopy[generate-id() = generate-id(key('receipt-copies-by-register-number', RegisterNumber)[1])]" mode="group"/>
    </xsl:template>

    <xsl:template match="AuditEvents">
        <xsl:apply-templates select="AuditEvent[generate-id() = generate-id(key('audit-events-by-register-number', RegisterNumber)[1])]" mode="group"/>
    </xsl:template>


    <xsl:template match="Shift" mode="group">
        <Shifts RegisterNumber="{RegisterNumber}">
            <xsl:apply-templates select="key('shifts-by-register-number', RegisterNumber)">
                <xsl:sort select="Date" data-type="number"/>
                <xsl:sort select="SequentialNumber" data-type="number"/>
            </xsl:apply-templates>
        </Shifts>
    </xsl:template>

    <xsl:template match="Receipt" mode="group">
        <Receipts RegisterNumber="{RegisterNumber}">
            <xsl:apply-templates select="key('receipts-by-register-number', RegisterNumber)">
                <xsl:sort select="Date" data-type="number"/>
                <xsl:sort select="SequentialNumber" data-type="number"/>
            </xsl:apply-templates>
        </Receipts>
    </xsl:template>

    <xsl:template match="ReceiptCopy" mode="group">
        <ReceiptCopies RegisterNumber="{RegisterNumber}">
            <xsl:apply-templates select="key('receipt-copies-by-register-number', RegisterNumber)">
                <xsl:sort select="Date" data-type="number"/>
                <xsl:sort select="SequentialNumber" data-type="number"/>
            </xsl:apply-templates>
        </ReceiptCopies>
    </xsl:template>

    <xsl:template match="AuditEvent" mode="group">
        <AuditEvents RegisterNumber="{RegisterNumber}">
            <xsl:apply-templates select="key('audit-events-by-register-number', RegisterNumber)">
                <xsl:sort select="Date" data-type="number"/>
                <xsl:sort select="SequentialNumber" data-type="number"/>
            </xsl:apply-templates>
        </AuditEvents>
    </xsl:template>


    <xsl:template match="Shift | Receipt | ReceiptCopy | AuditEvent">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()" />
        </xsl:copy>
    </xsl:template>

</xsl:stylesheet>
