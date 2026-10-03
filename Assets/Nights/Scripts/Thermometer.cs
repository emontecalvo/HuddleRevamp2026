using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace HuddleNights {

	public class Thermometer : MonoBehaviour
	{

		private static Thermometer _inst = null;

		public static Thermometer inst {
			get {
				if (_inst == null) {
					_inst = FindAnyObjectByType<Thermometer> ();
				}
				return _inst;
			}
		}

		public Image ThermoFillImg;
		public Image ThermoIceFill;
		public float fillAmount;
		public float randomFill;

		public ParticleSystem StormOneParticle;
		public ParticleSystem MainParticle;

		public bool IsItAStorm = false;

		public float TotalFill;

		public GameObject Thermo;

		void Update ()
		{

			if (GamePhaseMgr.inst.IsGame) {
				Thermo.SetActive (true);
			} else {
				Thermo.SetActive (false);
			}

			if (!GamePhaseMgr.inst.IsGame) {
				return;
			}

			float time = GamePhaseMgr.inst.GetGameTime ();
			if (!IsItAStorm) {
				fillAmount = 0.1f;
				randomFill = Mathf.Cos (time) * 0.05f;
				TotalFill = fillAmount + randomFill;
			} else {
				TotalFill -= 1 * Time.deltaTime;
				if (TotalFill < 0) {
					TotalFill = 0;
				}
			}

			ThermoFillImg.fillAmount = TotalFill;

			bool stormNow = IsStormAt (time);
			if (stormNow && !IsItAStorm) {
				BeginStorm ();
			} else if (!stormNow && IsItAStorm) {
				EndStorm ();
			}
		}

		bool IsStormAt (float time) {
			foreach (StormWindow storm in GamePhaseMgr.inst.Settings.Storms) {
				if (time >= storm.StartTime && time < storm.EndTime) {
					return true;
				}
			}
			return false;
		}

		// Seconds until the next storm starts, or -1 if no more storms tonight.
		public float TimeUntilNextStorm () {
			float time = GamePhaseMgr.inst.GetGameTime ();
			float soonest = -1f;
			foreach (StormWindow storm in GamePhaseMgr.inst.Settings.Storms) {
				float until = storm.StartTime - time;
				if (until > 0 && (soonest < 0 || until < soonest)) {
					soonest = until;
				}
			}
			return soonest;
		}

		void BeginStorm ()
		{
			IsItAStorm = true;
			MainParticle.gameObject.SetActive (false);
			StormOneParticle.gameObject.SetActive (true);
			ThermoIceFill.gameObject.SetActive (true);
			ThermoIceFill.DOFillAmount (1f, 1f);
		}

		void EndStorm ()
		{
			IsItAStorm = false;
			MainParticle.gameObject.SetActive (true);
			StormOneParticle.gameObject.SetActive (false);
			ThermoIceFill.DOFillAmount (0f, 1f).OnComplete (() => {
				ThermoIceFill.gameObject.SetActive (false);
			});
		}
	}
}
