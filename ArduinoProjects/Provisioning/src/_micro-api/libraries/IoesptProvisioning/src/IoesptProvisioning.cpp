// 
// 
// 

#include "IoesptProvisioning.h"

IoesptProvisioning::IoesptProvisioning()
{

}

void IoesptProvisioning::loadSettings(JsonObject& root)
{

}

void IoesptProvisioning::saveSettings(JsonObject& root)
{

}


void IoesptProvisioning::setupConfigPortal() {

	server.reset(new ESP8266WebServer(80));

	DEBUG_WMSL(F(""));

	start = millis();

	DEBUG_WMSL(F("Configuring access point... "));
	DEBUG_WMSL(_apName);

	if (_apPassword != NULL) {
		if (strlen(_apPassword) < 8 || strlen(_apPassword) > 63) {
			// fail passphrase to short or long!
			DEBUG_WMSL(F("Invalid AccessPoint password. Ignoring"));
			_apPassword = NULL;
		}
		DEBUG_WMSL(_apPassword);
	}

	//optional soft ip config
	if (_ap_static_ip) {
		DEBUG_WMSL(F("Custom AP IP/GW/Subnet"));
		WiFi.softAPConfig(_ap_static_ip, _ap_static_gw, _ap_static_sn);
	}

	if (_apPassword != NULL) {
		WiFi.softAP(_apName, _apPassword);//password option
	}
	else {
		WiFi.softAP(_apName);
	}

	delay(500); // Without delay I've seen the IP address blank

	DEBUG_WMSL(F("AP IP address: "));
	DEBUG_WMSL(WiFi.softAPIP());

	/* Setup web pages: root, wifi config pages, SO captive portal detectors and not found. */

	server->on("/", std::bind(&IoesptProvisioning::handleRoot, this));

	server->on("/listaccesspoints", std::bind(&IoesptProvisioning::listAccessPoints, this));

	server->begin(); // Web server start

	DEBUG_WMSL(F("HTTP server started"));

	while (true) {
		//HTTP
		server->handleClient();
	}
}

void IoesptProvisioning::handleRoot() {

	DEBUG_WMSL(F("Handle root"));

	server->send(200, "text/plain", "hello from esp8266!");
}

void IoesptProvisioning::listAccessPoints() {

	StaticJsonBuffer<2000> jsonBuffer;

	JsonObject& root = jsonBuffer.createObject();

	JsonArray& data = root.createNestedArray("WiFi");

	DEBUG_WMSL(F("List access points"));

	int n = WiFi.scanNetworks();
	DEBUG_WMSL(F("Scan done"));

	if (n == 0)
	{
		DEBUG_WMSL(F("No networks found"));
	}
	else
	{
		for (int i = 0; i < n; ++i)
		{
			JsonObject& wifi = jsonBuffer.createObject();

			DEBUG_WMS(WiFi.SSID(i));
			wifi["ssid"] = WiFi.SSID(i);

			DEBUG_WMC(" Power: ");
			DEBUG_WMF(WiFi.RSSI(i));

			int quality = getRSSIasQuality(WiFi.RSSI(i));

			wifi["quality"] = quality;

			data.add(wifi);
		}
	}

	char buffer[1024];
	root.printTo(buffer, sizeof(buffer));
	String json = buffer;
	server->send(200, "text/json", json);
}


int IoesptProvisioning::getRSSIasQuality(int RSSI) {
	int quality = 0;

	if (RSSI <= -100) {
		quality = 0;
	}
	else if (RSSI >= -50) {
		quality = 100;
	}
	else {
		quality = 2 * (RSSI + 100);
	}
	return quality;
}


///////////////////////////
// Diagnostics

template <typename Generic>
void IoesptProvisioning::DEBUG_WMSL(Generic text) {
	Serial.print("*IOESPT-Provisioning: ");
	Serial.println(text);
}

template <typename Generic>
void IoesptProvisioning::DEBUG_WMS(Generic text) {
	Serial.print("*IOESPT-Provisioning: ");
	Serial.print(text);
}

template <typename Generic>
void IoesptProvisioning::DEBUG_WMC(Generic text) {
	Serial.print(text);
}

template <typename Generic>
void IoesptProvisioning::DEBUG_WMF(Generic text) {
	Serial.println(text);
}