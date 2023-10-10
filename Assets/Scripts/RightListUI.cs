using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RightListUI : MonoBehaviour {
	public static RightListUI instance;

	public Text placeName;
	public Animator animator;
	public Image placeIcon;
	public PlaceGroup place;
	public Text placeVisitedTimes;
	public Text placeLastVisited;
	public RectTransform[] hours;
	public RectTransform[] weekDays;
	public GameObject[] placeAddressGroup;
	public Text placeAddress;

	public RectTransform iconsSpawn;
	public GameObject iconBoxPrefab;
	List<IconBox> customIcons = new List<IconBox>();
	public int maxChartHeight = 28;

	[Header("Current category")]
	public Image currentCatIcon;
	public Image currentCatCircle;
	public Text currentCatText;
	public Text currentCatTextMain;

	void Awake() {
		instance = this;
	}

	void Start() {
		SetupIcons();
	}

	public void NewPlace(PlaceGroup place, bool clickedOnMap = false) {
		ButtonTabs.instance.ClearCalendarView();

		bool wait = true;
		if (TopBar.instance.currentTab == 2)
			animator.SetTrigger("Change");
		else {
			wait = false;
			TopBar.instance.SwitchTab(2);
		}
		StopAllCoroutines();

		if (!clickedOnMap) {
			RenderMap.instance.UpdateMapSize(0.3f);
			GlobalVariables.instance.MoveCamera(place.mapObject.gameObject.transform.position);
			place.mapObject.Select(true);
		}
		this.place = place;
		if (wait)
			StartCoroutine(AfterAnimationChange());
		else
			TasksAfterWaiting();
	}
	IEnumerator AfterAnimationChange() {
		yield return new WaitForSeconds(0.1f);
		TasksAfterWaiting();
	}

	void TasksAfterWaiting() {
		placeName.text = place.placeInfo.name;
		if (placeIcon != null)
			placeIcon.sprite = PlacesRanking.instance.categoriesDictionary[place.categoryId].smallIcon;
		placeVisitedTimes.text = string.Format("Place visited {0} times", place.timesVisited);
		placeLastVisited.text = string.Format("Last visited {0}", place.lastVisited.ToShortDateString());
		ChangeSelectedIcon(place.categoryId);
		place.DisplayTimes(hours, maxChartHeight);
		place.DisplayWeekDays(weekDays, maxChartHeight);

		TryToGetAddress();
	}

	void ChangeSelectedIcon(string categoryId) {
		foreach (var item in customIcons) {
			item.MarkAsDeselected();
		}

		for (int i = 0; i < customIcons.Count; i++) {
			if (customIcons[i].categoryId == categoryId) {
				customIcons[i].MarkAsSelected();
				break;
			}
		}

		// Current category
		PlacesDataJson.PlaceCategory category = PlacesRanking.instance.categoriesDictionary[categoryId];
		currentCatCircle.color = category.placeTypeCategory.ColorConverted;
		currentCatIcon.sprite = category.bigIcon;
		currentCatText.text = category.displayName;
		currentCatTextMain.text = category.placeTypeCategory.displayName;
	}

	void SetupIcons() {
		int count = 0;
		List<PlacesDataJson.PlaceCategory> tempCategories = PlacesRanking.instance.categories;
		for (int i = 0; i < tempCategories.Count; i++) {
			PlacesDataJson.PlaceCategory item = tempCategories[i];
			GameObject tempIcon = Instantiate(iconBoxPrefab, transform.position, transform.rotation);
			tempIcon.transform.SetParent(iconsSpawn);
			tempIcon.transform.localScale = tempIcon.transform.lossyScale;
			tempIcon.SetActive(true);
			IconBox tempIconBox = tempIcon.GetComponent<IconBox>();
			tempIconBox.SetupIcon(item);
			customIcons.Add(tempIconBox);
			count++;
		}
	}
	void TryToGetAddress() {
		placeAddressGroup[0].SetActive(false);
		placeAddressGroup[1].SetActive(false);
		GoogleLocationApi.instance.GetPlaceAddress(place, AddressRecieved);
	}
	void AddressRecieved(string address) {
		placeAddressGroup[0].SetActive(true);
		placeAddressGroup[1].SetActive(true);
		placeAddress.text = address;
	}

	public void IconClicked(string id) {
		ChangeSelectedIcon(id);
		place.RefreshIcons(id);
		if (placeIcon != null)
			placeIcon.sprite = PlacesRanking.instance.categoriesDictionary[id].smallIcon;
		PlacesSave.IconChange(place.placeInfo.id, id);
	}
}
