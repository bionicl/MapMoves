using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilesBox : MonoBehaviour {

	public GameObject fileNamePrefab;

	public void SetupTexts(List<string> files) {
		foreach (var item in files) {
			GameObject go = Instantiate(fileNamePrefab, gameObject.transform.position, gameObject.transform.rotation);
			go.transform.SetParent(gameObject.transform);
			go.transform.localScale = go.transform.lossyScale;
			go.GetComponent<Text>().text = item;
		}
	}
}
