# Content Contribution Guide

This guide explains how to contribute content to **Know The City**.

## How to Add a City

1. Add a new city record to `TCSA.KnowTheCity.Core/Manifests/cities.json`.
2. Create a new city manifest in `TCSA.KnowTheCity.Core/Manifests` (for example, `seattle.json`).
3. Ensure both city and landmark translations are included for all supported languages:
   - `en-US`
   - `es-ES`
   - `pt-BR`
   - `it-IT`
   - `fr-FR`
   - `de-DE`

## How to Add a Landmark

1. Open the existing manifest for that city in `TCSA.KnowTheCity.Core/Manifests`.
2. Add the new landmark entry to the `monuments` collection in that manifest.
3. Ensure landmark translations are included for all supported languages:
   - `en-US`
   - `es-ES`
   - `pt-BR`
   - `it-IT`
   - `fr-FR`
   - `de-DE`

## Pull Request Requirements for Images

- Include the images in the PR.
- Use `.webp` format for all images.
- Add **5** images with ratio **3:2**.
- Add **5** images with ratio **2:3**.
- Use the GPT prompts provided by the maintainers for image generation.

## Local Testing Reminder

For local testing, make the image and manifest changes in the CDN simulator project (`TCSA.KnowTheCity.CdnSimulator/wwwroot`), including both pictures and manifest files.

## Prompts

Use the template below to ask AI to generate consistent image prompts and manifest drafts for a new city pack. Use the example prompts for Melbourne as a reference for style, composition, and content. The AI-generated manifest should follow the same structure as the existing city manifests in `TCSA.KnowTheCity.Core/Manifests`.

Upon generating the prompts and manifest drafts, you can use them to create the actual images and manifest files needed for the new city pack contribution.

You can then copy and paste the generated manifest content into a new file in `TCSA.KnowTheCity.Core/Manifests` and add the new city entry to `cities.json`. The images should be added to the CDN simulator's `wwwroot` folder for local testing. The C# Academy will later upload to the production CDN when ready for release.

Keep in mind that generated images are often large 

### Prompt Template

Use this prompt template:

```text
Generate New {CITY_NAME}:

Create content for Know The City with exactly 5 landmarks.
For each landmark, return:

1) Horizontal 3:2 prompt
2) Vertical 2:3 prompt

Also return:
- 1 city overview prompt (3:2 only)
- A draft `{CITY_SLUG}.json` manifest in the same structure used by Know The City
- A draft entry for `cities.json`

Rules:
- Realistic travel photography style
- Authentic colors and lighting
- Avoid flat symmetrical front-on views
- Include only a few people for scale when relevant
- All paths must use `.webp`
- Landmark image paths must include both desktop and mobile variants
- Include translations for: en-US, es-ES, pt-BR, it-IT, fr-FR, de-DE
```

### Example Prompt: Copy Everything Below Here To Prompt, Just Change City Name

```text
Generate New Melbourne:

Create content for Know The City with exactly 5 landmarks for Melbourne, Australia.
For each landmark, return:

1) Horizontal 3:2 prompt
2) Vertical 2:3 prompt

Also return:
- 1 city overview prompt (3:2 only)
- A draft `melbourne.json` manifest in the same structure used by Know The City
- A draft entry for `cities.json`
```

### Example Landmark Prompt Format

```text
Horizontal 3:2
Flinders Street Station, Melbourne, Australia, iconic yellow Edwardian railway station with central dome and clocks, photographed from a slight angle and slightly from above, wide landscape composition, include part of the intersection, tram tracks and a small number of pedestrians for scale, realistic travel photography, soft daylight, authentic urban atmosphere, detailed architecture, no flat front-on symmetry, horizontal 3:2 aspect ratio

Vertical 2:3
Flinders Street Station, Melbourne, Australia, vertical travel photo from a slight angle and slightly from above, emphasizing the central dome, clocks, facade, tram lines and surrounding city context, a few people only, realistic lighting, authentic colors, detailed architecture, avoid flat symmetrical front view, composed for 2:3 aspect ratio
```

### Example City Overview Prompt (3:2 only)

```text
Melbourne, Australia, panoramic city overview from above, wide landscape composition, shot at a slight angle and slightly from above, showing the CBD skyline, Yarra River, bridges, dense urban grid, recognizable mix of modern skyscrapers and historic architecture, soft natural daylight, realistic travel photography, lively but not overcrowded, cinematic clarity, rich detail, authentic colors, strong sense of place, no flat front-on composition, horizontal 3:2 aspect ratio.
```

### Example AI-Generated `melbourne.json`

