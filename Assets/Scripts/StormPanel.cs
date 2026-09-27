using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StormPanel : MonoBehaviour {

	private static StormPanel _inst = null;

	public static StormPanel inst {
		get {
			if (_inst == null) {
				_inst = FindAnyObjectByType<StormPanel> ();
			}
			return _inst;
		}
	}

	public GameObject StormPanel1;
	public GameObject StormPanel2;

	void Start () {
		StormPanel1.SetActive (false);
		StormPanel2.SetActive (false);
	}

	void Update () {
		if (GamePhaseMgr.inst.GetGameTime() > 20 && GamePhaseMgr.inst.GetGameTime() < 27) {
			StormPanel1.SetActive (true);
			StormPanel2.SetActive (true);
		} else if (GamePhaseMgr.inst.GetGameTime() > 40 && GamePhaseMgr.inst.GetGameTime() < 47) {
			StormPanel1.SetActive (true);
			StormPanel2.SetActive (true);
		} else {
			StormPanel1.SetActive (false);
			StormPanel2.SetActive (false);	
		}
	}


}
