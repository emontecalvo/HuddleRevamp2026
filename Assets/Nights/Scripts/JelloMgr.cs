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

		public List<Jello> AllJellos = new List<Jello> ();

		public void Register (Jello jello) {
			AllJellos.Add (jello);
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
