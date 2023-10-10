using System.Collections.Generic;

public class CommonPathChecker {

	// Settings
	public static int groupPathsAfterOccurences = 5;
	public static float startEndPosDeltaNonTransport = 0.1f;
	public static int startEndPosDeltaTransport = 2;

	// Cache
	static List<CommonPath> commonPaths = new List<CommonPath>();

	public static bool CheckIfPathCanBeIgnored(CommonPath path) {

		foreach (var commonPath in commonPaths)
		{
			if (commonPath.CheckIfTheSameCommonPath(path)) {
				commonPath.occurencesTimes++;
				return commonPath.occurencesTimes > groupPathsAfterOccurences;
			}
		}
		// If not found, add new common path
		commonPaths.Add(path);
		return false;
	}


}