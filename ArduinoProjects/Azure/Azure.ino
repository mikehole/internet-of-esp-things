#include <ArduinoJson.h>
#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>
#include <WiFiClient.h>
#include <WiFiClientSecure.h>
#include <WiFiServer.h>
#include <WiFiUdp.h>
#include <ESP8266WebServer.h>

#include "IoespAzure.h"

IoesptAzure azure;

void setup() {
	Serial.begin(115200);
	Serial.println("");
	Serial.println("*IOESP-Azure - Hello world.");

	WiFi.begin("edge", "P3n4rth#");

	Serial.println("");

	// Wait for connection
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.print(".");
	}

	Serial.println("");
	Serial.print("Connected to ");
	Serial.println("edge");
	Serial.print("IP address: ");
	Serial.println(WiFi.localIP());

	azure.start();
}

void loop() {
	azure.processRequests();;
}
