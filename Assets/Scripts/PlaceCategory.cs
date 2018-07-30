using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaceMainCategory {
	public string name;
	public Sprite icon;
	public Color color;
}

public enum PlaceMainCategoryEnum {
	Transport,
	Shopping,
	PublicPlace,
	Food,
	Entertainment,
	Health,
	Services,
	Education,
	PrivatePlace,
	Uncategorized
}

[System.Serializable]
public class PlaceCategory {
	public string name;
	public PlaceMainCategoryEnum category;
	public Sprite smallIcon;
	public Sprite bigIcon;

	public PlaceMainCategory Category {
		get {
			return PlacesRanking.instance.mainCategories[(int)category];
		}
	}
}
