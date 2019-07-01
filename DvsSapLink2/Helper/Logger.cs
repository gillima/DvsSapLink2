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
            this.stream = new StreamWriter(path);
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
        private readonly StreamWriter stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public SapTransferWriter(string path)
        {
            this.stream = new StreamWriter(path);
        }

        /// <summary>
        /// Writes a log entry
        /// </summary>
        /// <param name="key">Log entry key</param>
        /// <param name="message">Message to log</param>
        public void WriteFileAttributes(AttributeFile attributeFile)
        {

            //                                                                                                     Start-Pos, Beschreibung, Beispiel
            string A = "DOK01";                                                                                 //   1 Recordart
            string B = attributeFile.Attributes.First(a => a.Name == FileAttributeName.ZeichnungsNummer).Value; //   7 Dokument-Nummer      HTAM123456 Start: 1
            string C = attributeFile.Attributes.First(a => a.Name == FileAttributeName.Typ).Value;              //  32 Dokument-Art         ZAB
            string D = attributeFile.Attributes.First(a => a.Name == FileAttributeName.AeStand_aktuell).Value;  //  35 Dokument-Version     A
            //TODO: Blatt-Nummer muss zweistellig mit Nullen aufgefüllt sein (z.B. "02")
            string E = attributeFile.Attributes.First(a => a.Name == FileAttributeName.BlattNr).Value;          //  37 Teildokument         D01
            string F = "D";                                                                                     //  40 Sprache              D
            string G = " ";                                                                                     //  41 Änderungs-Nr.        
            string H = attributeFile.Attributes.First(a => a.Name == FileAttributeName.Haupttitel).Value;       //  53 Titel                Massbild
            string I = "CAD-DVS-Update";                                                                        // 308 Änderungsbeschreib.  CAD-DVS-Update
            //TODO: SapData auslesen (für DR Werte lesen, für RE nur Status)
            string J = "SapData.Status";                                                                        // 378 Dokument-Status      DR
            string K = "SapData.User";                                                                          // 380 Sachbearbeiter       221226
            string L = "SapData.Labor";                                                                         // 392 Labor/Büro           760
            string M = "ACD";                                                                                   // 395 Datei 1              ACD
            string N = "";                                                                                      // 398 Datenträger 1           
            string O = "";                                                                                      // 408 File-Name 1
            string P = "PDF";                                                                                   // 478 Datei 2              PDF         (TIF)
            string Q = "IM_PRE_V";                                                                              // 481 Datenträger 2        IM_PRE_V    (IM_CAD_V)
            string R = attributeFile.Title + ".tif";                                                            // 491 File-Name 2          HTAM123456-0-01.tif
            string S = "LABOR/BÜRO";                                                                            // 586 Bezugsort SAP        LABOR/BÜRO

            var line = $"{A,-6}{B,-25}{C,3}{D,2}D{E,3}{F,1}{G,12}{H,255}{I,70}{J,2}{K,12}{L,3}{M,3}{N,10}{O,70}{P,3}{Q,10}{S,14}";
            this.stream.WriteLine(line);


            //    strOutputRec(2) = Space(600)
            //    Mid(strOutputRec(2), 1) = "DOK02"                   'Recordart
            //    Mid(strOutputRec(2), 7) = StrDokNummerSap           'Dokumentennummer
            //    Mid(strOutputRec(2), 32) = "ZAB"                    'Dokumentenart
            //    Mid(strOutputRec(2), 35) = strAeSAP                 'Dokumentversion
            //    Mid(strOutputRec(2), 37) = "D" + strBlattNrFname    'Teildokument
            //    Mid(strOutputRec(2), 40) = "DOKTYP_ZAB_D"           'Merkmal Dokumenten-Typ
            //    Mid(strOutputRec(2), 70) = "mechanische Zeichnung"  'Merkmalwert


            //    strOutputRec(3) = Space(600)
            //    Mid(strOutputRec(3), 1) = "DOK02"                   'Recordart
            //    Mid(strOutputRec(3), 7) = StrDokNummerSap           'Dokumentennummer
            //    Mid(strOutputRec(3), 32) = "ZAB"                    'Dokumentenart
            //    Mid(strOutputRec(3), 35) = strAeSAP                 'Dokumentversion
            //    Mid(strOutputRec(3), 37) = "D" + strBlattNrFname    'Teildokument
            //    Mid(strOutputRec(3), 40) = "PROJ_D"                 'Merkmal Dokumenten-Typ
            //    Mid(strOutputRec(3), 70) = StrTyp                   'Merkmalwert


            //    strOutputRec(4) = Space(600)
            //    Mid(strOutputRec(4), 1) = "DOK02"                   'Recordart
            //    Mid(strOutputRec(4), 7) = StrDokNummerSap           'Dokumentennummer
            //    Mid(strOutputRec(4), 32) = "ZAB"                    'Dokumentenart
            //    Mid(strOutputRec(4), 35) = strAeSAP                 'Dokumentversion
            //    Mid(strOutputRec(4), 37) = "D" + strBlattNrFname    'Teildokument
            //    Mid(strOutputRec(4), 40) = "AUFTR_NR_D"             'Merkmal Dokumenten-Typ
            //    Mid(strOutputRec(4), 70) = strAuftragsNr            'Merkmalwert


            //    Print #intTransFile, strOutputRec(1)
            //    Print #intTransFile, strOutputRec(2)
            //    Print #intTransFile, strOutputRec(3)
            //    Print #intTransFile, strOutputRec(4)

            //    'Format ist für 3BHJ-Nummern nicht mehr aus der Nummer ableitbar.
            //    'Bei allen Dokumenten, die an fünfter Stelle nicht 0-4 haben, 
            //    'Format auch weglassen. 
            //    If(Left(StrDokNummerSap, 4) <> "3BHJ") Or _
            //      (Mid(strDrawingFileName, 5, 1) >= 0 And _
            //        Mid(strDrawingFileName, 5, 1) <= 4) Then
            //        strOutputRec(5) = Space(600)
            //        Mid(strOutputRec(5), 1) = "DOK02"                   'Recordart
            //        Mid(strOutputRec(5), 7) = StrDokNummerSap           'Dokumentennummer
            //        Mid(strOutputRec(5), 32) = "ZAB"                    'Dokumentenart
            //        Mid(strOutputRec(5), 35) = strAeSAP                 'Dokumentversion
            //        Mid(strOutputRec(5), 37) = "D" + strBlattNrFname    'Teildokument
            //        Mid(strOutputRec(5), 40) = "FORMAT_D"               'Merkmal Format
            //        Mid(strOutputRec(5), 70) = "A" + Mid(strDrawingFileName, 5, 1)  'Format


            //        Print #intTransFile, strOutputRec(5)

            //    End If
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
}