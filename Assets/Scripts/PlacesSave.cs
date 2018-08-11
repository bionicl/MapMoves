using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


[System.Serializable]
public class PlaceIconSave {
	public long id;
	public int iconNumber;

	public override string ToString() {
		return JsonConvert.SerializeObject(this);
	}

	public PlaceIconSave(long id, int iconNumber) {
		this.id = id;
		this.iconNumber = iconNumber;
	}
}

public class PlacesSave : MonoBehaviour {

	public static List<PlaceIconSave> iconSaves = new List<PlaceIconSave>();

	public static void IconChange(long id, int iconNumber) {
		PlaceIconSave loadedSave = FindIconSave(id);
		if (loadedSave != null) {
			loadedSave.iconNumber = iconNumber;
		} else {
			iconSaves.Add(new PlaceIconSave(id, iconNumber));
		}
	}

	public static void LoadCategories(List<PlaceIconSave> save) {
		iconSaves = save;
	}

	public static int? FindIcon(long placeId) {
		foreach (var item in iconSaves) {
			if (item.id == placeId) {
				return item.iconNumber;
			}
		}
		return null;
	}
	public static PlaceIconSave FindIconSave(long placeId) {
		foreach (var item in iconSaves) {
			if (item.id == placeId) {
				return item;
			}
		}
		return null;
	}

	public static void Clear() {
		iconSaves.Clear();
	}
}
