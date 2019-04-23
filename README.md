Check your **Moves app & Arc app** history on Map!

![][image-1]
### [Download newest version][1]
### Features:
- Import your GPX files from Arc App (and JSON from Moves app) and display them on the large map. Combine weeks, months or event years! 🤯
- View beautiful timeline of each day with transport details and places
- View your images from Google Photos on timeline 🏞
- View burned calories of each walk/cycling/running!
- Filter your map display to only show particular activities or transport types
- Detailed place info - place address, common visiting hours and weekdays, set custom categories (more features based on categories soon!)
- Full Mac Retina display support
### Support project
Map moves are completely free to use. If you’d like to support the project and suggest new features, you can buy me a coffee ☕️
[https://www.buymeacoffee.com/bionicl][2]
Thank you!

##### Data sources in MapMoves:
- JSON timeline from **Moves app**
- GPX day/month file from **Arc app**
- (API) Google Photos
## Development
### How to open Unity project:
1. Download Unity project
2. Download [Json .NET for unity][3] and place it in `Assets` folder
3. Run the app

### Used APIs:
- Google maps api to get location address.  
	Create file `Assets/Resources/API/googleAPI.txt` and paste your Google Geocoding API key there
- MapBox access token for static maps images  
	Create file `Assets/Resources/API/mapBoxApi.txt` and paste your MapBox Access token
- Google Photos API  
	Create file `Assets/Resources/API/googlePhotosAPI.txt` and paste (in each line: Google App ID, redirect URL, Encryption password


[1]:	https://github.com/bionicl/MapMoves/releases
[2]:	https://www.buymeacoffee.com/bionicl "buy me a coffee!"
[3]:	https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347

[image-1]:	https://i.imgur.com/G2issIL.png