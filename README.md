# Gestion Transport Scolaire

Application Windows Forms pour gérer les présences et modes de transport des élèves.

## 🚀 Build & Installation

### Prérequis
- .NET 6.0 SDK
- Visual Studio 2022 (ou Visual Studio Code)

### Build depuis les sources

```bash
# Cloner le repository
git clone [url-du-repo]
cd GestionTransportScolaire

# Restaurer les packages NuGet
dotnet restore

# Build en mode Debug
dotnet build

# Build en mode Release
dotnet build -c Release
```

### Créer un exécutable

```bash
# Version simple (nécessite .NET Runtime sur le PC cible)
dotnet publish -c Release -o ./publish

# Version autonome (inclut tout, plus volumineux)
dotnet publish -c Release -r win-x64 --self-contained -o ./publish-standalone
```

### Localisation des fichiers

- **Debug :** `bin/Debug/net6.0/`
- **Release :** `bin/Release/net6.0/`
- **Publish :** `./publish/` ou `./publish-standalone/`

### Packages NuGet utilisés

- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.Design

## 📦 Distribution

L'application génère automatiquement sa base de données SQLite (`school_transport.db`) au premier lancement.

Pour distribuer : copiez tout le dossier de sortie vers le PC cible.