using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FacePanelUI : MonoBehaviour {

	public Image FaceImage;
	public Text HappyorLonelyTxt;
	public Text FrozenOrToastyTxt;
	public GameObject Snowflake1;
	public GameObject Snowflake2;
	public GameObject Snowflake3;

	public GameObject StatPanelOne;
	public GameObject StatPanelTwo;
	public float NextPanelSwitchTime = 5.0f;
	public float period = 0.1f;
	bool IsFlipped = false;


	void Start () {
		StatPanelOne.SetActive (true);
		StatPanelTwo.SetActive (true);
		StatPanelTwo.transform.localScale = new Vector3 (0, 1, 1);

		Snowflake1.SetActive(false);
		Snowflake2.SetActive(false);
		Snowflake3.SetActive(false);
	}

	void Update () {
		SwitchUIJelloPanel ();
	}

	void SwitchUIJelloPanel() {
		if (Time.time > NextPanelSwitchTime) {
			Sequence sequence = DOTween.Sequence ();
			RectTransform RT1 = (RectTransform) StatPanelOne.transform;
			RectTransform RT2 = (RectTransform) StatPanelTwo.transform;
			if (!IsFlipped) {
				sequence.Append(RT1.DOScaleX(0, 0.1f));
				sequence.Append(RT2.DOScaleX(1, 0.1f));
				NextPanelSwitchTime += 5.0f;
				IsFlipped = true;
			} else {
				sequence.Append(RT2.DOScaleX(0, 0.1f));
				sequence.Append(RT1.DOScaleX(1, 0.1f));
				NextPanelSwitchTime += 5.0f * 3.0f;
				IsFlipped = false;
			}
		}
	}

	public void SetFace(Sprite faceSprite) {
		FaceImage.sprite = faceSprite;
	}

	public void SetJelloLonelyText (string happyOrLonely) {
		HappyorLonelyTxt.text = happyOrLonely;
	}

	public void SetJelloFrozenText (string frozenOrNot) {
		FrozenOrToastyTxt.text = frozenOrNot;
		if (frozenOrNot == "Frozen! Plz help!") {
			Snowflake1.SetActive(true);
			Snowflake2.SetActive(true);
			Snowflake3.SetActive(true);
		}
	}
}
