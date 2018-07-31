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
	PrivatePlace,
	Health,
	Services,
	Education,
	Entertainment,
	Uncategorized
}

[System.Serializable]
public class PlaceCategory {
	public string name;
	public PlaceMainCategoryEnum category;
	public Sprite smallIcon;
	public Sprite bigIcon;
	[HideInInspector]
	public int id;

	public PlaceMainCategory Category {
		get {
			return PlacesRanking.instance.mainCategories[(int)category];
		}
	}
}
