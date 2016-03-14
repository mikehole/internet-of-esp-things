#include <ESP8266WiFi.h>
#include <PubSubClient.h>

const char *ssid = "edge";
const char *password = "P3n4rth#";

const char* mqtt_server = "MikeHoleHome.azure-devices.net";

WiFiClientSecure espClient;

PubSubClient client(espClient, mqtt_server, 8883);

void callback(const MQTT::Publish& pub);

long lastMsg = 0;
char msg[50];
int value = 0;

#define BUFFER_SIZE 100

void setup() {
	Serial.begin(115200);
}

void callback(const MQTT::Publish& pub) {
	Serial.println(pub.topic());

	Serial.print("payload_len : ");
	Serial.println(pub.payload_len());
	
	if (pub.has_stream()) {
		Serial.println(" (stream) => ");

		uint8_t buf[BUFFER_SIZE];
		int read;
		while (read = pub.payload_stream()->read(buf, BUFFER_SIZE)) {
			Serial.write(buf, read);
		}
		pub.payload_stream()->stop();
		Serial.println("");
	}
	else
	{
		Serial.println(" (string) => ");
		Serial.println(pub.payload_string());
	}
}

void loop() {
	if (WiFi.status() != WL_CONNECTED) {
		Serial.print("Connecting to ");
		Serial.print(ssid);
		Serial.println("...");
		WiFi.begin(ssid, password);

		if (WiFi.waitForConnectResult() != WL_CONNECTED)
			return;
		Serial.println("WiFi connected");
	}

	if (WiFi.status() == WL_CONNECTED) {
		if (!client.connected()) {
			Serial.println("Connecting to MQTT server");
			if (client.connect(MQTT::Connect("TestDevice")
				.set_auth("MikeHoleHome.azure-devices.net/TestDevice", "SharedAccessSignature sr=MikeHoleHome.azure-devices.net%2fdevices%2fTestDevice&sig=yh6X5iYURyAtmaY33jhs375Q7nQZ%2by%2b5Qq05%2bbJwHF4%3d&se=1458934959"))) {
				
				Serial.println("Connected to MQTT server");
				
				client.set_callback(callback);
				//client.publish("devices/TestDevice/messages/events/", "hello world");
				
				client.subscribe("devices/TestDevice/messages/devicebound/#");
			}
			else {
				Serial.println("Could not connect to MQTT server");
			}
		}

		if (client.connected())
		{

			client.loop();
		}
	}
}