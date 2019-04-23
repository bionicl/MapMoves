using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using System.Security.Cryptography;
using System.IO;

public class GooglePhotosResponse {
	public MediaItem[] mediaItems;

	public void CalculateDates() {
		foreach (var item in mediaItems) {
			item.creationDate = DateTime.Parse(item.mediaMetadata.creationTime);
			//Debug.Log($"item creation time : {item.creationDate}, item link: {item.productUrl}");
		}
	}
}

public class MediaItem {
	public string id;
	public string description;
	public string productUrl;
	public string baseUrl;
	public string filename;
	public MediaMetadata mediaMetadata;
	public DateTime creationDate;
}

public class MediaMetadata {
	public string creationTime;
}

public class GoogleOauthResponse {
	public string access_token;
	public int expires_in;
	public string refresh_token;
	public long unixTimeEnd;

	public void SetEndTime() {
		unixTimeEnd = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + expires_in;
	}
}

public class GooglePhotosApi : MonoBehaviour {
	public static GooglePhotosApi instance;
	GoogleOauthResponse currentResponse;
	string clientId;
	string redirectUrl;
	string decriptPassword;
	bool waitingForToken = false;

	[Header("UI")]
	public GameObject[] afterSignInDisable;
	public GameObject[] afterSignInEnable;
	public GameObject couldNotRecognise;
	public GameObject loggedIn;
	public Animator animator;

	[Header("Settings")]
	public GameObject unlinkButton;
	public GameObject linkButton;
	public GameObject loadPhotosButton;
	public Text loadPhotosButtonText;

	// Encription
	SHA256 mySHA256;
	byte[] key;
	byte[] iv;

	private void Awake() {
		instance = this;
		OnLogout();
		TryToLoadApi();
		loadPhotosButtonText.text = "Load photos";
	}
	void TryToLoadApi() {
		UnityEngine.Object textFile;
		textFile = Resources.Load("API/googlePhotosApi");
		TextAsset temp = textFile as TextAsset;
		string[] keys = temp.text.Split('\n');
		clientId = keys[0];
		redirectUrl = keys[1];
		decriptPassword = keys[2];
		// Create sha256 hash
		mySHA256 = SHA256Managed.Create();
		key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(decriptPassword));

