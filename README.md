# DVS-SAP Link

Kurzer Projektbeschreib....

## Projekt Ordner

*DvsSapLink2*
Beinhaltet die C# Source-Code Dateien des DvsSapLink2 Programmes

*doc*
Beinhaltet die Sourcen der Sphinx Dokumentation, hauptsächlich in 
ReStructuredText (RST) geschrieben.

## Dokumentation erstellen

### Voraussetzungen

Die Dokumentation ist mit ReStructuredText geschrieben. Um die Dokumentation
zu generieren werden folgende Software Voraussetzungen benötigt:

* Python 3.x
* PIP
* Sphinx

Python kann auf der Webseite https://www.python.org/downloads/ heruntergeladen
und installiert werden. Beim Installer darauf achten dass PIP mitinstalliert
wird.

Um Sphinx mit allen Abhängigkeite zu installieren kann nun eine Eingabeaufforderung
als Administrator im Projekt Ordner geöffnet werden und der folgende Befehl eingegeben
werden:

`pip install -r requirements.txt`

### HTML generieren

Sphinx erlaubt das generieren der Dokumentation in verschiedene Formate.
Die detailierte Beschreibung ist auf der Sphinx Webseite https://www.sphinx-doc.org/
zu finden. Das Beispiel hier zeigt das erstellen der Dokumentation als HTML
welches dann in einem Webbrowser geöffnet werden kann. 
Zuerst wird eine Eingabeaufforderung im "doc" Ordner des Projekts geöffnet.
Danach kann mit dem folgenden Befehl die Dokumentation erstellt werden:

`sphinx-build -b html doc/source doc/build`
