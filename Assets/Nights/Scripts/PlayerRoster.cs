using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace HuddleNights {

	// Remembers which input device controls which player slot, across scenes.
	// Player 1 (slot 0) uses the keyboard (arrows or WASD) plus any gamepad nobody else has claimed.
	// Players 2-4 claim a gamepad by pressing a button on it when their jello is found.
	public static class PlayerRoster {

		public const int MaxPlayers = 4;

		static readonly InputDevice[] devices = new InputDevice[MaxPlayers];

		[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.SubsystemRegistration)]
		static void ResetRoster () {
			for (int i = 0; i < MaxPlayers; i++) {
				devices [i] = null;
			}
		}

		public static bool IsHuman (int slot) {
			return slot == 0 || GetDevice (slot) != null;
		}

		public static Vector2 GetMove (int slot) {
			if (slot < 0 || slot >= MaxPlayers) {
				return Vector2.zero;
			}

			Vector2 move = Vector2.zero;

			if (slot == 0) {
				Keyboard keyboard = Keyboard.current;
				if (keyboard != null) {
					move += ReadKeys (keyboard.upArrowKey, keyboard.downArrowKey, keyboard.leftArrowKey, keyboard.rightArrowKey);
					move += ReadKeys (keyboard.wKey, keyboard.sKey, keyboard.aKey, keyboard.dKey);
				}
				foreach (Gamepad gamepad in Gamepad.all) {
					if (!IsClaimed (gamepad)) {
						move += ReadGamepad (gamepad);
					}
				}
			} else {
				Gamepad gamepad = GetDevice (slot) as Gamepad;
				if (gamepad != null) {
					move += ReadGamepad (gamepad);
				}
			}

			move.x = Mathf.Clamp (move.x, -1f, 1f);
			move.y = Mathf.Clamp (move.y, -1f, 1f);
			return move;
		}

		// Gives the slot to the first unclaimed gamepad with a button pressed this frame.
		// Call every frame while waiting for someone to join.
		public static bool TryJoin (int slot) {
			if (slot <= 0 || slot >= MaxPlayers || GetDevice (slot) != null) {
				return false;
			}

			foreach (Gamepad gamepad in Gamepad.all) {
				if (!IsClaimed (gamepad) && gamepad.buttonSouth.wasPressedThisFrame) {
					devices [slot] = gamepad;
					return true;
				}
			}
			return false;
		}

		public static bool AnyButtonPressed () {
			Keyboard keyboard = Keyboard.current;
			if (keyboard != null && keyboard.anyKey.wasPressedThisFrame) {
				return true;
			}
			foreach (Gamepad gamepad in Gamepad.all) {
				if (gamepad.buttonSouth.wasPressedThisFrame || gamepad.startButton.wasPressedThisFrame) {
					return true;
				}
			}
			return false;
		}

		static Vector2 ReadKeys (KeyControl up, KeyControl down, KeyControl left, KeyControl right) {
			Vector2 move = Vector2.zero;
			if (up.isPressed) move.y += 1f;
			if (down.isPressed) move.y -= 1f;
			if (left.isPressed) move.x -= 1f;
			if (right.isPressed) move.x += 1f;
			return move;
		}

		static Vector2 ReadGamepad (Gamepad gamepad) {
			return gamepad.leftStick.ReadValue () + gamepad.dpad.ReadValue ();
		}

		static bool IsClaimed (InputDevice device) {
			for (int i = 1; i < MaxPlayers; i++) {
				if (GetDevice (i) == device) {
					return true;
				}
			}
			return false;
		}

		// A device that was unplugged gives its slot back.
		static InputDevice GetDevice (int slot) {
			if (devices [slot] != null && !devices [slot].added) {
				devices [slot] = null;
			}
			return devices [slot];
		}
	}
}
