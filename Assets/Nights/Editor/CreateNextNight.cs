using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HuddleNights.EditorTools {

	// Huddle > Create Next Night: copies the highest-numbered NightN scene and its settings
	// into Night(N+1), points the new scene at the new settings, and adds it to the build
	// profile's scene list. Applies the game outline (more jellos found, storms from Night 3,
	// big logs on Night 4) and arranges the jellos: found ones next to Bobo, the lost one far away.
	public static class CreateNextNight {

		const string SettingsFolder = "Assets/Nights/Settings/";
		const int LastNight = 10;
		// Candidate spots for the lost jello, as fractions of the camera view (0,0 = bottom left).
		static readonly Vector2[] LostJelloViewSpots = {
			new Vector2 (0.15f, 0.2f), new Vector2 (0.85f, 0.2f),
			new Vector2 (0.15f, 0.5f), new Vector2 (0.85f, 0.5f),
			new Vector2 (0.5f, 0.15f),
		};
		// Candidate spots for Night 4's big logs, as fractions of the camera view.
		static readonly Vector2[] BigLogViewSpots = {
			new Vector2 (0.3f, 0.25f), new Vector2 (0.7f, 0.25f), new Vector2 (0.35f, 0.55f),
			new Vector2 (0.65f, 0.55f), new Vector2 (0.5f, 0.35f), new Vector2 (0.2f, 0.4f),
		};
		const int BigLogCount = 3;
		const float BigLogClearance = 2.5f;
		const string WoodSprite = "Assets/Images/fire-tree-wood/wood.png";
		// Where already-found jellos start, relative to Bobo.
		static readonly Vector3[] FoundJelloOffsets = {
			new Vector3 (1.8f, 0f, 0f), new Vector3 (-1.8f, 0f, 0f), new Vector3 (0f, 0f, -1.8f),
		};

		[MenuItem ("Huddle/Create Next Night")]
		public static void Create () {
			int current = 1;
			while (File.Exists (ScenePath (current + 1))) {
				current++;
			}
			int next = current + 1;

			if (next > LastNight) {
				EditorUtility.DisplayDialog ("All nights exist", "Night" + LastNight + " already exists.", "OK");
				return;
			}
			if (!File.Exists (SettingsPath (current))) {
				EditorUtility.DisplayDialog ("Missing settings", "Couldn't find " + SettingsPath (current) + " to copy.", "OK");
				return;
			}
			if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ()) {
				return;
			}

			AssetDatabase.CopyAsset (ScenePath (current), ScenePath (next));
			AssetDatabase.CopyAsset (SettingsPath (current), SettingsPath (next));

			NightSettings settings = AssetDatabase.LoadAssetAtPath<NightSettings> (SettingsPath (next));
			settings.NightNumber = next;
			settings.Title = "Night " + next;
			settings.TutorialHints = false;
			ApplyOutline (settings, next);
			EditorUtility.SetDirty (settings);
			AssetDatabase.SaveAssets ();

			Scene scene = EditorSceneManager.OpenScene (ScenePath (next));
			foreach (GamePhaseMgr phase in FindAll<GamePhaseMgr> (scene)) {
				phase.Settings = settings;
				EditorUtility.SetDirty (phase);
			}
			ArrangeJellos (scene, settings);
			AddOutlineObjects (scene, settings);
			EditorSceneManager.SaveScene (scene);

			BuildScenes.Add (ScenePath (next));
			Selection.activeObject = settings;
			Debug.Log ("Created Night" + next + ". Its settings are selected in the Inspector.");
		}

		// Starting values from the game outline. Later nights inherit whatever the night
		// before had, so these only need to say what changes.
		static void ApplyOutline (NightSettings settings, int night) {
			// Nights 2-4: each night, one more jello is found.
			if (night <= 4) {
				settings.JellosInPlay = night;
				settings.LostJelloSlot = night - 1;
			} else {
				settings.LostJelloSlot = -1;
			}

			// Night 3: storms arrive, with a warning.
			if (night == 3) {
				settings.NightLength = 150f;
				settings.StormWarningTime = 7f;
				settings.Storms = new System.Collections.Generic.List<StormWindow> {
					new StormWindow { StartTime = 40f, EndTime = 55f },
					new StormWindow { StartTime = 95f, EndTime = 115f },
				};
			}
		}

		// Jellos found on earlier nights start next to Bobo; the lost one is placed
		// in whichever corner of the view is farthest from the fire and everyone else.
		static void ArrangeJellos (Scene scene, NightSettings settings) {
			Jello bobo = null;
			Jello lost = null;
			Jello[] jellos = FindAll<Jello> (scene);
			foreach (Jello jello in jellos) {
				if (jello.PlayerSlot == 0) {
					bobo = jello;
				}
				if (jello.PlayerSlot == settings.LostJelloSlot) {
					lost = jello;
				}
			}
			if (bobo == null) {
				return;
			}

			System.Collections.Generic.List<Vector3> taken = new System.Collections.Generic.List<Vector3> ();
			taken.Add (bobo.transform.position);
			foreach (Jello jello in jellos) {
				if (jello.PlayerSlot > 0 && jello.PlayerSlot < settings.JellosInPlay && jello != lost) {
					Vector3 offset = FoundJelloOffsets [(jello.PlayerSlot - 1) % FoundJelloOffsets.Length];
					MoveJello (jello, bobo.transform.position + offset);
					taken.Add (jello.transform.position);
				}
			}

			foreach (Fire fire in FindAll<Fire> (scene)) {
				taken.Add (fire.transform.position);
			}

			Camera cam = MainCamera (scene);
			if (lost == null || cam == null) {
				return;
			}

			Plane ground = new Plane (Vector3.up, lost.transform.position);
			Vector3 bestSpot = lost.transform.position;
			float bestDistance = -1f;
			foreach (Vector2 viewSpot in LostJelloViewSpots) {
				Ray ray = cam.ViewportPointToRay (new Vector3 (viewSpot.x, viewSpot.y, 0));
				if (!ground.Raycast (ray, out float hit)) {
					continue;
				}
				Vector3 spot = ray.GetPoint (hit);
				float closest = float.MaxValue;
				foreach (Vector3 other in taken) {
					closest = Mathf.Min (closest, Vector3.Distance (spot, other));
				}
				if (closest > bestDistance) {
					bestDistance = closest;
					bestSpot = spot;
				}
			}
			MoveJello (lost, bestSpot);
		}

		// Night 4: big logs that take two jellos to move, away from the fire and the jellos.
		static void AddOutlineObjects (Scene scene, NightSettings settings) {
			if (settings.NightNumber != 4) {
				return;
			}
			Camera cam = MainCamera (scene);
			Sprite wood = AssetDatabase.LoadAssetAtPath<Sprite> (WoodSprite);
			if (cam == null || wood == null) {
				Debug.LogWarning ("Night 4: couldn't place big logs (no main camera or wood sprite).");
				return;
			}

			System.Collections.Generic.List<Vector3> taken = new System.Collections.Generic.List<Vector3> ();
			foreach (Jello jello in FindAll<Jello> (scene)) {
				if (jello.PlayerSlot < settings.JellosInPlay) {
					taken.Add (jello.transform.position);
				}
			}
			foreach (Fire fire in FindAll<Fire> (scene)) {
				taken.Add (fire.transform.position);
			}

			GameObject group = new GameObject ("BigLogs");
			Plane ground = new Plane (Vector3.up, Vector3.zero);
			int placed = 0;
			foreach (Vector2 viewSpot in BigLogViewSpots) {
				Ray ray = cam.ViewportPointToRay (new Vector3 (viewSpot.x, viewSpot.y, 0));
				if (placed >= BigLogCount || !ground.Raycast (ray, out float hit)) {
					continue;
				}
				Vector3 spot = ray.GetPoint (hit);
				bool tooClose = false;
				foreach (Vector3 other in taken) {
					tooClose |= BigLog.FlatDistance (spot, other) < BigLogClearance;
				}
				if (tooClose) {
					continue;
				}
				MakeBigLog (group.transform, spot, wood);
				taken.Add (spot);
				placed++;
			}
		}

		// A placeholder big log: the wood pile sprite, doubled in size and tinted brown,
		// lying flat like the rest of the wood.
		static void MakeBigLog (Transform parent, Vector3 position, Sprite wood) {
			GameObject log = new GameObject ("BigLog");
			log.transform.SetParent (parent, false);
			log.transform.position = position;
			log.AddComponent<BigLog> ();

			GameObject view = new GameObject ("LogView");
			view.transform.SetParent (log.transform, false);
			view.transform.localRotation = Quaternion.Euler (90f, 0f, 0f);
			view.transform.localScale = Vector3.one * 2f;
			SpriteRenderer renderer = view.AddComponent<SpriteRenderer> ();
			renderer.sprite = wood;
			renderer.color = new Color (0.85f, 0.6f, 0.4f);
			view.AddComponent<global::AutoLayerSort> ();
		}

		static Camera MainCamera (Scene scene) {
			foreach (Camera cam in FindAll<Camera> (scene)) {
				if (cam.CompareTag ("MainCamera")) {
					return cam;
				}
			}
			return null;
		}

		static void MoveJello (Jello jello, Vector3 position) {
			jello.transform.position = position;
			PrefabUtility.RecordPrefabInstancePropertyModifications (jello.transform);
			EditorUtility.SetDirty (jello.transform);
		}

		static T[] FindAll<T> (Scene scene) where T : Component {
			System.Collections.Generic.List<T> found = new System.Collections.Generic.List<T> ();
			foreach (GameObject root in scene.GetRootGameObjects ()) {
				found.AddRange (root.GetComponentsInChildren<T> (true));
			}
			return found.ToArray ();
		}

		static string ScenePath (int night) {
			return "Assets/Night" + night + ".unity";
		}

		static string SettingsPath (int night) {
			return SettingsFolder + "Night" + night + "Settings.asset";
		}
	}
}
