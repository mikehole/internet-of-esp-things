#include <ESP8266SSDP.h>
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

ESP8266WebServer HTTP(8080);

bool SetupSSDP()
{
	SSDP.setHTTPPort(80);
	SSDP.setManufacturer("IOESPT");

	SSDP.setName(provisioning.device.FirmwareName);
	SSDP.setSerialNumber(provisioning.device.ChipId);
	SSDP.setModelName(provisioning.device.FirmwareName);
	SSDP.setModelNumber(provisioning.device.FirmwareVersion);

	SSDP.setModelURL("https://github.com/mikehole/internet-of-esp-things");
	SSDP.setManufacturerURL("https://github.com/mikehole/internet-of-esp-things");

	SSDP.setURL("index.html");
	SSDP.setSchemaURL("description.xml");
}


void setup()
{

	Serial.begin(115200);
	Serial.println(""); //Lets move away from the ugly stuff that gets sent on boot
	Serial.println("*IOESP-Slave Firmware Start - Hello world.");

	//Set the device details here:
	provisioning.device.ChipId = String(ESP.getChipId());
	provisioning.device.FirmwareName = "IOESPT-Slave";
	provisioning.device.FirmwareVersion = "0.1.0";
	provisioning.device.ModuleType = "ESP8266-Generic";

	provisioning.settingsChanged = &settingsChanged;

	//persistence.loadSettings(&GetSettings);
	
	if (provisioning.getConnected())
	{
		Serial.println("");
		Serial.print("Connected to ");
		Serial.println(provisioning.wifi.ssid);
		Serial.print("IP address: ");
		Serial.println(WiFi.localIP());

		SetupSSDP();

		HTTP.on("/index.html", HTTP_GET, []() {
			HTTP.send(200, "text/plain", "Hello World!");
		});
		HTTP.on("/description.xml", HTTP_GET, []() {
			SSDP.schema(HTTP.client());
		});

		azure.start(&HTTP);

		HTTP.begin();

		Serial.print("Starting SSDP...");
		if (SSDP.begin())
			Serial.println("OK");
		else
			Serial.println("Fail!");
	}
	
	//azure.publishToAzure("{'value':'Hello Mike'}");

	pinMode(2, OUTPUT);


}

void loop()
{

	HTTP.handleClient();

	digitalWrite(2, HIGH);   // turn the LED on (HIGH is the voltage level)
	delay(1000);              // wait for a second
	digitalWrite(2, LOW);
	delay(1000);              // wait for a second

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

void settingsChanged()
{
	persistence.saveSettings(&GiveSettings);
}




