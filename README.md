# TraderBot

Trader Bot purpose is to copy future orders from following traders from TraderWagon without TraderWagon commission.

Process:

1. Follow trader on TraderWagon with minimal commission rate
2. Get E-Mail notification from TraderWagon and TraderBot will parse and create new order with custom quantity in
   Binance

## Application schema

![Application schema](./Docs/TraderBot.png)

## Technologies and tools

* .NET 7.0
* HTTP/2 with Protocol Buffer v3
* RavenDb for order storage

## Third part dependencies

* MailKitLite, License: MIT
* Binance.Spot, License: MIT
* RavenDB.Client, License: MIT

## External services

* Email server with IMAP protocol
* Binance Future API

# Build and run

1. Create test account and API keys for Binance Future

https://www.binance.com/en/support/faq/how-to-test-my-functions-on-binance-testnet-ab78f9a1b8824cf0a106b4229c76496d

2. Run application

Option 1. Development:

Just use your favorite IDE

Option 2. User:

> cat ./.env.example > ./.env

Set telegram credentials in  `.env` file

`TELEGRAM_KEY` - Key from your Telegram Bot
`TELEGRAM_CHAT` - Default chat id where application will post messages

Start database first

> docker-compose up -d ravendb

Start application

> docker-compose up

3. Open [http://127.0.0.1:5173/](http://127.0.0.1:5173/) in browser

# Tests

All test projects should placed to ./Tests folder

Execute

> dotnet test

# CI/CD

Section WIP.

## SBOM Task

> TraderBot % sbom-tool generate -b ./SBOM -bc . -pn TraderBot -pv 1.0.0 -ps "TraderBot LLC" -nsb urn:TraderBot

# Change Log

All changes described in [CHANGELOG.md](./CHANGELOG.md)

# Guarantee and Security

This project is not covered by the security advisory policy. Use at your own risk.

# License

License described in [LICENSE.md](./LICENSE.md)

# Development

