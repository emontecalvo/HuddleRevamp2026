using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleIntroPanel3 : MonoBehaviour {

	private static VehicleIntroPanel3 _inst = null;

	public static VehicleIntroPanel3 inst {
		get {
			if (_inst == null) {
				_inst = FindAnyObjectByType<VehicleIntroPanel3> ();
			}
			return _inst;
		}
	}

	public Image CarCrash;

	void Start () {

	}

	void Update () {

	}

	public void SetVehicleCrash (Sprite sprite) {
		CarCrash.sprite = sprite;
	}
}
