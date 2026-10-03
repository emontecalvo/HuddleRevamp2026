using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
	public List <GameObject> FireFrames0 = new List<GameObject> ();
	public List <GameObject> FireFrames = new List<GameObject> ();
	public List <GameObject> FireFrames2 = new List<GameObject> ();
	public int CurrentFrame;
	public float TimeUntilNextFrame;
	public float TimeBetweenFrames;
	public int CurrentLevel;
	public ParticleSystem WoodAddToFireParticle;

	public float NumberOfWood;

	private static Fire _inst = null;

	public static Fire inst {
		get {
			if (_inst == null) {
				_inst = FindAnyObjectByType<Fire> ();
			}
			return _inst;
		}
	}
		
	void Start ()
	{
		NumberOfWood = 2.9f;
	}

	void Update ()
	{
		NumberOfWood -= 0.05f * Time.deltaTime;

		List <GameObject> ActiveFrames;

		if (CurrentLevel == 1) {
			for (int j = 0; j < FireFrames2.Count; j++) {
				FireFrames2 [j].SetActive (false);
			}
			FireFrames0 [0].SetActive (false);
		} else if (CurrentLevel == 2) {
			for (int k = 0; k < FireFrames.Count; k++) {
				FireFrames [k].SetActive (false);
			}
			FireFrames0 [0].SetActive (false);
		} else {
			FireFrames0 [0].SetActive (true);
			for (int m = 0; m < FireFrames.Count; m++) {
				FireFrames [m].SetActive (false);
				FireFrames2 [m].SetActive (false);
			}
		}

		if (NumberOfWood <= 0) {
			NumberOfWood = 0;
			CurrentLevel = 0;
		} else if (NumberOfWood < 3) {
			CurrentLevel = 1;
		} else {
			CurrentLevel = 2;
		}


		if (CurrentLevel == 1) {
			ActiveFrames = FireFrames;
		} else if (CurrentLevel == 0) {
			ActiveFrames = FireFrames0;
		}else {
			ActiveFrames = FireFrames2;
		}

		TimeUntilNextFrame -= Time.deltaTime;

		if (TimeUntilNextFrame <= 0) {
			TimeUntilNextFrame = TimeBetweenFrames;
			CurrentFrame = CurrentFrame + 1;
			if (CurrentFrame >= ActiveFrames.Count) {
				CurrentFrame = 0;
			}

			for (int i = 0; i < ActiveFrames.Count; i++) {
				if (i == CurrentFrame) {
					ActiveFrames [i].SetActive (true);
				} else {
					ActiveFrames [i].SetActive (false);
				}
			}
		}
	}

	public void ReceiveWood(PineTree tree) {
		Destroy (tree.gameObject);

		WoodAddToFireParticle.Clear ();
		WoodAddToFireParticle.Stop();
		WoodAddToFireParticle.Play();
		NumberOfWood += 1;
	}
}
