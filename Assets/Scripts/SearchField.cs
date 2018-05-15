using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SearchField : MonoBehaviour {

	public GameObject[] searchHelp;
	public InputField inputField;
	public Animator activityAnimator;
	public GameObject stopSearchingArea;
	public GameObject searchSuggestionsGroup;
	public SearchSuggestion[] suggestion;

	bool inSearchMode = false;

	// Start animation
	public void StartSearching() {
		inSearchMode = true;
		foreach (var item in searchHelp) {
			item.SetActive(false);
		}
		activityAnimator.SetTrigger("StartSearching");
		inputField.text = "";
		inputField.gameObject.SetActive(true);
		inputField.Select();
		stopSearchingArea.SetActive(true);
		searchSuggestionsGroup.SetActive(false);
	}

	// Esc/Dismiss
	void Update() {
		if (inSearchMode && Input.GetKey(KeyCode.Escape))
			StopSearching();
	}
	public void StopSearching() {
		inSearchMode = false;
		foreach (var item in searchHelp) {
			item.SetActive(true);
		}
		activityAnimator.SetTrigger("StopSearching");
		inputField.gameObject.SetActive(false);
		inputField.Select();
		stopSearchingArea.SetActive(false);
		searchSuggestionsGroup.SetActive(false);
	}

	// Typing suggestions
	public void TypingSuggestions(string text) {
		if (text == "") {
			searchSuggestionsGroup.SetActive(false);
			return;
		} else {
			searchSuggestionsGroup.SetActive(true);
			RecalcualteSuggestions(text);
		}
	}
	void RecalcualteSuggestions(string text) {
		List<PlaceGroup> output = PlacesRanking.instance.FindStartingWith(text);
		output.OrderBy(o => o.timesVisited).ToList();
		for (int i = 0; i < 5; i++) {
			if (i < output.Count)
				suggestion[i].SetupSuggestion(output[i], this);
			else
				suggestion[i].SetupSuggestion();
		}
	}
}
