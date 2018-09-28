using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArcJson {
	public class TimelineItem {
		public class Radius {
			public float mean;
			public float sd;
		}
		public class Center {
			public float longitude;
			public float latitude;
		}
		public class Sample {
			public class Location {
				public int verticalAccuracy;
				public int horizontalAccuracy;

				public float speed;
				public float longitude;
				public float latitude;
				public float course;
				public float altitude;

				public string timestamp;
			}
			public class Place {
				public string placeId;
				public Radius radius;
				public bool isHome;
				public string name;
				public Center center;
			}

			// Recording
			public string confirmedType;
			public float zAcceleration;
			public float xyAcceleration;
			public float courseVariance;
			public string recordingState;
			public string movingState;
			public string coreMotionActivityType;

			// Data
			public string date;
			public Location location;
			public float stepHz;

			// ID's
			public string timelineItemId;
			public string sampleId;
		}

		// Activity type
		public string activityType;
		public bool uncertainActivityType;
		public bool manualActivityType;
		public bool isVisit;
		public float activityTypeConfidenceScore;

		// Item ID's
		public string nextItemId;
		public string previousItemId;
		public string itemId;

		// Position
		public Radius radius;
		public Center center;
		public Sample[] samples;
		public float altitude;


		// Place
		public Place place;
		public bool manualPlace;
		public string streetAddress;

		// Stats
		public float maxHeartRate;
		public float averageHeartRate;
		public int stepCount;
		public float activeEnergyBurned;
		public int hkStepCount;

		// Date
		public string endDate;
		public string startDate;

		// Other

	}

	public TimelineItem[] timelineItems;
}

public class ReadArcAppJson : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
