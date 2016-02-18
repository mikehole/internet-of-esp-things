#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <ESP8266mDNS.h>
#include <ArduinoJson.h>

#include "IoesptProvisioning.h"

IoesptProvisioning prov;

void setup()
{
	Serial.begin(115200);
	Serial.println("");
	Serial.println("*IOESPT-Provisioning - Hello world.");

	prov.setupConfigPortal();
}

void loop()
{

  /* add main program code here */

}
