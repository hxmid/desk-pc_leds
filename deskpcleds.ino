#include <FastLED.h>

#define LED_TYPE WS2812B
#define ORDER GRB

#define DESK_DATA_PIN 3
#define PC_DATA_PIN 11

#define DESK_NUM_LEDS 53 //107
#define PC_NUM_LEDS 58

#define DESK_FULL_RAINBOW (255/DESK_NUM_LEDS)
#define PC_FULL_RAINBOW (255/PC_NUM_LEDS)

#define saturation 255
#define brightness 255

CRGB desk_led[DESK_NUM_LEDS];
CRGB pc_led[PC_NUM_LEDS];

void setup()
{
  FastLED.addLeds<LED_TYPE, DESK_DATA_PIN, ORDER>(desk_led, DESK_NUM_LEDS);
  FastLED.addLeds<LED_TYPE, PC_DATA_PIN, ORDER>(pc_led, PC_NUM_LEDS);
}
void loop()
{
  for(float colour = 0; colour < 256; colour++)
  {
    for(int current_led = 0; current_led < DESK_NUM_LEDS; current_led++)
    {
      desk_led[current_led] = CHSV(colour + ((float)current_led * DESK_FULL_RAINBOW), saturation, brightness);
    }

    for(int current_led = 0; current_led < PC_NUM_LEDS; current_led++)
    {
      pc_led[current_led] = CHSV(colour + ((float)current_led * PC_FULL_RAINBOW), saturation, brightness);
    }

    FastLED.show();
  }
}
