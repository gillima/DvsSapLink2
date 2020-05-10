using DvsSapLink2.Helper;

namespace DvsSapLink2.Model
{
    /// <summary>
    /// List of all possible file attributes
    /// </summary>
    public enum FileAttributeName
    {
        // ab hier Werte aus dem AttributFile

        [Elo("Dokument-Nummer", 8)]
        Zeichnungsnummer,

        [Elo("Typ bzw. Reihe", 22)]
        Typ,

        [Elo("-Haupttitel", 0)]
        Haupttitel,

        [Elo("-Untertitel", 0)]
        Untertitel,

        [Elo("Kundenauftrag", 20)]
        AuftragsNummer,

        // Ersteller,
        // Prüfer1,
        // Prüfer2,
        // Freigeber,
        // SLgleicherNr,
        // SLandererNr,

        [Elo("Entstanden aus", 28)]
        EntstandAus,

        [Elo("Ersatz für (Vorg.)", 26)]
        ErsatzFuer,

        // Massstab,
        // ZustandStelle,
        // UebernehmendeStelle,
        // DokumentArt,

        [Elo("Sprache", 13)]
        Sprache,

        [Elo("Format", 24)]
        BlattFormat,

        [Elo("Blatt-Nummer", 11)]
        BlattNr,

        // AnzBlatt,
        // ToleranzMittel,
        // ToleranzGrob,

        [Elo("Revision", 10)]
        AeStand_aktuell,

        // AeStand_1,
        // AeStand_2,
        // AeStand_3,
        // AeStand_4,
        // AeStand_5,
        // AeStand_6,
        // AeStand_7,
        // Bemerkung,


        // ab hier Werte, die berechnet werden oder aus sapData stammen

        [Elo("Dateiname", 1)]
        Dateiname,

        [Elo("Dateiversion", 2)]
        Dateiversion,

        [Elo("SAP ID", 3)]
        SapID,

        [Elo("Unterordner1", 4)]
        Unterordner1,

        [Elo("Kurzbezeichnung", 5)]
        Kurzbezeichnung,

        [Elo("Datum", 6)]
        DokDatum,

        [Elo("Titel", 7)]
        Titel,

        [Elo("Status", 9)]
        DokStatus,

        [Elo("Dokument-Typ", 12)]
        DokTyp,

        [Elo("Fertigungsprozess", 14)]
        FertigungsProzess,

        [Elo("Stand Überarbeitung", 15)]
        StandUeberarbeitung,

        [Elo("Verteilung", 16)]
        Verteilung,

        [Elo("ATEX relevant", 17)]
        ATEX,

        [Elo("Auftragsstatus", 18)]
        Auftragsstatus,

        [Elo("Klassifizierung", 19)]
        Klassifizierung,

        [Elo("Projektname", 21)]
        Projektname,

        [Elo("CAD Applikation", 23)]
        CadApp,

        [Elo("Dokument-Inhalt", 25)]
        DokInhalt,

        [Elo("Ersetzt durch (Nachf.)", 27)]
        ErsetztDurch,

        [Elo("Einordnungs-Nr.", 29)]
        EinordnungsNr,

        [Elo("Erstellt / Geändert am", 30)]
        ErstelltDatum,

        [Elo("-Erstellt", 0)]
        ErstelltName,

        [Elo("Erstellt / Geändert von", 31)]
        ErstelltNameELO,

        [Elo("Geprüft am", 32)]
        GeprueftDatum,

        [Elo("-Geprüft", 0)]
        GeprueftName,

        [Elo("Geprüft von", 33)]
        GeprueftNameELO,

        [Elo("Freigegeben am", 34)]
        FreigegebenDatum,

        [Elo("Freigegeben", 0)]
        FreigegebenName,

        [Elo("Freigegeben von", 35)]
        FreigegebenNameELO,

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