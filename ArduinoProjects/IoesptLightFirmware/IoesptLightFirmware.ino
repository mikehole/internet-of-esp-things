#include <ESP8266WiFi.h>
#include <PubSubClient.h>

const char *ssid = "edge";
const char *password = "P3n4rth#";

#define hubAddress "MikeHoleHome.azure-devices.net" 
#define hubName "TestDevice"
#define hubUser "MikeHoleHome.azure-devices.net/TestDevice"
#define hubPass "SharedAccessSignature sr=MikeHoleHome.azure-devices.net%2fdevices%2fTestDevice&sig=INAxoJtCXBSSUL6lYEBdFowsMKeRpz0BPBJoC8KbwkE%3d&se=1461310304"


#define hubTopic "devices/TestDevice/messages/devicebound/#"


WiFiClientSecure espClient;
PubSubClient client(espClient);

long lastMsg = 0;
char msg[50];
int value = 0;

#define BUFFER_SIZE 100

void setup() {
	Serial.begin(115200);

	Serial.println("Setting server for MQTT");
	//this will set the address of the hub and port on which it communicates
	client.setServer(hubAddress, 8883);

	Serial.println("Setting callback for MQTT");
	client.setCallback(callback);
}

void callback(char* topic, byte* payload, unsigned int length) {
	// handle message arrived
	String msg = "";
	for (int i = 0; i < length; i++) {
		msg += (char)payload[i];
	}
	
	Serial.print("We have a messge: ");
	Serial.println(msg);
}

void loop() {

	if (WiFi.status() != WL_CONNECTED) {
		Serial.print("Connecting to ");
		Serial.print(ssid);
		Serial.println(":");
		WiFi.begin(ssid, password);

		while (WiFi.status() != WL_CONNECTED) {
			delay(500);
			Serial.print(".");
		}
		Serial.println();

		Serial.println("WiFi connected");
	}

	if (WiFi.status() == WL_CONNECTED) {
		if (!client.connected()) {
			Serial.println(F("Connecting to MQTT server"));

			if (client.connect(hubName, hubUser, hubPass)) {

				Serial.println(F("Connected to MQTT server"));

				client.publish("devices/TestDevice/messages/events/", "Device Connected");

				client.subscribe(hubTopic);

			}
			else {
				Serial.println(F("Could not connect to MQTT server"));
			}
		}

		if (client.connected())
		{
			if (!client.loop()) {
				Serial.println(F("MQTT failed to loop"));
			}
		}
	}

}