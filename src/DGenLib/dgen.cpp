#include <windows.h>
#include <stdio.h>
#include <time.h>
#include <sdl.h>
#include <SDL_syswm.h>

#include "dgen.h"
extern "C" 
{
#include "md.h"
}

#include "sdl/pd-defs.h"
#include "musa/m68k.h"
#define	IS_MAIN_CPP
#include "rc-vars.h"

FILE *debug_log = NULL;

md*				s_DGenInstance = NULL;
SDL_Window*		g_SDLWindow = NULL;
SDL_Renderer*	g_SDLRenderer = NULL;
SDL_Texture*	g_BackBuffer = NULL;
HWND			g_HWND = NULL;

int usec = 0;
int newclk = 0, oldclk = 0, startclk = 0;
int frames_todo = 0;

static unsigned char*	mdpal = NULL;
static struct sndinfo	sndi;
static struct bmap		mdscr;

// Do a demo frame, if active
enum demo_status {
	DEMO_OFF,
	DEMO_RECORD,
	DEMO_PLAY
};

enum demo_status demo_status = DEMO_OFF;

void pd_message(const char *msg, ...)
{
/*
	va_list vl;

	va_start(vl, msg);
	vsnprintf(info.message, sizeof(info.message), msg, vl);
	va_end(vl);
	info.length = strlen(info.message);
	pd_message_process();*/
}

int InitDGen()
{
 	s_DGenInstance = new md(false, 'J');

	//	Init SDL
 	SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO);

	g_SDLWindow		= SDL_CreateWindow("DGen",				SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, 320, 240, SDL_WINDOW_OPENGL | SDL_WINDOW_HIDDEN | SDL_WINDOW_INPUT_FOCUS);
	g_SDLRenderer	= SDL_CreateRenderer(g_SDLWindow, -1,	SDL_RENDERER_ACCELERATED|SDL_RENDERER_PRESENTVSYNC|SDL_RENDERER_TARGETTEXTURE);
	g_BackBuffer	= SDL_CreateTexture(g_SDLRenderer,		SDL_PIXELFORMAT_ARGB8888, SDL_TEXTUREACCESS_STREAMING, 320, 240);

	//<	Init  screen
	mdscr.bpp	= 32;
	mdscr.w		= 320;
	mdscr.h		= 240;
	mdscr.pitch	= mdscr.w*4;
	mdscr.data	= (unsigned char*)malloc(mdscr.pitch * mdscr.h);

	mdpal		= NULL;

	return 1;
}

void	ShowSDLWindow()
{
	SDL_ShowWindow(g_SDLWindow);
}

void	HideSDLWindow()
{
	SDL_HideWindow(g_SDLWindow);
}

int		LoadRom(const char* path)
{
	s_DGenInstance->load(path);
	s_DGenInstance->debug_init();
	
	ShowSDLWindow();

	return 1;
}

int		Reset()
{
	s_DGenInstance->debug_init();
	return s_DGenInstance->reset();
}

int		Shutdown()
{
	SDL_DestroyTexture(g_BackBuffer);
	SDL_DestroyRenderer(g_SDLRenderer);
	SDL_DestroyWindow(g_SDLWindow);

	delete s_DGenInstance;

	return 1;
}

void	BeginFrame()
{
	SDL_SetRenderTarget(g_SDLRenderer, g_BackBuffer);
	SDL_SetRenderDrawColor(g_SDLRenderer, 120, 120, 120, 255);
	SDL_RenderClear(g_SDLRenderer);
}

void	EndFrame()
{
	SDL_RenderSetClipRect(g_SDLRenderer, NULL);

	//Back to screen
	SDL_Rect	dst;

	dst.x = dst.y = 0;
	dst.w = 320;
	dst.h = 240;

	SDL_SetRenderTarget(g_SDLRenderer, NULL);
	SDL_RenderCopy(g_SDLRenderer, g_BackBuffer, NULL, &dst);
	SDL_RenderPresent(g_SDLRenderer);
}

void uSleep(int waitTime) {
	__int64 time1 = 0, time2 = 0, freq = 0;

	QueryPerformanceCounter((LARGE_INTEGER *) &time1);
	QueryPerformanceFrequency((LARGE_INTEGER *)&freq);

	do {
		QueryPerformanceCounter((LARGE_INTEGER *) &time2);
	} while((time2-time1) < waitTime);
}

int gettimeofday(struct timeval * tp, struct timezone * tzp)
{
	// Note: some broken versions only have 8 trailing zero's, the correct epoch has 9 trailing zero's
	static const uint64_t EPOCH = ((uint64_t) 116444736000000000ULL);

	SYSTEMTIME  system_time;
	FILETIME    file_time;
	uint64_t    time;

	GetSystemTime( &system_time );
	SystemTimeToFileTime( &system_time, &file_time );
	time =  ((uint64_t)file_time.dwLowDateTime )      ;
	time += ((uint64_t)file_time.dwHighDateTime) << 32;

	tp->tv_sec  = (long) ((time - EPOCH) / 10000000L);
	tp->tv_usec = (long) (system_time.wMilliseconds * 1000);
	return 0;
}

