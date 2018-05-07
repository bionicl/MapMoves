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
		LoadIcon(place, (int sprite) => image.sprite = FacebookPlaces.instance.iconsImages[sprite]);
	}

	public void SetIcon(MovesJson.SegmentsInfo.PlaceInfo place, Image image) {
		LoadIcon(place, (int sprite) => image.sprite = FacebookPlaces.instance.iconsImages[sprite]);
	}

	public void SetIcon(MovesJson.SegmentsInfo.PlaceInfo place, Action<int> action) {
		LoadIcon(place, (int sprite) => action.Invoke(sprite));
	}

	void LoadIcon(MovesJson.SegmentsInfo.PlaceInfo place, Action<int> action) {
		PlaceType placeType = place.type;
		if (placeType == PlaceType.home)
			action.Invoke(4);
		else if (placeType == PlaceType.school)
			action.Invoke(9);
		else
			action.Invoke(6);
		if (placeType == PlaceType.facebook) {
			FacebookPlaces.instance.GetPlaceCategory(place.facebookPlaceId, targetImg);
		}
		
	}
}
