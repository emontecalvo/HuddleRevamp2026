using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace HuddleNights {

	// Remembers which input device controls which player slot, across scenes.
	// Player 1 (slot 0) uses the keyboard (WASD, plus the arrows unless someone else has them)
	// and keeps the first free gamepad they move with.
	// Players 2-4 claim a free gamepad by pressing A on it once their jello has been found.
	// One extra player can share the keyboard instead: pressing Enter gives them the arrow keys,
	// and player 1 moves to WASD.
	public static class PlayerRoster {

		public const int MaxPlayers = 4;

		static readonly InputDevice[] devices = new InputDevice[MaxPlayers];
		// The slot playing with the arrow keys on a shared keyboard, or -1.
		static int ArrowKeysSlot = -1;

		[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.SubsystemRegistration)]
		static void ResetRoster () {
			for (int i = 0; i < MaxPlayers; i++) {
				devices [i] = null;
			}
			ArrowKeysSlot = -1;
		}

		public static bool IsHuman (int slot) {
			return slot == 0 || GetDevice (slot) != null || slot == ArrowKeysSlot;
		}

		public static Vector2 GetMove (int slot) {
			if (slot < 0 || slot >= MaxPlayers) {
				return Vector2.zero;
			}

			Vector2 move = Vector2.zero;

			Keyboard keyboard = Keyboard.current;
			if (keyboard != null) {
				if (slot == 0) {
					move += ReadKeys (keyboard.wKey, keyboard.sKey, keyboard.aKey, keyboard.dKey);
				}
				if (slot == ArrowKeysSlot || (slot == 0 && ArrowKeysSlot < 0)) {
					move += ReadKeys (keyboard.upArrowKey, keyboard.downArrowKey, keyboard.leftArrowKey, keyboard.rightArrowKey);
				}
			}

			if (slot == 0) {
				// Player 1 keeps the first free gamepad they move with, so pressing A
				// on it can't hand it to another jello.
				if (GetDevice (0) == null) {
					foreach (Gamepad gamepad in Gamepad.all) {
						if (!IsClaimed (gamepad) && ReadGamepad (gamepad).sqrMagnitude > 0.25f) {
							devices [0] = gamepad;
							break;
						}
					}
				}
			}

			Gamepad mine = GetDevice (slot) as Gamepad;
			if (mine != null) {
				move += ReadGamepad (mine);
			}

			move.x = Mathf.Clamp (move.x, -1f, 1f);
			move.y = Mathf.Clamp (move.y, -1f, 1f);
			return move;
		}

		// Gives the slot to the first unclaimed gamepad with A pressed this frame, or to the
		// arrow keys if Enter was pressed. Call every frame while waiting for someone to join.
		public static bool TryJoin (int slot) {
			if (slot <= 0 || slot >= MaxPlayers || IsHuman (slot)) {
				return false;
			}

			Keyboard keyboard = Keyboard.current;
			if (ArrowKeysSlot < 0 && keyboard != null &&
				(keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)) {
				ArrowKeysSlot = slot;
				return true;
			}

			foreach (Gamepad gamepad in Gamepad.all) {
				if (!IsClaimed (gamepad) && gamepad.buttonSouth.wasPressedThisFrame) {
					devices [slot] = gamepad;
					return true;
				}
			}
			return false;
		}

		public static bool UsesArrowKeys (int slot) {
			return slot == ArrowKeysSlot;
		}

		public static bool IsKeyboardFree () {
			return ArrowKeysSlot < 0;
		}

		public static bool HasFreeGamepad () {
			foreach (Gamepad gamepad in Gamepad.all) {
				if (!IsClaimed (gamepad)) {
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
			for (int i = 0; i < MaxPlayers; i++) {
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
