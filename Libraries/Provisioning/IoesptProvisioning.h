#ifndef IoesptProvisioning_h
#define IoesptProvisioning_h

#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <ArduinoJson.h>



class IoesptProvisioning
{
  public:
    IoesptProvisioning();

    void setupConfigPortal();

  private:
    
    //Web server handlers
    void handleRoot();
    void listAccessPoints();

    //Utility
    int getRSSIasQuality(int RSSI);
    
    std::unique_ptr<ESP8266WebServer> server;

     template <typename Generic> 
     void          DEBUG_WM(Generic text); 

     unsigned long timeout                 = 0; 
     unsigned long start                   = 0; 

     const char*   _apName                 = "iotespt"; 
     const char*   _apPassword             = NULL; 

     IPAddress     _ap_static_ip; 
     IPAddress     _ap_static_gw; 
     IPAddress     _ap_static_sn; 
     IPAddress     _sta_static_ip; 
     IPAddress     _sta_static_gw; 
     IPAddress     _sta_static_sn; 

     static char result[1000];

};



#endif
