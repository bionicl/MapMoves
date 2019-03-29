Check your **Moves app & Arc app** history on Map!

![][image-1]

### Features:
- Import your GPX files from Arc App (and JSON from Moves app) and display them on the large map. Combine weeks, months or event years! 🤯
- View beautiful timeline of each day with transport details and places
- View burned calories of each walk/cycling/running! Data based on your weight and height
- Filter your map display to only show particular activities or transport types
- Detailed place info - place address, common visiting hours and weekdays, set custom categories (more features based on categories soon!)

## How to run:
1. Download [newest release][1]
2. Run and select location data files
3. Enjoy!

#### App supports currently:
- JSON timeline from **Moves app**
- GPX day/month file from **Arc app**

## Development
### How to open Unity project:
1. Download Unity project
2. Download [Json .NET for unity][2] and place it in `Assets` folder
3. Run the app

### Used APIs:
- Google maps api to get location address.  
	Create file `Assets/Resources/API/googleApi.txt` and paste your Google Geocoding API key there
- MapBox access token for static maps images  
	Create files `Assets/Resources/API/mapBoxApi.txt` and paste your MapBox Access token


[1]:	https://github.com/bionicl/MapMoves/releases
[2]:	https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347

[image-1]:	https://i.imgur.com/hcXWvBf.png