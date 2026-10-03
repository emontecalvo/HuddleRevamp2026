using UnityEngine;

namespace HuddleNights {

	// Steers jellos that no human is playing. In order of priority:
	// 1. A storm is here or coming: go to its own spot by the fire (if it's lit).
	// 2. A teammate froze: go huddle next to them to thaw them out.
	// 3. Otherwise: follow the nearest player around.
	// Throughout, it steps aside from other jellos so they never stack up.
	public static class JelloAI {

		const float FollowDistance = 1.5f;
		const float RescueDistance = 1f;
		// Each AI jello has its own spot in a ring in front of the fire, close enough
		// to be by the fire (< 3) and to huddle with its neighbours (< 2.25).
		const float FireSpotRadius = 1.4f;
		const float FireSpotReached = 0.2f;
		// Jellos closer than this gently push apart, so they never stack on one spot.
		const float SeparationRadius = 0.9f;
		const float SeparationStrength = 1.5f;

		public static Vector2 GetMove (Jello self) {
			Vector2 move = ChooseMove (self) + Separation (self);
			if (move.sqrMagnitude < 0.05f) {
				return Vector2.zero;
			}
			return move.sqrMagnitude > 1f ? move.normalized : move;
		}

		static Vector2 ChooseMove (Jello self) {
			if (Fire.inst.IsLit && IsStormNearby ()) {
				return MoveToward (self, FireSpot (self), FireSpotReached);
			}

			Jello frozen = NearestFrozenTeammate (self);
			if (frozen != null) {
				return MoveToward (self, frozen.transform.position, RescueDistance);
			}

			Jello leader = NearestPlayer (self);
			if (leader != null) {
				return MoveToward (self, leader.transform.position, FollowDistance);
			}
			return Vector2.zero;
		}

		static bool IsStormNearby () {
			if (Thermometer.inst.IsItAStorm) {
				return true;
			}
			float untilStorm = Thermometer.inst.TimeUntilNextStorm ();
			return untilStorm > 0 && untilStorm <= GamePhaseMgr.inst.Settings.StormWarningTime;
		}

		// Spots fan out across the front of the fire (210°, 270°, 330°), so nobody hides behind it.
		static Vector3 FireSpot (Jello self) {
			float angle = (210f + (self.PlayerSlot - 1) * 60f) * Mathf.Deg2Rad;
			return Fire.inst.transform.position + new Vector3 (Mathf.Cos (angle), 0f, Mathf.Sin (angle)) * FireSpotRadius;
		}

		static Vector2 Separation (Jello self) {
			Vector2 push = Vector2.zero;
			foreach (Jello other in JelloMgr.inst.AllJellos) {
				if (other == self) {
					continue;
				}
				Vector3 away = self.transform.position - other.transform.position;
				away.y = 0f;
				float distance = away.magnitude;
				if (distance >= SeparationRadius) {
					continue;
				}
				if (distance < 0.01f) {
					// Exactly on top of each other: split them apart by slot.
					away = new Vector3 (self.PlayerSlot - other.PlayerSlot, 0f, 0.5f);
				}
				push += new Vector2 (away.x, away.z).normalized * (1f - distance / SeparationRadius) * SeparationStrength;
			}
			return push;
		}

		static Vector2 MoveToward (Jello self, Vector3 target, float stopDistance) {
			Vector3 toTarget = target - self.transform.position;
			toTarget.y = 0f;
			if (toTarget.magnitude < stopDistance) {
				return Vector2.zero;
			}
			return new Vector2 (toTarget.x, toTarget.z).normalized;
		}

		// Lost jellos are left for the players to find.
		static Jello NearestFrozenTeammate (Jello self) {
			Jello nearest = null;
			float nearestDistance = float.MaxValue;
			foreach (Jello jello in JelloMgr.inst.AllJellos) {
				if (jello == self || !jello.AmIFrozen || jello.IsLost) {
					continue;
				}
				float distance = Vector3.Distance (jello.transform.position, self.transform.position);
				if (distance < nearestDistance) {
					nearest = jello;
					nearestDistance = distance;
				}
			}
			return nearest;
		}

		static Jello NearestPlayer (Jello self) {
			Jello nearest = null;
			float nearestDistance = float.MaxValue;
			foreach (Jello jello in JelloMgr.inst.AllJellos) {
				if (jello == self || jello.AmIFrozen || !PlayerRoster.IsHuman (jello.PlayerSlot)) {
					continue;
				}
				float distance = Vector3.Distance (jello.transform.position, self.transform.position);
				if (distance < nearestDistance) {
					nearest = jello;
					nearestDistance = distance;
				}
			}
			return nearest;
		}
	}
}
