using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace PlacesDataJson {
	public class PlaceMainCategory
	{
		public string id { get; set; }
		public string displayName { get; set; }
		public string color { get; set; }
		public int order { get; set; }
		[JsonIgnore]
		Color? _colorConverted = null;
		[JsonIgnore]
		public Color ColorConverted {
			get {
				if (!_colorConverted.HasValue) {
					ColorUtility.TryParseHtmlString("#" + color, out Color newColor);
					_colorConverted = newColor;
				}
				return _colorConverted.Value;
			}
		}
	}

	public class Keywords
	{
		public List<string> english { get; set; }
		public List<string> polish { get; set; }
		public List<string> german { get; set; }
		public List<string> french { get; set; }
		public List<string> spanish { get; set; }
		public List<string> italian { get; set; }
		public List<string> czech { get; set; }
		public List<string> portuguese { get; set; }

		[JsonIgnore]
		public List<string> _combined = null;
		[JsonIgnore]
		public List<string> Combined {
			get {
				if (_combined == null) {
					_combined = new List<string>();
					if (english != null)
						_combined.AddRange(english);
					if (polish != null)
						_combined.AddRange(polish);
					if (german != null)
						_combined.AddRange(german);
					if (french != null)
						_combined.AddRange(french);
					if (spanish != null)
						_combined.AddRange(spanish);
					if (italian != null)
						_combined.AddRange(italian);
					if (czech != null)
						_combined.AddRange(czech);
					if (portuguese != null)
						_combined.AddRange(portuguese);
					for (int i = 0; i < _combined.Count; i++) {
						_combined[i] = _combined[i].ToLower();
					}
				}
				return _combined;
			}
		}
	}

	public class ChainInfo
	{
		public List<string> usa { get; set; }
		public List<string> uk { get; set; }
		public List<string> poland { get; set; }
		public List<string> australia { get; set; }
		public List<string> germany { get; set; }
		public List<string> france { get; set; }
		public List<string> spain { get; set; }
		public List<string> italy { get; set; }
		public List<string> czech { get; set; }
		public List<string> portugal { get; set; }

		[JsonIgnore]
		public List<string> _combined = null;
		[JsonIgnore]
		public List<string> Combined {
			get {
				if (_combined == null) {
					_combined = new List<string>();
					if (usa != null)
						_combined.AddRange(usa);
					if (uk != null)
						_combined.AddRange(uk);
					if (poland != null)
						_combined.AddRange(poland);
					if (australia != null)
						_combined.AddRange(australia);
					if (germany != null)
						_combined.AddRange(germany);
					if (france != null)
						_combined.AddRange(france);
					if (spain != null)
						_combined.AddRange(spain);
					if (italy != null)
						_combined.AddRange(italy);
					if (czech != null)
						_combined.AddRange(czech);
					if (portugal != null)
						_combined.AddRange(portugal);

					for (int i = 0; i < _combined.Count; i++) {
						_combined[i] = _combined[i].ToLower();
					}
				}
				return _combined;
			}
		}
	}

	public class PlaceCategory
	{
		public string id { get; set; }
		public string displayName { get; set; }
		public string category { get; set; }
		public Keywords keywords { get; set; }
		public ChainInfo chains { get; set; }
		[JsonIgnore]
		public PlaceMainCategory placeTypeCategory { get; set; }
		[JsonIgnore]
		public Sprite smallIcon { get; set; }
		[JsonIgnore]
		public Sprite bigIcon { get; set; }

		public void SetupPlaceTypeCategory(List<PlaceMainCategory> mainCategories) {
			foreach (var item in mainCategories) {
				if (item.id == category) {
					placeTypeCategory = item;
					break;
				}
			}
			if (placeTypeCategory == null)
				Debug.LogError("Haven't found category for " + displayName);
		}
		public void SetupIcons() {
			smallIcon = Resources.Load<Sprite>("Icons/" + id + " 32px");
			bigIcon = Resources.Load<Sprite>("Icons/" + id + " 64px");
		}

		public string CheckIfMatchesKeywords(string name) {
			if (keywords == null)
				return null;
			foreach (var item in keywords.Combined) {
				if (name.Contains(item)) {
					return id;
				}
			}
			return null;
		}
		public string CheckIfMatchesChains(string name) {
			if (chains == null)
				return null;
			foreach (var item in chains.Combined) {
				if (name.Contains(item)) {
					return id;
				}
			}
			return null;
		}
	}

	public class PlacesDataJsonRoot
	{
		public List<PlaceMainCategory> placeMainCategories { get; set; }
		public List<PlaceCategory> placeCategories { get; set; }
	}
}