using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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


	public static void Load() {
		string destination = Application.persistentDataPath + "/save.dat";
		Debug.Log(destination);
		FileStream file;

		if (File.Exists(destination))
			file = File.OpenRead(destination);
		else {
			Debug.LogError("File not found");
			return;
		}

		BinaryFormatter bf = new BinaryFormatter();
		List<PlaceIconSave> data = (List<PlaceIconSave>)bf.Deserialize(file);
		file.Close();

		iconSaves = data;
		//iconSaves = (List<PlaceIconSave>)JsonConvert.DeserializeObject(data);
	}
	public static void Save() {
		string destination = Application.persistentDataPath + "/save.dat";
		FileStream file;

		if (File.Exists(destination))
			file = File.OpenWrite(destination);
		else
			file = File.Create(destination);

		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(file, iconSaves);
		file.Close();
		Debug.Log("Saved!");
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
}
