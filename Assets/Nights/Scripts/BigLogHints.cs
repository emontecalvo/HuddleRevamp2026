using UnityEngine;

namespace HuddleNights {

	// One-time hints for nights with big logs: too heavy alone, teamwork works,
	// and the payoff at the fire. Added by GamePhaseMgr when the scene has big logs.
	public class BigLogHints : MonoBehaviour {

		bool SaidTooHeavy = false;
		bool SaidTeamwork = false;
		bool SaidDelivered = false;

		void Update () {
			if (!GamePhaseMgr.inst.IsGame) {
				return;
			}

			foreach (BigLog log in BigLog.All) {
				if (!SaidTooHeavy && !log.IsMoving && log.HasHumanCarrier ()) {
					SaidTooHeavy = true;
					NightMessageUI.inst.ShowHint ("This log is too heavy for one jello! Get a friend to help", 5f);
				}
				if (!SaidTeamwork && log.IsMoving) {
					SaidTeamwork = true;
					NightMessageUI.inst.ShowHint ("Teamwork! Drag the big log to the fire", 4f);
				}
			}

			if (!SaidDelivered && Fire.inst.BigLogsDelivered > 0) {
				SaidDelivered = true;
				NightMessageUI.inst.ShowHint ("A big log! That'll keep the fire going for a long time", 4f);
			}
		}
	}
}
