using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalVariables : MonoBehaviour {
	public static GlobalVariables inst;

	public Sprite[] icons;
	public Color accentColor;
	public bool firstWeekMonday = true;
	[HideInInspector]
	public bool mapControls = true;

	void Awake() {
		inst = this;
	}

	public void MouseEnter() {
		mapControls = true;
	}

	public void MouseExit() {
		mapControls = false;
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
			FacebookPlaces.instance.GetPlaceCategory(place.facebookPlaceId, action);
		}
		
	}

	public void MoveCamera(Vector3 position) {
		Vector3 cameraPos = position;
		cameraPos.z = -40;
		cameraPos.x -= 160 * (3.555f / Screen.width) * RenderMap.instance.mapScale;
		cameraPos.y -= 34 * (3.555f / Screen.width) * RenderMap.instance.mapScale;
		Camera.main.transform.position = cameraPos;
	}
}
