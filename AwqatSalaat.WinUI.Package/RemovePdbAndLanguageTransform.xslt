<?xml version="1.0" encoding="utf-8"?>
<!--
    Based on AwqatSalaat.Package\RemovePdbTransform.xslt.
    In addition to removing .pdb components, this transform strips the @Language
    attribute that heat auto-assigns to harvested files.

    The self-contained WinUI publish bundles the Windows App SDK, which ships
    localized satellite files (e.g. Microsoft.ui.xaml.dll.mui). heat reads a
    language from those files that is not a valid Windows Installer LCID, which
    makes ICE03 ("Invalid Language Id") fail the build. Removing @Language makes
    the files language-neutral (which is correct for a plain file-copy install)
    and lets validation pass.
-->
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://wixtoolset.org/schemas/v4/wxs"
    xmlns="http://schemas.microsoft.com/wix/2006/wi"

    version="1.0"
    exclude-result-prefixes="xsl wix"
>

    <xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

    <xsl:strip-space elements="*" />

    <!-- Match .pdb components by the last 4 characters of the Source path (XSLT 1.0, no ends-with). -->
    <xsl:key
        name="PdbToRemove"
        match="wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 3 ) = '.pdb' ]"
        use="@Id"
    />

    <!-- By default, copy all elements and nodes into the output... -->
    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()" />
        </xsl:copy>
    </xsl:template>

    <!-- ...but drop .pdb components and their references... -->
    <xsl:template match="*[ self::wix:Component or self::wix:ComponentRef ][ key( 'PdbToRemove', @Id ) ]" />

    <!-- ...and drop the auto-assigned @Language on harvested files (fixes ICE03). -->
    <xsl:template match="wix:File/@Language" />

</xsl:stylesheet>
