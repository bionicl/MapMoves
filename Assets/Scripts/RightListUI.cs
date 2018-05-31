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
	public Text placeVisitedTimes;
	public Text placeLastVisited;
	public CanvasGroup[] hours;
	public CanvasGroup[] weekDays;
	public GameObject[] placeAddressGroup;
	public Text placeAddress;

	public RectTransform iconsSpawn;
	public GameObject iconBoxPrefab;
	List<IconBox> customIcons = new List<IconBox>();

	bool opened = false;

	void Awake() {
		instance = this;
	}

	void Start() {
		SetupIcons();
	}

	public void Close() {
		if (opened) {
			opened = false;
			animator.SetTrigger("Close");
			if (Place.currentlySelected != null)
				Place.currentlySelected.Deselect();
		}
	}

	public void NewPlace(PlaceGroup place, bool clickedOnMap = false) {
		if (!opened) {
			animator.SetTrigger("Open");
			opened = true;
		}
		if (!clickedOnMap) {
			RenderMap.instance.UpdateMapSize(0.3f);
			GlobalVariables.inst.MoveCamera(place.mapObject.gameObject.transform.position);
			place.mapObject.Select();
		}
		this.place = place;
		placeName.text = place.placeInfo.name;
		placeIcon.sprite = FacebookPlaces.instance.iconsImages[place.icon];
		placeVisitedTimes.text = string.Format("Place visited {0} times", place.timesVisited);
		placeLastVisited.text = string.Format("Last visited {0}", place.lastVisited.ToShortDateString());
		ChangeSelectedIcon(place.icon);
		place.DisplayTimes(hours);
		place.DisplayWeekDays(weekDays);

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
		foreach (var item in FacebookPlaces.instance.iconsImages) {
			GameObject tempIcon = Instantiate(iconBoxPrefab, transform.position, transform.rotation);
			tempIcon.transform.SetParent(iconsSpawn);
			tempIcon.transform.localScale = tempIcon.transform.lossyScale;
			tempIcon.SetActive(true);
			IconBox tempIconBox = tempIcon.GetComponent<IconBox>();
			tempIconBox.SetupIcon(item, count);
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
		placeIcon.sprite = FacebookPlaces.instance.iconsImages[place.icon];
		PlacesSave.IconChange(place.placeInfo.id, id);
	}
}
