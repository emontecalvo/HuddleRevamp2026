using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HuddleNights.EditorTools {

	// Huddle > Create Next Night: copies the highest-numbered NightN scene and its settings
	// into Night(N+1), points the new scene at the new settings, and adds it to the build
	// profile's scene list. Applies the game outline (more jellos found, storms from Night 3)
	// and arranges the jellos: found ones next to Bobo, the lost one far away.
	public static class CreateNextNight {

		const string SettingsFolder = "Assets/Nights/Settings/";
		const int LastNight = 10;
		// Candidate spots for the lost jello, as fractions of the camera view (0,0 = bottom left).
		static readonly Vector2[] LostJelloViewSpots = {
			new Vector2 (0.15f, 0.2f), new Vector2 (0.85f, 0.2f),
			new Vector2 (0.15f, 0.5f), new Vector2 (0.85f, 0.5f),
			new Vector2 (0.5f, 0.15f),
		};
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

			Camera cam = null;
			foreach (Camera c in FindAll<Camera> (scene)) {
				if (c.CompareTag ("MainCamera")) {
					cam = c;
				}
			}
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
