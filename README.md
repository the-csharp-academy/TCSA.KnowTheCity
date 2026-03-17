# Know The City

A .NET MAUI Blazor Hybrid quiz game that tests your ability to identify famous landmarks from cities around the world.

## Purpose of the App

Traveling to a new city is always more rewarding when you can recognize its landmarks at a glance. Studies show that visual memory training improves recall and spatial awareness — skills that come in handy when navigating unfamiliar places.

**Know The City** turns that preparation into a game. By repeatedly identifying landmarks from photos, you build a mental library of iconic sites so that when you arrive at your destination, you already feel like you know the place. Think of it as flashcard training for travelers: fun, fast, and surprisingly effective.

## How to Play

1. **Choose a city** — On the home screen, tap one of the city cards.
2. **Identify landmarks** — You are shown a photo of a landmark and have 10 seconds per landmark to finish the quiz.
3. **Type your answer** in the text field and tap **Submit** (or press Enter).
   - **Correct** — you score a point and move to the next landmark.
   - **Wrong** — the correct answer is revealed and you move on.
4. **Skip** — If you don't know, tap the **"I Don't Know"** button. The answer is revealed and you move to the next landmark.
5. **Results** — After all landmarks (or when the timer runs out), a summary shows which landmarks you got right, wrong, or skipped.
6. Tap **Back to Cities** to try another city.

## Architecture Overview

The solution is split into a .NET MAUI Blazor Hybrid app and a shared `TCSA.KnowTheCity.Core` project.

- **.NET MAUI** hosts a `BlazorWebView` that renders Razor components natively on each platform.
- **Razor components** (`Home.razor`, `Quiz.razor`) handle all UI via MudBlazor.
- **Manifest-driven catalog** keeps the content separate from the app binary. The app downloads `cities.json` first, then each city-specific manifest, so new cities or landmarks can be added by publishing new manifest files and assets to the CDN without shipping a new app update.
- **Sync-on-open strategy** keeps the local catalog fresh without hitting the CDN on every launch. When the home page loads, the sync service checks the last successful sync timestamp and only refreshes the manifests if the previous sync happened more than 24 hours ago.
- **Local persistence** uses SQLite on the device to store cities, landmarks, favourites, and sync metadata, so the app can work from a cached catalog after the initial sync.
- **CDN-backed assets** are resolved from manifest paths, while the image cache service downloads and reuses city, landmark, and flag images locally for better startup and scrolling performance.

## Running the App Locally

The repository includes a **CDN simulator** project so the MAUI app can be tested locally without pointing to a real remote CDN.

### 1. Start the CDN simulator

- Set **`TCSA.KnowTheCity.CdnSimulator`** as the startup project, or include it in a multiple startup profile.
- The simulator serves content from `TCSA.KnowTheCity.CdnSimulator/wwwroot` for city, landmark, and flag images
- By default it listens on `http://localhost:5264`.

### 2. Point the MAUI app to the simulator

In `TCSA.KnowTheCity/appsettings.json`, set:

- `Config:CdnUrl = http://localhost:5264`

This makes the app request manifests and images from the local simulator instead of the production CDN.

### 3. Run the MAUI app

- Start **`TCSA.KnowTheCity`**.
- On first load, the home page triggers the sync process.
- If the last sync is older than 24 hours, the app pulls the latest `cities.json`, downloads each city manifest, and updates the local SQLite catalog.

### 4. Test content changes locally

- Add or edit files in `TCSA.KnowTheCity.Core/Manifests` to simulate catalog updates.
- Add or replace images under `TCSA.KnowTheCity.CdnSimulator/wwwroot`.
- Restart the simulator if needed, then rerun the app or force a fresh sync by clearing the local app data.
