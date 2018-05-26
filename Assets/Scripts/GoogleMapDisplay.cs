using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoogleMapDisplay : MonoBehaviour {

	public GameObject tilePrefab;
	public float baseTileSize = 0.3826f;
	public Vector2[] zoomLevelTiers;

	int zoomLevel;
	float tileSize;

	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			RenderMap();
		}
	}

	void RenderMap() {
		CalculateMapZoom();
		StartCoroutine(DownloadMap(ReturnApiUrl(), CreateNewTile));
	}
	IEnumerator DownloadMap(string url, Action<Sprite> action) {
		// Start a download of the given URL
		using (WWW www = new WWW(url)) {
			// Wait for download to complete
			yield return www;

			// assign texture
			Sprite sprite = Sprite.Create(www.texture,
										  new Rect(0, 0, 600, 600),
										  new Vector2(0.5f, 0.5f));
			action.Invoke(sprite);
		}
	}

	void CreateNewTile(Sprite sprite) {
		Vector3 tempPos = Camera.main.transform.position;
		tempPos.z = 40;
		GameObject tempGo = Instantiate(tilePrefab, tempPos, transform.rotation);
		tempGo.GetComponent<SpriteRenderer>().sprite = sprite;
		tempGo.transform.localScale = new Vector3(tileSize, tileSize, 1);
	}

	void CalculateMapZoom() {
		float userMapScale = Camera.main.orthographicSize;
		for (int i = 0; i < zoomLevelTiers.Length; i++) {
			if (userMapScale <= zoomLevelTiers[i].x) {
				zoomLevel = (int)zoomLevelTiers[i].y;
				break;
			}
		}
		tileSize = baseTileSize * Mathf.Pow(2, 12 - zoomLevel);
	}

	string ReturnApiUrl() {
		Vector3 tempPos = Camera.main.transform.position;
		Vector2 metersPos = Conversion.MetersToLatLon(new Vector2(tempPos.x, tempPos.y));
		string output = string.Format("http://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom={2}&size=600x600&key={3}", metersPos.x, metersPos.y, zoomLevel, GoogleLocationApi.instance.apiKey);
		return output;
	}
}
