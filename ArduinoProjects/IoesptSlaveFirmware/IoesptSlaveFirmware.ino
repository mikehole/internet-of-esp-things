#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <ESP8266mDNS.h>
#include <ArduinoJson.h>


#include <sha256.h>
#include <Base64.h>
#include <IoesptAzure.h>
#include <IoesptPersistence.h>
#include <IoesptProvisioning.h>

IoesptPersistance persistence;

IoesptProvisioning provisioning;

IoesptAzure azure;


void setup()
{
	Serial.begin(115200);
	Serial.println(""); //Lets move away from the ugly stuff that gets sent on boot
	Serial.println("*IOESP-Slave Firmware Start - Hello world.");

	//provisioning.setupConfigPortal();
}

void loop()
{

  /* add main program code here */

}
