using System;
using System.Collections.Generic;

namespace ConverterLibrary {
	public class ArcExportConverter {

		/// <summary>
		/// Converts the gpx from Arc app to json from Moves app.
		/// </summary>
		/// <param name="gpxString">Gpx string.</param>
		/// <param name="weight">Weight.</param>
		public static string ConvertGpxToJson(string gpxString, float weight) {
			XmlReader xr = new XmlReader(gpxString, false, weight);

			// Split into days
			List<XmlReader> daysInXml = XmlReader.Split(xr);
			string output = JsonParser.Parse(daysInXml, xr.originalName + ".json");
			return output;
		}

	}
}
