using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconBox : MonoBehaviour {

	public Color unselectedColor;
	public Color selectedColor;
	public Image box;
	public Image icon;
	[HideInInspector]
	public int iconId;

	public void SetupIcon(Sprite icon, int id) {
		this.icon.sprite = icon;
		box.color = unselectedColor;
		iconId = id;
	}

	public void IconClicked() {
		RightListUI.instance.IconClicked(iconId);
	}

	public void MarkAsSelected() {
		box.color = selectedColor;
	}

	public void MarkAsDeselected() {
		box.color = unselectedColor;
	}
}
