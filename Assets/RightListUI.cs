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
		}
	}

	public void NewPlace(PlaceGroup place) {
		if (!opened) {
			animator.SetTrigger("Open");
			opened = true;
		}
		this.place = place;
		placeName.text = place.placeInfo.name;
		placeIcon.sprite = FacebookPlaces.instance.iconsImages[place.icon];
		placeVisitedTimes.text = string.Format("Place visited {0} times", place.timesVisited);
		placeLastVisited.text = string.Format("Last visited {0}", place.lastVisited.ToShortDateString());
		ChangeSelectedIcon(place.icon);
		place.DisplayTimes(hours);
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

	public void IconClicked(int id) {
		ChangeSelectedIcon(id);
		place.RefreshIcons(id);
		placeIcon.sprite = FacebookPlaces.instance.iconsImages[place.icon];
	}
}
