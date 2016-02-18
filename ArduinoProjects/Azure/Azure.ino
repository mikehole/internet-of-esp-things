#include "sha256.h"
#include "Base64.h"
#include <ArduinoJson.h>
#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>
#include <WiFiClient.h>
#include <WiFiClientSecure.h>
#include <WiFiServer.h>
#include <WiFiUdp.h>
#include <ESP8266WebServer.h>

#include "IoespAzure.h"

const char *ssid = "edge";
const char *password = "P3n4rth#"; 
//const char *ssid = "virginmedia5388578";
//const char *password = "wtjjldlr";

IoesptAzure azure;

void setup() {
	Serial.begin(115200);
	Serial.println("");
	Serial.println("*IOESPT-Azure - Hello world.");

	WiFi.begin(ssid, password);

	Serial.println("");

	//ID=TestDevice
	//PrimaryKey = ExnLPZH9oJAhW2BQf3IMgmJaYuMjep0t52Cfs7 + m1rs =
	//SecondaryKey = 6BZ2wpbJkzUq6ZIiPM1tbdh7UvGrFiA8xuw / okd9nWc =
	//HostName=MikeHoleHome.azure-devices.net;DeviceId=TestDevice;SharedAccessKey=ExnLPZH9oJAhW2BQf3IMgmJaYuMjep0t52Cfs7+m1rs=

	// Wait for connection
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.print(".");
	}

	Serial.println("");
	Serial.print("Connected to ");
	Serial.println(ssid);
	Serial.print("IP address: ");
	Serial.println(WiFi.localIP());

	azure.start();

	azure.publishToAzure("{'message':'Hello World'}");

}

void loop() {
	azure.processRequests();;
}
