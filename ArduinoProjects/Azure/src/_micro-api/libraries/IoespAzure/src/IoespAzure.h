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

#include <WiFiClientSecure.h>

#include <sha256.h>
#include <Base64.h>

enum CloudMode {
	IoTHub,
	EventHub
};


enum BoardType {
	NodeMCU,
	WeMos,
	SparkfunThing,
	Other
};

struct CloudConfig {

	CloudMode cloudMode = IoTHub;
	unsigned int publishRateInSeconds = 60; // defaults to once a minute
	unsigned int sasExpiryDate = 1737504000;  // Expires Wed, 22 Jan 2025 00:00:00 GMT

	const char *host;
	char *key;
	const char *id;
	const char *geo;
	unsigned long lastPublishTime = 0;
	String fullSas;
	String endPoint;
};

struct DeviceConfig {
	int WifiIndex = 0;
	unsigned long LastWifiTime = 0;
	int WiFiConnectAttempts = 0;
	int wifiPairs = 0;
	const char ** ssid;
	const char **pwd;
	BoardType boardType = Other;            // OperationMode enumeration: NodeMCU, WeMos, SparkfunThing, Other
	unsigned int deepSleepSeconds = 0;      // Number of seconds for the ESP8266 chip to deepsleep for.  GPIO16 needs to be tied to RST to wake from deepSleep http://esp8266.github.io/Arduino/versions/2.0.0/doc/libraries.html
};

class IoesptAzure
{
public:

	CloudConfig cloud;
	DeviceConfig device;

	IoesptAzure();

	void processRequests();

	void start();

	void publishToAzure(String data);

private:

	void initialiseAzure();
	void initialiseIotHub();
	void initialiseEventHub();
	void connectToAzure();
	String createIotHubSas(char *key, String url);
	String createEventHubSas(char *key, String url);
	String buildHttpRequest(String data);

	String urlEncode(const char* msg);

	WiFiClientSecure tlsClient;

	// Azure IoT Hub Settings
	const char* TARGET_URL = "/devices/";
	const char* IOT_HUB_END_POINT = "/messages/events?api-version=2015-08-15-preview";

	// Azure Event Hub settings
	const char* EVENT_HUB_END_POINT = "/ehdevices/publishers/nodemcu/messages";

	int sendCount = 0;
	char buffer[256];
	bool azureInitialised = false;


	//Request Handling
	void handleRoot();

	std::unique_ptr<ESP8266WebServer> server;

	//Diagnostics
	template <typename Generic>
	void          DEBUG_WM(Generic text);

};


#endif

