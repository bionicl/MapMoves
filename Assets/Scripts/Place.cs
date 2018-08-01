using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour {
	public static Place currentlySelected;

	public SpriteRenderer circle;
	public SpriteRenderer icon;
	public PlaceGroup place;
	public TextMesh text;

	public void SetupPlace(MovesJson.SegmentsInfo.PlaceInfo placeInfo) {
		Debug.Log("New place! " + placeInfo.name);
		place = PlacesRanking.instance.FindPlace(placeInfo, this);
		if (placeInfo.name == null)
			text.text = "???";
		else
			text.text = place.placeInfo.name;
	}

	void OnMouseDown() {
		Select();
	}

	public void ChangeIconVisible(float zoom) {
		Color currentColor = circle.color;
		float alpha = 0.85f;
		if (zoom > 1) {
			icon.gameObject.SetActive(false);
			alpha = 0.5f;
		} else {
			icon.gameObject.SetActive(true);
		}
		text.gameObject.SetActive(zoom <= 0.12f);
		currentColor.a = alpha;
		circle.color = currentColor;
	}

	public void Select() {
		if (currentlySelected != this) {
			RightListUI.instance.NewPlace(place, true);
			if (currentlySelected != null)
				currentlySelected.Deselect();
			currentlySelected = this;
			float currentAlpha = circle.color.a;
			Color tempColor = GlobalVariables.inst.accentColor;
			tempColor.a = currentAlpha;
			circle.color = tempColor;
		}
	}

	public void Deselect() {
		circle.color = Color.white;
		currentlySelected = null;
	}
}
