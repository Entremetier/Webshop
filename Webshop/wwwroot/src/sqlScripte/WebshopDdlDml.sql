-- drop database LapWebshop;

create database LapWebshop;

use LapWebshop;

-- nvarchar unterstuetzt Unicodezeichen, braucht aber doppelten Speicherplatz im Gegensatz zu varchar
create table Category(
Id int identity(1,1) primary key,
[Name] nvarchar(50) not null,
-- bei numeric und decimal werden vor dem komma (die 5) alle stellen angegeben, auch die nach dem komma stehen sollen (123,45)
-- nach dem komma steht auf wieviele stellen genau die zahl sein soll
TaxRate decimal(5,2) not null
);

create table Manufacturer (
Id int identity(1,1) primary key,
[Name] nvarchar(50) not null
);

create table Customer(
Id int identity(1,1) primary key,
Title nvarchar(10),
FirstName nvarchar(50) not null,
LastName nvarchar(50) not null,
Email nvarchar(50) not null,
Street nvarchar(80) not null,
Zip char(4) not null,
City nvarchar(50) not null,
PwHash varbinary(max) not null,
Salt varbinary(max) not null,
);

-- Customer Zip from int to char(4)
--alter table Customer
--alter column Zip char(4);


create table Product(
Id int identity(1,1) primary key,
ProductName nvarchar(100) not null,
NetUnitPrice decimal(6,2) not null,
ImagePath nvarchar(max) not null,
[Description] nvarchar(max) not null,
ManufacturerId int not null,
CategoryId int not null,
constraint FK_Product_Manufacturer foreign key (ManufacturerId) references Manufacturer(Id),
constraint FK_Product_Category foreign key (CategoryId) references Category(Id)
);

create table [Order] (
Id int identity(1,1) primary key,
CustomerId int not null,
PriceTotal decimal(7,2) default 0,
DateOrdered DateTime null,
Street nvarchar(80) not null,
Zip char(4) not null,
City nvarchar(50) not null,
FirstName nvarchar(50) not null,
LastName nvarchar(50) not null,
constraint FK_Order_Customer foreign key (CustomerId) references Customer(Id)
);

create table OrderLine(
Id int identity(1,1) primary key,
OrderId int not null,
ProductId int not null,
Amount int not null,
NetUnitPrice decimal(6,2) not null,
TaxRate decimal(4,2) not null,
constraint FK_OrderLine_Order foreign key (OrderId) references [Order](Id),
constraint FK_OrderLine_Product foreign key (ProductId) references Product(Id)
);

insert into Category values
('Smartphone', 20.00),
('Tablet', 20.00),
('Notebook', 20.00);

insert into Manufacturer values
('Samsung'),
('Xiaomi'),
('Apple'),
('Huawei'),
('Lenovo'),
('Dell'),
('Microsoft'),
('Panasonic'),
('Fujitsu'),
('Acer'),
('HP');

