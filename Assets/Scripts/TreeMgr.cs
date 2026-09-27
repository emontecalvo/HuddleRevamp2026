using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public List <Tree> AllTrees = new List<Tree> ();

	public void Register (Tree tree) {
		AllTrees.Add (tree);
	}

	public void Unregister (Tree tree) {
		AllTrees.Remove (tree);
	}
}
