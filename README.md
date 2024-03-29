# Website
My personal website

[![CI](https://github.com/DominicMaas/Website/actions/workflows/main.yml/badge.svg)](https://github.com/DominicMaas/Website/actions/workflows/main.yml)

## Technologies
 - ASP.NET Core 
 - Docker
 - Azure Key Vault
 - CloudFlare R2 (via AWS SDK)

## Neso

Neso is the VPS that this site runs off of. It runs this website, a docker registry, watchtower a SQLite browser all within docker.

## Database Platform

Dynamic data is stored within a SQLite database stored on Neso. [Lightstream](https://litestream.io/) is used to replicate the database.

