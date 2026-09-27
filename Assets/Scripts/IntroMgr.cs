using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroMgr : MonoBehaviour
{


	private static IntroMgr _inst = null;

	public static IntroMgr inst {
		get {
			if (_inst == null) {
				_inst = FindAnyObjectByType<IntroMgr> ();
			}
			return _inst;
		}
	}

	public GameObject Panel0;
	public GameObject Panel1;
	public GameObject Panel2;
	public GameObject Panel3;

	public Button Button0;
	public Button Button1;
	public Button Button2;
	public Button Button3;

	int CurrentPanel = 0;

	void Start ()
	{
		vehicleIntroPanel1.inst.enabled = true;
		VehicleIntroPanel2.inst.enabled = true;
		VehicleIntroPanel3.inst.enabled = true;
		Panel0.SetActive (true);
		Panel1.SetActive (false);
		Panel2.SetActive (false);
		Panel3.SetActive (false);
		Button0.onClick.AddListener (Button0Clicked);
		Button1.onClick.AddListener (Button1Clicked);
		Button2.onClick.AddListener (Button2Clicked);
		Button3.onClick.AddListener (Button3Clicked);
	}


	void Update ()
	{
		
	}

	void SetCurrentPanel(int newCurrentPanel) {
		CurrentPanel = newCurrentPanel;
		Panel0.SetActive (CurrentPanel == 0);
		Panel1.SetActive (CurrentPanel == 1);
		Panel2.SetActive (CurrentPanel == 2);
		Panel3.SetActive (CurrentPanel == 3);
	}

	void Button0Clicked() {
		SetCurrentPanel (1);
	}

	void Button1Clicked() {
		SetCurrentPanel (2);
	}

	void Button2Clicked() {
		SetCurrentPanel (3);
	}

	void Button3Clicked() {
		SetCurrentPanel (4);
		GamePhaseMgr.inst.BeginGame ();
	}
}
