using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePhaseMgr : MonoBehaviour {

	private static GamePhaseMgr _inst = null;

	public static GamePhaseMgr inst {
		get {
			if (_inst == null) {
				_inst = FindAnyObjectByType<GamePhaseMgr> ();
			}
			return _inst;
		}
	}

	public bool IsIntro = true;
	public bool IsGame = false;
	public bool IsExtro = false;
	public float GameStartTime;

	public float GameLength = 180f;


	void Start () {
		
	}

	void Update () {
		if (IsGame && GetGameTime () > GameLength) {
			BeginExtro ();
		}
	}

	public float GetGameTime() {
		return Time.time - GameStartTime;
	}

	public void BeginGame() {
		IsIntro = false;
		IsGame = true;
		IsExtro = false;
		GameStartTime = Time.time;
	}

	void BeginExtro() {
		IsIntro = false;
		IsGame = false;
		IsExtro = true;
		GameStartTime = Time.time;
	}
}
