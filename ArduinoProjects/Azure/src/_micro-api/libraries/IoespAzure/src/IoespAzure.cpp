// 
// 
// 

#include "IoespAzure.h"

IoesptAzure::IoesptAzure()
{

}

void IoesptAzure::start()
{
	server.reset(new ESP8266WebServer(8080));

	server->on("/", std::bind(&IoesptAzure::handleRoot, this));

	server->begin();
	Serial.println("HTTP server started");
}

void IoesptAzure::processRequests()
{
	server->handleClient();
}


///////////////////////////
// Server Handlers

void IoesptAzure::handleRoot() {

	DEBUG_WM(F("Handle root"));

	server->send(200, "text/plain", "hello from esp8266!");
}

///////////////////////////
// Diagnostics


template <typename Generic>
void IoesptAzure::DEBUG_WM(Generic text) {
	Serial.print("*IOESP-Provisioning: ");
	Serial.println(text);
}