/**
 * Elapsed time in microseconds.
 * @return Microseconds.
 */
unsigned long pd_usecs(void)
{
	struct timeval tv;

	gettimeofday(&tv, NULL);
	return (unsigned long)((tv.tv_sec * 1000000) + tv.tv_usec);
}

/**
 *	Process SDL inputs
 */
void	ProcessInputs()
{
	SDL_Event event;

	const Uint8 *keystate = SDL_GetKeyboardState(NULL);

	while(SDL_PollEvent(&event))
	{
		switch(event.type)
		{
		case SDL_KEYDOWN:
			switch(event.key.keysym.sym)
			{
			case SDLK_LEFT:
			case SDLK_j:
				s_DGenInstance->pad[0] &=~ MD_LEFT_MASK;
				break;

			case SDLK_RIGHT:
			case SDLK_l:
				s_DGenInstance->pad[0] &=~ MD_RIGHT_MASK;
				break;

			case SDLK_DOWN:
			case SDLK_k:
				s_DGenInstance->pad[0] &=~ MD_DOWN_MASK;
				break;

			case SDLK_UP:
			case SDLK_i:
				s_DGenInstance->pad[0] &=~ MD_UP_MASK;
				break;

			case SDLK_a:
				s_DGenInstance->pad[0] &=~ MD_A_MASK;
				break;
			case SDLK_s:
				s_DGenInstance->pad[0] &=~ MD_B_MASK;
				break;
			case SDLK_d:
				s_DGenInstance->pad[0] &=~ MD_C_MASK;
				break;
			}
			break;

		case SDL_KEYUP:
			switch(event.key.keysym.sym)
			{
			case SDLK_LEFT:
			case SDLK_j:
 				s_DGenInstance->pad[0] |= MD_LEFT_MASK;
				break;

			case SDLK_RIGHT:
			case SDLK_l:
				s_DGenInstance->pad[0] |= MD_RIGHT_MASK;
				break;

			case SDLK_DOWN:
			case SDLK_k:
				s_DGenInstance->pad[0] |= MD_DOWN_MASK;
				break;

			case SDLK_UP:
			case SDLK_i:
				s_DGenInstance->pad[0] |= MD_UP_MASK;
				break;

			case SDLK_a:
				s_DGenInstance->pad[0] |= MD_A_MASK;
				break;
			case SDLK_s:
				s_DGenInstance->pad[0] |= MD_B_MASK;
				break;
			case SDLK_d:
				s_DGenInstance->pad[0] |= MD_C_MASK;
				break;
			}
			break;
		}
	}
}

/**
 *	Update a frame of DGEN
 *	@return success
 */
int UpdateDGen()
{
	ProcessInputs();
	BeginFrame();

	const unsigned int usec_frame = (1000000 / dgen_hz);

	newclk = pd_usecs();

	// Measure how many frames to do this round.
	usec += ((newclk - oldclk) & 0x3fffff); // no more than 4 secs
	frames_todo = (usec / usec_frame);
	usec %= usec_frame;
	oldclk = newclk;

	if (frames_todo == 0) {
		// No frame to do yet, relax the CPU until next one.
		int tmp = (usec_frame - usec);
		if (tmp > 1000) {
			// Never sleep for longer than the 50Hz value
			// so events are checked often enough.
			if (tmp > (1000000 / 50))
				tmp = (1000000 / 50);
			tmp -= 1000;
			uSleep(tmp);
		}
	}
	else
	{
		s_DGenInstance->md_set_musa(true);

		int pc = s_DGenInstance->m68k_get_pc();

		// 			for(int i = 0; i < 32 ;++i)
		// 			{
		// 				unsigned int instrsize;
		// 				const char* code = m68ki_disassemble_quick(pc, M68K_CPU_TYPE_68000, &instrsize);
		// 				SDL_Log("#$%x\t%s", pc, code);
		// 				pc += instrsize;
		// 			}

		s_DGenInstance->one_frame(&mdscr, mdpal, &sndi);
		//pd_sound_write();
	}

	void*	pixels = NULL;
	int		pitch = 0;
	SDL_LockTexture(g_BackBuffer, NULL, &pixels, &pitch);
	memcpy(pixels, mdscr.data, mdscr.h * mdscr.pitch);
	SDL_UnlockTexture(g_BackBuffer);

	EndFrame();
	return 1;
}

/**
 *	Add code breakpoint at the specified address
 *	@return success
 */
int		AddBreakpoint(int addr)
{
	s_DGenInstance->debug_set_bp_m68k(addr);
	return 1;
}

void	ClearBreakpoints()
{
	s_DGenInstance->debug_clear_bp_m68k();
}

int StepInto()
{
	return s_DGenInstance->debug_cmd_step(0, NULL);
}

int Resume()
{
	return s_DGenInstance->debug_cmd_cont(0, NULL);
}

int Break()
{
	return s_DGenInstance->debug_cmd_step(0, NULL);
}

int IsDebugging()
{
	return s_DGenInstance->debug_trap;
}

int GetCurrentPC()
{
	if(!s_DGenInstance)
		return 0;

	return s_DGenInstance->m68k_get_pc();
}