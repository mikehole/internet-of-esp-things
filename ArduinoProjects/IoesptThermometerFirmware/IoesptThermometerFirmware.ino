///////////////////////////////
// NOTE WORK IN PROGRESS

#include <OneWire.h>
#include "DallasTemperature\DallasTemperature.h"

#include <ESP8266SSDP.h>
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <ArduinoJson.h>
#include "Adafruit_MQTT.h"
#include "Adafruit_MQTT_Client.h"


#define ONE_WIRE_BUS 2  // DS18B20 pin

OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature DS18B20(&oneWire);

ESP8266WebServer HTTP(80);

/************************* Adafruit.io Setup *********************************/

#define AIO_SERVER      "io.adafruit.com"
#define AIO_SERVERPORT  1883
#define AIO_USERNAME    "mikehole"
#define AIO_KEY         "d510a83f97a1b6dc2e7730837f38dde618e6e686"

/************ Global State (you don't need to change this!) ******************/

// Create an ESP8266 WiFiClient class to connect to the MQTT server.
WiFiClient client;

// Store the MQTT server, client ID, username, and password in flash memory.
// This is required for using the Adafruit MQTT library.
const char MQTT_SERVER[] PROGMEM = AIO_SERVER;

// Set a unique MQTT client ID using the AIO key + the date and time the sketch
// was compiled (so this should be unique across multiple devices for a user,
// alternatively you can manually set this to a GUID or other random value).
const char MQTT_CLIENTID[] PROGMEM = AIO_KEY __DATE__ __TIME__;
const char MQTT_USERNAME[] PROGMEM = AIO_USERNAME;
const char MQTT_PASSWORD[] PROGMEM = AIO_KEY;

// Setup the MQTT client class by passing in the WiFi client and MQTT server and login details.
Adafruit_MQTT_Client mqtt(&client, MQTT_SERVER, AIO_SERVERPORT, MQTT_CLIENTID, MQTT_USERNAME, MQTT_PASSWORD);

/****************************** Feeds ***************************************/

// Notice MQTT paths for AIO follow the form: <username>/feeds/<feedname>
const char OFFICETEMP_FEED[] PROGMEM = AIO_USERNAME "/feeds/office-temperature";
Adafruit_MQTT_Publish officeTemp = Adafruit_MQTT_Publish(&mqtt, OFFICETEMP_FEED);


/*************************** Sketch Code ************************************/


const char *ssid = "edge";
const char *password = "P3n4rth#";

float temp;

void MQTT_connect();

unsigned long timer; // the timer
unsigned long INTERVAL = 20000; // the repeat interval
					
bool SetupSSDP()
{
	SSDP.setHTTPPort(80);
	SSDP.setManufacturer(F("IOESPT"));

	String deviceName = "IOESPT-Thermometer-" + String(ESP.getChipId());

	SSDP.setName(deviceName);
	SSDP.setSerialNumber(String(ESP.getChipId()));
	SSDP.setModelName(F("IOESPT-Thermometer"));
	SSDP.setModelNumber("0.0.1");

	SSDP.setModelURL(F("https://github.com/mikehole/internet-of-esp-things"));
	SSDP.setManufacturerURL(F("https://github.com/mikehole/internet-of-esp-things"));

	SSDP.setURL(F("index.html"));
	SSDP.setSchemaURL(F("description.xml"));

	return SSDP.begin();
}

void setup()
{
	Serial.begin(115200);
	Serial.println(""); //Lets move away from the ugly stuff that gets sent on boot
	Serial.println("*IOESP- Thermometer Start - Hello world.");

	WiFi.begin(ssid, password);

	Serial.print(F("Connecting"));
	// Wait for connection
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.print(".");
	}
	Serial.println();

	HTTP.on("/index.html", HTTP_GET, []() {
		HTTP.send(200, "text/plain", "Hello World!");
	});
	HTTP.on("/description.xml", HTTP_GET, []() {
		SSDP.schema(HTTP.client());
	});
	HTTP.on("/gettemperature", HTTP_GET, []() {

		String json = "{'temperature' :" + String(temp) + "}";

		HTTP.send(200, "text/json", json);
	});

	HTTP.begin();

	SetupSSDP();

	timer = millis(); // start timer

}

void loop()
{
	HTTP.handleClient();

	MQTT_connect();

	do {
		DS18B20.requestTemperatures();
		temp = DS18B20.getTempCByIndex(0);
		Serial.print("Temperature: ");
		Serial.println(temp);
	} while (temp == 85.0 || temp == (-127.0));

	if ((millis() - timer) > INTERVAL) {
		// timed out
		timer += INTERVAL;// reset timer by moving it along to the next interval 

		// Now we can publish stuff!
		Serial.print(F("\nSending temp val "));
		Serial.print("...");
		if (!officeTemp.publish(temp)) {
			Serial.println(F("Failed"));
		}
		else {
			Serial.println(F("OK!"));
		}
	}

	delay(1);
}

// Function to connect and reconnect as necessary to the MQTT server.
// Should be called in the loop function and it will take care if connecting.
void MQTT_connect() {
	int8_t ret;

	// Stop if already connected.
	if (mqtt.connected()) {
		return;
	}

	Serial.print("Connecting to MQTT... ");

	uint8_t retries = 3;
	while ((ret = mqtt.connect()) != 0) { // connect will return 0 for connected
		//Serial.println(mqtt.connectErrorString(ret));
		Serial.println("Retrying MQTT connection in 5 seconds...");
		mqtt.disconnect();
		delay(5000);  // wait 5 seconds
		retries--;
		if (retries == 0) {
			// basically die and wait for WDT to reset me
			while (1);
		}
	}
	Serial.println("MQTT Connected!");
}