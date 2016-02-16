#include <ArduinoJson.h>

#include "IoesptAzure.h"

const char *ssid = "virginmedia5388578";
const char *password = "wtjjldlr";

IoesptAzure azure;

void setup() {
  WiFi.begin ( ssid, password);
  
  Serial.println ( "" );

  // Wait for connection
  while ( WiFi.status() != WL_CONNECTED ) {
    delay ( 500 );
    Serial.print ( "." );
  }

  Serial.println ( "" );
  Serial.print ( "Connected to " );
  Serial.println ( ssid );
  Serial.print ( "IP address: " );
  Serial.println ( WiFi.localIP() );


}

void loop() {
  
  azure.processRequests();;

}
