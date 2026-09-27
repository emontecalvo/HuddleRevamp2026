using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExtroMgr : MonoBehaviour
{

	private static ExtroMgr _inst = null;

	public static ExtroMgr inst {
		get {
			if (_inst == null) {
				_inst = FindAnyObjectByType<ExtroMgr> ();
			}
			return _inst;
		}
	}

	public SpriteRenderer SnowBackground;
	public SpriteRenderer BeachBackground;
	bool IsExtroLast = false;
	public GameObject ExtroPanel;
	public Button PlayAgainBtn;
	public Button QuitBtn;
	float GamePhaseTimeLast = 0;
	float ExtroPanelEnterTime = 3f;


	void Start ()
	{
		ExtroPanel.SetActive (false);
		BeachBackground.color = new Color (1, 1, 1, 0);
		PlayAgainBtn.onClick.AddListener (PlayAgain);
		QuitBtn.onClick.AddListener (ExitProgram);
	}
		
	void Update ()
	{
		float time = GamePhaseMgr.inst.GetGameTime ();
		if (GamePhaseMgr.inst.IsExtro && !IsExtroLast) {
			SnowBackground.DOFade (0f, 1f);
			BeachBackground.DOFade (1f, 1f);
		}

		IsExtroLast = GamePhaseMgr.inst.IsExtro;

		if (GamePhaseMgr.inst.IsExtro && time > ExtroPanelEnterTime && GamePhaseTimeLast < ExtroPanelEnterTime) {
			ExtroPanel.SetActive (true);
		}

		GamePhaseTimeLast = time;
	}

	void PlayAgain() {
		SceneManager.LoadScene (0);
	}

	void ExitProgram() {
		Application.Quit();
	}
}
