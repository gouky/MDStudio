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

#ifdef WITH_MUSA
extern "C" {
#include "musa/m68k.h"
}
#endif

#define	IS_MAIN_CPP
#include "rc-vars.h"

#define AUDIO_CHANNELS 2

FILE *debug_log = NULL;

md*				s_DGenInstance = NULL;
SDL_Window*		g_SDLWindow = NULL;
SDL_Renderer*	g_SDLRenderer = NULL;
SDL_Texture*	g_BackBuffer = NULL;
SDL_AudioSpec	g_AudioSpec;
HWND			g_HWND = NULL;

int usec = 0;
int newclk = 0, oldclk = 0, startclk = 0;
int frames_todo = 0;

int sdlWindowWidth;
int sdlWindowHeight;

static unsigned char*	mdpal = NULL;
static struct sndinfo	sndi;
static struct bmap		mdscr;

/// Circular buffer and related functions.
typedef struct
{
	size_t i; ///< data start index
	size_t s; ///< data size
	size_t size; ///< buffer size
	union
	{
		uint8_t *u8;
		int16_t *i16;
	} data; ///< storage
} cbuf_t;

cbuf_t cbuf;

size_t cbuf_write(cbuf_t *cbuf, uint8_t *src, size_t size)
{
	size_t j;
	size_t k;

	if(size > cbuf->size) {
		src += (size - cbuf->size);
		size = cbuf->size;
	}
	k = (cbuf->size - cbuf->s);
	j = ((cbuf->i + cbuf->s) % cbuf->size);
	if(size > k) {
		cbuf->i = ((cbuf->i + (size - k)) % cbuf->size);
		cbuf->s = cbuf->size;
	}
	else
		cbuf->s += size;
	k = (cbuf->size - j);
	if(k >= size) {
		memcpy(&cbuf->data.u8[j], src, size);
	}
	else {
		memcpy(&cbuf->data.u8[j], src, k);
		memcpy(&cbuf->data.u8[0], &src[k], (size - k));
	}
	return size;
}

size_t cbuf_read(uint8_t *dst, cbuf_t *cbuf, size_t size)
{
	if(size > cbuf->s)
		size = cbuf->s;
	if((cbuf->i + size) > cbuf->size) {
		size_t k = (cbuf->size - cbuf->i);

		memcpy(&dst[0], &cbuf->data.u8[(cbuf->i)], k);
		memcpy(&dst[k], &cbuf->data.u8[0], (size - k));
	}
	else
		memcpy(&dst[0], &cbuf->data.u8[(cbuf->i)], size);
	cbuf->i = ((cbuf->i + size) % cbuf->size);
	cbuf->s -= size;
	return size;
}


struct SDLInputMapping
{
	Uint32 sdlKey;
	Uint32 dgenKey;
};

SDLInputMapping sdlInputMapping[eInput_COUNT] =
{
	{ SDLK_i, MD_UP_MASK },
	{ SDLK_k, MD_DOWN_MASK },
	{ SDLK_j, MD_LEFT_MASK },
	{ SDLK_l, MD_RIGHT_MASK },
	{ SDLK_s, MD_B_MASK },
	{ SDLK_d, MD_C_MASK },
	{ SDLK_a, MD_A_MASK },
	{ SDLK_SPACE, MD_START_MASK },
	{ SDLK_c, MD_Z_MASK },
	{ SDLK_x, MD_Y_MASK },
	{ SDLK_z, MD_X_MASK },
	{ SDLK_m, MD_MODE_MASK }
};

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

void DGenAudioCallback(void *userdata, Uint8 * stream, int len)
{
	size_t wrote = cbuf_read(stream, &cbuf, len);
	if(wrote != (size_t)len)
	{
		//Fill remainder with silence
		memset(&stream[wrote], 0, ((size_t)len - wrote));
	}
}

