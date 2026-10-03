using System.Collections.Generic;
using UnityEngine;

namespace HuddleNights {

	public class TreeMgr : MonoBehaviour {

		private static TreeMgr _inst = null;

		public static TreeMgr inst {
			get {
				if (_inst == null) {
					_inst = FindAnyObjectByType<TreeMgr> ();
				}
				return _inst;
			}
		}

		public List<PineTree> AllTrees = new List<PineTree> ();

		public void Register (PineTree tree) {
			AllTrees.Add (tree);
		}

		public void Unregister (PineTree tree) {
			AllTrees.Remove (tree);
		}
	}
}
