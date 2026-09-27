using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoLayerSort : MonoBehaviour {

	SpriteRenderer mySpriteRenderer;
	public int layerOffset = 0;

	void Start () {
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update () {
		Vector3 pos = transform.position;
		float heightRatio = (transform.position.z - -10) / (10 - -10);
		int sortingOrder = 1000 - (int)(heightRatio * 1000);
		sortingOrder += layerOffset;
		mySpriteRenderer.sortingOrder = sortingOrder;
	}
}
