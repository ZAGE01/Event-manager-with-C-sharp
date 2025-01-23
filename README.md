# Harjoitustyö

## Kääntäminen ja ajo
Ohjelma voidaan kääntää Windowsilla esimerkiksi Powershellin kautta.

Pura zip paketin sisältö haluamaasi paikkaan ja siirry kyseiseen hakemistoon Powershellillä.

Tämän jälkeen ohjelmaa voi kääntää ja ajaa ”dotnet run” -komennolla. Voit esimerkiksi kirjoittaa ”dotnet run help” näkeäksesi kaikki mahdolliset parametrit ja lisäoptiot.

Ohjelman kansiossa sijaitsee ”events.csv” -tiedosto, jota käytetään ohjelman ajamiseen. Tiedostosta voidaan näyttää ja poistaa tapahtumia sekä sinne voidaan lisätä uusia tapahtumia.

Ohjelma toimii kutakuinkin samalla tavalla kuin harjoitustyön ohjeiden esimerkit. Ohjelmaa ei kuitenkaan ajeta ”days” komennolla vaan tavallisella ”dotnet run”:lla tyyliin: ”dotnet run list --before-date 2024-1-1 --after-date 2023-1-1”.

Toinen ero harjoitustyön ohjeiden esimerkkeihin on ”--exclude”. Sen sijaan, että --exclude laitettaisiin listattujen kategorioiden perään, sitä käytetään omana yksittäisenä optionaan tyyliin: ”dotnet run list --exclude microsoft,holidays”. Tämä näyttää kaikki muut kategoriat paitsi ”microsoft” ja ”holidays”.

## Käytetyt työkalut ja kirjastot
Tein harjoitustyön .NET versiolla 8.0.200 ja C# kielellä.

Käytin Windowsia ja kehitysympäristönä toimi Visual Studio Code.

Käytin Powershelliä ohjelman ajamiseen ja testaukseen.

Käytin ohjelmassa C# kielen standardikirjastojen lisäksi System.IO -kirjastoa tiedostojen käsittelyyn sekä System.Linq -kirjastoa LINQ ominaisuuden hyödyntämiseen.

## Ohjelman rakenne
Ohjelma sisältää kaksi pääluokkaa ’Program’ ja ’Event’. Tämän lisäksi samassa
kansiossa on myös ’events.csv’ -tiedosto, jossa tapahtumat sijaitsevat.

#### 1. ’Program’ -luokka toimii ohjelman käyttöliittymänä ja sisältää toiminnot sisältävän switchin. Luokka sisältää myös kaikki funktiot ja ohjelman ajossa tarvittavat toiminnallisuudet.

Luokasta löytyy funktiot:

• EventsHelp(), tulostaa käyttäjälle mahdolliset toiminnot ja lisäoptiot

• ListEvents(), tapahtumien listaus

• FilterEvents(), tapahtumien lajittelu

• AddEvent(), tapahtumien lisäys

• SaveEvent(), lisättyjen tapahtumien tallennus tiedostoon

• DeleteEvents(), tapahtumien poistaminen

• SaveEvents(), poistettujen tapahtumien päivitys tiedostoon

• ShouldDelete(), tarkistetaan, mitkä tapahtumat kuuluu poistaa

• ReadEvents(), tapahtumien luku tiedostosta, käytetään listaukseen ja poistamiseen.

#### 2 ’Event’ -luokka kuvaa yksittäistä tapahtumaa ja sisältää sen ominaisuudet sekä vertailuun ja muotoiluun liittyviä toimintoja.

## Ongelmat
Minulla oli aluksi hieman vaikeuksia C# kielen käytössä, koska tämä oli ensimmäinen isompi tehtävä, jonka tein kyseisellä kielellä. Vastaan tuli paljon ongelmia päivämäärien sekä DateOnlyn ja DateTimen käytössä, koska yritin aluksi käyttää DateTimeä ja lopulta päädyin käyttämään DateOnlyä. Tämän jälkeen koodissani oli useita kohtia, joita piti muuttaa DateOnlyä varten. 

Tapahtumia lisätessä suurin ongelma oli tiedostoon lisättyjen tapahtumien muotoilu. Koodissani oli pari pientä virhettä, joiden vuoksi lisättyjen tapahtumien kategoriat ja kuvaukset kirjoitettiin tiedostoon väärässä järjestyksessä ja tämän takia tapahtumien listaaminen kategorioiden perusteella ei toiminut kuin pitäisi.

Tapahtumien poistamisen kanssa minulla oli eniten ongelmia. ’--all’ toiminto ei aluksi suostunut toimimaan kuin piti ja kaikki tapahtumat poistettiin aina joka tapauksessa. Myös dry-runin toteuttamisessa esiintyi paljon virheitä, se ei esimerkiksi näyttänyt oikeaa määrää poistettavia tapahtumia eikä muutenkaan toiminut kuten halusin. Lopulta onnistuin korjaamaan virheet suunnittelemalla poistofunktion toimintaa hieman tarkemmin ja miettimällä funktiossa käytettävää logiikkaa.

# Omaa pohdintaa projektistani
Mielestäni ohjelma toimii hyvin ja suurin piirtein niin kuin pitäisi. ’Program’ -luokka on kuitenkin hieman sekava ja paikoittain turhan pitkä sekä monimutkainen. Minun olisi pitänyt luoda enemmän luokkia ja mahdollisesti jonkinlainen EventManager -luokka, johon osan toiminnallisuuksista olisi voinut sisällyttää. Olisin voinut myös suunnitella toteutusta hieman paremmin.
