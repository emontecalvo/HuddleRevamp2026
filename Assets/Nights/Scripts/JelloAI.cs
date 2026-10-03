using UnityEngine;

namespace HuddleNights {

	// Steers jellos that no human is playing. For now it just follows the nearest
	// player around so they huddle together; it'll learn more as the nights need it.
	public static class JelloAI {

		const float FollowDistance = 1.5f;

		public static Vector2 GetMove (Jello self) {
			Jello leader = NearestPlayer (self);
			if (leader == null) {
				return Vector2.zero;
			}

			Vector3 toLeader = leader.transform.position - self.transform.position;
			toLeader.y = 0f;
			if (toLeader.magnitude < FollowDistance) {
				return Vector2.zero;
			}
			return new Vector2 (toLeader.x, toLeader.z).normalized;
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
