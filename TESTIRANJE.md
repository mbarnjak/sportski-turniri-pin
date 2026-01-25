# Vodič za testiranje aplikacije

## Preduvjeti za testiranje

### 1. Instalacija .NET 8.0 SDK

**Na Linux-u:**
```bash
# Dodaj Microsoft repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Instaliraj .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

**Provjera instalacije:**
```bash
dotnet --version
# Trebalo bi prikazati: 8.0.x ili noviji
```

### 2. SQL Server LocalDB ili SQLite

**Na Windows-u:** SQL Server LocalDB je uključen u Visual Studio.

**Na Linux-u:** Preporučeno je koristiti SQLite. Ako želiš koristiti SQLite:

1. Dodaj SQLite paket u `Turniri.csproj`:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
```

2. Promijeni `Program.cs` da koristi SQLite:
```csharp
// Umjesto UseSqlServer, koristi:
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
```

3. Promijeni connection string u `appsettings.json`:
```json
"DefaultConnection": "Data Source=turniri.db"
```

Ili koristi `appsettings.Linux.json` koji već postoji u projektu.

## Koraci za testiranje

### Korak 1: Restore paketa
```bash
cd /home/pezo/NetBeansProjects/turniri
dotnet restore
```

Očekivani rezultat: Paketi se preuzimaju i instaliraju.

### Korak 2: Build projekta
```bash
dotnet build
```

Očekivani rezultat: 
- Build uspješan
- Nema grešaka kompilacije
- Može biti upozorenje o SQL Server LocalDB ako nije instaliran (to je OK)

### Korak 3: Pokretanje aplikacije
```bash
dotnet run
```

Očekivani rezultat:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
      Now listening on: http://localhost:5000
```

### Korak 4: Otvaranje u pregledniku

1. Otvori web preglednik
2. Idi na: `https://localhost:5001` ili `http://localhost:5000`
3. Ako vidiš upozorenje o certifikatu:
   - Klikni "Napredno" ili "Advanced"
   - Klikni "Nastavi na localhost" ili "Proceed to localhost"

## Scenariji testiranja

### Test 1: Registracija korisnika

1. Klikni na "Registriraj se" ili idi na `/Account/Register`
2. Unesi podatke:
   - Korisničko ime: `testuser`
   - Email: `test@example.com`
   - Lozinka: `Test123`
   - Potvrdi lozinku: `Test123`
3. Klikni "Registriraj se"

**Očekivani rezultat:**
- Korisnik je registriran
- Automatski si prijavljen
- Preusmjeren si na stranicu s turnirima

### Test 2: Prijava korisnika

1. Ako si već prijavljen, odjavi se
2. Klikni na "Prijava" ili idi na `/Account/Login`
3. Unesi:
   - Email: `test@example.com`
   - Lozinka: `Test123`
4. Klikni "Prijavi se"

**Očekivani rezultat:**
- Uspješna prijava
- Preusmjeren si na stranicu s turnirima
- Vidiš svoje korisničko ime u headeru

### Test 3: Kreiranje turnira

1. Prijavi se kao korisnik
2. Klikni na "+ Objavi turnir"
3. Unesi podatke:
   - Sport: `Nogomet`
   - Naziv turnira: `Testni turnir`
   - Broj igrača: `10`
   - Datum: Odaberi budući datum
   - Vrijeme: `18:00`
4. Klikni "Objavi turnir"

**Očekivani rezultat:**
- Turnir je kreiran
- Pojavljuje se na listi turnira
- Vidiš svoje ime kao organizatora

### Test 4: Prijava na turnir

1. Ako si organizator turnira, odjavi se i prijavi se kao drugi korisnik
2. Ili kreiraj novi korisnički račun
3. Na listi turnira, klikni "Prijavi se" na nekom turniru

**Očekivani rezultat:**
- Poruka o uspješnoj prijavi
- Broj prijavljenih igrača se povećava
- Gumb se mijenja u "Odjavi se"

### Test 5: Odjava s turnira

1. Klikni "Odjavi se" na turniru na koji si prijavljen

**Očekivani rezultat:**
- Poruka o uspješnoj odjavi
- Broj prijavljenih igrača se smanjuje
- Gumb se mijenja natrag u "Prijavi se"

### Test 6: Validacija podataka

1. Pokušaj registrirati korisnika s neispravnim podacima:
   - Email bez @ znaka
   - Lozinke koje se ne podudaraju
   - Prazna polja

**Očekivani rezultat:**
- Poruke o greškama se prikazuju
- Registracija ne prolazi

### Test 7: Zaštita od duplih prijava

1. Pokušaj se prijaviti na isti turnir dva puta

**Očekivani rezultat:**
- Druga prijava ne prolazi
- Poruka o grešci se prikazuje

### Test 8: Provjera kapaciteta

1. Kreiraj turnir s 2 igrača
2. Prijavi se kao prvi korisnik
3. Odjavi se i prijavi se kao drugi korisnik
4. Prijavi se na turnir
5. Pokušaj se prijaviti kao treći korisnik

**Očekivani rezultat:**
- Nakon 2 prijave, turnir je pun
- Gumb "Prijavi se" je onemogućen
- Prikazuje se "Turnir je pun"

## Provjera baze podataka

Ako želiš provjeriti bazu podataka:

1. Otvori SQL Server Management Studio ili drugi SQL klijent
2. Poveži se na: `(localdb)\mssqllocaldb`
3. Baza podataka se zove: `TurniriDb`
4. Provjeri tablice:
   - `AspNetUsers` - korisnici
   - `Tournaments` - turniri
   - `TournamentRegistrations` - prijave na turnire

## Rješavanje problema

### Problem: "dotnet: command not found"
**Rješenje:** Instaliraj .NET SDK (vidi gore)

### Problem: "Cannot connect to SQL Server"
**Rješenje:** 
- Provjeri je li SQL Server LocalDB instaliran
- Ili promijeni connection string na SQLite za Linux

### Problem: "Port already in use"
**Rješenje:**
```bash
# Pronađi proces koji koristi port
sudo lsof -i :5001
# Zaustavi proces ili promijeni port u launchSettings.json
```

### Problem: "Certificate error"
**Rješenje:** 
- To je normalno za development
- Klikni "Napredno" → "Nastavi na localhost"

## Checklist testiranja

- [ ] Aplikacija se pokreće bez grešaka
- [ ] Registracija korisnika radi
- [ ] Prijava korisnika radi
- [ ] Odjava korisnika radi
- [ ] Kreiranje turnira radi
- [ ] Prijava na turnir radi
- [ ] Odjava s turnira radi
- [ ] Validacija podataka radi
- [ ] Zaštita od duplih prijava radi
- [ ] Provjera kapaciteta radi
- [ ] Dizajn je responzivan
- [ ] Poruke o greškama se prikazuju
- [ ] Poruke o uspjehu se prikazuju

## Napomene

- Prvo pokretanje može biti sporije jer se preuzimaju paketi
- Baza podataka se automatski kreira pri prvom pokretanju
- Ako vidiš upozorenja o SQL Server-u, to je normalno ako nije instaliran - aplikacija će pokušati kreirati bazu pri prvom pristupu

