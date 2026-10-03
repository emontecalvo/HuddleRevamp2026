using UnityEngine;
using UnityEngine.UI;

namespace HuddleNights {

	// On-screen messages for a night: the title card, tutorial hints, and win/lose banners.
	// Builds its own canvas at runtime, so it needs no setup in the scene.
	public class NightMessageUI : MonoBehaviour {

		private static NightMessageUI _inst = null;

		public static NightMessageUI inst {
			get {
				if (_inst == null) {
					_inst = FindAnyObjectByType<NightMessageUI> ();
				}
				return _inst;
			}
		}

		public Font MessageFont;

		CanvasGroup BannerGroup;
		Text BannerTitle;
		Text BannerSubtitle;

		CanvasGroup HintGroup;
		Text HintText;
		float HintHideTime = -1f;

		const float FadeSpeed = 4f;
		bool BannerShowing = false;

		void Update () {
			EnsureBuilt ();
			if (HintHideTime > 0 && Time.time > HintHideTime) {
				HintHideTime = -1f;
			}
			Fade (BannerGroup, BannerShowing);
			Fade (HintGroup, HintHideTime > 0);
		}

		public void ShowBanner (string title, string subtitle = "") {
			EnsureBuilt ();
			BannerTitle.text = title;
			BannerSubtitle.text = subtitle;
			BannerShowing = true;
		}

		public void HideBanner () {
			BannerShowing = false;
		}

		// Shows a hint along the bottom of the screen for a few seconds.
		public void ShowHint (string hint, float seconds = 5f) {
			EnsureBuilt ();
			HintText.text = hint;
			HintHideTime = Time.time + seconds;
		}

		// Moves the hint area, as fractions of the screen (0,0 = bottom left).
		public void SetHintAnchors (Vector2 anchorMin, Vector2 anchorMax) {
			EnsureBuilt ();
			RectTransform rt = (RectTransform) HintGroup.transform;
			rt.anchorMin = anchorMin;
			rt.anchorMax = anchorMax;
		}

		void Fade (CanvasGroup group, bool show) {
			float target = show ? 1f : 0f;
			group.alpha = Mathf.MoveTowards (group.alpha, target, FadeSpeed * Time.deltaTime);
		}

		// Built on first use, so whoever adds this component can set MessageFont first.
		void EnsureBuilt () {
			if (BannerGroup != null) {
				return;
			}
			if (MessageFont == null) {
				MessageFont = Resources.GetBuiltinResource<Font> ("LegacyRuntime.ttf");
			}
			BuildCanvas ();
		}

		void BuildCanvas () {
			GameObject canvasObj = new GameObject ("NightMessageCanvas", typeof (RectTransform));
			canvasObj.transform.SetParent (transform, false);
			Canvas canvas = canvasObj.AddComponent<Canvas> ();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 100;
			CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler> ();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2 (800, 600);
			scaler.matchWidthOrHeight = 1f;

			// Banner: dimmed screen with a big title and a subtitle.
			GameObject banner = MakeRect ("Banner", canvasObj.transform, Vector2.zero, Vector2.one);
			BannerGroup = banner.AddComponent<CanvasGroup> ();
			BannerGroup.alpha = 0f;
			BannerGroup.blocksRaycasts = false;
			Image dim = banner.AddComponent<Image> ();
			dim.color = new Color (0.05f, 0.08f, 0.2f, 0.55f);
			BannerTitle = MakeText ("Title", banner.transform, new Vector2 (0, 0.5f), new Vector2 (1, 0.75f), 64);
			BannerSubtitle = MakeText ("Subtitle", banner.transform, new Vector2 (0, 0.35f), new Vector2 (1, 0.5f), 30);

			// Hint: a line of text near the bottom of the screen.
			GameObject hint = MakeRect ("Hint", canvasObj.transform, new Vector2 (0.1f, 0.22f), new Vector2 (0.9f, 0.34f));
			HintGroup = hint.AddComponent<CanvasGroup> ();
			HintGroup.alpha = 0f;
			HintGroup.blocksRaycasts = false;
			HintText = MakeText ("HintText", hint.transform, Vector2.zero, Vector2.one, 30);
		}

		GameObject MakeRect (string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax) {
			GameObject obj = new GameObject (name, typeof (RectTransform));
			obj.transform.SetParent (parent, false);
			RectTransform rt = (RectTransform) obj.transform;
			rt.anchorMin = anchorMin;
			rt.anchorMax = anchorMax;
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
			return obj;
		}

		Text MakeText (string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, int size) {
			GameObject obj = MakeRect (name, parent, anchorMin, anchorMax);
			Text text = obj.AddComponent<Text> ();
			text.font = MessageFont;
			text.fontSize = size;
			text.alignment = TextAnchor.MiddleCenter;
			text.color = Color.white;
			text.horizontalOverflow = HorizontalWrapMode.Wrap;
			text.verticalOverflow = VerticalWrapMode.Overflow;
			Shadow shadow = obj.AddComponent<Shadow> ();
			shadow.effectColor = new Color (0, 0, 0, 0.7f);
			shadow.effectDistance = new Vector2 (2, -2);
			return text;
		}
	}
}
