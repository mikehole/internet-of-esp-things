// 
// 
// 

#include "IoesptProvisioning.h"

IoesptProvisioning::IoesptProvisioning()
{

}

void IoesptProvisioning::setupConfigPortal() {

	server.reset(new ESP8266WebServer(80));

	DEBUG_WM(F(""));

	start = millis();

	DEBUG_WM(F("Configuring access point... "));
	DEBUG_WM(_apName);

	if (_apPassword != NULL) {
		if (strlen(_apPassword) < 8 || strlen(_apPassword) > 63) {
			// fail passphrase to short or long!
			DEBUG_WM(F("Invalid AccessPoint password. Ignoring"));
			_apPassword = NULL;
		}
		DEBUG_WM(_apPassword);
	}

	//optional soft ip config
	if (_ap_static_ip) {
		DEBUG_WM(F("Custom AP IP/GW/Subnet"));
		WiFi.softAPConfig(_ap_static_ip, _ap_static_gw, _ap_static_sn);
	}

	if (_apPassword != NULL) {
		WiFi.softAP(_apName, _apPassword);//password option
	}
	else {
		WiFi.softAP(_apName);
	}

	delay(500); // Without delay I've seen the IP address blank

	DEBUG_WM(F("AP IP address: "));
	DEBUG_WM(WiFi.softAPIP());

	/* Setup web pages: root, wifi config pages, SO captive portal detectors and not found. */

	server->on("/", std::bind(&IoesptProvisioning::handleRoot, this));

	server->on("/listaccesspoints", std::bind(&IoesptProvisioning::listAccessPoints, this));

	server->begin(); // Web server start

	DEBUG_WM(F("HTTP server started"));

	while (true) {
		//HTTP
		server->handleClient();
	}
}

void IoesptProvisioning::handleRoot() {

	DEBUG_WM(F("Handle root"));

	server->send(200, "text/plain", "hello from esp8266!");
}

void IoesptProvisioning::listAccessPoints() {

	StaticJsonBuffer<2000> jsonBuffer;

	JsonObject& root = jsonBuffer.createObject();

	JsonArray& data = root.createNestedArray("WiFi");

	DEBUG_WM(F("List access points"));

	int n = WiFi.scanNetworks();
	DEBUG_WM(F("Scan done"));

	if (n == 0)
	{
		DEBUG_WM(F("No networks found"));
	}
	else
	{
		for (int i = 0; i < n; ++i)
		{
			JsonObject& wifi = jsonBuffer.createObject();

			DEBUG_WM(WiFi.SSID(i));
			wifi["ssid"] = WiFi.SSID(i);

			DEBUG_WM(WiFi.RSSI(i));

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


template <typename Generic>
void IoesptProvisioning::DEBUG_WM(Generic text) {
	Serial.print("*IOESP-Provisioning: ");
	Serial.println(text);
}

