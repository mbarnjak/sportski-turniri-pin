# Platforma za prijavu i organizaciju sportskih turnira

Web aplikacija za organizaciju i prijavu na sportske turnire, izrađena u ASP.NET Core MVC.

## Brzi start

### Preduvjeti
- .NET 8.0 SDK
- ASP.NET Core Runtime 8.0 (na Linux-u: `sudo pacman -S aspnet-runtime-8.0`)

### Pokretanje

```bash
# Restore paketa
dotnet restore

# Pokreni aplikaciju
dotnet run
```

Aplikacija će biti dostupna na `http://localhost:5000`

**Napomena za Linux:** Ako imaš problema s dozvolama, postavi:
```bash
export DOTNET_HOME=/home/pezo/NetBeansProjects/turniri/.dotnet
mkdir -p .dotnet
```

## Funkcionalnosti

- ✅ Registracija i prijava korisnika
- ✅ Kreiranje turnira
- ✅ Pregled dostupnih turnira
- ✅ Prijava na turnire
- ✅ Odjava s turnira
- ✅ Validacija podataka
- ✅ Responsive dizajn s tamnom temom
- ✅ Moderni UI s Tailwind CSS

## Tehnologije

- ASP.NET Core 8.0 MVC
- Entity Framework Core
- ASP.NET Core Identity
- SQLite (za Linux kompatibilnost)
- Tailwind CSS (CDN)

## Prvi koraci

1. Otvori preglednik i idi na `http://localhost:5000`
2. Klikni "Registriraj se" ili idi na `/Account/Register`
3. Unesi podatke i registriraj se
4. Nakon registracije, možeš kreirati turnir klikom na "+ Objavi turnir"
5. Možeš se prijaviti na postojeće turnire

## Dokumentacija

Detaljna dokumentacija dostupna u `DOKUMENTACIJA.md`

