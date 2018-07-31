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

	public void SetupIcon(Sprite icon, int id, Color boxColor) {
		this.icon.sprite = icon;
		Color tempColor = boxColor;
		tempColor.a = 0.75f;
		box.color = tempColor;
		iconId = id;
	}

	public void IconClicked() {
		RightListUI.instance.IconClicked(iconId);
	}

	public void MarkAsSelected() {
		Color tempColor = box.color;
		tempColor.a = 1;
		box.color = tempColor;
	}

	public void MarkAsDeselected() {
		Color tempColor = box.color;
		tempColor.a = 0.5f;
		box.color = tempColor;
	}
}
