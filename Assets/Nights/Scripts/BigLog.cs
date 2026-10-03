using System.Collections.Generic;
using UnityEngine;

namespace HuddleNights {

	// A log too heavy for one jello. Jellos that walk up to it grab on; once at least
	// RequiredCarriers are holding it, it trails behind them like chopped wood.
	// Dragged to the fire, it's worth several pieces of wood.
	public class BigLog : MonoBehaviour {

		public static readonly List<BigLog> All = new List<BigLog> ();

		public int RequiredCarriers = 2;
		public float WoodValue = 3f;

		const float GrabRadius = 1.6f;		// a jello this close grabs on...
		const float HoldRadius = 2.6f;		// ...and lets go once farther than this
		const float FollowDistance = 1.4f;	// how far the log trails behind its carriers
		const float FireDistance = 1.75f;

		public readonly List<Jello> Carriers = new List<Jello> ();

		public bool IsMoving { get { return Carriers.Count >= RequiredCarriers; } }

		void OnEnable () {
			All.Add (this);
		}

		void OnDisable () {
			All.Remove (this);
			foreach (Jello carrier in Carriers) {
				if (carrier != null) {
					carrier.CarryingLog = null;
				}
			}
			Carriers.Clear ();
		}

		void Update () {
			if (!GamePhaseMgr.inst.IsGame) {
				return;
			}

			UpdateCarriers ();

			if (IsMoving) {
				Drag ();
				if (FlatDistance (transform.position, Fire.inst.transform.position) <= FireDistance) {
					Fire.inst.ReceiveBigLog (this);
				}
			}
		}

		void UpdateCarriers () {
			for (int i = Carriers.Count - 1; i >= 0; i--) {
				Jello carrier = Carriers [i];
				if (carrier == null || carrier.AmIFrozen || FlatDistance (carrier.transform.position, transform.position) > HoldRadius) {
					if (carrier != null) {
						carrier.CarryingLog = null;
					}
					Carriers.RemoveAt (i);
				}
			}

			foreach (Jello jello in JelloMgr.inst.AllJellos) {
				if (jello.CarryingLog == null && jello.BeingHauled == null && !jello.AmIFrozen &&
					FlatDistance (jello.transform.position, transform.position) <= GrabRadius) {
					jello.CarryingLog = this;
					Carriers.Add (jello);
				}
			}
		}

		void Drag () {
			Vector3 center = Vector3.zero;
			foreach (Jello carrier in Carriers) {
				center += carrier.transform.position;
			}
			center /= Carriers.Count;
			center.y = transform.position.y;

			Vector3 toLog = transform.position - center;
			float distance = toLog.magnitude;
			if (distance > FollowDistance) {
				transform.position = center + toLog / distance * FollowDistance;
			}
		}

		public bool HasHumanCarrier () {
			foreach (Jello carrier in Carriers) {
				if (PlayerRoster.IsHuman (carrier.PlayerSlot)) {
					return true;
				}
			}
			return false;
		}

		public static float FlatDistance (Vector3 a, Vector3 b) {
			a.y = 0f;
			b.y = 0f;
			return Vector3.Distance (a, b);
		}
	}
}
