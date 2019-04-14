using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilesBox : MonoBehaviour {
	public static FilesBox instance;

	public GameObject fileNamePrefab;
	public GameObject textSpawner;

	void Awake() {
		instance = this;
	}

	public void SetupTexts(List<string> files) {
		for (int i = 0; i < textSpawner.transform.childCount; i++) {
			Destroy(textSpawner.transform.GetChild(i).gameObject);
		}
		foreach (var item in files) {
			GameObject go = Instantiate(fileNamePrefab, textSpawner.transform.position, textSpawner.transform.rotation);
			go.transform.SetParent(textSpawner.transform);
			go.transform.localScale = go.transform.lossyScale;
			go.GetComponent<Text>().text = item;
		}
	}
}
