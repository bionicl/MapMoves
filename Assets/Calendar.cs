using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour {
	public static Calendar instance;

	public GameObject[] blanks;
	public CalendarButton[] calendarButtons;
	DateTime currentMonth;
	public Text monthText;

	// Use this for initialization
	void Awake () {
		instance = this;

		SetupNumbers();
		//SetDay(new DateTime(2018, 2, 20));
	}
	
	// Update is called once per frame
	public void OnCalendarEnable () {
		if (ReadJson.instance.selectedDay != null)
			SetDay(ReadJson.instance.selectedDay);
	}

	public void SetDay(DateTime date) {
		if (currentMonth == null || date.Month != currentMonth.Month || date.Year != currentMonth.Year) {
			ChangeMonth(date);
		}
		calendarButtons[date.Day - 1].Select();
	}

	void ChangeMonth(DateTime newDate) {
		currentMonth = newDate;
		int daysInMonth = DateTime.DaysInMonth(newDate.Year, newDate.Month);
		for (int i = 0; i < 31; i++) {
			DateTime tempDay = new DateTime(newDate.Year, newDate.Month, i + 1);
			calendarButtons[i].gameObject.SetActive(i < daysInMonth);
			calendarButtons[i].button.interactable = ReadJson.instance.days.ContainsKey(tempDay);
			calendarButtons[i].Setup(tempDay);
		}

		DateTime firstDate = new DateTime(newDate.Year, newDate.Month, 1);
		for (int i = 0; i < 6; i++) {
			blanks[i].SetActive(i < (int)firstDate.DayOfWeek);
		}
		monthText.text = newDate.ToString("MMMMM yyyy");
	}

	void SetupNumbers() {
		for (int i = 0; i < 31; i++) {
			calendarButtons[i].Setup(new DateTime());
		}
	}

	public void ChangeDay(DateTime changedDate) {
		ReadJson.instance.ChangeDay(changedDate);
	}

}
