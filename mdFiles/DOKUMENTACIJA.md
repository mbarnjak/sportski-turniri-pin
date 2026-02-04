# Dokumentacija - Platforma za prijavu i organizaciju sportskih turnira

## 1. Uvod

Aplikacija "Platforma za prijavu i organizaciju sportskih turnira" je web aplikacija izrađena u ASP.NET Core MVC tehnologiji koja omogućava korisnicima registraciju, prijavu i organizaciju sportskih turnira. Korisnici mogu kreirati turnire, pregledavati dostupne turnire i prijavljivati se na njih.

## 2. Korištene tehnologije

- **ASP.NET Core 8.0 MVC** - Glavni framework za izradu web aplikacije
- **Entity Framework Core 8.0** - ORM za rad s bazom podataka
- **ASP.NET Core Identity** - Sustav za autentifikaciju i autorizaciju korisnika
- **SQLite** - Relacijska baza podataka (za Linux kompatibilnost)
- **Tailwind CSS** - CSS framework za stilizaciju (korišten preko CDN-a)
- **Razor Views** - Template engine za generiranje HTML-a

## 3. Karakteristike aplikacije

### 3.1. Autentifikacija i autorizacija
- Registracija novih korisnika s validacijom podataka
- Prijava korisnika s opcijom "Zapamti me"
- Odjava korisnika
- Zaštita ruta koje zahtijevaju autentifikaciju

### 3.2. Upravljanje turnirima
- **Kreiranje turnira**: Autentificirani korisnici mogu kreirati nove turnire s podacima:
  - Naziv turnira
  - Sport (slobodan unos)
  - Broj igrača
  - Datum i vrijeme održavanja
- **Pregled turnira**: Svi korisnici (autentificirani i neautentificirani) mogu pregledavati listu dostupnih turnira
- **Prijava na turnir**: Autentificirani korisnici mogu se prijaviti na turnire
- **Odjava s turnira**: Korisnici mogu se odjaviti s turnira na koji su se prijavili
- **Zaštita od duplih prijava**: Sustav sprječava višestruke prijave istog korisnika na isti turnir
- **Provjera kapaciteta**: Sustav provjerava je li turnir pun prije prijave

### 3.3. Korisničko sučelje
- Moderni, responzivni dizajn s tamnom temom
- Intuitivna navigacija
- Poruke o uspjehu i greškama
- Validacija podataka na klijentskoj i serverskoj strani

## 4. Struktura baze podataka

### 4.1. Tablice

#### ApplicationUser (nasljeđuje IdentityUser)
- `Id` - Jedinstveni identifikator korisnika
- `UserName` - Korisničko ime
- `Email` - Email adresa
- `Ime` - Ime korisnika
- `DatumRegistracije` - Datum registracije

#### Tournament
- `Id` - Jedinstveni identifikator turnira
- `Naziv` - Naziv turnira
- `Sport` - Vrsta sporta
- `BrojIgraca` - Maksimalni broj igrača
- `Datum` - Datum održavanja turnira
- `Vrijeme` - Vrijeme održavanja turnira
- `OrganizatorId` - ID organizatora (FK na ApplicationUser)
- `CreatedAt` - Datum kreiranja turnira

#### TournamentRegistration
- `Id` - Jedinstveni identifikator prijave
- `TournamentId` - ID turnira (FK na Tournament)
- `UserId` - ID korisnika (FK na ApplicationUser)
- `DatumPrijave` - Datum prijave

### 4.2. Relacije
- Jedan korisnik može kreirati više turnira (1:N)
- Jedan korisnik može se prijaviti na više turnira (N:M preko TournamentRegistration)
- Jedan turnir može imati više prijava (1:N)

## 5. Kako namjestiti aplikaciju na računalo

### 5.1. Preduvjeti
- **.NET 8.0 SDK** ili noviji - Preuzeti s [Microsoftove stranice](https://dotnet.microsoft.com/download)
- **SQL Server LocalDB** - Uključen u Visual Studio ili se može instalirati zasebno
- **Visual Studio 2022** ili **Visual Studio Code** (opcionalno, ali preporučeno)

### 5.2. Koraci za instalaciju

1. **Kloniranje ili preuzimanje projekta**
   ```bash
   git clone <repository-url>
   cd turniri
   ```

2. **Instalacija ovisnosti**
   ```bash
   dotnet restore
   ```

3. **Konfiguracija baze podataka**
   - Aplikacija koristi SQLite koji se automatski kreira pri prvom pokretanju
   - Connection string je već konfiguriran u `appsettings.json`:
     ```
     Data Source=turniri.db
     ```
   - Baza podataka se kreira kao `turniri.db` u root direktoriju projekta

4. **Pokretanje aplikacije**
   ```bash
   # Na Linux-u, možda trebaš postaviti DOTNET_HOME
   export DOTNET_HOME=/home/pezo/NetBeansProjects/turniri/.dotnet
   mkdir -p .dotnet
   
   dotnet run
   ```
   Aplikacija će se pokrenuti na `http://localhost:5000`

5. **Pristup aplikaciji**
   - Otvoriti web preglednik i navigirati na `http://localhost:5000`
   - Aplikacija automatski preusmjerava na `/Tournament` stranicu

### 5.3. Prvo korištenje
1. Kliknuti na "Registriraj se" ili navigirati na `/Account/Register`
2. Unijeti podatke za registraciju (korisničko ime, email, lozinka)
3. Nakon registracije, automatski ćete biti prijavljeni
4. Možete kreirati novi turnir klikom na "+ Objavi turnir"
5. Možete se prijaviti na postojeće turnire

## 6. Struktura projekta

```
turniri/
├── Controllers/          # MVC kontroleri
│   ├── AccountController.cs
│   ├── HomeController.cs
│   └── TournamentController.cs
├── Data/                 # Data access layer
│   └── ApplicationDbContext.cs
├── Models/               # Domain modeli
│   ├── ApplicationUser.cs
│   ├── Tournament.cs
│   └── TournamentRegistration.cs
├── Views/                # Razor view-ovi
│   ├── Account/
│   ├── Home/
│   ├── Tournament/
│   └── Shared/
├── ViewModels/           # View modeli
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   └── TournamentViewModel.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs            # Entry point aplikacije
├── appsettings.json      # Konfiguracija
└── Turniri.csproj       # Projektna datoteka
```

## 7. Sigurnost

- Lozinke se hashiraju pomoću ASP.NET Core Identity sustava
- Zaštita od CSRF napada korištenjem anti-forgery tokena
- Validacija podataka na klijentskoj i serverskoj strani
- Autorizacija za zaštićene rute (kreiranje turnira, prijava na turnir)
- SQL injection zaštita korištenjem Entity Framework Core parametriziranih upita

## 8. Buduća poboljšanja

- Dodavanje profila korisnika s dodatnim informacijama
- Mogućnost uređivanja i brisanja turnira
- Pretraga i filtriranje turnira
- Email notifikacije za prijave na turnire
- Administratorski panel za upravljanje korisnicima i turnirima
- Komentari i ocjene turnira
- Kalendar prikaza turnira

## 9. Autorska prava

© 2025 Sportski turniri. Sva prava pridržana.

