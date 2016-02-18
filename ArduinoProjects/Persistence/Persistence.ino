#include <ArduinoJson.h>
#include <EEPROM.h>

#include "IoesptPersistence.h"

IoesptPersistance settings;

void setup()
{
	Serial.begin(115200);
	Serial.println("");
	Serial.println("*IOESPT-Provisioning - Hello world.");
	
	settings.saveSettings(&GiveSettings);

	settings.saveSettings(&GetSettings);
}

void loop()
{

  /* add main program code here */


}

void GiveSettings(JsonObject& root)
{
	root["hello"] = "world";
	root["never"] = "gonna";
	root["give"] = "you";
}

void GetSettings(JsonObject& root)
{
	Serial.println(root["hello"]);
	Serial.println(root["never"]);
	Serial.println(root["give"]);
}