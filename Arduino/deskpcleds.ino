// include FastLED library
#include <FastLED.h>

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

// parsing commands with more options than just which mode is being set
namespace parse
{
	int red(String data_in)
	{
		String data_input = data_in;
		data_input.remove(0, data_input.indexOf('R') + 1);
		data_input.remove(data_input.indexOf('G'), data_input.length() - data_input.indexOf('G'));

		return data_input.toInt();
	}

	int green(String data_in)
	{
		String data_input = data_in;
		data_input.remove(0, data_input.indexOf('G') + 1);
		data_input.remove(data_input.indexOf('B'), data_input.length() - data_input.indexOf('B'));

		return data_input.toInt();
	}

	int blue(String data_in)
	{
		String data_input = data_in;
		data_input.remove(0, data_input.indexOf('B') + 1);

		return data_input.toInt();
	}

	int saturation(String data_in)
	{
		String data_input = data_in;
		data_input.remove(0, data_input.indexOf('S') + 1);
		data_input.remove(data_input.indexOf('B'), data_input.length() - data_input.indexOf('B'));

		return data_input.toInt();
	}

	int brightness(String data_in)
	{
		String data_input = data_in;
		data_input.remove(0, data_input.indexOf('B') + 1);

		if (data_input.indexOf('H') > 0 && data_input.indexOf('H') < data_input.length() - 1)
			data_input.remove(data_input.indexOf('B'), data_input.length() - data_input.indexOf('B'));

		return data_input.toInt();
	}

	int hue(String data_in)
	{
		String data_input = data_in;
		data_input.remove(0, data_input.indexOf('H') + 1);

		return data_input.toInt();
	}
}

// all my different profiles in one
namespace profile
{
	// int for multiple profiles, to be controlled by winform
	String current = "0";

	void all_off()
	{
		FastLED.clear();
		for (int current_led; current_led < DESK_NUM_LEDS; current_led++)
		{
			desk_led[current_led] = CRGB::Black;
			if (Serial.available())
				return;
		}
		for (int current_led = 0; current_led < PC_NUM_LEDS; current_led++)
		{
			pc_led[current_led] = CRGB::Black;
			if (Serial.available())
				return;
		}

		// update leds
		FastLED.show();
	}

	void frictionless_rainbow()
	{
		// cycle through one full hue cycle, then reset; frictionless (skjut mig)
		for (float colour = 0; colour < 256; colour++)
		{
			if (Serial.available())
				return;

			for (int current_led = 0; current_led < DESK_NUM_LEDS - 1; current_led++)
			{
				desk_led[current_led] = CHSV(colour + ((float)current_led * DESK_FULL_RAINBOW), parse::saturation(profile::current), parse::brightness(profile::current));
				if (Serial.available())
					return;
			}

			for (int current_led = 0; current_led < PC_NUM_LEDS - 1; current_led++)
			{
				pc_led[current_led] = CHSV(colour + ((float)current_led * PC_FULL_RAINBOW), parse::saturation(profile::current), parse::brightness(profile::current));
				if (Serial.available())
					return;
			}

			FastLED.show();
		}
	}

	void custom()
	{
		for (int current_led = 0; current_led < DESK_NUM_LEDS - 1; current_led++)
		{
			desk_led[current_led].r = parse::red(profile::current);
			desk_led[current_led].g = parse::green(profile::current);
			desk_led[current_led].b = parse::blue(profile::current);
			if (Serial.available())
				return;
		}

		for (int current_led = 0; current_led < PC_NUM_LEDS - 1; current_led++)
		{
			pc_led[current_led].r = parse::red(profile::current);
			pc_led[current_led].g = parse::green(profile::current);
			pc_led[current_led].b = parse::blue(profile::current);
			if (Serial.available())
				return;
		}
		FastLED.show();
	}

	void visualiser()
	{
		
	}
}

// Arduino functions
void setup()
{
	// baudrate
	Serial.begin(9600);

	// initialise strips
	FastLED.addLeds<LED_TYPE, DESK_DATA_PIN, ORDER>(desk_led, DESK_NUM_LEDS);
	FastLED.addLeds<LED_TYPE, PC_DATA_PIN, ORDER>(pc_led, PC_NUM_LEDS);
}

void loop()
{
	while (Serial.available())
	{
		char in_char = (char)Serial.read();
		if (in_char == '>')
			profile::current = "";
		profile::current += in_char;
		if (in_char == '<')
			Serial.println((String)profile::current);
	}
	
	// serial input format: >PM1###2###3###< 	 		 	\
	>	 - command prefix								 	\
	P	 - program id sending input (in case of multiple)	\
	M 	 - mode; profile number					 			\
	1### - red value / saturation value			 			\
	2### - green value / brightness	value		 			\
	3### - blue value / hue value				 			\
	<	 - marks end of command

	
	if (profile::current.charAt(0) == '>' && profile::current.charAt(profile::current.length() - 1) == '<')
	{
		if (profile::current.charAt(1) == '0')
		{
			if (profile::current.charAt(2) == '1')
				profile::frictionless_rainbow();

			else if (profile::current.charAt(2) == '2')
				profile::custom();

			else
				profile::all_off();
		}
		else
			profile::all_off();
	}
	else
		profile::all_off();
}
