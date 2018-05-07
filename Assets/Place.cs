using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour {

	public SpriteRenderer circle;
	public SpriteRenderer icon;
	public PlaceGroup place;

	public void SetupPlace(MovesJson.SegmentsInfo.PlaceInfo placeInfo) {
		place = PlacesRanking.instance.FindPlace(placeInfo, this);
	}

	void OnMouseDown() {
		RightListUI.instance.NewPlace(place);
	}
}