int InitDGen(int windowWidth, int windowHeight, HWND parent, int pal, char region)
{
 	s_DGenInstance = new md(pal, region);

	//	Init SDL
 	SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO);

	g_SDLWindow		= SDL_CreateWindow("DGen",				SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, windowWidth, windowHeight, SDL_WINDOW_OPENGL | SDL_WINDOW_HIDDEN | SDL_WINDOW_INPUT_FOCUS);
	g_SDLRenderer	= SDL_CreateRenderer(g_SDLWindow, -1,	SDL_RENDERER_ACCELERATED|SDL_RENDERER_PRESENTVSYNC|SDL_RENDERER_TARGETTEXTURE);
	g_BackBuffer	= SDL_CreateTexture(g_SDLRenderer,		SDL_PIXELFORMAT_ARGB8888, SDL_TEXTUREACCESS_STREAMING, windowWidth, windowHeight);

	sdlWindowWidth = windowWidth;
	sdlWindowHeight = windowHeight;

	//<	Init  screen
	mdscr.bpp	= 32;
	mdscr.w		= windowWidth;
	mdscr.h		= windowHeight;
	mdscr.pitch	= mdscr.w*4;
	mdscr.data	= (unsigned char*)malloc(mdscr.pitch * mdscr.h);

	mdpal		= NULL;

	//	Set parent window
	SDL_SysWMinfo wmInfo;
	
	SDL_GetWindowWMInfo(g_SDLWindow, &wmInfo);

	SetParent(wmInfo.info.win.window, parent);

	// Init audio
	g_AudioSpec.channels = 2;
	g_AudioSpec.samples = dgen_soundsamples;
	g_AudioSpec.size = 0;
	g_AudioSpec.freq = dgen_soundrate;
	g_AudioSpec.callback = DGenAudioCallback;
	g_AudioSpec.userdata = NULL;

#ifdef WORDS_BIGENDIAN
	g_AudioSpec.format = AUDIO_S16MSB;
#else
	g_AudioSpec.format = AUDIO_S16LSB;
#endif

	if(int audioResult = SDL_OpenAudio(&g_AudioSpec, &g_AudioSpec) < 0)
	{
		printf("SDL_OpenAudio() failed with 0x%08x", audioResult);
	}

	// Alloc audio buffer
	sndi.len = (dgen_soundrate / dgen_hz);
	sndi.lr = (int16_t *)calloc(2, (sndi.len * sizeof(sndi.lr[0])));

	// Alloc ringbuffer
	cbuf.size = (dgen_soundsegs * (dgen_soundrate / dgen_hz)) + (g_AudioSpec.samples * (2 * (16 / 8)));
	cbuf.data.i16 = (int16_t *)calloc(1, cbuf.size);
	cbuf.i = 0;
	cbuf.s = 0;

	return 1;
}

void	SetDGenWindowPosition(int x, int y)
{
	SDL_SetWindowPosition(g_SDLWindow, x, y);
}

int		GetDGenWindowXPosition()
{
	int x, y;

	SDL_GetWindowPosition(g_SDLWindow, &x, &y);

	return x;
}

int		GetDGenWindowYPosition()
{
	int x, y;

	SDL_GetWindowPosition(g_SDLWindow, &x, &y);

	return y;
}

void	BringToFront()
{
	SDL_RaiseWindow(g_SDLWindow);
}

void	ShowSDLWindow()
{
	SDL_ShowWindow(g_SDLWindow);
}

void	HideSDLWindow()
{
	SDL_HideWindow(g_SDLWindow);
}

void	SetInputMapping(int input, int mapping)
{
	sdlInputMapping[input].sdlKey = mapping;
}

int		GetInputMapping(int input)
{
	return sdlInputMapping[input].sdlKey;
}

