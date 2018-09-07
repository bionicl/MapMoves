using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarButton : MonoBehaviour {

	public static CalendarButton cuttentlySelected;

	public Image selectImage;
	public Text dayText;
	public Button button;
	public DateTime date;

	public void Deselect() {
		selectImage.enabled = false;
	}

	public void Select() {
		selectImage.enabled = true;
		if (cuttentlySelected != null)
			cuttentlySelected.Deselect();
		cuttentlySelected = this;
	}

	public void Clicked() {
		Select();
		Calendar.instance.ChangeDay(date);
	}

	public void Setup(int id) {
		dayText.text = id.ToString();
	}

	public void Setup(DateTime newDate) {
		date = newDate;
		dayText.text = newDate.Day.ToString();
	}

}