		// Create secret IV
		iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
		Debug.Log("<b>GOOGLE PHOTOS</b> - Loaded api keys from file!");
		if (PlayerPrefs.HasKey("GoogleOauthResponse")) {
			string tempString = DecryptString(PlayerPrefs.GetString("GoogleOauthResponse"), key, iv);
			currentResponse = JsonConvert.DeserializeObject<GoogleOauthResponse>(tempString);
			OnLogin();
		}
	}

	// Logging in
	public void OpenLoginPage() {
		if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(redirectUrl))
			return;
		string url = $"https://accounts.google.com/o/oauth2/v2/auth?scope=https://www.googleapis.com/auth/photoslibrary.readonly&include_granted_scopes=true&state=state_parameter_passthrough_value&redirect_uri={redirectUrl}&response_type=code&client_id={clientId}&prompt=consent&access_type=offline";
		Application.OpenURL(url);
		foreach (var item in afterSignInEnable) {
			item.SetActive(true);
		}
		foreach (var item in afterSignInDisable) {
			item.SetActive(false);
		}
		waitingForToken = true;
	}
	public void Cancel() {
		waitingForToken = false;
		animator.SetTrigger("Close");
	}
	public void OpenLoginDialog() {
		foreach (var item in afterSignInEnable) {
			item.SetActive(false);
		}
		foreach (var item in afterSignInDisable) {
			item.SetActive(true);
		}
		couldNotRecognise.SetActive(false);
		loggedIn.SetActive(false);
		animator.gameObject.SetActive(true);
	}
	public void Restart() {
		currentResponse = null;
		PlayerPrefs.DeleteKey("GoogleOauthResponse");
		OnLogout();
	}
	public void RefreshToken() {
		//string url = $"http://teal-fire.com/MapMoves/refresh.php?refresh={currentResponse.refresh_token}";
		//Application.OpenURL(url);
		StartCoroutine(GetRefreshToken());
	}
	IEnumerator GetRefreshToken() {
		UnityWebRequest webRequest = UnityWebRequest.Get($"http://teal-fire.com/MapMoves/refresh.php?refresh={currentResponse.refresh_token}");
		yield return webRequest.SendWebRequest();

		if (webRequest.isNetworkError || webRequest.isHttpError) {
			Debug.Log("<b>GOOGLE PHOTOS</b> - Could not refresh access token");
		} else {
			Debug.Log("<b>GOOGLE PHOTOS</b> - Access token refreshed");
			GoogleOauthResponse response = JsonConvert.DeserializeObject<GoogleOauthResponse>(webRequest.downloadHandler.text);
			response.SetEndTime();
			PlayerPrefs.SetString("GoogleOauthResponse", EncryptString(JsonConvert.SerializeObject(response), key, iv));
			currentResponse = response;
			SendRequest();
		}
	}

	// On login change
	void OnLogin() {
		unlinkButton.SetActive(true);
		linkButton.SetActive(false);
		loadPhotosButton.SetActive(true);
	}
	void OnLogout() {
		unlinkButton.SetActive(false);
		linkButton.SetActive(true);
		loadPhotosButton.SetActive(false);
	}

	// Checking copied link
	private void OnApplicationFocus(bool focus) {
		if (focus && waitingForToken) {
			TryGetTokenJson();
		}
	}
	void TryGetTokenJson() {
		string tempString = EditorGUIUtility.systemCopyBuffer;
		try {
			string decrypted = this.DecryptString(tempString, key, iv);
			Debug.Log(decrypted);
			GoogleOauthResponse response = JsonConvert.DeserializeObject<GoogleOauthResponse>(decrypted);
			response.SetEndTime();
			PlayerPrefs.SetString("GoogleOauthResponse", EncryptString(JsonConvert.SerializeObject(response), key, iv));
			Debug.Log("<b>GOOGLE PHOTOS</b> - Recieved api keys");
			currentResponse = response;
			waitingForToken = false;
			animator.SetTrigger("Close");
			couldNotRecognise.SetActive(false);
			loggedIn.SetActive(true);
			OnLogin();
			SendRequest();
		} catch (Exception ex) {
			Debug.Log("Invalid JSON!");
			couldNotRecognise.SetActive(true);
		}
	}
	public string DecryptString(string cipherText, byte[] key, byte[] iv) {
		// Instantiate a new Aes object to perform string symmetric encryption
		Aes encryptor = Aes.Create();

		encryptor.Mode = CipherMode.CBC;

		// Set key and IV
		byte[] aesKey = new byte[32];
		Array.Copy(key, 0, aesKey, 0, 32);
		encryptor.Key = aesKey;
		encryptor.IV = iv;

		// Instantiate a new MemoryStream object to contain the encrypted bytes
		MemoryStream memoryStream = new MemoryStream();

		// Instantiate a new encryptor from our Aes object
		ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

		// Instantiate a new CryptoStream object to process the data and write it to the 
		// memory stream
		CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

		// Will contain decrypted plaintext
		string plainText = String.Empty;

		try {
			// Convert the ciphertext string into a byte array
			byte[] cipherBytes = Convert.FromBase64String(cipherText);

			// Decrypt the input ciphertext string
			cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

			// Complete the decryption process
			cryptoStream.FlushFinalBlock();

			// Convert the decrypted data from a MemoryStream to a byte array
			byte[] plainBytes = memoryStream.ToArray();

			// Convert the decrypted byte array to string
			plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
		} finally {
			// Close both the MemoryStream and the CryptoStream
			memoryStream.Close();
			cryptoStream.Close();
		}

		// Return the decrypted data as a string
		return plainText;
	}
	public string EncryptString(string plainText, byte[] key, byte[] iv) {
		// Instantiate a new Aes object to perform string symmetric encryption
		Aes encryptor = Aes.Create();

		encryptor.Mode = CipherMode.CBC;

		// Set key and IV
		byte[] aesKey = new byte[32];
		Array.Copy(key, 0, aesKey, 0, 32);
		encryptor.Key = aesKey;
		encryptor.IV = iv;

		// Instantiate a new MemoryStream object to contain the encrypted bytes
		MemoryStream memoryStream = new MemoryStream();

		// Instantiate a new encryptor from our Aes object
		ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

		// Instantiate a new CryptoStream object to process the data and write it to the 
		// memory stream
		CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

		// Convert the plainText string into a byte array
		byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);

		// Encrypt the input plaintext string
		cryptoStream.Write(plainBytes, 0, plainBytes.Length);

		// Complete the encryption process
		cryptoStream.FlushFinalBlock();

		// Convert the encrypted data from a MemoryStream to a byte array
		byte[] cipherBytes = memoryStream.ToArray();

		// Close both the MemoryStream and the CryptoStream
		memoryStream.Close();
		cryptoStream.Close();

		// Convert the encrypted byte array to a base64 encoded string
		string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

		// Return the encrypted data as a string
		return cipherText;
	}
	// Downloading timeline day
	public void SendRequest() {
		//OutputTimeLineItems();
		loadPhotosButtonText.text = "Connecting...";
		if (currentResponse == null) {
			Debug.Log("<b>GOOGLE PHOTOS</b> - api not connected! Opening login window...");
			OpenLoginDialog();
			return;
		}
		StartCoroutine(Upload());
	}
	IEnumerator Upload() {
		DateTime day = ReadJson.instance.selectedDay;
		string body = "{\"pageSize\":100,\"filters\":{\"dateFilter\":{\"dates\":[{\"day\":" + day.Day + ",\"month\":" + day.Month + ",\"year\":" + day.Year + "}]}}}";

		var request = new UnityWebRequest("https://photoslibrary.googleapis.com/v1/mediaItems:search", "POST");
		byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
		request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");

		request.SetRequestHeader("Authorization", "Bearer " + currentResponse.access_token);

		yield return request.SendWebRequest();

		//Debug.Log("Status Code: " + request.responseCode);

		if (request.isNetworkError || request.isHttpError) {
			Debug.Log("<b>GOOGLE PHOTOS</b> - Could not connect, attempting refresh...");
			RefreshToken();
		} else {
			Debug.Log("<b>GOOGLE PHOTOS</b> - Api connected, downloading photos...!");
			loadPhotosButtonText.text = "Downloading...";
			//Debug.Log(request.downloadHandler.text);
			ReadJSONResponse(request.downloadHandler.text);
		}
	}
	void ReadJSONResponse(string text) {
		GooglePhotosResponse response = JsonConvert.DeserializeObject<GooglePhotosResponse>(text);
		response.CalculateDates();
		AddPhotosToTimeLineItems(response);
		//StartCoroutine(DownloadImage(response.mediaItems[0].baseUrl + "=w400-h400-c", imageToDisplay));

	}
	void AddPhotosToTimeLineItems(GooglePhotosResponse response) {
		List<MediaItem> images = response.mediaItems.ToList();

		List<ActivityUI> timeline = ReadJson.instance.activitiesList;
		List<MediaItem> tempImages = new List<MediaItem>();
		List<ActivityUI> timeLinesWithPhotos = new List<ActivityUI>();
		DateTime startTime = new DateTime();
		for (int i = timeline.Count - 1; i >= 0; i--) {
			tempImages.Clear();
			if (i - 1 >= 0)
				startTime = timeline[i - 1].endTime;
			else {
				DateTime tempD = timeline[i].endTime;
				startTime = new DateTime(tempD.Year, tempD.Month, tempD.Day, 0, 0, 0);
			}
			for (int j = 0; j < images.Count; j++) {
				//Debug.Log("TIME RANGE: " + timeline[i].endTime + " <> " + startTime);
				if (images[j].creationDate < timeline[i].endTime && images[j].creationDate >= startTime) {
					tempImages.Add(images[j]);
					//images.Remove(images[j]);
				}
			}
			if (tempImages.Count > 0) {
				
				timeLinesWithPhotos.Add(timeline[i]);
				timeline[i].DownloadPhotos(tempImages);

			}
		}
		//foreach (var item in timeLinesWithPhotos) {
		//	item.DownloadPhotos();
		//}
	}
	public IEnumerator DownloadImage(string url, Image targetImage) {
		// Start a download of the given URL
		var www = new WWW(url);
		// wait until the download is done
		yield return www;
		// Create a texture in DXT1 format
		Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);

		// assign the downloaded image to sprite
		www.LoadImageIntoTexture(texture);
		Rect rec = new Rect(0, 0, texture.width, texture.height);
		Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 200);

		targetImage.sprite = spriteToUse;

		www.Dispose();
		www = null;
		loadPhotosButtonText.text = "Load photos";


	}
}
