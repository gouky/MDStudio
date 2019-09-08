//////////////////////////////////////////////////////////////////////////
// File:		sdl_pad.cpp
// Date:		8th September 2019
// Authors:		Matt Phillips
// Description:	SDL gamepad handler
//////////////////////////////////////////////////////////////////////////
// Ripped and modified (with permission) from:
// https://github.com/BigEvilCorporation/ion-engine/tree/master/ion/input
//////////////////////////////////////////////////////////////////////////

#include "sdl_pad.h"

#include <stdio.h>
#include <string.h>

namespace sdl
{
	const char* Gamepad::s_additionalPads[AdditionalPadCount] =
	{
		"03000000790000002418000000000000,Mayflash MD USB Adapter,platform:Windows,a:b1,b:b2,x:b0,start:b9,dpup:h0.1,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,"
	};

	bool Gamepad::s_sdlSubsystemInitialised = false;

	Gamepad* Gamepad::FindAvailableController(int index)
	{
		if (!s_sdlSubsystemInitialised)
		{
			if (SDL_InitSubSystem(SDL_INIT_GAMECONTROLLER) != 0)
			{
				printf("Gamepad::Gamepad() - SDL_InitSubSystem(SDL_INIT_GAMECONTROLLER) failed");
				return nullptr;
			}

			//Add additional mappings
#if defined ION_INPUT_SDL_GAMEPAD_MAPPINGS
			for (int i = 0; i < AdditionalPadCount; i++)
			{
				SDL_GameControllerAddMapping(s_additionalPads[i]);
			}
#endif

			s_sdlSubsystemInitialised = true;
		}

		Gamepad* impl = nullptr;

		//Try SDL game controller
		if ((index < SDL_NumJoysticks()) && SDL_IsGameController(index))
		{
			SDL_GameController* sdlController = SDL_GameControllerOpen(index);

			if (sdlController)
			{
				impl = new Gamepad(index, sdlController);
			}
		}

		//Try SDL legacy joystick
#if defined ION_INPUT_SDL_GAMEPAD_LEGACY
		if (!impl)
		{
			if ((index < SDL_NumJoysticks()) && SDL_JoystickOpen(index))
			{
				SDL_Joystick* sdlJoystick = SDL_JoystickOpen(index);

				if (sdlJoystick)
				{
					impl = new GamepadSDLLegacy(index, sdlJoystick);
				}
			}
		}
#endif

		return impl;
	}

	Gamepad::Gamepad(int index, SDL_GameController* sdlController)
	{
		m_index = index;
		m_sdlController = sdlController;
		m_connected = false;
		memset(&m_axis, 0, sizeof(int16_t) * SDL_CONTROLLER_AXIS_MAX);
		memset(&m_buttons, 0, sizeof(int8_t) * SDL_CONTROLLER_BUTTON_MAX);
	}

	void Gamepad::Poll()
	{
		m_connected = m_sdlController && SDL_GameControllerGetAttached(m_sdlController);
		if (m_connected)
		{
			for (int i = 0; i < SDL_CONTROLLER_AXIS_MAX; i++)
			{
				m_axis[i] = SDL_GameControllerGetAxis(m_sdlController, (SDL_GameControllerAxis)i);
			}

			for (int i = 0; i < SDL_CONTROLLER_BUTTON_MAX; i++)
			{
				m_prevButtons[i] = m_buttons[i];
				m_buttons[i] = SDL_GameControllerGetButton(m_sdlController, (SDL_GameControllerButton)i);
			}
		}
	}

	bool Gamepad::IsConnected() const
	{
		return m_connected;
	}

	int Gamepad::ToPlatformButton(GamepadButtons button) const
	{
		int platformButton = 0;

		switch (button)
		{
		case GamepadButtons::DPAD_UP:			platformButton = SDL_CONTROLLER_BUTTON_DPAD_UP; break;
		case GamepadButtons::DPAD_DOWN:			platformButton = SDL_CONTROLLER_BUTTON_DPAD_DOWN; break;
		case GamepadButtons::DPAD_LEFT:			platformButton = SDL_CONTROLLER_BUTTON_DPAD_LEFT; break;
		case GamepadButtons::DPAD_RIGHT:		platformButton = SDL_CONTROLLER_BUTTON_DPAD_RIGHT; break;
		case GamepadButtons::BUTTON_A:			platformButton = SDL_CONTROLLER_BUTTON_A; break;
		case GamepadButtons::BUTTON_B:			platformButton = SDL_CONTROLLER_BUTTON_B; break;
		case GamepadButtons::BUTTON_X:			platformButton = SDL_CONTROLLER_BUTTON_X; break;
		case GamepadButtons::BUTTON_Y:			platformButton = SDL_CONTROLLER_BUTTON_Y; break;
		case GamepadButtons::START:				platformButton = SDL_CONTROLLER_BUTTON_START; break;
		case GamepadButtons::SELECT:			platformButton = SDL_CONTROLLER_BUTTON_BACK; break;
		case GamepadButtons::LEFT_SHOULDER:		platformButton = SDL_CONTROLLER_BUTTON_LEFTSHOULDER; break;
		case GamepadButtons::RIGHT_SHOULDER:	platformButton = SDL_CONTROLLER_BUTTON_RIGHTSHOULDER; break;
		case GamepadButtons::LEFT_STICK_CLICK:	platformButton = SDL_CONTROLLER_BUTTON_LEFTSTICK; break;
		case GamepadButtons::RIGHT_STICK_CLICK:	platformButton = SDL_CONTROLLER_BUTTON_RIGHTSTICK; break;
		}

		return platformButton;
	}

	Vector2 Gamepad::GetLeftStick() const
	{
		Vector2 leftStick((float)m_axis[SDL_CONTROLLER_AXIS_LEFTX], (float)-m_axis[SDL_CONTROLLER_AXIS_LEFTY]);

		if (leftStick.x != 0.0f)
			leftStick.x /= 32768;

		if (leftStick.y != 0.0f)
			leftStick.y /= 32768;

		return leftStick;
	}

	Vector2 Gamepad::GetRightStick() const
	{
		Vector2 rightStick((float)m_axis[SDL_CONTROLLER_AXIS_RIGHTX], (float)m_axis[SDL_CONTROLLER_AXIS_RIGHTY]);

		if (rightStick.x != 0.0f)
			rightStick.x /= 32768;

		if (rightStick.y != 0.0f)
			rightStick.y /= 32768;

		return rightStick;
	}

	bool Gamepad::CheckButton(GamepadButtons button) const
	{
		int sdlButton = ToPlatformButton(button);
		return m_buttons[sdlButton] != 0;
	}

	bool Gamepad::CheckPrevButton(GamepadButtons button) const
	{
		int sdlButton = ToPlatformButton(button);
		return m_prevButtons[sdlButton] != 0;
	}
}
