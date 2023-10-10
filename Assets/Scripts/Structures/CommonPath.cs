using UnityEngine;

public class CommonPath {
	Vector3 startPos;
	Vector3 endPos;
	ActivityType activityType;
	public int occurencesTimes = 1;

	public CommonPath(Vector3 startPos, Vector3 endPos, ActivityType activityType) {
		this.startPos = startPos;
		this.endPos = endPos;
		this.activityType = activityType;
	}

	public bool CheckIfTheSameCommonPath(CommonPath otherCommonPath) {
		// 1. Check if activity type is the same
		if (activityType != otherCommonPath.activityType) {
			return false;
		}

		// 2. Check if start and end positions are close enough
		bool isNotTransport = activityType == ActivityType.walking || activityType == ActivityType.running || activityType == ActivityType.cycling;
		float startEndPosDelta = isNotTransport ? CommonPathChecker.startEndPosDeltaNonTransport : CommonPathChecker.startEndPosDeltaTransport;
		if (Vector3.Distance(startPos, otherCommonPath.startPos) < startEndPosDelta &&
			Vector3.Distance(endPos, otherCommonPath.endPos) < startEndPosDelta) {
				return true;
		}
		return false;
	}
}