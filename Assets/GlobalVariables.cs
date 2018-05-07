using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalVariables : MonoBehaviour {
	public static GlobalVariables inst;

	public Sprite[] icons; 

	void Awake() {
		inst = this;
	}

	Image targetImg;
	public void SetIcon(MovesJson.SegmentsInfo.PlaceInfo place, SpriteRenderer image) {
		LoadIcon(place, (Sprite sprite) => image.sprite = sprite);
	}

	public void SetIcon(MovesJson.SegmentsInfo.PlaceInfo place, Image image) {
		LoadIcon(place, (Sprite sprite) => image.sprite = sprite);
	}

	public void SetIcon(MovesJson.SegmentsInfo.PlaceInfo place, Action<Sprite> action) {
		LoadIcon(place, (Sprite sprite) => action.Invoke(sprite));
	}

	void LoadIcon(MovesJson.SegmentsInfo.PlaceInfo place, Action<Sprite> action) {
		PlaceType placeType = place.type;
		if (placeType == PlaceType.home)
			action.Invoke(icons[0]);
		else if (placeType == PlaceType.school)
			action.Invoke(icons[2]);
		else
			action.Invoke(icons[1]);
		if (placeType == PlaceType.facebook) {
			FacebookPlaces.instance.GetPlaceCategory(place.facebookPlaceId, targetImg);
		}
		
	}
}
