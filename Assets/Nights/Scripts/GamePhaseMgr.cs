using UnityEngine;
using UnityEngine.SceneManagement;

namespace HuddleNights {

	public enum NightPhase {
		TitleCard,	// "Night 1" shows, nobody moves yet
		Game,		// surviving the night
		Dawn,		// survived; moving on to the next night
		Lost,		// every jello froze; waiting for a button to retry
		Extro,		// final night only: spring comes, off to the beach
	}

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

		const string HighestNightKey = "Huddle.HighestNightReached";

		public NightSettings Settings;
		public Font MessageFont;

		public NightPhase Phase = NightPhase.TitleCard;
		public float GameStartTime;

		[Tooltip ("Seconds the dawn message shows before the next night loads")]
		public float DawnMessageTime = 4f;
		[Tooltip ("Seconds before a button press can restart a lost night")]
		public float LostRetryDelay = 1.5f;

		public bool IsIntro { get { return Phase == NightPhase.TitleCard; } }
		public bool IsGame { get { return Phase == NightPhase.Game; } }
		public bool IsExtro { get { return Phase == NightPhase.Extro; } }

		NightMessageUI Messages;

		void Awake () {
			if (Settings == null) {
				Debug.LogWarning ("GamePhaseMgr has no NightSettings assigned; using the defaults.");
				Settings = ScriptableObject.CreateInstance<NightSettings> ();
			}
			Messages = gameObject.AddComponent<NightMessageUI> ();
			Messages.MessageFont = MessageFont;
		}

		void Start () {
			SetPhase (NightPhase.TitleCard);
			Messages.ShowBanner (Settings.Title);
		}

		void Update () {
			float time = GetGameTime ();

			switch (Phase) {
			case NightPhase.TitleCard:
				if (time > Settings.TitleCardTime) {
					Messages.HideBanner ();
					SetPhase (NightPhase.Game);
				}
				break;

			case NightPhase.Game:
				if (JelloMgr.inst.AreAllFrozen ()) {
					SetPhase (NightPhase.Lost);
					Messages.ShowBanner ("Everyone froze!", "Press any button to try again");
				} else if (time > Settings.NightLength) {
					BeginMorning ();
				}
				break;

			case NightPhase.Dawn:
				if (time > DawnMessageTime) {
					LoadNextNight ();
				}
				break;

			case NightPhase.Lost:
				if (time > LostRetryDelay && PlayerRoster.AnyButtonPressed ()) {
					SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
				}
				break;
			}
		}

		public float GetGameTime () {
			return Time.time - GameStartTime;
		}

		// Seconds left until dawn, for anything that wants to show a countdown.
		public float GetTimeLeft () {
			if (!IsGame) {
				return 0f;
			}
			return Mathf.Max (0f, Settings.NightLength - GetGameTime ());
		}

		void SetPhase (NightPhase phase) {
			Phase = phase;
			GameStartTime = Time.time;
		}

		void BeginMorning () {
			int highest = PlayerPrefs.GetInt (HighestNightKey, 1);
			if (Settings.NightNumber + 1 > highest) {
				PlayerPrefs.SetInt (HighestNightKey, Settings.NightNumber + 1);
				PlayerPrefs.Save ();
			}

			if (Settings.IsFinalNight) {
				SetPhase (NightPhase.Extro);
			} else {
				SetPhase (NightPhase.Dawn);
				Messages.ShowBanner ("Morning!", "You survived " + Settings.Title);
			}
		}

		void LoadNextNight () {
			string nextScene = "Night" + (Settings.NightNumber + 1);
			if (Application.CanStreamedLevelBeLoaded (nextScene)) {
				SceneManager.LoadScene (nextScene);
			} else {
				// Not built yet (or not added to the build profile's scene list).
				Messages.ShowBanner ("Morning!", nextScene + " is coming soon...");
				enabled = false;
			}
		}

		public static int HighestNightReached () {
			return PlayerPrefs.GetInt (HighestNightKey, 1);
		}
	}
}
