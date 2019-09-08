//////////////////////////////////////////////////////////////////////////
// File:		sdl_pad.h
// Date:		8th September 2019
// Authors:		Matt Phillips
// Description:	SDL gamepad handler
//////////////////////////////////////////////////////////////////////////
// Ripped and modified (with permission) from:
// https://github.com/BigEvilCorporation/ion-engine/tree/master/ion/input
//////////////////////////////////////////////////////////////////////////

#pragma once

#include <SDL.h>

namespace sdl
{
	struct Vector2
	{
		Vector2() { x = 0.0f, y = 0.0f; }
		Vector2(float _x, float _y) { x = _x, y = _y; }

		float x;
		float y;
	};

	enum class GamepadButtons : int
	{
		DPAD_UP,
		DPAD_DOWN,
		DPAD_LEFT,
		DPAD_RIGHT,

		BUTTON_A,
		BUTTON_B,
		BUTTON_X,
		BUTTON_Y,

		START,
		SELECT,

		LEFT_SHOULDER,
		RIGHT_SHOULDER,

		LEFT_STICK_CLICK,
		RIGHT_STICK_CLICK,

		COUNT
	};

	enum class GamepadSticks : int
	{
		LEFT,
		RIGHT,

		COUNT
	};

	class Gamepad
	{
	public:
		static Gamepad* FindAvailableController(int index);

		void Poll();
		bool IsConnected() const;
		int ToPlatformButton(GamepadButtons button) const;
		Vector2 GetLeftStick() const;
		Vector2 GetRightStick() const;
		bool CheckButton(GamepadButtons button) const;
		bool CheckPrevButton(GamepadButtons button) const;

	private:
		Gamepad(int index, SDL_GameController* sdlController);

		int m_index;
		bool m_connected;

		SDL_GameController* m_sdlController;
		int16_t m_axis[SDL_CONTROLLER_AXIS_MAX];
		int8_t m_buttons[SDL_CONTROLLER_BUTTON_MAX];
		int8_t m_prevButtons[SDL_CONTROLLER_BUTTON_MAX];

		enum AdditionalPadTypes
		{
			MayflashMDUSB,

			AdditionalPadCount
		};

		static const char* s_additionalPads[AdditionalPadCount];
		static bool s_sdlSubsystemInitialised;
	};
}