int		LoadRom(const char* path)
{
	s_DGenInstance->load(path);
	s_DGenInstance->debug_init();
	
	ShowSDLWindow();
	SDL_PauseAudio(0);

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
	SDL_CloseAudio();

	delete s_DGenInstance;
	free(sndi.lr);
	free(cbuf.data.i16);

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
	SDL_Rect	src;
	SDL_Rect	dst;

	src.x = src.y = 0;
	src.w = 320;
	src.h = 240;

	dst.x = dst.y = 0;
	dst.w = sdlWindowWidth;
	dst.h = sdlWindowHeight;

	SDL_SetRenderTarget(g_SDLRenderer, NULL);
	SDL_RenderCopy(g_SDLRenderer, g_BackBuffer, &src, &dst);
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

	while (SDL_PollEvent(&event))
	{
		switch(event.type)
		{
			case SDL_KEYDOWN:
			{
				for (int i = 0; i < eInput_COUNT; i++)
				{
					if (event.key.keysym.sym == sdlInputMapping[i].sdlKey)
					{
						s_DGenInstance->pad[0] &= ~sdlInputMapping[i].dgenKey;
					}
				}
			}
			break;

			case SDL_KEYUP:
			{
				for (int i = 0; i < eInput_COUNT; i++)
				{
					if (event.key.keysym.sym == sdlInputMapping[i].sdlKey)
					{
						s_DGenInstance->pad[0] |= sdlInputMapping[i].dgenKey;
					}
				}
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
#ifdef WITH_MUSA
		s_DGenInstance->md_set_musa(true);
#endif

#ifdef WITH_STAR
		s_DGenInstance->md_set_star(true);
#endif

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

	//Write sound buffer to ringbuffer
	SDL_LockAudio();
	cbuf_write(&cbuf, (uint8_t*)sndi.lr, (sndi.len * 4));
	SDL_UnlockAudio();

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

int AddWatchPoint(int fromAddr, int toAddr)
{
	s_DGenInstance->debug_set_wp_m68k(fromAddr, toAddr);
	return 1;
}

void ClearWatchpoints()
{
	// TODO
}

int	KeyPressed(int vkCode, int keyDown)
{
	SDL_Keycode	keycode = SDLK_UNKNOWN;
	bool		pushEvent = false;

	switch (vkCode)
	{
	case VK_LEFT:
		keycode = SDLK_LEFT;
		break;
	case VK_RIGHT:
		keycode = SDLK_RIGHT;
		break;
	case VK_UP:
		keycode = SDLK_UP;
		break;
	case VK_DOWN:
		keycode = SDLK_DOWN;
		break;
	case VK_SPACE:
		keycode = SDLK_SPACE;
		break;
	}

	if (keycode != SDLK_UNKNOWN)
	{
		SDL_Event event;

		event.type = keyDown == 1 ? SDL_KEYDOWN : SDL_KEYUP;
		event.key.keysym.sym = keycode;

		return SDL_PushEvent(&event);
	}

	return 0;
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

unsigned int* GetProfilerResults(int* instructionCount)
{
#ifdef WITH_PROFILER
	return s_DGenInstance->md_profiler_get_instr_run_counts(instructionCount);
#else
	*instructionCount = 0;
	return NULL;
#endif
}

unsigned int GetInstructionCycleCount(unsigned int address)
{
#ifdef WITH_PROFILER
	return s_DGenInstance->md_profiler_get_instr_num_cycles(address);
#else
	return 0;
#endif
}

int GetDReg(int index)
{
	return s_DGenInstance->debug_m68k_get_reg((m68k_register_t)((int)M68K_REG_D0 + index));
}

int GetAReg(int index)
{
	return s_DGenInstance->debug_m68k_get_reg((m68k_register_t)((int)M68K_REG_A0 + index));
}

int GetSR()
{
	return s_DGenInstance->debug_m68k_get_reg(M68K_REG_SR);
}

int GetCurrentPC()
{
	if(!s_DGenInstance)
		return 0;

	return s_DGenInstance->m68k_get_pc();
}

unsigned char ReadByte(unsigned int address)
{
	return s_DGenInstance->misc_readbyte(address);
}

unsigned short ReadWord(unsigned int address)
{
	return s_DGenInstance->misc_readword(address);
}

unsigned int ReadLong(unsigned int address)
{
	short hi = s_DGenInstance->misc_readword(address);
	short lo = s_DGenInstance->misc_readword(address + 2);
	return (hi << 16) | lo;
}

void ReadMemory(unsigned int address, unsigned int size, BYTE* memory)
{
	for(unsigned int i = 0; i < size; i++)
	{
		memory[i] = s_DGenInstance->misc_readbyte(address + i);
	}
}

int GetPaletteEntry(int i)
{
	unsigned char* cram = s_DGenInstance->vdp.cram;

	int r, g, b;
	b = (cram[i * 2 + 0] & 0x0e) << 4;
	g = (cram[i * 2 + 1] & 0xe0);
	r = (cram[i * 2 + 1] & 0x0e) << 4;

	return (r<<16) | (g << 8) | b;
}

unsigned char GetVDPRegisterValue(int index)
{
	return s_DGenInstance->vdp.reg[index];
}