using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

[Serializable]
class SaveData {
	public Dictionary<DateTime, DayClass> loadedJson;
	public List<PlaceIconSave> placesSave;
	public List<string> uploadedFiles;
	public int dayNumber;

	public SaveData(ReadJson main, List<PlaceIconSave> placesSave, List<string> uploadedFiles) {
		this.loadedJson = main.days;
		this.placesSave = placesSave;
		this.uploadedFiles = uploadedFiles;
		this.dayNumber = main.dayNumber;
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
		string destination = Application.persistentDataPath + "/save2.dat";
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
		ReadJson.instance.uploadedFiles = data.uploadedFiles;
		ReadJson.instance.days = data.loadedJson;
	}

	public static void Save() {
		// Setup
		string destination = Application.persistentDataPath + "/save2.dat";
		SaveData saveData = new SaveData(ReadJson.instance,
		                                 PlacesSave.iconSaves,
		                                 ReadJson.instance.uploadedFiles);

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
