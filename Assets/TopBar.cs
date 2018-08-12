using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour {
	public static TopBar instance;

	public GameObject[] tabs;
	public TabItem[] tabButtons;
	TabItem selectedTab;
	public int? currentTab;

	void Awake() {
		instance = this;
	}

	void Start() {
		SwitchTab(1);
	}

	public void SwitchTab(int id) {
		if (selectedTab == tabButtons[id])
			return;

		if (id == 2) {
			tabButtons[2].GetComponent<Button>().interactable = true;
		}

		foreach (var item in tabs) {
			item.SetActive(false);
		}
		tabs[id].SetActive(true);
		if (selectedTab != null)
			selectedTab.Deselect();
		selectedTab = tabButtons[id];
		selectedTab.Select();
		currentTab = id;
	}

	public void Clear() {
		SwitchTab(1);
		tabButtons[2].GetComponent<Button>().interactable = false;
	}
}
