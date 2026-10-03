using System.Collections.Generic;
using UnityEditor;

namespace HuddleNights.EditorTools {

	public static class BuildScenes {

		// Adds a scene to the end of the build profile's scene list, if it isn't there already.
		public static void Add (string path) {
			List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene> (EditorBuildSettings.scenes);
			foreach (EditorBuildSettingsScene existing in scenes) {
				if (existing.path == path) {
					return;
				}
			}
			scenes.Add (new EditorBuildSettingsScene (path, true));
			EditorBuildSettings.scenes = scenes.ToArray ();
		}
	}
}
