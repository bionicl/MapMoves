using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ConverterLibrary {
	public class JsonMoves {
		[JsonConverter(typeof(StringEnumConverter))]
		public enum SegmentType {
			place,
			move
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum ActivityGroup {
			walking,
			cycling,
			running,
			transport
		}

		public class Day {
			public class Summary {
				public ActivityType activity;
				public ActivityGroup group;
				public double duration;
				public double distance;
				public double? calories;

				public Summary(ActivityType activity, float duration, double distance, float calories) {
					this.activity = activity;
					group = HelpMethods.ReturnGroup(activity);
					this.duration = duration;
					this.distance = Math.Round(distance * 1000);
					if (calories > 0)
						this.calories = calories;
				}
			}
			public class Segment {
				public class Place {
					public class Location {
						public double lat;
						public double lon;

						public Location(double lat, double lon) {
							this.lat = lat;
							this.lon = lon;
						}
					}
					public int id;
					public string name;
					public string type = "user";
					public Location location;

					// Place
					public string facebookPlaceId;
					public string foursquareId;

					public Place(string name, Location location, DateTime startTime, DateTime endTime, string link) {
						this.name = name;
						this.location = location;
						this.id = PlacesManager.ReturnPlaceId(this, startTime, endTime);

						if (!string.IsNullOrEmpty(link)) {
							if (link.StartsWith("http://facebook.com/", StringComparison.Ordinal)) {
								type = "facebook";
								facebookPlaceId = link.Replace("http://facebook.com/", "");
							} else if (link.StartsWith("http://foursquare.com/", StringComparison.Ordinal)) {
								type = "foursquare";
								foursquareId = link.Replace("http://foursquare.com/venue/", "");
							}
						}
					}

				}
				public class Activity {
					public class TrackPoint {
						public double lat;
						public double lon;
						public string time;

						public TrackPoint(double lat, double lon, DateTime time) {
							this.lat = lat;
							this.lon = lon;
							this.time = HelpMethods.ConvertToIso1601(time);
						}

					}

					public ActivityType activity;
					public ActivityGroup group;
					public bool manual = false;
					public string startTime;
					public string endTime;
					public double duration;
					public double distance;
					public double calories;
					public TrackPoint[] trackPoints;

					public Activity(ActivityType type, ActivityGroup group, DateTime startTime,
									DateTime endTime, double duration, double distance,
									double calories, List<TrackPoint> trackpoints) {
						activity = type;
						this.group = group;
						this.startTime = HelpMethods.ConvertToIso1601(startTime);
						this.endTime = HelpMethods.ConvertToIso1601(endTime);
						this.duration = duration;
						this.distance = Math.Round(distance * 1000);
						if (calories > 0)
							this.calories = calories;
						this.trackPoints = trackpoints.ToArray();
					}

				}

				public SegmentType type;
				public string startTime;
				public string endTime;
				public Place place;
				public Activity[] activities;
				public string lastUpdate;

				public Segment(SegmentType type, DateTime startTime, DateTime endTime) {
					this.type = type;
					this.startTime = HelpMethods.ConvertToIso1601(startTime);
					this.endTime = HelpMethods.ConvertToIso1601(endTime);
					lastUpdate = HelpMethods.ConvertToIso1601(DateTime.Now);
				}

			}
			public string date;
			public Summary[] summary;
			public Segment[] segments;
			public string caloriesIdle;
			public string lastUpdate;

			public Day(DateTime date) {
				this.date = date.ToString("yyyyMMdd");
				lastUpdate = HelpMethods.ConvertToIso1601(DateTime.Now);
			}
		}
		public Day[] day;
	}



	public static class JsonParser {
		public static string Parse(List<XmlReader> xml, string fileName, bool writeToFile = false) {
			JsonMoves json = new JsonMoves();
			json.day = new JsonMoves.Day[xml.Count];
			for (int i = 0; i < xml.Count; i++) {
				// Date
				json.day[i] = new JsonMoves.Day(xml[i].date);

				// Summary
				ParseSummary(i, json, xml);

				// Segments
				ParseSegments(i, json, xml);
			}
			string output = JsonConvert.SerializeObject(json,
														Newtonsoft.Json.Formatting.None,
														new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			});
			output = CleanUpJson(output);
			if (writeToFile)
				WriteToFile(output, fileName);
			return output;
		}


		static void ParseSummary(int i, JsonMoves json, List<XmlReader> xml) {
			List<ActivitySummary> summaries = new List<ActivitySummary>();
			for (int o = 0; o < 10; o++) {
				if (xml[i].summary[o].duration > 0)
					summaries.Add(xml[i].summary[o]);
			}
			List<JsonMoves.Day.Summary> convertedSummaries = new List<JsonMoves.Day.Summary>();
			foreach (var item in summaries) {
				convertedSummaries.Add(item.ToMoves());
			}
			json.day[i].summary = convertedSummaries.ToArray();
		}

		// Summary parsing
		static void ParseSegments(int i, JsonMoves json, List<XmlReader> xml) {
			List<JsonMoves.Day.Segment> segments = new List<JsonMoves.Day.Segment>();
			foreach (var item in xml[i].timelineItems) {
				if (item.type == XmlTimeline.TimelineItemType.place)
					segments.Add(SegmentPlace(item.place));
				else
					segments.Add(SegmentMove(item.activity));
			}
			json.day[i].segments = segments.ToArray();
		}
		static JsonMoves.Day.Segment SegmentPlace(XmlTimeline.Place item) {
			JsonMoves.Day.Segment output = new JsonMoves.Day.Segment(JsonMoves.SegmentType.place, item.startTime.Value, item.endTime.Value);
			output.place = new JsonMoves.Day.Segment.Place(item.name,
														new JsonMoves.Day.Segment.Place.Location(item.position.lat, item.position.lon),
														item.startTime.Value,
														item.endTime.Value,
														item.link);
			return output;
		}
		static JsonMoves.Day.Segment SegmentMove(XmlTimeline.Activity item) {
			JsonMoves.Day.Segment output = new JsonMoves.Day.Segment(JsonMoves.SegmentType.move, item.startTime, item.endTime);

			// TrackPoints
			List<JsonMoves.Day.Segment.Activity.TrackPoint> trackpoints = new List<JsonMoves.Day.Segment.Activity.TrackPoint>();
			foreach (var waypoint in item.waypoints) {
				trackpoints.Add(new JsonMoves.Day.Segment.Activity.TrackPoint(waypoint.lat, waypoint.lon, waypoint.time.Value));
			}

			// Setup
			List<JsonMoves.Day.Segment.Activity> activity = new List<JsonMoves.Day.Segment.Activity>();
			activity.Add(new JsonMoves.Day.Segment.Activity(item.activity,
															HelpMethods.ReturnGroup(item.activity),
															item.startTime,
															item.endTime,
															item.Duration,
															item.Distance,
															item.Calories,
															trackpoints));
			output.activities = activity.ToArray();
			return output;
		}

		// Clean up to match moves json output
		static string CleanUpJson(string json) {
			json = json.Substring(7);
			json = json.Remove(json.Length - 1);
			return json;
		}

		static void WriteToFile(string json, string fileName) {
			StreamWriter sw = new StreamWriter(fileName);
			sw.Write(json);
			sw.Close();
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine(string.Format("Json file created!\n") + fileName);
		}
	}
}

