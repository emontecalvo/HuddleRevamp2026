using UnityEngine;

namespace HuddleNights {

	// Night 1's hints. Teaches by watching what the players do instead of stopping the game:
	// a step hint stays up until the player does it (move, chop, haul wood to the fire),
	// and one-time hints pop up when something happens (getting cold, fire running low).
	// Added by GamePhaseMgr when the night's settings have TutorialHints turned on.
	public class NightTutorial : MonoBehaviour {

		enum Step { Move, Chop, Haul, Done }

		const string MoveHint = "Move with the arrow keys, WASD, or a gamepad";
		const string ChopHint = "The fire needs wood! Walk into a tree to chop it";
		const string HaulHint = "Drag the wood back to the fire!";

		const float StepHintDelay = 0.75f;		// let the title card fade first
		const float PopupTime = 4f;
		const float ColdTemp = 5f;
		const float FireLowWood = 1f;
		const float AlmostMorningTime = 15f;

		Step CurrentStep = Step.Move;
		float PopupUntil = -1f;
		Vector3[] StartPositions;

		bool SaidCold = false;
		bool SaidFireLow = false;
		bool SaidFireOut = false;
		bool SaidHalfway = false;
		bool SaidAlmostMorning = false;

		void Update () {
			GamePhaseMgr phase = GamePhaseMgr.inst;
			if (!phase.IsGame) {
				return;
			}

			UpdateStep ();
			CheckPopups (phase);

			bool popupShowing = Time.time < PopupUntil;
			if (!popupShowing && CurrentStep != Step.Done && phase.GetGameTime () > StepHintDelay) {
				// Re-sent every frame, so the hint stays up until the step is done.
				NightMessageUI.inst.ShowHint (StepText (), 0.25f);
			}
		}

		void UpdateStep () {
			switch (CurrentStep) {
			case Step.Move:
				if (AnyJelloMoved ()) {
					CurrentStep = Step.Chop;
				}
				break;

			case Step.Chop:
				foreach (Jello jello in JelloMgr.inst.AllJellos) {
					if (jello.BeingHauled != null) {
						CurrentStep = Step.Haul;
					}
				}
				break;

			case Step.Haul:
				if (Fire.inst.WoodDelivered > 0) {
					CurrentStep = Step.Done;
					Popup ("The fire's roaring! Keep it fed until morning");
				}
				break;
			}
		}

		void CheckPopups (GamePhaseMgr phase) {
			if (!SaidCold) {
				foreach (Jello jello in JelloMgr.inst.AllJellos) {
					if (jello.MyTemp < ColdTemp && !jello.IsNextToFire) {
						SaidCold = true;
						Popup ("Brrr! Get back to the fire to warm up");
					}
				}
			}

			// Fire hints wait until the players have learned to feed it.
			if (CurrentStep == Step.Done) {
				if (!SaidFireOut && !Fire.inst.IsLit) {
					SaidFireOut = true;
					Popup ("The fire went out! Bring wood to light it again");
				} else if (!SaidFireLow && Fire.inst.IsLit && Fire.inst.NumberOfWood < FireLowWood) {
					SaidFireLow = true;
					Popup ("The fire is getting low. Fetch more wood!");
				}
			}

			if (!SaidHalfway && phase.GetGameTime () > phase.Settings.NightLength / 2f) {
				SaidHalfway = true;
				Popup ("Halfway to morning!");
			}

			if (!SaidAlmostMorning && phase.GetTimeLeft () < AlmostMorningTime) {
				SaidAlmostMorning = true;
				Popup ("Almost morning... hang in there!");
			}
		}

		bool AnyJelloMoved () {
			var jellos = JelloMgr.inst.AllJellos;
			if (StartPositions == null || StartPositions.Length != jellos.Count) {
				StartPositions = new Vector3[jellos.Count];
				for (int i = 0; i < jellos.Count; i++) {
					StartPositions [i] = jellos [i].transform.position;
				}
				return false;
			}
			for (int i = 0; i < jellos.Count; i++) {
				if (Vector3.Distance (jellos [i].transform.position, StartPositions [i]) > 1f) {
					return true;
				}
			}
			return false;
		}

		string StepText () {
			switch (CurrentStep) {
			case Step.Move: return MoveHint;
			case Step.Chop: return ChopHint;
			case Step.Haul: return HaulHint;
			}
			return "";
		}

		void Popup (string text) {
			NightMessageUI.inst.ShowHint (text, PopupTime);
			PopupUntil = Time.time + PopupTime;
		}
	}
}
