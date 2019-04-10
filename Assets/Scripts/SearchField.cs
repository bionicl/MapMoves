using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SearchField : MonoBehaviour {

	public GameObject[] searchHelp;
	public InputField inputField;
	//public Animator activityAnimator;
	public GameObject stopSearchingArea;
	public GameObject searchSuggestionsGroup;
	public SearchSuggestion[] suggestion;
	public Animator animator;

	bool inSearchMode = false;

	// Start animation
	public void StartSearching() {
		if (ReadJson.instance.uploadedFiles.Count == 0)
			return;
		animator.SetTrigger("Open");
		inSearchMode = true;
		foreach (var item in searchHelp) {
			item.SetActive(false);
		}
		//activityAnimator.SetTrigger("StartSearching");
		inputField.text = "";
		inputField.gameObject.SetActive(true);
		inputField.Select();
		stopSearchingArea.SetActive(true);
		searchSuggestionsGroup.SetActive(false);
	}

	// Esc/Dismiss
	void Update() {
		if (!inSearchMode && (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKey(KeyCode.F))
			StartSearching();
		if (!inSearchMode)
			return;
		if (Input.GetKey(KeyCode.Escape))
			StopSearching();
		else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			SearchSuggestion.DownArrow(suggestion);
		} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			SearchSuggestion.UpArrow(suggestion);
		} else if (Input.GetKeyDown(KeyCode.Return)) {
			SearchSuggestion.selected.ClickOnPlace();
		}
	}
	public void StopSearching() {
		animator.SetTrigger("Close");
		inSearchMode = false;
		foreach (var item in searchHelp) {
			item.SetActive(true);
		}
		//activityAnimator.SetTrigger("StopSearching");
		inputField.gameObject.SetActive(false);
		inputField.Select();
		stopSearchingArea.SetActive(false);
		searchSuggestionsGroup.SetActive(false);
	}

	// Typing suggestions
	public void TypingSuggestions(string text) {
		SearchSuggestion.ResetSelection();
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
		List<PlaceGroup> outputContaining = new List<PlaceGroup>();
		output.OrderBy(o => o.timesVisited).ToList();
		if (output.Count < 5) {
			outputContaining = PlacesRanking.instance.FindContaining(text, output);
		}
		int containingIndex = 0;
		for (int i = 0; i < 5; i++) {
			if (i < output.Count)
				suggestion[i].SetupSuggestion(output[i], this);
			else if (containingIndex < outputContaining.Count){
				suggestion[i].SetupSuggestion(outputContaining[containingIndex], this);
				containingIndex++;
			} else
				suggestion[i].SetupSuggestion();
		}
	}
}
