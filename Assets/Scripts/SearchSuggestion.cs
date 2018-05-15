using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchSuggestion : MonoBehaviour {

	PlaceGroup place;
	SearchField searchField;

	Text text;
	bool isEnabled = false;

	void Awake() {
		text = GetComponent<Text>();
	}

	public void ClickOnPlace() {
		if (isEnabled) {
			RightListUI.instance.NewPlace(place);
			searchField.StopSearching();
		}
	}
	public void SetupSuggestion(PlaceGroup place, SearchField searchField) {
		isEnabled = true;
		this.place = place;
		this.searchField = searchField;
		text.text = place.placeInfo.name;
	}
	/// <summary>
	/// Sets as an empty suggestion
	/// </summary>
	public void SetupSuggestion() {
		isEnabled = false;
		text.text = "";
	}
}
