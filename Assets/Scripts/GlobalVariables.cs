using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalVariables : MonoBehaviour {
	public static GlobalVariables instance;

	public Color accentColor;
	public Color disabledColor;
	public bool firstWeekMonday = true;
	[HideInInspector]
	public bool mapControls = false;

	void Awake() {
		instance = this;
	}

	public void MouseEnter() {
		mapControls = true;
	}

	public void MouseExit() {
		mapControls = false;
	}

	public void SetIcon(MovesJson.SegmentsInfo.PlaceInfo place, SpriteRenderer image) {
		LoadIcon(place, (categoryId) => image.sprite = PlacesRanking.instance.categoriesDictionary[categoryId].smallIcon);
	}
	public void SetIcon(MovesJson.SegmentsInfo.PlaceInfo place, Image image) {
		LoadIcon(place, (categoryId) => image.sprite = PlacesRanking.instance.categoriesDictionary[categoryId].smallIcon);
	}
	public void SetIcon(MovesJson.SegmentsInfo.PlaceInfo place, Action<string> action) {
		LoadIcon(place, (categoryId) => action.Invoke(categoryId));
	}
	void LoadIcon(MovesJson.SegmentsInfo.PlaceInfo place, Action<string> action) {
		PlaceSourceType placeType = place.type;
		if (placeType == PlaceSourceType.home)
			action.Invoke("HOME");
		else if (placeType == PlaceSourceType.school)
			action.Invoke("SCHOOL");
		else
			action.Invoke("MARKER");

		string customIcon = PlacesSave.FindIcon(place.id);
		if (!string.IsNullOrEmpty(customIcon)) {
			action.Invoke(customIcon);
		}
		
	}

	public Vector3 TransformToCenter(Vector3 position) {
		Vector3 newPos = position;
		newPos.x += 160 * (3.555f / Screen.width) * RenderMap.instance.mapScale;
		newPos.y += 34 * (3.555f / Screen.width) * RenderMap.instance.mapScale;
		return newPos;
	}
	public void MoveCamera(Vector3 position) {
		Vector3 cameraPos = position;
		cameraPos.z = -40;
		cameraPos.x -= 160 * (3.555f / Screen.width) * RenderMap.instance.mapScale;
		cameraPos.y -= 34 * (3.555f / Screen.width) * RenderMap.instance.mapScale;
		Camera.main.transform.position = cameraPos;
	}
}
