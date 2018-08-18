<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	 <xsl:template match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>
    <xsl:template match="package/metadata/authors/text()">Pocket XAF</xsl:template>
    <xsl:template match="package/metadata/owners/text()">Pocket XAF</xsl:template>
    <xsl:template match="package/metadata/licenseUrl/text()">https://github.com/eXpandFramework/PocketXAF/blob/master/LICENSE</xsl:template>
    <xsl:template match="package/metadata/projectUrl/text()">https://github.com/eXpandFramework/PocketXAF</xsl:template>
    <xsl:template match="package/metadata/tags/text()">DevExpress XAF PocketXAF</xsl:template>
</xsl:stylesheet>
