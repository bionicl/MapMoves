Simple app to check Moves app history in Unity.

### How to install:

1. Download Unity project
2. Download [Json .NET for unity][1] and place it in `Assets` folder
3. Export your data from [Moves website][2] (Sign in -\> Export data), then move `json/full/storyline.json` from downloaded package into `Assets/Resources` folder of Unity project
4. Run the app

### Current features:
- Display Moves story data with summary
- Use right/left arrows to switch days
- Activity filters
- Places icons from FacebookAPI (waiting for app review)
- Custom places icons (not saved between sessions yet);

### Planned features:
- week days view
- Moves API integration
- Calendar view to change day
- Activity charts
- Favourite/Top visited places
- **Map with route points**
	- Date range filters
	- Save range filter as event

[1]:	https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347
[2]:	http://moves-app.com