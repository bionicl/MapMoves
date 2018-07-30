using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SpecialIcons {
	public string name;
	public int iconId;
}

public class FacebookPlaces : MonoBehaviour {
	public static FacebookPlaces instance;

	Dictionary<string, int> placesMemory = new Dictionary<string, int>();
	Dictionary<string, int> customIcons = new Dictionary<string, int>();

	void Awake() {
		instance = this;
	}
}
