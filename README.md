# Ensembl Data Service

Shared architecture, development workflows, and agent instructions:
[UNITE project documentation](https://github.com/dkfz-unite/.github/blob/main/README.md).

## General
Ensembl data service is a WEB wrapper over Ensembl homo sapience database and provides the access to it's data.
- [Ensembl Data API](./Docs/api.md) - REST API for accessing required information about Ensembl genes, transcripts and proteins.


## Rationale
Reasons why custom API is required:
- Web based - Existing Ensembl API is a CLI interface, but UNITE is WEB based application which requires WEB services to operate.
- Simple - Current API is very simple to install and use.
- Small - Current API includes only parts critically required for UNITE and doesn't include other Enseml data.
- Fast - Current API is installed locally close to existing services and is used only by running instance of the UNITE application, so it neglects most of network response times.


## Dependencies
- [MySQL](https://github.com/dkfz-unite/unite-environment/tree/main/programs/mysql) - MySQL server with Ensembl homo sapience database.


## Access
Environment|Address|Port
-----------|-------|----
Host|http://localhost:5200|5200
Docker|http://data.ensembl.unite.net|80


## Configuration
To configure the application, change environment variables in either docker or [launchSettings.json](Ensembl.Data.Web/Properties/launchSettings.json) file (if running locally):
Variable|Description|Default(Local)|Default(Docker)
--------|-----------|--------------|---------------
ASPNETCORE_ENVIRONMENT|ASP.NET environment|Debug|Release
ENSEMBL_SQL_HOST|SQL server host|localhost|mysql.unite.net
ENSEMBL_SQL_PORT|SQL server port|3306|3306
ENSEMBL_SQL_USER|SQL server user||
ENSEMBL_SQL_PASSWORD|SQL server password||

The service uses Ensembl release 116 (the last release published on ftp.ensembl.org): databases `homo_sapiens_core_116_37` (GRCh37) and `homo_sapiens_core_116_38` (GRCh38).
The genome build is selected per request with the `grch` parameter (`37` by default), only the database of the installation genome build has to be installed.


## Installation

### Docker Compose
The easiest way to install the application is to use docker-compose:
- Environment configuration and installation scripts: https://github.com/dkfz-unite/unite-environment
- Ensembl data service configuration and installation scripts: https://github.com/dkfz-unite/unite-environment/tree/main/applications/unite-ensembl-data

### Docker
[Dockerfile](Dockerfile) is used to build an image of the application.
To build an image run the following command:
```
docker build -t unite.ensembl.data:latest .
```

All application components should run in the same docker network.
To create common docker network if not yet available run the following command:
```bash
docker network create unite
```

To run application in docker run the following command:
```bash
docker run \
--name unite.ensembl.data \
--restart unless-stopped \
--net unite \
--net-alias data.ensembl.unite.net \
-p 127.0.0.1:5200:80 \
-e ASPNETCORE_ENVIRONMENT=Release \
-e ENSEMBL_SQL_HOST=mysql.unite.net \
-e ENSEMBL_SQL_PORT=3306 \
-e ENSEMBL_SQL_USER=[sql_user] \
-e ENSEMBL_SQL_PASSWORD=[sql_password] \
-d \
unite.donors.feed:latest
```
