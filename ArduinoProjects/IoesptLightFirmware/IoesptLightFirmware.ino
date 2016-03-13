#include <ESP8266WiFi.h>
#include <PubSubClient.h>

const char *ssid = "virginmedia5388578";
const char *password = "wtjjldlr";

const char* mqtt_server = "MikeHoleHome.azure-devices.net";

WiFiClientSecure espClient;

PubSubClient client(espClient);

long lastMsg = 0;
char msg[50];
int value = 0;

void setup() {
	Serial.begin(115200);
	setup_wifi();

	client.setServer(mqtt_server, 8883);
	client.setCallback(callback);
}

void setup_wifi() {

	delay(10);
	// We start by connecting to a WiFi network
	Serial.println();
	Serial.print("Connecting to ");
	Serial.println(ssid);

	WiFi.begin(ssid, password);

	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.print(".");
	}

	Serial.println("");
	Serial.println("WiFi connected");
	Serial.println("IP address: ");
	Serial.println(WiFi.localIP());
}

void callback(char* topic, byte* payload, unsigned int length) {
	Serial.print("Message arrived [");
	Serial.print(topic);
	Serial.print("] ");
	for (int i = 0; i < length; i++) {
		Serial.print((char)payload[i]);
	}
	Serial.println();

	// Switch on the LED if an 1 was received as first character
	if ((char)payload[0] == '1') {

	}
	else {

	}

}

void reconnect() {
	// Loop until we're reconnected
	while (!client.connected()) {
		Serial.print("Attempting MQTT connection...");
		// Attempt to connect

		if (client.connect("TestDevice", "MikeHoleHome.azure-devices.net/TestDevice", "SharedAccessSignature sr=MikeHoleHome.azure-devices.net%2fdevices%2fTestDevice&sig=yh6X5iYURyAtmaY33jhs375Q7nQZ%2by%2b5Qq05%2bbJwHF4%3d&se=1458934959" )) {
			Serial.print("connected : State: ");
			Serial.println(client.state());
			
			// Once connected, publish an announcement...
			client.publish("devices/TestDevice/messages/events/", "hello world");

			Serial.print("post publish an announcement : State: ");
			Serial.println(client.state());

			// ... and resubscribe
			client.subscribe("devices/TestDevice/messages/devicebound/");

			Serial.print("post resubscribe : State: ");
			Serial.println(client.state());
		}
		else {
			Serial.print("failed, rc=");
			Serial.print(client.state());
			Serial.println(" try again in 5 seconds");
			// Wait 5 seconds before retrying
			delay(5000);
		}
	}
}

void loop() {

	if (!client.connected()) {
		reconnect();
	}

	client.loop();

	long now = millis();
	if (now - lastMsg > 2000) {
		
		lastMsg = now;
		
		++value;
		
		snprintf(msg, 75, "hello world #%ld", value);
		
		Serial.print("Publish message: ");
		
		Serial.println(msg);
		
		client.publish("outTopic", msg);
	}
}