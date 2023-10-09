using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ConverterLibrary {
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ActivityType {
		walking,
		cycling,
		running,
		car,
		transport,
		train,
		bus,
		motorcycle,
		airplane,
		boat
	}

	public static class BurnedCalCalculator {
		public static float[] activityTypeMultiplayer = { 0.79f, 0.37f, 1.03f };
		// multiplayer values based on https://www.topendsports.com/weight-loss/energy-met.htm
		// and Moves app data

		public static int Calcualate(ActivityType type, float time, float avgSpeed, float weight) {
			if ((int)type > 2)
				return 0;
			float value = activityTypeMultiplayer[(int)type] * time / 3600 * avgSpeed * weight;
			return (int)Math.Round(value);
		}

	}
}
