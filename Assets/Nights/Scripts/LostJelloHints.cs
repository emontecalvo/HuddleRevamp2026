using UnityEngine;

namespace HuddleNights {

	// Hints for nights with a lost jello to find: go look, huddle to thaw them, and
	// (when there's a free gamepad) how another player can take them over.
	// Added by GamePhaseMgr when the night's settings have a LostJelloSlot.
	public class LostJelloHints : MonoBehaviour {

		const float SearchHintTime = 1f;
		const float NearDistance = 4f;
		const float JoinHintDelay = 4.5f;

		Jello Lost;
		bool SaidSearch = false;
		bool SaidFound = false;
		bool SaidJoin = false;
		float FoundTime;

		void Update () {
			GamePhaseMgr phase = GamePhaseMgr.inst;
			if (!phase.IsGame) {
				return;
			}

			if (Lost == null) {
				Lost = FindLostJello ();
				if (Lost == null) {
					return;
				}
			}

			string name = Lost.JelloName;

			if (!SaidSearch && phase.GetGameTime () > SearchHintTime) {
				SaidSearch = true;
				NightMessageUI.inst.ShowHint (name + " is lost somewhere in the snow. Go find " + name + "!", 5f);
			}

			if (Lost.IsLost) {
				if (IsAnyoneNear ()) {
					// Re-sent every frame, so it stays up while they're thawing.
					NightMessageUI.inst.ShowHint (name + " is frozen solid! Stay right next to " + name + " to thaw the ice", 0.25f);
				}
				return;
			}

			if (!SaidFound) {
				SaidFound = true;
				FoundTime = Time.time;
				NightMessageUI.inst.ShowHint ("You found " + name + "! Huddle together to stay warm", 4f);
			}

			if (!SaidJoin && Time.time > FoundTime + JoinHintDelay) {
				SaidJoin = true;
				if (!PlayerRoster.IsHuman (Lost.PlayerSlot) && PlayerRoster.HasFreeGamepad ()) {
					NightMessageUI.inst.ShowHint ("Player " + (Lost.PlayerSlot + 1) + ": press A on a gamepad to play as " + name, 6f);
				}
			}
		}

		Jello FindLostJello () {
			foreach (Jello jello in JelloMgr.inst.AllJellos) {
				if (jello.IsLost) {
					return jello;
				}
			}
			return null;
		}

		bool IsAnyoneNear () {
			foreach (Jello jello in JelloMgr.inst.AllJellos) {
				if (jello != Lost && !jello.AmIFrozen &&
					Vector3.Distance (jello.transform.position, Lost.transform.position) < NearDistance) {
					return true;
				}
			}
			return false;
		}
	}
}
