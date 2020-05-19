// include FastLED library
#include <FastLED.h>
#include <string>

// setup LED strip details
#define LED_TYPE WS2812B
#define ORDER GRB

// pins on Arduino which each strip's LED data comes from 
#define DESK_DATA_PIN 3
#define PC_DATA_PIN 11

// number of leds on each strip
#define DESK_NUM_LEDS 54 //108
#define PC_NUM_LEDS 59

// show one full hue range on leds at any given time
#define DESK_FULL_RAINBOW (255/DESK_NUM_LEDS)
#define PC_FULL_RAINBOW (255/PC_NUM_LEDS)

// init array of rgb leds in strips
CRGB desk_led[DESK_NUM_LEDS];
CRGB pc_led[PC_NUM_LEDS];

// brightness and saturation as global variables, so i can control them with a winform in c#
float desk_saturation = 255;
float desk_brightness = 255;
float pc_saturation = 255;
float pc_brightness = 255;

// all my different profiles in one
namespace profile
{
	// int for multiple profiles, to be controlled by winform
	std::string current = "0";
	void all_off()
	{
		FastLED.clear();
		for (int current_led; current_led < DESK_NUM_LEDS; current_led++)
		{
			desk_led[current_led] = CRGB::Black;
		}
		for (int current_led = 0; current_led < PC_NUM_LEDS; current_led++)
		{
			pc_led[current_led] = CRGB::Black;
		}
		// update leds
		FastLED.show();
	}
	void frictionless_rainbow()
	{
		// cycle through one full hue cycle, then reset; frictionless (skjut mig)
		for (float colour = 0; colour < 256; colour++)
		{
			for (int current_led = 0; current_led < DESK_NUM_LEDS - 1; current_led++)
			{
				desk_led[current_led] = CHSV(colour + ((float)current_led * DESK_FULL_RAINBOW), desk_saturation, desk_brightness);
			}

			for (int current_led = 0; current_led < PC_NUM_LEDS - 1; current_led++)
			{
				pc_led[current_led] = CHSV(colour + ((float)current_led * PC_FULL_RAINBOW), pc_saturation, pc_brightness);
			}

			// update leds
			FastLED.show();
		}
	}
	void visualiser()
	{
		
	}
}

// Arduino functions
void setup()
{
	// set the baudrate at which it's expecting input from the WinForm
	Serial.begin(9600);
	// initialise strips
	FastLED.addLeds<LED_TYPE, DESK_DATA_PIN, ORDER>(desk_led, DESK_NUM_LEDS);
	FastLED.addLeds<LED_TYPE, PC_DATA_PIN, ORDER>(pc_led, PC_NUM_LEDS);
}


void loop()
{
	if (Serial.available())
		profile::current = Serial.read();

	switch(profile::current.at(0))
	{
	case '1':
		profile::frictionless_rainbow();
		break;
	default:
		profile::all_off();
		break;
	}
}