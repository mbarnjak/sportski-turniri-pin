# Brze upute za pokretanje

## Preduvjeti
- .NET 8.0 SDK (`sudo pacman -S dotnet-sdk-8.0`)
- ASP.NET Core Runtime 8.0 (`sudo pacman -S aspnet-runtime-8.0`)

## Pokretanje

```bash
# Postavi DOTNET_HOME ako imaš problema s dozvolama (Linux)
export DOTNET_HOME=/home/pezo/NetBeansProjects/turniri/.dotnet
mkdir -p .dotnet

# Restore paketa
dotnet restore

# Pokreni aplikaciju
dotnet run
```

Aplikacija će biti dostupna na: `http://localhost:5000`

## Prvi koraci

1. Otvori preglednik i idi na `http://localhost:5000`
2. Klikni "Registriraj se" ili idi na `/Account/Register`
3. Unesi podatke i registriraj se
4. Nakon registracije, možeš kreirati turnir klikom na "+ Objavi turnir"
5. Možeš se prijaviti na postojeće turnire

## Važne napomene

- Baza podataka (SQLite) se automatski kreira pri prvom pokretanju kao `turniri.db`
- Aplikacija koristi SQLite umjesto SQL Server LocalDB za Linux kompatibilnost
- Connection string je već konfiguriran u `appsettings.json`
- Ako aplikacija ne pokrene, provjeri je li runtime instaliran

## Struktura projekta

- `Controllers/` - MVC kontroleri
- `Models/` - Modeli podataka
- `Views/` - Razor view-ovi
- `Data/` - DbContext i konfiguracija baze
- `ViewModels/` - View modeli za validaciju

## Detaljna dokumentacija

Pogledaj `DOKUMENTACIJA.md` za potpunu dokumentaciju.

