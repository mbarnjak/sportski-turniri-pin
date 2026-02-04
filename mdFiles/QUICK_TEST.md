# Brzi test - Koraci za pokretanje

## 1. Provjeri .NET SDK
```bash
dotnet --version
```
Ako nije instaliran, vidi `TESTIRANJE.md` za upute.

## 2. Restore i Build
```bash
cd /home/pezo/NetBeansProjects/turniri
dotnet restore
dotnet build
```

## 3. Pokreni aplikaciju
```bash
dotnet run
```

## 4. Otvori u pregledniku
- Idi na: `https://localhost:5001`
- Ako vidiš upozorenje o certifikatu, klikni "Napredno" → "Nastavi"

## 5. Testiraj osnovne funkcionalnosti

### Test registracije:
1. Klikni "Registriraj se"
2. Unesi podatke i registriraj se
3. Trebao bi biti automatski prijavljen

### Test kreiranja turnira:
1. Klikni "+ Objavi turnir"
2. Unesi podatke turnira
3. Klikni "Objavi turnir"
4. Turnir bi se trebao pojaviti na listi

### Test prijave na turnir:
1. Odjavi se i registriraj novog korisnika
2. Klikni "Prijavi se" na nekom turniru
3. Trebao bi vidjeti poruku o uspjehu

## Ako imaš problema

- **"dotnet: command not found"** → Instaliraj .NET SDK
- **"Cannot connect to SQL Server"** → Koristi SQLite (vidi `TESTIRANJE.md`)
- **Port zauzet** → Promijeni port u `Properties/launchSettings.json`

Za detaljne upute, vidi `TESTIRANJE.md`.

