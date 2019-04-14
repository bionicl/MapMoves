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
	public VerticalLayoutGroup activityList;

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

	public void ResetActivityListPadding() {
		activityList.padding.top = 64;
		LayoutRebuilder.MarkLayoutForRebuild(activityList.GetComponent<RectTransform>());
	}

	void ChangeMonth(DateTime newDate) {
		currentMonth = newDate;
		int daysInMonth = DateTime.DaysInMonth(newDate.Year, newDate.Month);
		for (int i = 0; i < 31; i++) {
			calendarButtons[i].gameObject.SetActive(i < daysInMonth);
			if (i < daysInMonth) {
				DateTime tempDay = new DateTime(newDate.Year, newDate.Month, i + 1);
				calendarButtons[i].button.interactable = ReadJson.instance.days.ContainsKey(tempDay);
				calendarButtons[i].Setup(tempDay);
			}
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
		activityList.padding.top = 240;
	}

	public void MoveCalendar(bool right) {
		int addValue = -1;
		if (right)
			addValue = 1;
		ChangeMonth(currentMonth.AddMonths(addValue));
		if (currentMonth.Year == ReadJson.instance.selectedDay.Year &&
		    currentMonth.Month == ReadJson.instance.selectedDay.Month) {
			calendarButtons[ReadJson.instance.selectedDay.Day - 1].Select();
		} else {
			if (CalendarButton.cuttentlySelected != null)
			CalendarButton.cuttentlySelected.Deselect();
		}
	}

}
