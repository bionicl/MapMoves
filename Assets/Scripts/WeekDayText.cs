using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeekDayText : MonoBehaviour {

	Text text;
	public string dayIfFirstMonday;
	public string dayIfFirstSunday;

	void Awake() {
		text = GetComponent<Text>();
	}

	void Start () {
		if (GlobalVariables.instance.firstWeekMonday)
			text.text = dayIfFirstMonday;
		else
			text.text = dayIfFirstSunday;
	}
}
