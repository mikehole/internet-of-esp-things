#include <EEPROM.h>
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

	//persistence.saveSettings(&GiveSettings);
	
	
	persistence.loadSettings(&GetSettings);
	
	WiFi.begin(provisioning.wifi.ssid, provisioning.wifi.password);
	Serial.println("");

	// Wait for connection
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.print(".");
	}

	Serial.println("");
	Serial.print("Connected to ");
	Serial.println(provisioning.wifi.ssid);
	Serial.print("IP address: ");
	Serial.println(WiFi.localIP());


	azure.start();

	//azure.publishToAzure("{'value':'Hello Mike'}");

}

void loop()
{

	azure.processRequests();

}

void GiveSettings(JsonObject& root)
{
	azure.saveSettings(root);
	provisioning.saveSettings(root);
}

void GetSettings(JsonObject& root)
{
	azure.loadSettings(root);
	provisioning.loadSettings(root);
}