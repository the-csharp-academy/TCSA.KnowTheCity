# Know The City

A .NET MAUI Blazor Hybrid quiz game that tests your ability to identify famous landmarks from cities around the world.

## Purpose of the Game

Traveling to a new city is always more rewarding when you can recognize its landmarks at a glance. Studies show that visual memory training improves recall and spatial awareness — skills that come in handy when navigating unfamiliar places.

**Know The City** turns that preparation into a game. By repeatedly identifying landmarks from photos, you build a mental library of iconic sites so that when you arrive at your destination, you already feel like you know the place. Think of it as flashcard training for travelers: fun, fast, and surprisingly effective.

## How to Play

1. **Choose a city** — On the home screen, tap one of the city cards.
2. **Identify landmarks** — You are shown a photo of a landmark and have **60 seconds** to work through all 5.
3. **Type your answer** in the text field and tap **Submit** (or press Enter).
   - **Correct** — you score a point and move to the next landmark.
   - **Wrong** — the correct answer is revealed and you move on.
4. **Skip** — If you don't know, tap the **"I Don't Know"** button. The answer is revealed and you move to the next landmark.
5. **Results** — After all 5 landmarks (or when the timer runs out), a summary shows which landmarks you got right, wrong, or skipped.
6. Tap **Back to Cities** to try another city.

## Architecture Overview

The solution is split into a .NET MAUI Blazor Hybrid app and a shared `TCSA.KnowTheCity.Core` project.

### Why a Core project?

`TCSA.KnowTheCity.Core` contains the shared domain models, Entity Framework Core data access, options, HTTP clients, and sync logic. Keeping this code outside the MAUI app is important because the application uses Blazor Hybrid UI on top of .NET MAUI, while the business logic still needs to be exercised independently in tests. By placing the data and synchronization logic in the Core project, the integration test project can reference the same code without depending on MAUI platform infrastructure or UI components.

### How the CDN works

The app does not hardcode all city and monument content directly into the client. Instead, it reads a base CDN URL from configuration and uses it to fetch JSON manifests and images:

- `manifests/cities.json` provides the list of available cities.
- Each city points to its own manifest with monument details.
- City and monument images are also requested from the same CDN base URL.

This keeps the app lightweight and makes content updates easier, because new or updated cities can be published to the CDN without requiring a full application rebuild.

### How caching works

Caching happens at two levels:

1. **Structured content cache**  
   City and monument metadata fetched from the CDN is synchronized into the local SQLite database through `SyncService`. The app checks the last sync timestamp and only refreshes when the cached data is older than the sync window. This means the UI reads from local persisted data most of the time instead of repeatedly requesting manifests.

2. **Image file cache**  
   Images are downloaded on demand from the CDN and stored in the device cache directory by `ImageCacheService`. If an image already exists locally, the cached file path is reused instead of downloading it again. This reduces network traffic and improves perceived performance.

Together, these two layers allow the app to work efficiently with remote content while still feeling local and responsive.

### Favorites and studying flow

The app stores favorite cities and favorite landmarks in the local database. Besides acting as a bookmarking feature, this also opens the door to a useful study workflow: favorites can serve as a smaller personalized subset of content to review repeatedly instead of studying the full catalog every time.

### Image consistency

Most of the images used in the app are AI-generated. This makes it easier to keep a consistent visual style across cities and monuments, and it also simplifies formatting so assets fit more naturally within the app's layout and design system.