insert into Product values
('Galaxy S22 Ultra', 1249.00, 
'~/src/images/smartphones/samsung-galaxy-s22-ultra.jpg', 
'Für alle, die höher hinaus wollen! Vom Arbeitsspeicher, über den Akku bis hin zur Kamera übertrifft das Galaxy S22 Ultra 
alles bisher Dagewesene. Wer sagt, dass man sich mit dem Standard zufriedengeben muss? Wer entscheidet über die Gültigkeit einer Norm? 
Das Galaxy S22 Ultra bricht kurzerhand mit allen Regeln, statt dem Mainstream zu folgen. 
Den Regeln des Lichts, der Zeit, der Schwerkraft und der Leistung. Der Standard wird neu definiert!',
1, 1),
('Galaxy S22', 1049.00,
'~/src/images/smartphones/samsung-galaxy-s22.jpg',
'Video ist für dich ein unersetzlicher Weg, deine Gefühle auszudrücken. 
Was du erlebst, wird mit der Handykamera für immer festgehalten. Video ist die Art, wie du lernst, wie du einkaufst, 
wie du dich mit anderen verbindest. Ja, es ist deine Art, Langeweile gemeinsam mit deinen Freunden zu bekämpfen, 
und das in Echtzeit. Das Galaxy S22+ versteht, dass Video für dich die beste Art ist, dich auszudrücken. 
Und bricht deshalb mit allen Regeln, die für Video gegolten haben. Damit du bekommst, was dein Leben ausmacht. Und noch viel mehr.',
1, 1),
('Galaxy A53s', 329.00,
'~/src/images/smartphones/samsung-galaxy-a53s.jpg',
'Einsteigermodell für den Normaluser.',
1, 1),
('Redmi Note 10', 195.00, 
'~/src/images/smartphones/xiaomi-redmi-note-10.jpg',
'Dieses Smartphone verbindet alle Features, die Sie sich wünschen, mit makellosem Design: das XIAOMI REDMI NOTE 10 5G 4+128GB
NIGHTTIME BLUE! Beim Design des Geräts wurde schließlich eine Gestaltung in Nighttime Blue gewählt, 
was für ein besonders tolles Erscheinungsbild sorgt.',
2, 1),
('iPhone 11', 579.00, 
'~/src/images/smartphones/iphone-11.jpg', 
'Telefon mit 5 Jahren Security-Updates, mehr muss man nicht sagen.',
3, 1),
('iPhone SE (2022)', 499.00, 
'~/src/images/smartphones/iphone-se-2022.jpg',
'Der A15 Bionic macht alles besser. Apps laden blitz­schnell und fühlen sich flüssig an.',
3, 1),
('iPhone 12', 769.00, 
'~/src/images/smartphones/iphone-12.jpg', 
'Das iPhone 12 Pro Max hat das bisher größte iPhone Display und die höchste Auflösung mit fast 3,5 Millionen Pixeln sorgt 
für ein realistisches Seherlebnis. Diese OLED-Displays erwecken HDR-Videoinhalte förmlich zum Leben und erreichen in der Spitze eine 
Helligkeit von 1200 Nits.',
3, 1),
('iPhone 13', 929.00, 
'~/src/images/smartphones/iphone-13.jpg', 
'Die neuen iPhones kommen in einem wunderschönen Design mit eleganten flachen Rändern und in fünf großartigen neuen Farben. 
Außerdem sind sie superschnell und energieeffizient, dank dem A15 Bionic Chip. 
Sie haben eine längere Batterielaufzeit und ein helleres Super Retina XDR Display, auf dem Inhalte lebendig werden.',
3, 1),
('P30 Pro', 303.00, 
'~/src/images/smartphones/huawei-p30-pro.jpg', 
'Unglaublich was Huawei in die Entwicklung dieser Handykamera investiert hat. 
Noch nie dagewesene Bildqualität und eine Vielzahl an hilfreichen Funktionen bietet dieses vollständig erneuerte refurbed™ Huawei P30 Pro.',
4, 1),
('Mate 20 Pro', 231.00,
'~/src/images/smartphones/huawai-p20-pro.jpg',
'Das 6,39 Zoll große Display bietet eine Auflösung von 1.440 x 3.120 Pixeln bei einer Pixeldichte von 538 ppi. 
Erhältlich ist das Smartphone in Twilight, Midnight Blue, Black und Emerald Green. Die Kamera des Huawei Mate 20 Pro nimmt Fotos 
mit einer Auflösung von 40 Megapixeln auf.',
4, 1),
('P30 Lite', 204.00, 
'~/src/images/smartphones/huawei-p30-lite.jpg', 
'Es ist 152,9 mm hoch, 72,7 mm breit, 7,4 mm dick und wiegt 159 g. 
Das 6,15 Zoll große Display bietet eine Auflösung von 1.080 x 2.312 Pixeln bei einer Pixeldichte von 415 ppi.',
4, 1),
('Mi 10', 341.00,
'~/src/images/smartphones/xiaomi-mi-10.jpg',
'Mit dem Xiaomi Mi 10 Lite 5G besitzt ihr ein Mittelklasse-Smartphone mit 5G-Unterstützung, hellem und scharfen 
Display sowie ausdauerndem Akku. Bei GIGA könnt ihr euch die zugehörige Bedienungsanleitung kostenlos, auf Deutsch und im PDF-Format 
herunterladen.',
2, 1),
('Mi 10T Pro', 329.99, 
'~/src/images/smartphones/xiaomi-mi-10t-pro.jpg', 
'Das Mi 10T Pro besitzt einen AI 64MP Hauptsensor mit OIS, und kann Ultra-Weitwinkel-Photos sowie Landschaftsaufnahmen machen,
Portrait Modus, und Macro Modus. Das Mi 10T Pro kommt mit einer 20mp Punch-Hole Selfiekamera. Das Mi 10T verwendet den neusten 5G 
Qualcomm Snapdragon 865 Processor, und eine Octa-Core CPU.',
2, 1),
('11T Pro', 495.99,
'~/src/images/smartphones/xiaomi-11t-pro.jpg',
'Das Xiaomi 11T Pro ist ein weiteres leistungsfähiges Xiaomi-Handy zu einem sehr wettbewerbsfähigen Preis. 
Es ist schnell, hat eine solide Hauptkamera, ein gutes Display und eine außergewöhnlich schnelle Ladezeit zu einem vernünftigen Preis.',
2, 1),
('Galaxy S20 FE', 444.00, 
'~/src/images/smartphones/samsung-galaxy-s20-fe.jpg', 
'Anders als bei der normalen S20-Flotte kommt bei dem S20 Fan Edition eine Triple-Kamera rund um einen neuen 
12-Megapixel-Sensor mit größeren Pixeln in der Hauptkamera zum Einsatz. Hinzu gesellen sich eine 12-Megapixel-Weitwinkel-Kamera 
und eine Tele-Linse mit 8 Megapixeln und dreifachem (optischem) Zoom.',
1, 1),
('Galaxy A52', 339.99,
'~/src/images/smartphones/samsung-galaxy-a52.jpg',
'Das Herzstück des vollständig erneuerten Smartphones bildet der starke Snapdragon 750G Octa-Core Prozessor. 
Zusammen mit bis zu 8 GB RAM bekommst du hier einen gehörigen Leistungsschub.
Der interne Speicher von bis zu 256 GB bietet genügend Platz für Aufnahmen, Apps und mehr und kann mit einer micro-SD Karte 
um bis zu 1 TB erweitert werden.',
1, 1),
('Galaxy Z Fold 3', 1015.99,
'~/src/images/smartphones/samsung-galaxy-z-fold-3.jpg', 
'Das 7,6 Zoll große Display des vollständig erneuerten Samsung Galaxy Z Fold 3 5G wirkt durch die Faltmöglichkeit noch 
riesiger, als ohnehin. Umso verblüffender, dass auch sonst keine Abstriche notwendig sind. Auch in Sachen Performance hat dieses 
Modell die Nase vorn. Der Power-Prozessor von Snapdragon bietet einwandfreie Leistung mit gigantischen 12 GB Arbeitsspeicher. 
Die größte Stärke liegt in der Dreifachkamera mit Teleobjektiv und Weitwinkel, welche sich in Kombination mit dem faltbaren Screen 
selbst für professionelle fotografische Zwecke eignet und so Tablets ernsthaft Konkurrenz macht.',
1, 1),
('Galaxy Z Flip ', 644.99,
'~/src/images/smartphones/samsung-glxy-z-flip.jpg',
'Mal abgesehen vom stylischen Punkt, den so ein faltbares Telefon unweigerlich mitbringt, hat der Faltmechanismus 
auch andere Vorteile: Videotelefonie, zum Beispiel im Home Office benötigt kein Stativ mehr, anwinkeln und lossprechen. 
Fotos machen aus ungewöhnlichen Positionen, die sonst wirklich schwer zu bewältigen sind: Anwinkeln, Foto machen.',
1, 1),
('Galaxy Note 20 Ultra', 628.99,
'~/src/images/smartphones/samsung-glxy-note-20-ultra.jpg',
'Das Samsung Galaxy Note 20 Ultra geizt an keiner einzigen Stelle mit Top-Ausstattung. 
Extrem hochauflösendes und bis zu 120 Hz schnelles AMOLED Display, einer der schnellsten Prozessoren des Jahres 2020, 
gewaltige Kamerafunktionen mit einer Hauptkamera bis 108 MP, ein Stylus - so wie es sich für die Note-Serie gehört - und vielen 
weiteren Funktionen.',
1, 1),
('Galaxy Z Flip3', 599.00,
'~/src/images/smartphones/samsung-galaxy-z-flip-3.jpg',
'Das Samsung Galaxy Z Flip 3 5G ist ein high-end Foldable, bei dem du nicht zwischen Design und Performance wählen musst. 
Sein 8 GB starker Arbeitsspeicher ist nahezu jeder Aufgabe gewachsen. Sein faltbares, 6,7 Zoll großes AMOLED-Display sorgt mit 120 Hz 
und HDR10+ für eine realitätsgetreue Bildausgabe ohne Ruckeln. Adreno Grafikkarte und achtkerniger Snapdragon-Prozessor 
schlagen dem Fass endgültig den Boden aus. Dank Zweitdisplay und Zweifachkamera mit 12 MP und Weitwinkel ist das Smartphone so 
vielseitig verwendbar wie nur wenige andere Geräte.',
1, 1),
----------------------------------------------------------------------------------------------
('ThinkPad X1 Tablet', 470.00,
'~/src/images/tablets/lenovo-thinkPad-x1.jpg',
'Laptop oder Tablet? Beides! Die einfache Handhabung eines Tablets vereint mit der Leistungsfähigkeit eines Notebooks — das 
ist das Lenovo ThinkPad X1 Tablet. Der 12 Zoll große Bildschirm bietet bei einer Auflösung von 2160 x 1440 genügend Platz 
für produktives Arbeiten. Für einen effizienten Workflow kann das Multi-Touch-Display ganz intuitiv mit dem Fingern bedient werden. 
Mit dem mitgelieferten Touch-Stift können auch komplexere Freihandzeichnungen mühelos und wie auf Papier angefertigt werden. ',
5, 2),
('iPad Pro 5', 1133.99,
'~/src/images/tablets/ipad-pro-5.jpg',
'Das iPad Pro kommt mit dem leistungsstarken Apple M1 Chip für Performance auf einem optimalen Level und Batterie für 
den ganzen Tag. Einem faszinierenden 12,9 Zoll Liquid Retina XDR Display, um HDR Fotos und Videos anzusehen und zu bearbeiten. 
Und einer Frontkamera mit dem Folgemodus Im Bild behalten, der sich bei einem Videoanruf automatisch ausrichtet, damit du in der 
Mitte des Bildes bleibst. Das iPad Pro hat Pro Kameras und einen LiDAR Scanner für beeindruckende Fotos, Videos und immersive AR. 
Thunderbolt / USB 4 für die Verbindung von leistungsstarkem Zubehör. Du kannst den Apple Pencil dazu nehmen, um Notizen zu machen, 
zu zeichnen und Dokumente zu kommentieren. Und das Magic Keyboard für reaktionsschnelles Tippen und mit integriertem Trackpad.',
3, 2),
('Canvas 27', 999.99,
'~/src/images/tablets/dell-canvas-27.jpg',
'So vertraut wie Papier und Bleistift: Die 27 Zoll große LCD-Anzeige mit Adobe RGB und QHD-Auflösung ist mit 
reflexionsarmem Corning® Gorilla® Glass verstärkt, das die Verzögerung zwischen Stift und Display minimiert. 
Damit zeichnen Sie fast wie auf Papier.',
6, 2),
('iPad Air 5', 971.00,
'~/src/images/tablets/ipad-air-5.jpg',
'Das iPad Air (intern iPad13,16) ist das Mid-Range Tablet von Apple zwischen dem Einsteigergerät iPad 9 und dem High-End 
iPad Pro 11 Zoll 3. Gen. Durch den Einsatz des selben M1-Prozessors wie im Pro bleiben nur noch wenige Unterschiede. 
Das 120-Hz-Display, Face ID und das Kamerasystem vermisst das iPad Air.',
3, 2),
('Surface Pro X (2019)', 940.00,
'~/src/images/tablets/microsoft-surface-pro-x.jpg',
'Das ultradünne und leichte Pro-Gerät, mit dem Sie immer vernetzt und produktiv sind. Das ultradünne 
Gerät mit randlosem Touchscreen und 4G LTE-Konnektivität. Jetzt in Platin. Touchscreen. Windows 10.',
7, 2),
('SurfaceBook', 928.00,
'~/src/images/tablets/microsoft-surfacebook.jpg', 
'Das Microsoft Surface Book 3 hat eine Breite von 312 mm. Das Microsoft Surface Book 3 hat eine Tiefe von 232 mm. 
Die Bildschirmauflösung des Microsoft Surface Book 3 ist 3000 x 2000 Pixel. Die Bildschirmgröße des Microsoft 
Surface Book 3 ist 13.5 Zoll.',
7, 2),
('Galaxy Tab S8 Ultra', 825.99,
'~/src/images/tablets/samsung-galaxy-tab-s8-ultra.jpg',
'Das Galaxy Tab S8 Ultra hat einen 14,6 Zoll großen AMOLED-Bildschirm mit einer Auflösung von 2.960 x 1.848 Pixeln. 
Die Bildrate beträgt bis zu 120 Hz. Wie in Leaks vor der Veröffentlichung bereits zu sehen war, hat das Tablet eine auffällige Notch, 
in der zwei 12-Megapixel-Kameras für Videotelefonie eingebaut sind. Neben einer Weitwinkelkamera gibt es noch eine Superweitwinkelkamera.',
1, 2),
('Surface Book 2', 799.99, 
'~/src/images/tablets/microsoft-surface-book-2.jpg',
'Entdecken Sie den Laptop, der Ihren größten Anforderungen gerecht wird. Der bisher leistungsstärkste Surface Laptop 
kombiniert Geschwindigkeit, Grafik und beeindruckendes Gaming mit der Vielseitigkeit eines Laptops, Tablets und mobilen Studios. 
Erhältlich in den Größen 13,5-Zoll oder 15-Zoll mit einem hochauflösenden Touchscreen.',
7, 2),
('Latitude 5290 2-in-1 Tablet', 729.00,
'~/src/images/tablets/dell-latitude-5290.jpg',
'Dieses vollständig erneuerte Luxusgerät ist ein Hybrid zwischen Tablet und Laptop. 
Es nimmt das Beste aus beiden Welten: Dell hat hier die praktische Kompaktheit eines Tablets und die arbeitstaugliche Leistungskraft 
eines Notebooks perfekt kombiniert. Somit ist das Dell Latitude 5290 leicht und flexibel genug für den Freizeitgebrauch, aber 
auch kraftvoll und schnell genug, um deinen Arbeitsalltag zu bewältigen – mit diesem Powergerät musst du nie wieder Kompromisse eingehen. ',
6, 2),
('Latitude 7200 2-in-1 Tablet', 708.90 ,
'~/src/images/tablets/dell-latitude-7200.jpg',
'Mit dem leistungsstarken Prozessor und einer beeindruckenden System-Performance kannst du im Nu jede Arbeitsaufgabe 
erledigen. Ein weiteres Feature das dieses Tablet besonders Office-tauglich macht ist die für Tablets außergewöhnlich hohe Anzahl an 
Erweiterungsanschlüssen. Dank dem besonders hellen Display kann man dieses 2-in-1 Tablet auch problemlos im Freien verwenden und 
das stabile Gehäuse und leichte Gewicht, ermöglichen einen stressfreien Transport.',
6, 2),
('Galaxy Tab S7 FE', 566.99,
'~/src/images/tablets/samsung-glxy-tab-s7-fe.jpg',
'Das Samsung Galaxy Tab S7 FE hat eine Tiefe von 6.3 mm. Die Bildschirmauflösung des Samsung Galaxy Tab S7 FE ist 
2560 x 1600 Pixel. Das Samsung Galaxy Tab S7 FE verwendet Android als Betriebssystem. Der Samsung Galaxy Tab S7 FE hat eine 
Speicherkapazität von 128 GB.',
1, 2),
('IdeaPad Miix 510-12IKB', 514.99,
'~/src/images/tablets/lenovo-ideapad-miix-510-12ikb.jpg',
'Mit dem Ideapad MIXX 510-12IKB hat Lenovo einen Surface ähnliches Gerät geschaffen, das sich hinter dem original von 
Microsoft nicht verstecken muss. Wie beim Surface besteht dieses 2-in-1-Device aus einer Tablet- und einer Tastatureinheit. 
Das Ideapad MIXX 510-12IKB besitzt ein 12,2 Zoll großes Touchdisplay, welches man auch mit einem Active Pen von Lenovo für Windows 
Ink verwenden kann. Bei diesem Modell gehört der Stift jedoch nicht zum Lieferumfang.',
5, 2),
('Latitude 7285', 408.90,
'~/src/images/tablets/dell-latitude-7285.jpg',
'Das Dell Latitude 7285 2-in-1 Convertible/Tablet ist ideal für alle, die viel unterwegs sind und nicht immer gleich 
ein ganzes Notebook benötigen. Das Herzstück dieses 2-in-1 Gerätes ist eindeutig der hochauflösende 3K Touchscreen-Bildschirm, 
der auch Eingabestifte unterstützt. Ansonsten hat man hier noch zwei schnelle Thunderbolt 3 Anschlüsse, für externe SSDs, Docks 
und sogar externe Grafikkarten. Mit dem Tastatur-Dock verwandelt sich das Hochleistungstablet ruckzuck in ein normales, Business-Notebook 
mit reichlich Leistung und immernoch äußerst handlichen Abmessungen.',
6, 2),
('iPad mini 5', 370.99,
'~/src/images/tablets/ipad-mini-5.jpg',
'Das iPad Mini 5 bietet ein hochauflösendes (2048 x 1536 Pixel) 7.9 Zoll großes Retina (IPS) Display, 
den pfeilschnellen Apple A12 Bionic Prozessor und modernste Schnittstellen wie 2x2 ac-WLAN, Bluetooth 5.0 und Lightning. 
Der separat erhältliche Apple Pencil wird nun unterstützt und erweitert die Nutzbarkeit des Apple iPad mini 5 
immens, vom einfachen Einkaufslisten schreiben und ans iPhone senden, bis hin zum Zeichnen und Konstruieren. Mit leichten 300g und 
nur 6.1 mm Dicke passt das iPad mini schnell in den Rucksack und größere Handtaschen.',
3, 2),
('MatePad Pro', 345.99,
'~/src/images/tablets/huawei-matepad-pro.jpg',
'Das 32 Zentimeter durchmessende Huawei MatePad Pro 12.6 Tablet ist eines der größten Android-Tablets. 
Auch die Leistung ist Spitze, denn mit seinem High-End-SoC HiSilicon Kirin 9000E geht das OLED-Tablet in direkte Konkurrenz 
zu Schwergewichten wie dem Apple iPad Pro 12.9 und dem Samsung Galaxy Tab S7 Plus.',
4, 2),
('ToughPad FZ-G1', 429.99,
'~/src/images/tablets/panasonic-toughpad-fz-g1.jpg',
'Das TOUGHBOOK G1 setzt neue Maßstäbe für Tablets mit blendfreiem Outdoor-Display und ist damit der ideale Begleiter 
für Mitarbeiter im Außeneinsatz. Sein kapazitives 10-Finger Multi-Touch Display, der Digitizer-Stift und die flexibel 
konfigurierbaren Ports ermöglichen es diesem Windows 10 Pro Gerät, HD-Dokumentation und -Bilder im Freien anzuzeigen und 
gleichzeitig von seiner hervorragenden Konnektivität zu profitieren, um sicherzustellen, dass Daten bei Bedarf immer verfügbar 
sind. Der flexibel konfigurierbare Port ermöglicht es Mitarbeitern, in einem kompakten, leichten Gehäuse der Full-Ruggedized 
Kategorie, auf Legacy-Port-Optionen zurückzugreifen.',
8, 2),
('Galaxy Tab Active 3', 426.99,
'~/src/images/tablets/samsung-glx-tab-active-3.jpg',
'Der eingebaute Exynos 9810 kam schon in den Topmodellen der letzten Generation, also im Samsung Galaxy S9 (Testbericht) 
zum Einsatz, entsprechend ist der nicht mehr ganz taufrisch. Ausreichende Leistung bringt er aber durchaus noch, auch für die gehobene 
Mittelklasse kann sich der Chip noch sehen lassen. Zusammen mit 4 GByte RAM sorgt er im Alltag für überwiegend flüssige Bedienung und 
selbst bei Spielen reicht die Leistung noch in den meisten Fällen aus.',
1, 2),
('Surface Go', 275.99,
'~/src/images/tablets/microsoft-surface-go.jpg',
'Das Microsoft Surface Go ist das bisher kleinste und leichteste Surface, welches Microsoft anbietet. 
Mit nur 522 Gramm und 10 Zoll Bildschirm passt es wie ein normales Tablet in die meisten Taschen und lässt sich problemlos auch 
lange in der Hand halten. Gleichzeitig bietet das Surface Go aber die gewohnten Vorteile der Surface-Serie: x86 Prozessor mit 
echtem Windows Betriebssystem, volle Verfügbarkeit zum Beispiel für Officeprogramme wie Outlook, Word und Excel und die Möglichkeit 
es mit dem optional erhältlichen Type Cover in ein Notebook zu verwandeln.',
7, 2),
('MediaPad M5 10', 260.00,
'~/src/images/tablets/huawei-mediapad-m5-10.jpg',
'Das Huawei MediaPad M5 10 unterscheidet sich in nur sehr wenigen Punkten vom Pro-Modell. 
Wer keinen Eingabestift benötigt, hat hier ein ähnlich leistungsstarkes Tablet bei geringeren Preis. 
Mit 4 GB RAM und Octa-Core Prozessor ist dieses Tablet auch dank seines hochauflösenden Displays ideal für Filme und Dokus 
schauen oder auch für die meisten Androidspiele gleichsam geeignet. Trotz der 10.8 Zoll Diagonale wiegt das wie neu aufbereitete 
Tablet nur knapp ein halbes Kilo, was es geradezu einladend leicht macht um es auf Reisen und im Alltag dabei zu haben.',
4, 2),
('MediaPad M3', 181.99,
'~/src/images/tablets/huawei-mediapad-m3.jpg',
'Das qualitativ sehr hochwertig verarbeitete Gehäuse von dem Huawei MediaPad M3 hat unter der Haube einiges zu bieten. 
Das Tablet läuft mit einem 8-Kern-Prozessor von Qualcomm (Snapdragon 435). Dank des 3 GByte großen Arbeitsspeicher sind somit auch 
Ruckler so gut wie ausgeschlossen. Der interne Speicherplatz von 32 GByte lässt sich mittels Android 7 und dem microSD-Kartenlesers 
problemlos erweitern. Ein Highlight des Gerätes ist das verbaute LTE-Modem (Cat. 7), das mittels nanoSIM-Karte bis zu 300 MBit/s 
unterwegs zur Verfügung stellen kann.',
4, 2),
------------------------------------------------------------------------------------------------
('Precision 5530', 3000.00,
'~/src/images/notebooks/dell-precision-5530.jpg',
'Das Dell Precision 5530 Notebook bietet beeindruckende Leistung und eignet sich somit perfekt als mobile 
Workstation - egal ob für Editing, Simulationen, Rechenprogrammen oder Ähnliches - das Dell Precision 5530 hat ausreichend 
Power um jede Aufgabe mit Leichtigkeit zu bewältigen.',
6, 3),
('Precision 5550', 2777.99,
'~/src/images/notebooks/dell-precision-5550.jpg',
'Angetrieben von Nvidia Quadro. Wenngleich traditionelle Workstations, wie das Precision 7550, 
so schnell nicht verschwinden werden, gibt es dennoch einen stetig wachsenden Markt für stylische ultradünne Workstations. 
Im Precision 5550 stecken statt der GeForce-GPUs des XPS 15 9500 Nvidias Turing-Quadro-GPUs, die sich an ein professionelles Publikum 
richten.',
6, 3),
('MacBook Pro 2019', 1722.99,
'~/src/images/notebooks/apple-macbook-pro-2019.jpg',
'Das Apple MacBook Pro 2019 ist für alle gedacht, die ein extrem leistungsstarkes MacBook Pro haben möchten. 
Aus ihren eigenen Schuhen gewachsen ist Apple hier mit dem - im Vergleich zu den letzten Jahren - größeren 16 Zoll Display, was es 
echt in sich hat. Ebenfalls wieder dabei ist das beliebte Magic Keyboard - solide und genau so wie ein Keyboard sein soll.',
3, 3),
('Yoga Slim 9', 1658.99,
'~/src/images/notebooks/lenovo-yoga-slim-9.jpg',
'Das Notebook Yoga Slim 9 liefert beeindruckende Leistung da, wo Sie sie am meisten brauchen. 
Bis zu Intel® Evo™ und Intel® Core™ Prozessoren der 12. Generation sorgen für eine revolutionäre Hybridperformance. 
Dadurch sparen Sie Zeit, haben mehr Leistung zur Verfügung, können nahtlos 4K-Videos bearbeiten und alle Anwendungen ausführen, 
die Sie benötigen. Eine Grafik bis zur neuen integrierten NVIDIA® Geforce RTX™ 2050 ermöglicht eine bisher ungekannte Performance.',
5, 3),
('Precision 7520', 1370.00,
'~/src/images/notebooks/dell-precision-7520.jpg',
'Die leistungsstärkste mobile 15-Zoll-Workstation der Welt, mehr müssen wir nun wirklich nicht mehr sagen.',
6, 3),
('Galaxy Book Pro 360', 1308.99,
'~/src/images/notebooks/samsung-glxy-book-pro-360.jpg',
'Das Samsung Galaxy Book Pro 360 hat eine Breite von 354.85 mm. Das Samsung Galaxy Book Pro 360 hat eine Tiefe 
von 227.97 mm. Die Bildschirmauflösung des Samsung Galaxy Book Pro 360 ist 1920 x 1080 Pixel. Die Bildschirmgröße des Samsung 
Galaxy Book Pro 360 ist 15.6 Zoll.',
1, 3),
('Lifebook U7411', 1304.99,
'~/src/images/notebooks/fujitsu-lifebook-u7411.jpg',
'Das Fujitsu Lifebook tanzt in diversen Punkten deutlich aus der Reihe. Das beginnt beim Core i5 und der 
vergleichsweise kleinen SSD mit nur 512 GByte Kapazität und setzt sich beim Design fort. Momentan dominiert bei Notebooks 
ja eher die Unauffälligkeit, doch auf dem Lifebook trennt eine fette, rote Linie die Tastatur vom unteren Teil mit dem kleinen Touchpad.',
9, 3),
('Galaxy Book Pro', 1208.99,
'~/src/images/notebooks/samsung-glxy-book-pro.jpg', 
'Samsung stellt schon seit Jahren Ultrabooks her, die sich mehr als die meisten Konkurrenten auf Tragbarkeit und 
Flachheit konzentrieren. Die Samsung-Book-9-Serie etwa wurde mit schmalen Displayrändern ausgeliefert, bevor Dell das Konzept 
einige Jahre später mit seiner XPS-13-Serie populär machte.',
1, 3),
('Nitro 5 AN517-54-79F6', 1121.99, 
'~/src/images/notebooks/acer-nitro-5-n517-54-79f6.jpg',
'Gib Vollgas mit einem Mobilprozessor der AMD Ryzen™ 5000 Serie, GeForce RTX™ 30 Series, Laptop-GPUs und einem QHD 165 Hz. 
Beim Gaming zählt die Geschwindigkeit und der Nitro 5 ist genau dafür gemacht!',
10, 3),
('ThinkPad T590', 1014.99, 
'~/src/images/notebooks/lenovo-tp-t590.jpg', 
'Das Lenovo ThinkPad T590 ist die 15.6 Zoll Variante des beliebten ThinkPads. Zur 14 Zoll Variante unterscheidet sich hier - recht 
offensichtlich - das größere Display, aber auch ein Nummernblock wird hier genutzt, ideal also für alle, die viele Zahlen eintippen 
müssen - sei es in der Buchhaltung oder beim Bearbeiten von großen Listen.
Mit dem leistungsfähigen nativen QuadCore Prozessor Intel Core i5-8265U hat man stets genügend Leistungsreserven für fordernde 
Programme, Websites und auch Multimedia. Die stabile und wertige Verarbeitung des vollständig erneuerten Lenovo ThinkPad T590 
machen das Notebook zum idealen täglichen Begleiter im Büro, auf Reise oder auch einfach daheim.',
5, 3),
('MacBook Air M1 2020', 982.99,
'~/src/images/notebooks/apple-macbook-air-m1-2020.jpg',
'Mit dem MacBook Air M1 2020 hat Apple ein kompaktes Notebook im Programm, das vor allem auf eine lange Akkulaufzeit 
ausgelegt ist. Dies wird durch einen sparsamen Prozessor erreicht, der gleichzeitig auch die Grafikeinheit beheimatet. 
Das gesamte Gerät ist außerdem sehr leicht. Das Apple MacBook Air M1 2020 überzeugt durch sein stabiles, aus Aluminium gefertigtes 
Gehäuse. Gleichzeitig setzt Apple auf ein passives Kühlkonzept. So gibt es keine Lüfter im Apple MacBook Air M1 2020, was 
dafür sorgt, dass das Arbeiten in angenehmer Stille möglich ist. Trotz des kompakten Displays ermöglicht das MacBook Air M1 2020 
eine hohe Bildschirmauflösung.',
3, 3),
('ThinkPad X1 Carbon G5', 873.99, 
'~/src/images/notebooks/lenovo-x1-carbon-g5.jpg',
'Das ThinkPad X1 Carbon G5 von Lenovo ist ein kompaktes und leichtes Gerät aus der Klasse der Ultrabooks. 
Durch das kohlefaserverstärkte Gehäuse reduziert sich das Gesamtgewicht, was das ThinkPad X1 Carbon G5 zu einer guten Plattform 
für das mobile Arbeiten macht. Das Gehäuse inklusive Display ist besonders flach und so passt das ThinkPad X1 Carbon G5 gut in jede 
Tasche oder den Rucksack. Passend dazu ist das Ultrabook auf eine sehr lange Akkulaufzeit getrimmt. 
Darüber hinaus besitzt das Ultrabook eine Schnellladefunktion, sodass sich der Akku innerhalb von einer Stunde zu einem großen 
Teil aufladen lässt. Der energieeffiziente Prozessor stellt dank mehreren CPU-Kernen ausreichend Leistung für alle 
Arbeiten im Büro oder Homeoffice zur Verfügung.',
5, 3),
('Latitude 15 5590', 869.99, 
'~/src/images/notebooks/dell-latitude-5590.jpg', 
'Das Dell Latitude 15 5590 ist ein Premiumklasse Laptop, der speziell fürs Office entwickelt wurde.
Dell ist bekannt für ihre office-tauglichen Powergeräte und auch mit diesem Modell haben sie wieder einmal alle Erwartungen 
übertroffen. Egal ob Home-Office, Büro oder sogar Außenbereich - dieses vollständig erneuerte Notebook ist der perfekte 
Begleiter für jede Umgebung. Das minimalistische und robuste Design passt perfekt für jeden Anlass und sieht immer professionell aus.',
6, 3),
('ThinkPad X1 Yoga G2', 863.99, 
'~/src/images/notebooks/lenovo-tp-x1-yoga-g2.jpg', 
'Das Lenovo ThinkPad X1 Yoga G2 ist ein Convertible wie aus dem Bilderbuch: leicht, schlank, leistungskräftig und mit 
hochauflösenden Bildschirm. Als täglicher Begleiter im Beruf oder privat - oder auch gern beides: klassisch als Notebook im Job und 
daheim als Multimedia-Tablet - die Grenzen sind hier verschwommen. Das vollständig erneuerte Lenovo ThinkPad X1 Yoga G2 
überzeugt aber auch bei der Ausstattung. Thunderbolt 3 Anschlüsse, leistungskräftiger Intel Core i7 Prozessor, Digitizer im 
Touchscreen-Display, extrem stabile Bauweise, USB-C Ladeanschluss und vieles mehr machen dieses Convertible zu einem modernen 
und großartigen Gerät.',
5, 3),
('ZBook 15 G4', 847.99, 
'~/src/images/notebooks/hp-zbk-15g4.jpg',
'Der HP ZBook 15 G4 ist eine erstaunlich leistungskräftige mobile Workstation. Mit dem Intel Core i7-7820HQ - ein echter
Vierkerner mit Turbotakt bis 3.9 GHz und konstanten Basetakt bei 2.9 GHz wird hier die Basis gesetzt für problemlose Berechnung
von aufwendigen Programmen. In Kombination mit der Quadro M2200 von NVIDIA hat man hier für CAD, Videobearbeitung und Co
reichlich Leistung. Vorbei sind auch die Zeiten, wo solch kräftige Notebooks noch 4 kg und mehr wogen - trotz großem 15.6 Zoll
Displays, Nummernpad und reichlich Leistung bleibt das vollständig erneuerte Notebook mit 2.60 kg und gerade einmal 26mm Höhe
noch erstaunlich portabel.',
11, 3),
('Galaxy Book', 842.99, 
'~/src/images/notebooks/samsung-glxy.jpg',
'Das Galaxy Book mit einem durchweg hohen Arbeitstempo, die Akkulaufzeit ist lang, die Ausstattung nahezu vollzählig.
Das Display hat eine hohe Farbtreue. Das Samsung Galaxy Book mit Core i5-CPU hat 8 Gigabyte Arbeitsspeicher und 512-GB-SSD.',
1, 3),
('MacBook Air 2013', 233.99,
'~/src/images/notebooks/apple-macbook-air-2013.jpg',
'Außen klein - Innen fein Dieses Apple MacBook Air 2013 besticht vor allem durch sein besonderes Design, das in der Welt
der Laptops vergeblich seinesgleichen sucht. Es ist mit 1.08 kg nicht nur ein Federgewicht, sondern sogar so klein, dass es
bequem in einen B4-Briefumschlag passt. Selbst an seiner dicksten Stelle ist dieses MacBook Air nur 1.7 cm hoch und an der
dünnsten nur atemberaubende 3 mm! Mit diesen kompakten Maßen ist das Gerät ideal zum Reisen oder für unterwegs geeignet.
Das Modell ist aber trotz des schlanken Designs immer noch äußerst robust und steht dank der vier Gummifüße rutschfest und sicher
auf glatten Flächen!',
3, 3),
('Aspire 5 A515-44', 633.99, 
'~/src/images/notebooks/acer-aspire-5-a515-44.jpg', 
'Acer Aspire 5 (A515-44-R0NR) 39,6 cm (15,6 Zoll Full-HD IPS matt) Multimedia Laptop (AMD Ryzen 5 4500U, 8 GB RAM,
256 GB PCIe SSD, AMD Radeon Graphics, Win 10 Home) silber.',
10, 3),
('ZBook 15', 602.99, 
'~/src/images/notebooks/hp-zbook-15-i7-15.jpg',
'Mit der mobilen Workstation HP ZBook 15 können Sie fast überall arbeiten. Mobilität und Leistungsstärke sorgen
für eine unschlagbare Kombination aus Design, Funktion und Tragbarkeit in einem flachen und leichten Formfaktor mit 15.6 Zoll Diagonale.',
11, 3),
('ThinkPad T480', 538.99,
'~/src/images/notebooks/lenovo-tp-t480.jpg',
'Das Lenovo ThinkPad T480 ist ein top modernes Business-Notebook, welches auf zuverlässigen dauerhaften Betrieb ausgelegt
wurde und daher stabil und solide gebaut wurde. Mit Features wie Akkuwechsel im laufenden Betrieb, Thunderbolt-Anschluss und Docking
Möglichkeiten richtet sich das vollständig erneuerte Lenovo ThinkPad T480 definitiv an Power-User, aber kann natürlich auch als ganz
normaler Laptop daheim und unterwegs genutzt werden. Der leistungskräftige Intel Core i5-8350U arbeitet zuverlässig auch multiple
Programme ab und bietet immer genügend Leistungsreserven!',
5, 3);