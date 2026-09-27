using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tree : MonoBehaviour {

	bool AmIChopped = false;

	public GameObject TreeView;
	public GameObject WoodView;

	public GameObject TreeBend1;
	public GameObject TreeBend2;
	public GameObject TreeBend3;
	public GameObject PalmTree;
	public float NextPanelSwitchTimeTree;

	public float TurnIntoPalmTreeTime;

	void Start () {
		TurnIntoPalmTreeTime = Random.Range (0f, 1f);

		TreeMgr.inst.Register(this);
		TreeView.SetActive (true);
		WoodView.SetActive (false);

		TreeBend1.SetActive (false);
		TreeBend2.SetActive (false);
		TreeBend3.SetActive (false);
		PalmTree.SetActive(false);

		NextPanelSwitchTimeTree = -1;
	}

	void OnDestroy() {
		if (TreeMgr.inst != null) {
			TreeMgr.inst.Unregister (this);
		}
	}

	float TimeLast = 0;

	void Update () {
		float time = GamePhaseMgr.inst.GetGameTime ();
		// make trees at top appear smaller:
		float heightRatio = (transform.position.z - -10) / (10 - -10);
		float scale = 1.2f - heightRatio * 0.4f;
		transform.localScale = new Vector3 (scale, scale, scale);
		if (GamePhaseMgr.inst.IsGame) {
			SwitchTreePanels ();
		}

		if (GamePhaseMgr.inst.IsExtro && time > TurnIntoPalmTreeTime && TimeLast < TurnIntoPalmTreeTime) {
			PalmTree.SetActive(true);
			PalmTree.transform.localScale = new Vector3 (0f, 2f, 1f);
			Sequence sequence = DOTween.Sequence ();
			sequence.Insert(0, TreeView.transform.DOScale(new Vector3 (0f, 2f, 1f), 0.2f));
			sequence.Insert(0, TreeBend1.transform.DOScale(new Vector3 (0f, 2f, 1f), 0.2f));
			sequence.Insert(0, TreeBend2.transform.DOScale(new Vector3 (0f, 2f, 1f), 0.2f));
			sequence.Insert(0, TreeBend3.transform.DOScale(new Vector3 (0f, 2f, 1f), 0.2f));
			sequence.Insert(0, WoodView.transform.DOScale(new Vector3 (0f, 2f, 1f), 0.2f));
			sequence.Append (PalmTree.transform.DOScale (new Vector3 (1f, 1f, 1f), 0.2f));
		}
		TimeLast = time;
	}

	public void GetChopped() {
		AmIChopped = true;
		WoodView.SetActive (true);
		TreeView.SetActive (false);
		TreeBend1.SetActive (false);
		TreeBend2.SetActive (false);
		TreeBend3.SetActive (false);
	}

	void SwitchTreePanels() {
		if (GamePhaseMgr.inst.IsGame) {
			if (Thermometer.inst.IsItAStorm) {
				if (!AmIChopped) {
					if (NextPanelSwitchTimeTree < 0) {
						NextPanelSwitchTimeTree = Time.time + UnityEngine.Random.Range (0f, 1f);
					}
					if (Time.time > NextPanelSwitchTimeTree) {
						if (TreeView.activeSelf) {
							TreeView.SetActive (false);
							TreeBend1.SetActive (true);
							NextPanelSwitchTimeTree += 0.5f;
						} else if (TreeBend1.activeSelf) {
							TreeBend1.SetActive (false);
							TreeBend2.SetActive (true);
							NextPanelSwitchTimeTree += 0.5f;
						} else if (TreeBend2.activeSelf) {
							TreeBend2.SetActive (false);
							TreeBend3.SetActive (false);
							NextPanelSwitchTimeTree += 0.5f;
						} else if (TreeBend3.activeSelf) {
							TreeBend3.SetActive (false);
							TreeView.SetActive (true);
							NextPanelSwitchTimeTree += 0.5f;
						}
					}
				}		
			} else {
				NextPanelSwitchTimeTree = -1;
			}
		}

		if (!WoodView.activeSelf && !TreeBend1.activeSelf && !TreeBend2.activeSelf && !TreeBend3.activeSelf) {
			TreeView.SetActive (true);
		}
	}

}
