using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FiltersBox : MonoBehaviour {

	public CanvasGroup[] transportDisabled;
	bool[] transportTypeIsOn = new bool[5];

	public FilterButton[] filterButtons;

	public void EnableTransport(bool enable) {
		if (enable) {
			foreach (CanvasGroup item in transportDisabled) {
				item.alpha = 1f;
				item.blocksRaycasts = true;
			}
			for (int i = 0; i < 5; i++) {
				RenderMap.instance.ChangeFilter(filterButtons[i].filterType, transportTypeIsOn[i]);
			}
		} else {
			foreach (CanvasGroup item in transportDisabled) {
				item.alpha = 0.4f;
				item.blocksRaycasts = false;
			}
			for (int i = 0; i < 5; i++) {
				transportTypeIsOn[i] = filterButtons[i].isOn;
			}
			RenderMap.instance.ChangeFilter(FilterTypes.car, false);
			RenderMap.instance.ChangeFilter(FilterTypes.bus, false);
			RenderMap.instance.ChangeFilter(FilterTypes.train, false);
			RenderMap.instance.ChangeFilter(FilterTypes.plane, false);
			RenderMap.instance.ChangeFilter(FilterTypes.otherTransport, false);
		}
	}

	public void ColorsSwitch(bool colorsEnabled) {
		RenderMap.instance.ChangeFilterColor(!colorsEnabled);
		foreach (var item in filterButtons) {
			item.ChangeButtonColor(!colorsEnabled);
		}
	}
}
