using System.Collections.Generic;
using UnityEngine;

namespace HuddleNights {

	public class JelloMgr : MonoBehaviour {

		private static JelloMgr _inst = null;

		public static JelloMgr inst {
			get {
				if (_inst == null) {
					_inst = FindAnyObjectByType<JelloMgr> ();
				}
				return _inst;
			}
		}

		static readonly string[] Names = { "Bobo", "Lulu", "Bartholomew", "Theodore" };

		public List<Jello> AllJellos = new List<Jello> ();

		public static string NameOf (int playerSlot) {
			if (playerSlot < 0 || playerSlot >= Names.Length) {
				return "Jello";
			}
			return Names [playerSlot];
		}

		public void Register (Jello jello) {
			AllJellos.Add (jello);
		}

		void Update () {
			if (GamePhaseMgr.inst.IsGame) {
				TryJoinPlayer ();
			}
		}

		// Pressing A on a free gamepad (or Enter, for the arrow keys) takes over the first
		// found jello the AI is playing.
		void TryJoinPlayer () {
			Jello aiJello = null;
			foreach (Jello jello in AllJellos) {
				if (!jello.IsLost && !PlayerRoster.IsHuman (jello.PlayerSlot) &&
					(aiJello == null || jello.PlayerSlot < aiJello.PlayerSlot)) {
					aiJello = jello;
				}
			}

			if (aiJello != null && PlayerRoster.TryJoin (aiJello.PlayerSlot)) {
				string joined = "Player " + (aiJello.PlayerSlot + 1) + " is now " + aiJello.JelloName + "!";
				if (PlayerRoster.UsesArrowKeys (aiJello.PlayerSlot)) {
					joined = "Player " + (aiJello.PlayerSlot + 1) + " is now " + aiJello.JelloName + " on the arrow keys. Bobo now uses WASD!";
				}
				NightMessageUI.inst.ShowHint (joined, 4f);
			}
		}

		// A frozen jello thaws when it's near a burning fire or another jello,
		// so if every jello is frozen at once, nobody can warm back up.
		public bool AreAllFrozen () {
			if (AllJellos.Count == 0) {
				return false;
			}
			foreach (Jello jello in AllJellos) {
				if (!jello.AmIFrozen) {
					return false;
				}
			}
			return true;
		}
	}
}
