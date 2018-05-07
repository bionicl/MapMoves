using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightListUI : MonoBehaviour {
	public static RightListUI instance;

	public Text placeName;
	public Image placeIcon;
	public Animator animator;
	public PlaceGroup place;

	bool opened = false;

	void Awake() {
		instance = this;
	}

	public void Close() {
		if (opened) {
			opened = false;
			animator.SetTrigger("Close");
		}
	}

	public void NewPlace(PlaceGroup place) {
		if (!opened) {
			animator.SetTrigger("Open");
			opened = true;
		}
		this.place = place;
		placeName.text = place.placeInfo.name;
		placeIcon.sprite = place.icon;
	}
}
