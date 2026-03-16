# Know The City

A .NET MAUI Blazor Hybrid quiz game that tests your ability to identify famous landmarks from cities around the world.

## Purpose of the Game

Traveling to a new city is always more rewarding when you can recognize its landmarks at a glance. Studies show that visual memory training improves recall and spatial awareness — skills that come in handy when navigating unfamiliar places.

**Know The City** turns that preparation into a game. By repeatedly identifying landmarks from photos, you build a mental library of iconic sites so that when you arrive at your destination, you already feel like you know the place. Think of it as flashcard training for travelers — fun, fast, and surprisingly effective.

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

The app follows the **Blazor Hybrid** pattern:

- **.NET MAUI** hosts a `BlazorWebView` that renders Razor components natively on each platform.
- **Razor components** (`Home.razor`, `Quiz.razor`) handle all UI via MudBlazor.
- **`CityData`** is a static helper class that serves as the single source of truth for city names, landmarks, and image path generation.
- Image paths follow a normalized naming convention: `{city}-{landmark}.png` (lowercase, alphanumeric only), produced by `CityData.GetLandmarkImagePath()`.

- Test
