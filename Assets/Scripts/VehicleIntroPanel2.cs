using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleIntroPanel2 : MonoBehaviour {

	private static VehicleIntroPanel2 _inst = null;

	public static VehicleIntroPanel2 inst {
		get {
			if (_inst == null) {
				_inst = FindAnyObjectByType<VehicleIntroPanel2> ();
			}
			return _inst;
		}
	}

	public Image Vehicle1;
	public Image Vehicle2;
	public Image Vehicle3;
	public Image Vehicle4;
	public Image Vehicle5;

	void Start () {
		
	}
	

	void Update () {
		
	}

	public void SetVehicle (Sprite sprite) {
		Vehicle1.sprite = sprite;
		Vehicle2.sprite = sprite;
		Vehicle3.sprite = sprite;
		Vehicle4.sprite = sprite;
		Vehicle5.sprite = sprite;
	}
}
