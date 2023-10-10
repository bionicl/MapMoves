using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


[System.Serializable]
public class PlaceIconSave {
	public long id;
	public string categoryId;

	public override string ToString() {
		return JsonConvert.SerializeObject(this);
	}

	public PlaceIconSave(long id, string categoryId) {
		this.id = id;
		this.categoryId = categoryId;
	}
}

public class PlacesSave : MonoBehaviour {

	public static List<PlaceIconSave> iconSaves = new List<PlaceIconSave>();

	public static void IconChange(long id, string categoryId) {
		PlaceIconSave loadedSave = FindIconSave(id);
		if (loadedSave != null) {
			loadedSave.categoryId = categoryId;
		} else {
			iconSaves.Add(new PlaceIconSave(id, categoryId));
		}
	}

	public static void LoadCategories(List<PlaceIconSave> save) {
		iconSaves = save;
	}

	public static string FindIcon(long placeId) {
		foreach (var item in iconSaves) {
			if (item.id == placeId) {
				return item.categoryId;
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
