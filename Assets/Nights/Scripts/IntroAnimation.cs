using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace HuddleNights {

	// Placeholder intro: the jellos ride a vehicle toward the beach, hit a snow pile,
	// and get scattered, leaving Bobo (player 1) alone. Built from sprites at runtime.
	// Any button skips straight to Night 1.
	public class IntroAnimation : MonoBehaviour {

		public Sprite Background;
		public Sprite Tree;
		public Sprite Snowflake;
		public Sprite SnowPile;
		[Tooltip ("A plain white sprite, tinted black for the fade-out")]
		public Sprite WhiteSquare;
		[Tooltip ("One is picked at random each time")]
		public Sprite[] Vehicles;
		[Tooltip ("Bobo first: player 1's jello is the one left behind")]
		public Sprite[] JelloBodies;
		public Sprite[] JelloFaces;
		public Sprite BoboLookLeft;
		public Sprite BoboLookRight;
		public Sprite BoboLookDown;
		public Font CaptionFont;
		public string NextScene = "Night1";

		const float GroundY = -1.8f;
		const float JelloScale = 0.45f;
		const float VehicleWidth = 4f;
		const float SkipDelay = 0.5f;
		const int FlakeCount = 40;
		const float FlakeSpeed = 0.8f;

		NightMessageUI Captions;
		SpriteRenderer Fader;
		SpriteRenderer BoboFace;
		List<Transform> Flakes = new List<Transform> ();
		float HalfWidth;
		float HalfHeight;
		bool Leaving = false;

		void Awake () {
			Captions = gameObject.AddComponent<NightMessageUI> ();
			Captions.MessageFont = CaptionFont;
		}

		void Start () {
			Camera cam = Camera.main;
			HalfHeight = cam.orthographicSize;
			HalfWidth = HalfHeight * cam.aspect;

			Captions.SetHintAnchors (new Vector2 (0.05f, 0.8f), new Vector2 (0.95f, 0.95f));

			BuildScenery ();
			Transform ride = BuildRide (out SpriteRenderer vehicle, out List<Transform> riders);
			PlayStory (cam, ride, vehicle, riders);
		}

		void Update () {
			foreach (Transform flake in Flakes) {
				Vector3 pos = flake.position;
				pos.y -= FlakeSpeed * Time.deltaTime;
				pos.x += Mathf.Sin (Time.time + pos.y) * 0.2f * Time.deltaTime;
				if (pos.y < -HalfHeight - 0.5f) {
					pos.y = HalfHeight + 0.5f;
				}
				flake.position = pos;
			}

			if (!Leaving && Time.timeSinceLevelLoad > SkipDelay && PlayerRoster.AnyButtonPressed ()) {
				Leave (0.3f);
			}
		}

		void BuildScenery () {
			SpriteRenderer bg = MakeSprite ("Background", Background, Vector3.zero, 0);
			float bgScale = Mathf.Max (HalfWidth * 2f / Background.bounds.size.x, HalfHeight * 2f / Background.bounds.size.y);
			bg.transform.localScale = Vector3.one * bgScale;

			// A row of trees along the horizon.
			for (float x = -HalfWidth + 0.5f; x < HalfWidth; x += Random.Range (1.2f, 2.2f)) {
				SpriteRenderer tree = MakeSprite ("Tree", Tree, new Vector3 (x, 2.4f + Random.Range (-0.2f, 0.2f), 0), 5);
				tree.transform.localScale = Vector3.one * Random.Range (0.45f, 0.65f);
			}

			for (int i = 0; i < FlakeCount; i++) {
				Vector3 pos = new Vector3 (Random.Range (-HalfWidth, HalfWidth), Random.Range (-HalfHeight, HalfHeight), 0);
				SpriteRenderer flake = MakeSprite ("Snowflake", Snowflake, pos, 50);
				flake.transform.localScale = Vector3.one * Random.Range (0.04f, 0.1f);
				Flakes.Add (flake.transform);
			}

			Fader = MakeSprite ("Fade", WhiteSquare, Vector3.zero, 1000);
			Fader.color = new Color (0, 0, 0, 0);
			Fader.transform.localScale = new Vector3 (
				(HalfWidth * 2f + 1f) / WhiteSquare.bounds.size.x,
				(HalfHeight * 2f + 1f) / WhiteSquare.bounds.size.y, 1f);
		}

		// The vehicle with the four jellos riding on top, starting off the left edge.
		Transform BuildRide (out SpriteRenderer vehicle, out List<Transform> riders) {
			Sprite vehicleSprite = Vehicles [Random.Range (0, Vehicles.Length)];
			float vehicleScale = VehicleWidth / vehicleSprite.bounds.size.x;
			float vehicleHalfHeight = vehicleSprite.bounds.extents.y * vehicleScale;
			bool isPlane = vehicleSprite.name.Contains ("plane");

			GameObject ride = new GameObject ("Ride");
			float rideY = GroundY + vehicleHalfHeight + (isPlane ? 1.5f : 0f);
			ride.transform.position = new Vector3 (-HalfWidth - VehicleWidth, rideY, 0);

			vehicle = MakeSprite ("Vehicle", vehicleSprite, Vector3.zero, 20, ride.transform);
			vehicle.transform.localScale = Vector3.one * vehicleScale;

			riders = new List<Transform> ();
			float jelloHalf = JelloBodies [0].bounds.extents.y * JelloScale;
			for (int i = 0; i < JelloBodies.Length; i++) {
				float x = (i - (JelloBodies.Length - 1) / 2f) * 0.9f;
				Vector3 local = new Vector3 (x, vehicleHalfHeight + jelloHalf - 0.15f, 0);
				SpriteRenderer body = MakeSprite ("Jello" + i, JelloBodies [i], local, 30 + i * 2, ride.transform);
				body.transform.localScale = Vector3.one * JelloScale;
				SpriteRenderer face = MakeSprite ("Face", JelloFaces [i], Vector3.zero, 31 + i * 2, body.transform);
				if (i == 0) {
					BoboFace = face;
				}
				riders.Add (body.transform);
			}

			// The snow pile they're about to hit.
			SpriteRenderer pile = MakeSprite ("SnowPile", SnowPile, Vector3.zero, 25);
			pile.transform.localScale = Vector3.one * 0.5f;
			pile.transform.position = new Vector3 (VehicleWidth / 2f + 0.6f, GroundY + SnowPile.bounds.extents.y * 0.5f - 0.2f, 0);

			return ride.transform;
		}

		void PlayStory (Camera cam, Transform ride, SpriteRenderer vehicle, List<Transform> riders) {
			const float driveTime = 3.5f;
			const float bumpTime = driveTime;
			const float lookTime = 8.5f;
			const float endTime = 12.2f;

			Sequence story = DOTween.Sequence ();

			story.InsertCallback (0.3f, () => Captions.ShowHint ("The jellos were off to the beach!", 3f));
			story.Insert (0f, ride.DOMoveX (0f, driveTime).SetEase (Ease.OutSine));
			for (int i = 0; i < riders.Count; i++) {
				Transform rider = riders [i];
				story.Insert (i * 0.08f, rider.DOLocalMoveY (rider.localPosition.y + 0.15f, 0.22f)
					.SetLoops (14, LoopType.Yoyo).SetEase (Ease.InOutSine));
			}

			story.InsertCallback (bumpTime, () => {
				Captions.ShowHint ("BUMP!", 1.5f);
				cam.transform.DOShakePosition (0.5f, 0.4f);
				vehicle.transform.DOPunchRotation (new Vector3 (0, 0, 12f), 0.5f);
				Scatter (ride, riders);
			});

			story.InsertCallback (bumpTime + 1.8f, () => Captions.ShowHint ("Everyone got scattered across the snowy woods!", 3f));

			story.InsertCallback (lookTime, () => {
				Captions.ShowHint ("Bobo is all alone... and it's getting cold.", 3.5f);
				BoboFace.sprite = BoboLookLeft;
			});
			story.InsertCallback (lookTime + 0.8f, () => BoboFace.sprite = BoboLookRight);
			story.InsertCallback (lookTime + 1.6f, () => BoboFace.sprite = BoboLookLeft);
			story.InsertCallback (lookTime + 2.4f, () => {
				BoboFace.sprite = BoboLookDown;
				riders [0].DOScaleY (JelloScale * 0.85f, 0.4f);
			});

			story.InsertCallback (endTime, () => Leave (0.8f));
		}

		// Bobo lands in the middle; everyone else flies off screen and the vehicle skids away.
		void Scatter (Transform ride, List<Transform> riders) {
			Vector3[] targets = {
				new Vector3 (0f, -2.6f, 0),
				new Vector3 (-HalfWidth - 2f, 2.5f, 0),
				new Vector3 (HalfWidth + 2f, 3f, 0),
				new Vector3 (-HalfWidth * 0.3f, HalfHeight + 2f, 0),
			};

			for (int i = 0; i < riders.Count; i++) {
				Transform rider = riders [i];
				rider.DOKill ();
				rider.SetParent (null, true);
				float flightTime = i == 0 ? 1.4f : 1.1f;
				rider.DOJump (targets [i % targets.Length], i == 0 ? 4f : 3f, 1, flightTime).SetEase (Ease.Linear);
				rider.DORotate (new Vector3 (0, 0, i % 2 == 0 ? 720f : -720f), flightTime, RotateMode.FastBeyond360);
			}

			// Bobo squashes into the snow on landing.
			riders [0].DOPunchScale (new Vector3 (0.15f, -0.15f, 0), 0.4f).SetDelay (1.4f);

			ride.DOMoveX (HalfWidth + VehicleWidth * 1.5f, 1.6f).SetEase (Ease.InQuad).SetDelay (0.2f);
			ride.DORotate (new Vector3 (0, 0, -15f), 0.4f).SetLoops (4, LoopType.Yoyo);
		}

		void Leave (float fadeTime) {
			if (Leaving) {
				return;
			}
			Leaving = true;
			Fader.DOFade (1f, fadeTime).OnComplete (() => {
				DOTween.KillAll ();
				SceneManager.LoadScene (NextScene);
			});
		}

		SpriteRenderer MakeSprite (string name, Sprite sprite, Vector3 localPos, int order, Transform parent = null) {
			GameObject obj = new GameObject (name);
			obj.transform.SetParent (parent, false);
			obj.transform.localPosition = localPos;
			SpriteRenderer renderer = obj.AddComponent<SpriteRenderer> ();
			renderer.sprite = sprite;
			renderer.sortingOrder = order;
			return renderer;
		}
	}
}
