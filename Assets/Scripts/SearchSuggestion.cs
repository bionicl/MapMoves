using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchSuggestion : MonoBehaviour {
	public static SearchSuggestion selected;
	public static int? currentlySelected;

	PlaceGroup place;
	SearchField searchField;
	public Color deselectedColor;

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

	// Arrows control
	public void Select() {
		text.color = Color.white;
		Debug.Log("Selected " + place.placeInfo.name);
	}
	public void Deselect() {
		text.color = deselectedColor;
	}

	public static void ResetSelection() {
		if (selected != null) {
			currentlySelected = null;
			selected.Deselect();
			selected = null;
		}
	}
	public static void DownArrow(SearchSuggestion[] suggestions) {
		if (!suggestions[0].isEnabled)
			return;
		if (currentlySelected == null) {
			currentlySelected = 0;
		} else if (currentlySelected < 4 && suggestions[currentlySelected.Value + 1].isEnabled) {
			currentlySelected++;
		}
		if (selected != suggestions[currentlySelected.Value]) {
			if (selected != null)
				selected.Deselect();
			selected = suggestions[currentlySelected.Value];
			selected.Select();
		}
	}
	public static void UpArrow(SearchSuggestion[] suggestions) {
		if (!suggestions[0].isEnabled)
			return;
		if (currentlySelected == 0)
			ResetSelection();
		else {
			currentlySelected--;
			selected.Deselect();
			selected = suggestions[currentlySelected.Value];
			selected.Select();
		}
	}
}
