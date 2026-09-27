using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionPanelMgr : MonoBehaviour {

	private static InstructionPanelMgr _inst = null;

	public static InstructionPanelMgr inst {
		get {
			if (_inst == null) {
				_inst = FindAnyObjectByType<InstructionPanelMgr> ();
			}
			return _inst;
		}
	}

	public GameObject InstructPanel0;
	public GameObject InstructPanel1;
	public GameObject InstructPanel2;
	public GameObject InstructPanel3;

	public Button InstructButton0;
	public Button InstructButton1;
	public Button InstructButton2;
	public Button InstructButton3;

	int CurrentPanel = 0;

	void Start ()
	{
		InstructPanel0.SetActive (true);
		InstructPanel1.SetActive (false);
		InstructPanel2.SetActive (false);
		InstructPanel3.SetActive (false);
		InstructButton0.onClick.AddListener (IButton0Clicked);
		InstructButton1.onClick.AddListener (IButton1Clicked);
		InstructButton2.onClick.AddListener (IButton2Clicked);
		InstructButton3.onClick.AddListener (IButton3Clicked);
	}

	void Update () {
		
	}

	void SetCurrentPanel(int newCurrentPanel) {
		CurrentPanel = newCurrentPanel;
		InstructPanel0.SetActive (CurrentPanel == 0);
		InstructPanel1.SetActive (CurrentPanel == 1);
		InstructPanel2.SetActive (CurrentPanel == 2);
		InstructPanel3.SetActive (CurrentPanel == 3);
	}

	void IButton0Clicked() {
		SetCurrentPanel (1);
	}

	void IButton1Clicked() {
		SetCurrentPanel (2);
	}

	void IButton2Clicked() {
		SetCurrentPanel (3);
	}

	void IButton3Clicked() {
		InstructPanel0.SetActive (true);
		InstructPanel1.SetActive (true);
		InstructPanel2.SetActive (true);
		InstructPanel3.SetActive (true);

		InstructButton0.gameObject.SetActive (false);
		InstructButton1.gameObject.SetActive (false);
		InstructButton2.gameObject.SetActive (false);
		InstructButton3.gameObject.SetActive (false);
	}
}



