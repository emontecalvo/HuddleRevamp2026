using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuddleNights {

	// Shows the "storm approaching!" panels for the night's StormWarningTime before each storm.
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
			bool warn = false;
			float warningTime = GamePhaseMgr.inst.Settings.StormWarningTime;

			if (GamePhaseMgr.inst.IsGame && warningTime > 0) {
				float untilStorm = Thermometer.inst.TimeUntilNextStorm ();
				warn = untilStorm > 0 && untilStorm <= warningTime;
			}

			StormPanel1.SetActive (warn);
			StormPanel2.SetActive (warn);
		}
	}
}