```json
{
  "cityRemoteId": "melbourne-au",
  "version": "1.0",
  "cityNames": [
    { "languageCode": "en-US", "name": "Melbourne" },
    { "languageCode": "es-ES", "name": "Melbourne" },
    { "languageCode": "pt-BR", "name": "Melbourne" },
    { "languageCode": "it-IT", "name": "Melbourne" },
    { "languageCode": "fr-FR", "name": "Melbourne" },
    { "languageCode": "de-DE", "name": "Melbourne" }
  ],
  "monuments": [
    {
      "remoteId": "flindersstreetstation",
      "name": "Flinders Street Station",
      "imagePath": "landmarks/melbourne-flindersstreetstation.webp",
      "mobileImagePath": "landmarks/melbourne-flindersstreetstation-mobile.webp",
      "imageVersion": "1",
      "names": [
        { "languageCode": "en-US", "name": "Flinders Street Station" },
        { "languageCode": "es-ES", "name": "Estación Flinders Street" },
        { "languageCode": "pt-BR", "name": "Estação Flinders Street" },
        { "languageCode": "it-IT", "name": "Stazione di Flinders Street" },
        { "languageCode": "fr-FR", "name": "Gare de Flinders Street" },
        { "languageCode": "de-DE", "name": "Flinders-Street-Bahnhof" }
      ],
      "isActive": true
    },
    {
      "remoteId": "federationsquare",
      "name": "Federation Square",
      "imagePath": "landmarks/melbourne-federationsquare.webp",
      "mobileImagePath": "landmarks/melbourne-federationsquare-mobile.webp",
      "imageVersion": "1",
      "names": [
        { "languageCode": "en-US", "name": "Federation Square" },
        { "languageCode": "es-ES", "name": "Plaza de la Federación" },
        { "languageCode": "pt-BR", "name": "Praça da Federação" },
        { "languageCode": "it-IT", "name": "Piazza della Federazione" },
        { "languageCode": "fr-FR", "name": "Place de la Fédération" },
        { "languageCode": "de-DE", "name": "Federation Square" }
      ],
      "isActive": true
    },
    {
      "remoteId": "stpaulscathedral",
      "name": "St Paul's Cathedral",
      "imagePath": "landmarks/melbourne-stpaulscathedral.webp",
      "mobileImagePath": "landmarks/melbourne-stpaulscathedral-mobile.webp",
      "imageVersion": "1",
      "names": [
        { "languageCode": "en-US", "name": "St Paul's Cathedral" },
        { "languageCode": "es-ES", "name": "Catedral de San Pablo" },
        { "languageCode": "pt-BR", "name": "Catedral de São Paulo" },
        { "languageCode": "it-IT", "name": "Cattedrale di San Paolo" },
        { "languageCode": "fr-FR", "name": "Cathédrale Saint-Paul" },
        { "languageCode": "de-DE", "name": "St.-Pauls-Kathedrale" }
      ],
      "isActive": true
    },
    {
      "remoteId": "shrineofremembrance",
      "name": "Shrine of Remembrance",
      "imagePath": "landmarks/melbourne-shrineofremembrance.webp",
      "mobileImagePath": "landmarks/melbourne-shrineofremembrance-mobile.webp",
      "imageVersion": "1",
      "names": [
        { "languageCode": "en-US", "name": "Shrine of Remembrance" },
        { "languageCode": "es-ES", "name": "Santuario del Recuerdo" },
        { "languageCode": "pt-BR", "name": "Santuário da Lembrança" },
        { "languageCode": "it-IT", "name": "Santuario della Memoria" },
        { "languageCode": "fr-FR", "name": "Sanctuaire du Souvenir" },
        { "languageCode": "de-DE", "name": "Schrein der Erinnerung" }
      ],
      "isActive": true
    },
    {
      "remoteId": "royalbotanicgardens",
      "name": "Royal Botanic Gardens",
      "imagePath": "landmarks/melbourne-royalbotanicgardens.webp",
      "mobileImagePath": "landmarks/melbourne-royalbotanicgardens-mobile.webp",
      "imageVersion": "1",
      "names": [
        { "languageCode": "en-US", "name": "Royal Botanic Gardens" },
        { "languageCode": "es-ES", "name": "Jardines Botánicos Reales" },
        { "languageCode": "pt-BR", "name": "Jardins Botânicos Reais" },
        { "languageCode": "it-IT", "name": "Giardini Botanici Reali" },
        { "languageCode": "fr-FR", "name": "Jardins botaniques royaux" },
        { "languageCode": "de-DE", "name": "Königliche Botanische Gärten" }
      ],
      "isActive": true
    }
  ]
}
```

### Example AI-Generated Entry in `cities.json`

```json
{
  "remoteId": "melbourne-au",
  "name": "Melbourne",
  "country": "Australia",
  "continent": "Oceania",
  "manifestPath": "/manifests/melbourne.json",
  "imagePath": "/cities/melbourne.webp",
  "dateAdded": "2026-03-26",
  "isActive": true
}
```

