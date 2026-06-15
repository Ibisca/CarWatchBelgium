# CarWatch Belgium

CarWatch Belgium is a web application and browser extension for monitoring car listings from Belgium, mainly from AutoScout24.be and Facebook Marketplace.

The project is built as a local MVP with a clean backend API, a Vue dashboard, and a passive browser extension that reads only the listings visible in the user's browser page.

## Features

* User registration and login with JWT authentication
* Saved car searches with filters:

  * make and model
  * price range
  * year range
  * maximum mileage
  * fuel type
  * transmission
  * minimum power
  * seller type
  * city and radius
  * required and excluded keywords
* Listing ingestion from browser extension
* Price history tracking
* Price drop detection
* Listing states per user:

  * new
  * seen
  * favorite
  * ignored
* Web dashboard for managing searches and listings
* Chrome/Edge extension for scanning visible listings
* Swagger documentation for API testing

## Tech Stack

### Backend

* C# ASP.NET Core Web API
* Entity Framework Core
* SQLite
* JWT Bearer Authentication
* Swagger / OpenAPI
* Simplified Clean Architecture

### Frontend

* Vue 3
* TypeScript
* Pinia
* Vue Router
* PrimeVue
* Axios
* Vite

### Browser Extension

* Chrome / Edge Extension
* Manifest V3
* Vue 3
* TypeScript
* MutationObserver
* Chrome notifications

## Project Structure

```txt
CarWatchBelgium
├── CarWatchBelgium.Api
├── CarWatchBelgium.Application
├── CarWatchBelgium.Domain
├── CarWatchBelgium.Infrastructure
├── CarWatchBelgium.Web
├── CarWatchBelgium.Extension
└── README.md
```

## Backend Architecture

The backend follows a simplified Clean Architecture structure:

```txt
Domain
- Entities
- Enums

Application
- DTOs
- Service interfaces

Infrastructure
- EF Core DbContext
- Service implementations

Api
- Controllers
- Authentication setup
- Swagger
```

## Core Domain Model

Main entities:

* `User`
* `SavedSearch`
* `Listing`
* `SavedSearchMatch`
* `PriceHistory`
* `UserListingState`

Important design decision:

`Listing` represents a real car listing and is independent from `SavedSearch`.

A listing can match multiple saved searches through `SavedSearchMatch`.

This avoids duplicated price history and duplicated favorite/seen states for the same real listing.

## Browser Extension Rules

The extension is passive and respects the following restrictions:

* It does not use paid APIs.
* It does not bypass authentication.
* It does not bypass CAPTCHA.
* It does not bypass platform protections.
* It does not send AutoScout24 or Facebook cookies, passwords, or platform tokens to the backend.
* It only extracts data from the DOM already rendered and visible to the user.
* Scanning is triggered manually by the user.

## API Base URL

Default local API URL:

```txt
http://localhost:5003
```

## Getting Started

### 1. Backend

From the root folder:

```bash
dotnet build
```

Run the API:

```bash
dotnet run --project CarWatchBelgium.Api
```

The API should be available at:

```txt
http://localhost:5003
```

Swagger:

```txt
http://localhost:5003/swagger
```

### 2. Database

The project uses SQLite.

If the database does not exist, run:

```bash
dotnet ef database update --project CarWatchBelgium.Infrastructure --startup-project CarWatchBelgium.Api
```

### 3. Web Dashboard

Go to the frontend folder:

```bash
cd CarWatchBelgium.Web
npm install
npm run dev
```

Default frontend URL:

```txt
http://localhost:5173
```

Build frontend:

```bash
npm run build
```

### 4. Browser Extension

Go to the extension folder:

```bash
cd CarWatchBelgium.Extension
npm install
npm run build
```

Then load the extension manually:

1. Open Chrome or Edge
2. Go to `chrome://extensions`
3. Enable Developer Mode
4. Click `Load unpacked`
5. Select the extension `dist` folder

## Basic Test Flow

1. Start the backend API.
2. Open Swagger.
3. Register a user.
4. Login and copy the JWT token.
5. Create a saved search.
6. Open the web dashboard and login.
7. Open AutoScout24.be or Facebook Marketplace.
8. Use the browser extension to scan visible listings.
9. Return to the dashboard and check matched listings.
10. Mark listings as seen, favorite, or ignored.

## Main API Endpoints

### Auth

```txt
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/me
```

### Saved Searches

```txt
GET    /api/saved-searches
POST   /api/saved-searches
GET    /api/saved-searches/{id}
PUT    /api/saved-searches/{id}
PATCH  /api/saved-searches/{id}/deactivate
DELETE /api/saved-searches/{id}
```

### Listings

```txt
POST /api/listings/ingest
GET  /api/listing-queries/saved-search/{savedSearchId}
GET  /api/listing-queries/favorites
GET  /api/listing-queries/unseen
GET  /api/listing-queries/{listingId}
```

### Listing States

```txt
PATCH /api/listing-states/{listingId}
PATCH /api/listing-states/{listingId}/seen
PATCH /api/listing-states/{listingId}/favorite/toggle
PATCH /api/listing-states/{listingId}/ignore
POST  /api/listing-states/mark-seen
```

## Environment Notes

Do not commit:

```txt
node_modules
bin
obj
dist
.env
*.db
```

The JWT key used in development must be replaced before production use.

## Current Status

This project is currently a local MVP.

Implemented:

* Backend API
* JWT authentication
* Saved searches
* Listing ingestion
* Price history
* Price drop detection
* Listing states
* Vue web dashboard
* Browser extension MVP

Planned improvements:

* Better browser extension selectors
* Better UI polish
* Advanced filters and sorting
* Optional Telegram notifications
* Production deployment configuration
* Refresh token support
* Automated tests

## Disclaimer

This project is not affiliated with AutoScout24, Facebook, or Meta.

The browser extension is designed to work only with content already visible to the user in their own browser session.
