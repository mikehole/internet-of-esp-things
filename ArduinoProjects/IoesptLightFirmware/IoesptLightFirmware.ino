/*
Basic ESP8266 MQTT example

This sketch demonstrates the capabilities of the pubsub library in combination
with the ESP8266 board/library.

It connects to an MQTT server then:
- publishes "hello world" to the topic "outTopic" every two seconds
- subscribes to the topic "inTopic", printing out any messages
it receives. NB - it assumes the received payloads are strings not binary
- If the first character of the topic "inTopic" is an 1, switch ON the ESP Led,
else switch it off

It will reconnect to the server if the connection is lost using a blocking
reconnect function. See the 'mqtt_reconnect_nonblocking' example for how to
achieve the same result without blocking the main loop.

To install the ESP8266 board, (using Arduino 1.6.4+):
- Add the following 3rd party board manager under "File -> Preferences -> Additional Boards Manager URLs":
http://arduino.esp8266.com/stable/package_esp8266com_index.json
- Open the "Tools -> Board -> Board Manager" and click install for the ESP8266"
- Select your ESP8266 in "Tools -> Board"

*/

#include <sha256.h>
#include <Base64.h>
#include <ESP8266WiFi.h>
#include <PubSubClient.h>

// Update these with values suitable for your network.

const char* IOT_HUB_END_POINT = "/messages/events?api-version=2015-08-15-preview";

const char* EVENT_HUB_END_POINT = "/ehdevices/publishers/nodemcu/messages";


const char* ssid = "edge";
const char* password = "P3n4rth#";

const char* mqtt_server = "MikeHoleHome.azure-devices.net";

//HostName =MikeHoleHome.azure-devices.net;DeviceId=TestDevice;SharedAccessKey=ExnLPZH9oJAhW2BQf3IMgmJaYuMjep0t52Cfs7+m1rs=

WiFiClient espClient;

PubSubClient client(espClient);

String urlEncode(const char* msg);
String createIotHubSas(char *key, String url);

long lastMsg = 0;
char msg[50];
int value = 0;

void setup() {
	pinMode(BUILTIN_LED, OUTPUT);     // Initialize the BUILTIN_LED pin as an output
	Serial.begin(115200);
	setup_wifi();
	client.setServer(mqtt_server, 1883);
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

		String url = urlEncode("https://") + urlEncode(mqtt_server) + urlEncode(EVENT_HUB_END_POINT);

		if (client.connect("TestDevice", "MikeHoleHome.azure-devices.net/TestDevice", createIotHubSas("ExnLPZH9oJAhW2BQf3IMgmJaYuMjep0t52Cfs7+m1rs=", url).c_str()) ) {
			Serial.println("connected");
			
			// Once connected, publish an announcement...
			client.publish("devices/TestDevice/messages/events/", "hello world");
			
			// ... and resubscribe
			client.subscribe("devices/TestDevice/messages/devicebound/");
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

String createIotHubSas(char *key, String url) {
	String stringToSign = url + "\n" + 1737504000;

	// START: Create signature
	// https://raw.githubusercontent.com/adamvr/arduino-base64/master/examples/base64/base64.ino

	int keyLength = strlen(key);

	int decodedKeyLength = base64_dec_len(key, keyLength);
	char decodedKey[decodedKeyLength];  //allocate char array big enough for the base64 decoded key

	base64_decode(decodedKey, key, keyLength);  //decode key

	Sha256.initHmac((const uint8_t*)decodedKey, decodedKeyLength);
	Sha256.print(stringToSign);
	char* sign = (char*)Sha256.resultHmac();
	// END: Create signature

	// START: Get base64 of signature
	int encodedSignLen = base64_enc_len(HASH_LENGTH);
	char encodedSign[encodedSignLen];
	base64_encode(encodedSign, sign, HASH_LENGTH);

	// SharedAccessSignature
	return "sr=" + url + "&sig=" + urlEncode(encodedSign) + "&se=" + 1737504000;
	// END: create SAS  
}

String urlEncode(const char* msg)
{
	const char *hex = "0123456789abcdef";
	String encodedMsg = "";

	while (*msg != '\0') {
		if (('a' <= *msg && *msg <= 'z')
			|| ('A' <= *msg && *msg <= 'Z')
			|| ('0' <= *msg && *msg <= '9')) {
			encodedMsg += *msg;
		}
		else {
			encodedMsg += '%';
			encodedMsg += hex[*msg >> 4];
			encodedMsg += hex[*msg & 15];
		}
		msg++;
	}
	return encodedMsg;
}

