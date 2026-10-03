using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HuddleNights.EditorTools {

	// Huddle > Create Next Night: copies the highest-numbered NightN scene and its settings
	// into Night(N+1), points the new scene at the new settings, and adds it to the build
	// profile's scene list. For Nights 2-4 it also sets up the jello waiting to be found.
	public static class CreateNextNight {

		const string SettingsFolder = "Assets/Nights/Settings/";
		const int LastNight = 10;
		// Where the lost jello is placed, as a fraction of the camera view (0,0 = bottom left).
		static readonly Vector2 LostJelloViewSpot = new Vector2 (0.2f, 0.2f);

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
			// Nights 2-4: each night, one more jello is found.
			if (next <= 4) {
				settings.JellosInPlay = next;
				settings.LostJelloSlot = next - 1;
			} else {
				settings.LostJelloSlot = -1;
			}
			EditorUtility.SetDirty (settings);
			AssetDatabase.SaveAssets ();

			Scene scene = EditorSceneManager.OpenScene (ScenePath (next));
			foreach (GamePhaseMgr phase in FindAll<GamePhaseMgr> (scene)) {
				phase.Settings = settings;
				EditorUtility.SetDirty (phase);
			}
			if (settings.LostJelloSlot >= 0) {
				PlaceLostJello (scene, settings.LostJelloSlot);
			}
			EditorSceneManager.SaveScene (scene);

			BuildScenes.Add (ScenePath (next));
			Selection.activeObject = settings;
			Debug.Log ("Created Night" + next + ". Its settings are selected in the Inspector.");
		}

		// Moves the lost jello toward a corner of the view, away from where everyone starts.
		static void PlaceLostJello (Scene scene, int slot) {
			Camera cam = null;
			foreach (Camera c in FindAll<Camera> (scene)) {
				if (c.CompareTag ("MainCamera")) {
					cam = c;
				}
			}

			foreach (Jello jello in FindAll<Jello> (scene)) {
				if (jello.PlayerSlot != slot || cam == null) {
					continue;
				}
				Plane ground = new Plane (Vector3.up, jello.transform.position);
				Ray ray = cam.ViewportPointToRay (new Vector3 (LostJelloViewSpot.x, LostJelloViewSpot.y, 0));
				if (ground.Raycast (ray, out float distance)) {
					jello.transform.position = ray.GetPoint (distance);
					PrefabUtility.RecordPrefabInstancePropertyModifications (jello.transform);
					EditorUtility.SetDirty (jello.transform);
				}
			}
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
