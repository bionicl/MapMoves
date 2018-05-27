using UnityEngine;

public class CameraDrag : MonoBehaviour {
	public float dragSpeed = 2;
	private Vector3 dragOrigin;
	Vector3 dragMouseOrigin;

	void Update() {
		if (Input.GetMouseButtonDown(0) && GlobalVariables.inst.mapControls) {
			dragOrigin = transform.position;
			dragMouseOrigin = Input.mousePosition;
			return;
		}

		if (!Input.GetMouseButton(0) || !GlobalVariables.inst.mapControls) {
			return;
		}

		//Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
		//Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

		Vector3 mouseMove = dragMouseOrigin - Input.mousePosition;

		transform.position = dragOrigin + mouseMove * (3.555f / Screen.width) * RenderMap.instance.mapScale;

		GoogleMapDisplay.instance.MapMoved();
	}


}