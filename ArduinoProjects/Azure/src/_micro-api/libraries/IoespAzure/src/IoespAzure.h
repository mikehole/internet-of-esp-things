// IoespAzure.h

#ifndef _IOESPAZURE_h
#define _IOESPAZURE_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <ArduinoJson.h>

class IoesptAzure
{
public:

	IoesptAzure();

	void processRequests();

	void start();

private:

	//Request Handling
	void handleRoot();

	std::unique_ptr<ESP8266WebServer> server;

	//Diagnostics
	template <typename Generic>
	void          DEBUG_WM(Generic text);

};


#endif

