using System.Collections;
using System.Collections.Generic;
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

	bool savePlacesAfterReload = false;

	void Awake() {
		instance = this;
	}

	void Start() {
		SetupIcons();
	}

	public void NewPlace(PlaceGroup place, bool clickedOnMap = false) {

		// Save places
		if (savePlacesAfterReload) {
			savePlacesAfterReload = false;
			PlacesSave.Save();
		}

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
			GlobalVariables.inst.MoveCamera(place.mapObject.gameObject.transform.position);
			place.mapObject.Select();
		}
		this.place = place;
		StartCoroutine(AfterAnimationChange(wait));
	}
	IEnumerator AfterAnimationChange(bool wait = true) {
		if (wait)
			yield return new WaitForSeconds(0.1f);
		placeName.text = place.placeInfo.name;
		if (placeIcon != null)
			placeIcon.sprite = PlacesRanking.instance.categories[place.icon].smallIcon;
		placeVisitedTimes.text = string.Format("Place visited {0} times", place.timesVisited);
		placeLastVisited.text = string.Format("Last visited {0}", place.lastVisited.ToShortDateString());
		ChangeSelectedIcon(place.icon);
		place.DisplayTimes(hours, maxChartHeight);
		place.DisplayWeekDays(weekDays, maxChartHeight);

		TryToGetAddress();
	}

	void ChangeSelectedIcon(int id) {
		foreach (var item in customIcons) {
			item.MarkAsDeselected();
		}
		customIcons[id].MarkAsSelected();
	}

	void SetupIcons() {
		int count = 0;
		foreach (var item in PlacesRanking.instance.categories) {
			GameObject tempIcon = Instantiate(iconBoxPrefab, transform.position, transform.rotation);
			tempIcon.transform.SetParent(iconsSpawn);
			tempIcon.transform.localScale = tempIcon.transform.lossyScale;
			tempIcon.SetActive(true);
			IconBox tempIconBox = tempIcon.GetComponent<IconBox>();
			tempIconBox.SetupIcon(item.smallIcon, count, item.Category.color);
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

	public void IconClicked(int id) {
		ChangeSelectedIcon(id);
		place.RefreshIcons(id);
		if (placeIcon != null)
			placeIcon.sprite = PlacesRanking.instance.categories[place.icon].smallIcon;
		PlacesSave.IconChange(place.placeInfo.id, id);
		savePlacesAfterReload = true;
	}
}
