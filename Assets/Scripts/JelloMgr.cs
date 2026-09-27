using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JelloMgr : MonoBehaviour
{
	private static JelloMgr _inst = null;

	public static JelloMgr inst {
		get {
			if (_inst == null) {
				_inst = FindAnyObjectByType<JelloMgr> ();
			}
			return _inst;
		}
	}

	public List <Jello> AllJellos = new List<Jello> ();

	public void Register (Jello jello) {

		AllJellos.Add (jello);
		
	}
}
