using System;
using System.IO;
using System.Linq;
using DvsSapLink2.Model;
namespace DvsSapLink2.Helper
{
    public class Logger : IDisposable
    {
        private readonly StreamWriter stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public Logger(string path)
        {
            this.stream = new StreamWriter(path, append: true);
        }

        /// <summary>
        /// Writes a log entry
        /// </summary>
        /// <param name="key">Log entry key</param>
        /// <param name="message">Message to log</param>
        public void Write(string key, string message)
        {
            var now = DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss");
            var line = $"{Environment.UserName,-14}{now,-23}{key,-7}{message}";
            this.stream.WriteLine(line);
            this.stream.Flush();
        }

        /// <summary>
        /// Releases all resources used by this <see cref="Logger"/> instance
        /// </summary>
        public void Dispose()
        {
            this.stream?.Dispose();
        }
    }

    public class SapTransferWriter : IDisposable
    {
        private readonly StreamWriter streamSapTransfer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public SapTransferWriter(string path)
        {
            this.streamSapTransfer = new StreamWriter(path);
        }

        /// <summary>
        /// Writes a log entry
        /// </summary>
        /// <param name="key">Log entry key</param>
        /// <param name="message">Message to log</param>
        public void WriteFileAttributes(AttributeFile attributeFile, SapData sapData)
        {

            // Zeile 1                                                                      Start-Pos, Beschreibung, Beispiel
            string A = "DOK01";                                                             //   1 Recordart
            string B = attributeFile[FileAttributeName.ZeichnungsNummer];                   //   7 Dokument-Nummer          HTAM123456
            string C = "ZAB";                                                               //  32 Dokument-Art             ZAB
            string D = attributeFile[FileAttributeName.AeStand_aktuell];                    //  35 Dokument-Version         A
            string E = string.Format("D{0:D2}", 
                       int.Parse(attributeFile[FileAttributeName.BlattNr]));                //  37 Teildokument             D01
            string F = "D";                                                                 //  40 Sprache                  D
            string G = " ";                                                                 //  41 Änderungs-Nr.        
            string H = attributeFile[FileAttributeName.Haupttitel];                         //  53 Titel                    Massbild
            string I = "CAD-DVS-Update";                                                    // 308 Änderungsbeschreib.      CAD-DVS-Update
            string J = sapData.State;                                                       // 378 Dokument-Status          DR
            string K = sapData.User;                                                        // 380 Sachbearbeiter           221226
            string L = sapData.Labor.ToString();                                            // 392 Labor/Büro               760
            string M = "ACD";                                                               // 395 Datei 1                  ACD
            string N = "";                                                                  // 398 Datenträger 1           
            string O = "";                                                                  // 408 File-Name 1
            string P = "PDF";                                                               // 478 Datei 2                  PDF         (TIF)
            string Q = "IM_PRE_V";                                                          // 481 Datenträger 2            IM_PRE_V    (IM_CAD_V)
            string R = attributeFile.Title + ".tif";                                        // 491 File-Name 2              HTAM123456-0-01.tif
            string S = "LABOR/BÜRO";                                                        // 586 Bezugsort SAP            LABOR/BÜRO

            var line = $"{A,-6}{B,-25}{C,-3}{D,-2}{E,-3}{F,-1}{G,-12}{H,-255}{I,-70}{J,-2}{K,-12}{L,-3}{M,-3}{N,-10}{O,-70}{P,-3}{Q,-10}{R,-95}{S,-14}";
            this.streamSapTransfer.WriteLine(line);


            // Zeile 2
            A = "DOK02";                                                                    //   1 Recordart
            F = "DOKTYP_ZAB_D";                                                             //  40 Merkmal Dokumenten-Typ   DOKTYP_ZAB_D
            G = "mechanische Zeichnung";                                                    //  70 Merkmalwert              mechanische Zeichnung

            line = $"{A,-6}{B,-25}{C,-3}{D,-2}{E,-3}{F,-30}{G,-530}";
            this.streamSapTransfer.WriteLine(line);


            // Zeile 3
            A = "DOK02";                                                                    //   1 Recordart
            F = "PROJ_D";                                                                   //  40 Merkmal Dokumenten-Typ   PROJ_D
            G = attributeFile[FileAttributeName.Typ];                                       //  70 Merkmalwert              uQWG+ 630...

            line = $"{A,-6}{B,-25}{C,-3}{D,-2}{E,-3}{F,-30}{G,-530}";
            this.streamSapTransfer.WriteLine(line);


            // Zeile 4
            A = "DOK02";                                                                    //   1 Recordart
            F = "AUFTR_NR_D";                                                               //  40 Merkmal Dokumenten-Typ   AUFTR_NR_D
            G = attributeFile[FileAttributeName.AuftragsNummer];                            //  70 Merkmalwert              11005084-000101

            line = $"{A,-6}{B,-25}{C,-3}{D,-2}{E,-3}{F,-30}{G,-530}";
            this.streamSapTransfer.WriteLine(line);


            // Zeile 5
            A = "DOK02";                                                                    //   1 Recordart
            F = "FORMAT_D";                                                                 //  40 Merkmal Dokumenten-Typ   FORMAT_D
            G = attributeFile[FileAttributeName.Format];                                    //  70 Merkmalwert              11005084-000101

            line = $"{A,-6}{B,-25}{C,-3}{D,-2}{E,-3}{F,-30}{G,-530}";
            this.streamSapTransfer.WriteLine(line);


            this.streamSapTransfer.Flush();
        }

        /// <summary>
        /// Releases all resources used by this <see cref="Logger"/> instance
        /// </summary>
        public void Dispose()
        {
            this.streamSapTransfer?.Dispose();
        }
    }
}