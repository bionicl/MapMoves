using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTabs : MonoBehaviour {

	public TabsButton[] buttons;
	public GameObject[] tabs;
	int? currentlyOpened;
	public Text saveButtonText;

	void Start () {
		foreach (var item in tabs) {
			item.SetActive(false);
		}
		Open(1);
	}

	public void Open(int id) {
		if (currentlyOpened == id) {
			buttons[id].Disable();
			tabs[id].SetActive(false);
			currentlyOpened = null;
		} else {
			if (currentlyOpened.HasValue) {
				buttons[currentlyOpened.Value].Disable();
				tabs[currentlyOpened.Value].SetActive(false);
			}
			buttons[id].Enable();
			tabs[id].SetActive(true);
			currentlyOpened = id;
		}
	}

	public void Save() {
		SaveSystem.Save();
		saveButtonText.text = "Saved!";
		StartCoroutine(RevertAfterTime());
	}
	IEnumerator RevertAfterTime() {
		yield return new WaitForSeconds(1.5f);
		saveButtonText.text = "Save";
	}
}
