# CarWatch Belgium Browser Extension

MVP browser extension for Chrome/Edge to scan auto listings from AutoScout24 and Facebook Marketplace.

## Features

- ✅ User authentication with email/password
- ✅ JWT token storage in chrome.storage.local
- ✅ Load and select SavedSearch criteria
- ✅ Manual scan of visible listings on page
- ✅ MutationObserver for dynamic content detection
- ✅ DOM-based extraction (no page API access)
- ✅ API integration for listing ingestion
- ✅ Visual badges on scanned listings
- ✅ Chrome notifications for new/price-drop listings
- ✅ Background service worker for API calls

## Build & Setup

### Prerequisites

- Node.js 18+ and npm
- Chrome/Edge browser
- CarWatch Belgium API running on `http://localhost:5003`

### Installation

```bash
cd CarWatchBelgium.Extension
npm install
npm run build
```

Output will be in `dist/` folder.

### Load in Chrome/Edge

**Chrome:**
1. Open `chrome://extensions/`
2. Enable "Developer mode" (top-right toggle)
3. Click "Load unpacked"
4. Select the `dist/` folder

**Edge:**
1. Open `edge://extensions/`
2. Enable "Developer mode" (left sidebar toggle)
3. Click "Load unpacked"
4. Select the `dist/` folder

## Testing Workflow

### 1. Start Backend API

```bash
cd ..
dotnet run --project CarWatchBelgium.Api
# Should run on http://localhost:5003
```

### 2. Create Test User & SavedSearch

**Via Swagger** (`http://localhost:5003/swagger`):

- **Register user:**
  ```
  POST /api/auth/register
  {
    "email": "test@carwatch.local",
    "password": "Test1234",
    "displayName": "Test User"
  }
  ```

- **Create SavedSearch:**
  ```
  POST /api/saved-searches
  {
    "name": "Belgian BMW",
    "make": "BMW",
    "countryCode": "BE",
    "fuelType": "Unknown",
    "transmission": "Unknown",
    "sellerType": "Unknown"
  }
  ```

### 3. Build & Load Extension

```bash
npm run build
# Then load dist/ folder in Chrome/Edge extensions page
```

### 4. Test Login Flow

1. Open extension popup
2. Enter credentials: `test@carwatch.local` / `Test1234`
3. Click "Login"
4. Extension should load saved searches
5. Select "Belgian BMW" from dropdown
6. Status shows: "Connected ✓"

### 5. Test Scan Flow

1. Navigate to `https://www.autoscout24.be/` or Facebook Marketplace
2. In extension popup, click "Scan Current Page"
3. Extension extracts listings from visible cards
4. Sends to API via POST `/api/listings/ingest`
5. Results show in popup:
   - Received count
   - Inserted/Updated/Price Drops
6. Listings are marked with badge: "CarWatch: NEW" or "PRICE DROP"
7. Chrome notification appears if insertedCount > 0 or priceDroppedCount > 0

### 6. Verify in API

**Swagger** - Check listing query:
```
GET /api/listing-queries/saved-search/{savedSearchId}
```

Should show scanned listings with MatchedAtUtc = today.

## Architecture

### Popup (`src/popup/App.vue`)
- Login form
- SavedSearch selector
- Scan button
- Results display
- Status info

### Content Script (`src/content/contentScript.ts`)
- Runs on AutoScout24 & Facebook Marketplace pages
- Detects DOM changes via MutationObserver
- Responds to scan requests from popup
- Marks listings with visual badges

### Background Service Worker (`src/background/serviceWorker.ts`)
- Receives scan data from content script
- Calls API with Bearer token
- Sends Chrome notifications
- Handles authentication via chrome.storage.local

### Scanners
- `autoscout24Scanner.ts`: Defensive selectors, price/mileage/year extraction
- `facebookMarketplaceScanner.ts`: Marketplace-specific extraction

### Shared Utilities
- `apiClient.ts`: login, getSavedSearches, ingestListings
- `authStorage.ts`: JWT token & user info management
- `extensionStorage.ts`: Selected search persistence
- `types.ts`: TypeScript interfaces
- `constants.ts`: API endpoints, storage keys

## Customization

### Change API Base URL

Edit `src/shared/constants.ts`:

```typescript
export const API_BASE_URL = 'http://your-api-url:port'
```

### Add More Platforms

1. Create `src/content/scanners/yourPlatformScanner.ts`
2. Implement scanner function
3. Update `scanCurrentPage()` in `scannerTypes.ts`
4. Add to `manifest.json` content_scripts

## Limitations

- Manual scan only (no automatic background scanning)
- Extracts only visible DOM elements
- No scraping of non-rendered content
- Requires user interaction (cannot automate login)
- Does not store platform credentials

## Security

- JWT tokens stored in `chrome.storage.local` (isolated per extension)
- No password storage (only JWT)
- No API credentials exposed to content scripts
- Background worker handles all API calls
- CORS handled by API backend

## Future Improvements

- Refresh token support
- Advanced filtering in popup
- Favorites/bookmarks panel
- Search history
- Browser sync support
- Scheduled background scans (with user consent)
- Multiple account support

## Troubleshooting

### Extension not loading
- Check `npm run build` completed without errors
- Ensure manifest.json is in dist/
- Verify all JS files are in dist/ (not dist/src/)

### API calls fail
- Verify backend is running on http://localhost:5003
- Check JWT token in chrome.storage (DevTools → Application → Local Storage)
- Look at background service worker console (Extensions page → Service Worker)

### Listings not found
- Check page is loaded completely
- Look at content script console (Inspect page → Console)
- Verify selectors match current HTML structure

## Development

```bash
# Watch mode (not yet configured in vite.config, use npm run build repeatedly)
npm run build

# Preview (if needed)
npm run preview
```

All files are built to `dist/` which is ready for unpacking in Chrome/Edge.

## License

Internal use for CarWatch Belgium project.
