// IoesptProvisioning.h

#ifndef _IOESPTPROVISIONING_h
#define _IOESPTPROVISIONING_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <ArduinoJson.h>



class IoesptProvisioning
{
public:
	IoesptProvisioning();

	void setupConfigPortal();

	void loadSettings(JsonObject& root);

	void saveSettings(JsonObject& root);

private:

	//Web server handlers
	void handleRoot();
	void listAccessPoints();

	//Utility
	int getRSSIasQuality(int RSSI);

	std::unique_ptr<ESP8266WebServer> server;

	unsigned long timeout = 0;
	unsigned long start = 0;

	const char*   _apName = "iotespt";
	const char*   _apPassword = NULL;

	IPAddress     _ap_static_ip;
	IPAddress     _ap_static_gw;
	IPAddress     _ap_static_sn;
	IPAddress     _sta_static_ip;
	IPAddress     _sta_static_gw;
	IPAddress     _sta_static_sn;

	//Diagnostics
	template <typename Generic>
	void DEBUG_WMSL(Generic text);

	template <typename Generic>
	void DEBUG_WMS(Generic text);

	template <typename Generic>
	void DEBUG_WMC(Generic text);

	template <typename Generic>
	void DEBUG_WMF(Generic text);

};


#endif

