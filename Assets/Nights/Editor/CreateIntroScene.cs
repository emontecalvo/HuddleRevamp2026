using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HuddleNights.EditorTools {

	// Huddle > Create Intro Scene: builds Assets/Intro.unity with a camera and the
	// placeholder IntroAnimation, and adds it to the build profile's scene list.
	public static class CreateIntroScene {

		const string ScenePath = "Assets/Intro.unity";
		const string Images = "Assets/Images/";

		[MenuItem ("Huddle/Create Intro Scene")]
		public static void Create () {
			if (File.Exists (ScenePath) &&
				!EditorUtility.DisplayDialog ("Intro scene exists", ScenePath + " already exists. Replace it?", "Replace", "Cancel")) {
				return;
			}
			if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ()) {
				return;
			}

			Scene scene = EditorSceneManager.NewScene (NewSceneSetup.EmptyScene, NewSceneMode.Single);

			GameObject camObj = new GameObject ("Main Camera");
			camObj.tag = "MainCamera";
			camObj.transform.position = new Vector3 (0, 0, -10);
			Camera cam = camObj.AddComponent<Camera> ();
			cam.orthographic = true;
			cam.orthographicSize = 5;
			cam.clearFlags = CameraClearFlags.SolidColor;
			cam.backgroundColor = new Color (0.29f, 0.29f, 0.29f);
			camObj.AddComponent<AudioListener> ();

			IntroAnimation intro = new GameObject ("IntroAnimation").AddComponent<IntroAnimation> ();
			intro.Background = LoadSprite ("background/background.png");
			intro.Tree = LoadSprite ("fire-tree-wood/pinetree.png");
			intro.Snowflake = LoadSprite ("snowflakes/snowflake-white.png");
			intro.SnowPile = LoadSprite ("snowflakes/snowpile.png");
			intro.WhiteSquare = LoadSprite ("jello-color-ui/white-background.png");
			intro.Vehicles = new Sprite[] {
				LoadSprite ("transportation/car.png"),
				LoadSprite ("transportation/plane.png"),
				LoadSprite ("transportation/boat.png"),
				LoadSprite ("transportation/train.png"),
			};
			intro.JelloBodies = new Sprite[] {
				LoadSprite ("jello-color-ui/bobo-color.png"),
				LoadSprite ("jello-color-ui/lulu-color.png"),
				LoadSprite ("jello-color-ui/bartholomew-color.png"),
				LoadSprite ("jello-color-ui/theodore-color.png"),
			};
			intro.JelloFaces = new Sprite[] {
				LoadSprite ("jello-faces/booboo-right.png"),
				LoadSprite ("jello-faces/lulu-right.png"),
				LoadSprite ("jello-faces/bartholomew-right.png"),
				LoadSprite ("jello-faces/theodore-right.png"),
			};
			intro.BoboLookLeft = LoadSprite ("jello-faces/booboo-left.png");
			intro.BoboLookRight = LoadSprite ("jello-faces/booboo-right.png");
			intro.BoboLookDown = LoadSprite ("jello-faces/booboo-down.png");
			intro.CaptionFont = AssetDatabase.LoadAssetAtPath<Font> ("Assets/Letters for Learners.ttf");

			EditorSceneManager.SaveScene (scene, ScenePath);
			BuildScenes.Add (ScenePath);
			Debug.Log ("Created " + ScenePath + ". Press Play to watch the intro.");
		}

		static Sprite LoadSprite (string path) {
			Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite> (Images + path);
			if (sprite == null) {
				Debug.LogError ("Intro: couldn't find sprite " + Images + path);
			}
			return sprite;
		}
	}
}
