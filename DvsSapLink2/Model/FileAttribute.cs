namespace DvsSapLink2.Model
{
    /// <summary>
    /// List of all possible file attributes
    /// </summary>
    public enum FileAttributeName
    {
        ZeichnungsNummer,
        Typ,
        Haupttitel,
        Untertitel,
        AuftragsNummer,
        Ersteller,
        Prüfer1,
        Prüfer2,
        Freigeber,
        SLgleicherNr,
        SLandererNr,
        EntstandAus,
        ErsatzFuer,
        Massstab,
        ZustandStelle,
        UebernehmendeStelle,
        DokumentArt,
        Sprache,
        Format,
        BlattNr,
        AnzBlatt,
        ToleranzMittel,
        ToleranzGrob,
        AeStand_aktuell,
        AeStand_1,
        AeStand_2,
        AeStand_3,
        AeStand_4,
        AeStand_5,
        AeStand_6,
        AeStand_7,
        Bemerkung,
    }

    public class FileAttribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FileAttribute"/> class
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="rawValue">Raw value of the attribute</param>
        /// <param name="value">Processed value of the attribute</param>
        public FileAttribute(FileAttributeName name, string rawValue, string value)
        {
            this.Name = name;
            this.RawValue = rawValue;
            if (name == FileAttributeName.Haupttitel || name == FileAttributeName.Untertitel)
            {
                value = value
                    .Replace("%%c", "Ø")
                    .Replace("%%C", "Ø");
            }
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the attribute
        /// </summary>
        public FileAttributeName Name { get; }

        /// <summary>
        /// Gets the original value of the attribute
        /// </summary>
        public string RawValue { get; }

        /// <summary>
        /// Gets the processed value of the attribute
        /// </summary>
        public string Value { get; }
    }
}