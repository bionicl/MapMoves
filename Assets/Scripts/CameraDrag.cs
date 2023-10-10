using UnityEngine;

public class CameraDrag : MonoBehaviour {
	public static CameraDrag instance;
	public float dragSpeed = 2;
	private Vector3 dragOrigin;
	Vector3 dragMouseOrigin;
	public bool blocked = false;

	void Awake() {
		instance = this;
	}

	void Update() {
		if (blocked)
			return;

		if (Input.GetMouseButtonDown(0) && GlobalVariables.instance.mapControls) {
			dragOrigin = transform.position;
			dragMouseOrigin = Input.mousePosition;
			ButtonTabs.instance.ClearCalendarView();
			return;
		}

		if (!Input.GetMouseButton(0) || !GlobalVariables.instance.mapControls) {
			return;
		}

		if (Input.GetMouseButton(0)) {

			//Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
			//Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

			Vector3 mouseMove = dragMouseOrigin - Input.mousePosition;

			transform.position = dragOrigin + mouseMove * (3.555f / Screen.width) * RenderMap.instance.mapScale;

			GoogleMapDisplay.instance.MapMoved();
			RenderMap.instance.MapMoved();
		}
	}


}