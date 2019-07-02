using System;
using System.IO;
using System.Linq;
using System.Windows;
using DvsSapLink2.Helper;
using DvsSapLink2.Model;
using DvsSapLink2.ViewModel;
using static DvsSapLink2.Resources.Strings;

namespace DvsSapLink2.Command
{
    public class ArchiveCommand : CopyCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveCommand"/> class.
        /// </summary>
        public ArchiveCommand(Configuration configuration)
            : base(configuration, TXT_DO_ARCHIVE)
        {
        }

        /// <summary>
        /// Executes the prepare archive command
        /// </summary>
        /// <param name="parameter"><see cref="AttributeFile"/> to be copied</param>
        public override void Execute(object parameter)
        {
            var viewModel = (MainViewModel)parameter;
            var file = viewModel.File.File;

            var timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");
            var logFile = Path.Combine(this.configuration.LogDirectory, file.Title + ".log");
            var sapTransferFileTemp = Path.Combine(this.configuration.SourceDirectory, file.Title + $"{timeStamp}.dat");
            var archiveDir = LogParser.ReadMessages(logFile, "A_DIR");
            //MessageBox.Show($"Archive Directory: {archiveDir.FirstOrDefault()}");

            using (var logger = new Logger(logFile))
            {
                using (var stream = new StreamWriter(sapTransferFileTemp))
                {
                    this.WriteFileAttributes(stream, file, viewModel.Sap.Data);
                    this.WriteDocumentInfo(stream, file, "DOKTYP_ZAB_D", "mechanische Zeichnung");
                    this.WriteDocumentInfo(stream, file, "PROJ_D", file[FileAttributeName.Typ]);
                    this.WriteDocumentInfo(stream, file, "AUFTR_NR_D", file[FileAttributeName.AuftragsNummer]);
                    this.WriteDocumentInfo(stream, file, "FORMAT_D", file[FileAttributeName.Format]);
                }

                //TODO: Text durch Textkonstante ersetzen
                logger.Write("LOG", "Transferdaten für SAP erstellt");

                //TODO: wieder aktivieren, da zum Testen vom log-File auskommentiert
                this.CopyFile(file, ".dwg", this.configuration.DestinationDirectory);
                this.CopyFile(file, ".dwg", this.configuration.ConversionDirectory);
                this.CopyFile(file, ".txt", this.configuration.TxtDirectory);
                this.CopyFile(file, $"{timeStamp}.dat", this.configuration.SapTransferDirectory);
                this.CopyFile(file, $"{timeStamp}.dat", this.configuration.ArchiveSapTransferDirectory);

                this.DeleteFile(file, ".pdf");
                this.DeleteFile(file, $"{timeStamp}.dat");
                this.DeleteFile(file, ".txt");
                this.DeleteFile(file, ".dwg");

                //TODO: Text durch Textkonstante ersetzen
                logger.Write("LOG", "Zeichnung archiviert");
            }

            MessageBox.Show(TXT_FILE_ARCHIVED, this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            
            // HACK: force update of file list
            viewModel.Configuration.SourceDirectory = viewModel.Configuration.SourceDirectory;
        }

        /// <summary>
        /// Writes a log entry
        /// </summary>
        /// <param name="stream">Stream of the file to write</param>
        /// <param name="file">Attribute file to write into the file</param>
        /// <param name="sapData">SAP data to write into the file</param>
        private void WriteFileAttributes(StreamWriter stream, AttributeFile file, SapData sapData)
        {
            var A = "DOK01";                                                    //   1 Recordart
            var B = file[FileAttributeName.ZeichnungsNummer];                   //   7 Dokument-Nummer          HTAM123456
            var C = "ZAB";                                                      //  32 Dokument-Art             ZAB
            var D = file[FileAttributeName.AeStand_aktuell];                    //  35 Dokument-Version         A
            var E = $"D{int.Parse(file[FileAttributeName.BlattNr]):D2}";        //  37 Teildokument             D01
            var F = "D";                                                        //  40 Sprache                  D
            var G = " ";                                                        //  41 Änderungs-Nr.
            var H = file[FileAttributeName.Haupttitel];                         //  53 Titel                    Massbild
            var I = "CAD-DVS-Update";                                           // 308 Änderungsbeschreib.      CAD-DVS-Update
            var J = sapData.State;                                              // 378 Dokument-Status          DR
            var K = sapData.User;                                               // 380 Sachbearbeiter           221226
            var L = sapData.Labor.ToString();                                   // 392 Labor/Büro               760
            var M = "ACD";                                                      // 395 Datei 1                  ACD
            var N = "";                                                         // 398 Datenträger 1
            var O = "";                                                         // 408 File-Name 1
            var P = "PDF";                                                      // 478 Datei 2                  PDF         (TIF)
            var Q = "IM_PRE_V";                                                 // 481 Datenträger 2            IM_PRE_V    (IM_CAD_V)
            var R = file.Title + ".tif";                                        // 491 File-Name 2              HTAM123456-0-01.tif
            var S = "LABOR/BÜRO";                                               // 586 Bezugsort SAP            LABOR/BÜRO

            var line = $"{A,-6}{B,-25}{C,-3}{D,-2}{E,-3}{F,-1}{G,-12}{H,-255}{I,-70}{J,-2}{K,-12}{L,-3}{M,-3}{N,-10}{O,-70}{P,-3}{Q,-10}{R,-95}{S,-14}";
            stream.WriteLine(line);
        }

        /// <summary>
        /// Write document information as fix width attributes into the file
        /// </summary>
        private void WriteDocumentInfo(StreamWriter stream, AttributeFile file, string type, string changeNr)
        {
            if (string.IsNullOrWhiteSpace(type)) return;

            var record = "DOK02";
            var number = file[FileAttributeName.ZeichnungsNummer];
            var docType = "ZAB";
            var version = file[FileAttributeName.AeStand_aktuell];
            var page = $"D{int.Parse(file[FileAttributeName.BlattNr]):D2}";

            stream.WriteLine($"{record,-6}{number,-25}{docType,-3}{version,-2}{page,-3}{type,-30}{changeNr,-530}");
        }
    }
}