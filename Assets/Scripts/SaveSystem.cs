using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

[Serializable]
class SaveData {
	public List<MovesJson> loadedJson;
	public List<PlaceIconSave> placesSave;

	public SaveData(List<MovesJson> loadedJson, List<PlaceIconSave> placesSave) {
		this.loadedJson = loadedJson;
		this.placesSave = placesSave;
	}
}

public class SaveSystem
{
	static Action _OnSaveChange;
	public static Action OnSaveChange {
		set {
			_OnSaveChange = value;
		}
	}

	public static void Load() {
		string destination = Application.persistentDataPath + "/save.dat";
		FileStream file;

		if (File.Exists(destination))
			file = File.OpenRead(destination);
		else {
			Debug.LogError("File not found");
			return;
		}

		BinaryFormatter bf = new BinaryFormatter();
		SaveData data = (SaveData)bf.Deserialize(file);
		file.Close();

		PlacesSave.LoadCategories(data.placesSave);
	}

	public static void Save() {
		// Setup
		string destination = Application.persistentDataPath + "/save.dat";
		SaveData saveData = new SaveData(new List<MovesJson>(), PlacesSave.iconSaves);

		// Open/Create file
		FileStream file;

		if (File.Exists(destination))
			file = File.OpenWrite(destination);
		else
			file = File.Create(destination);

		// Save to file
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(file, saveData);
		file.Close();
		Debug.Log("Saved!");
	}

	public static void SaveChange() {
		_OnSaveChange.Invoke();
	}

}
