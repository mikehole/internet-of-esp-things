#include <ArduinoJson.h>
#include <EEPROM.h>

#include "IoesptPersistence.h"

IoesptPersistance settings;

void setup()
{
	Serial.begin(115200);
	Serial.println("");
	Serial.println("*IOESPT-Persistence - Hello world.");
	
	//settings.saveSettings(&GiveSettings);

	settings.loadSettings(&GetSettings);
}

void loop()
{
  /* add main program code here */
}

void GiveSettings(JsonObject& root)
{
	root["r1"] = "never";
	root["r2"] = "gonna";
	root["r3"] = "give";
	root["r4"] = "you";
	root["r5"] = "up";
	root["r6"] = "...";
}

void GetSettings(JsonObject& root)
{
	Serial.println("Values are:");

	String val = root["r1"];
	Serial.println(val);

	String val2 = root["r2"];
	Serial.println(val2);

	String val3 = root["r3"];
	Serial.println(val3);

	String val4 = root["r4"];
	Serial.println(val4);

	String val5 = root["r5"];
	Serial.println(val5);

	String val6 = root["r6"];
	Serial.println(val6);
}