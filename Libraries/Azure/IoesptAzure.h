#ifndef IoesptProvisioning_h
#define IoesptProvisioning_h

#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <ArduinoJson.h>

class IoesptAzure
{
  public:

    IoesptAzure();

    void processRequests();

  private:

    //Request Handling
    void handleRoot();

    std::unique_ptr<ESP8266WebServer> server;
  
    //Diagnostics
    template <typename Generic> 
    void          DEBUG_WM(Generic text);

};

#endif
