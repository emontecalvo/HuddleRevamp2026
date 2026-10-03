using System;
using System.Collections.Generic;
using UnityEngine;

namespace HuddleNights {

	[Serializable]
	public class StormWindow {
		public float StartTime;
		public float EndTime;
	}

	// One of these per night. Create with Assets > Create > Huddle > Night Settings.
	// The defaults match the original Huddle demo.
	[CreateAssetMenu (menuName = "Huddle/Night Settings", fileName = "NightSettings")]
	public class NightSettings : ScriptableObject {

		[Header ("Night")]
		public int NightNumber = 1;
		public string Title = "Night 1";
		[Tooltip ("Seconds the players must survive until dawn")]
		public float NightLength = 180f;
		[Tooltip ("Seconds the night's title shows before play begins")]
		public float TitleCardTime = 2.5f;
		[Tooltip ("The final night ends with spring and the beach instead of moving to the next night")]
		public bool IsFinalNight = false;
		[Tooltip ("Show Night 1's tutorial hints (move, chop, feed the fire, stay warm)")]
		public bool TutorialHints = false;

		[Header ("Jellos")]
		[Tooltip ("How many jellos are in this night (1-4). Jellos with a higher player slot are hidden.")]
		[Range (1, 4)]
		public int JellosInPlay = 4;
		[Tooltip ("Player slot of the jello that starts lost and frozen, waiting to be found. -1 = nobody is lost.")]
		[Range (-1, 3)]
		public int LostJelloSlot = -1;
		public float StartingTemp = 10f;
		[Tooltip ("A frozen jello thaws once it warms up to this temperature")]
		public float ThawTemp = 3f;
		[Tooltip ("Degrees per second a frozen jello warms while someone huddles next to it (or it's by the fire)")]
		public float RescueWarmRate = 1f;
		[Tooltip ("Degrees per second lost when alone and away from the fire")]
		public float AloneCoolingRate = 0.2f;
		[Tooltip ("Degrees per second lost when alone and away from the fire during a storm")]
		public float StormCoolingRate = 0.5f;

		[Header ("Storms")]
		public List<StormWindow> Storms = new List<StormWindow> {
			new StormWindow { StartTime = 30f, EndTime = 50f },
			new StormWindow { StartTime = 120f, EndTime = 150f },
		};
		[Tooltip ("Seconds of 'storm approaching!' warning before each storm. 0 = no warning.")]
		public float StormWarningTime = 0f;

		[Header ("Fire")]
		public float StartingWood = 2.9f;
		[Tooltip ("Wood burned per second")]
		public float WoodBurnRate = 0.05f;
	}
}
