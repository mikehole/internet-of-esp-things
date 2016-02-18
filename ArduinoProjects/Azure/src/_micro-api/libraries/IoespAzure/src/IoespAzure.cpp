// 
// 
// 

#include "IoespAzure.h"

String IoesptAzure::urlEncode(const char* msg)
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

IoesptAzure::IoesptAzure()
{

}

void IoesptAzure::start()
{
	server.reset(new ESP8266WebServer(8080));

	server->on("/", std::bind(&IoesptAzure::handleRoot, this));

	server->begin();
	DEBUG_WM("HTTP server started");
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
// Azure Publish

void IoesptAzure::initialiseAzure() {
	if (azureInitialised) { return; }

	switch (cloud.cloudMode) {
	case IoTHub:
		initialiseIotHub();
		break;
	case EventHub:
		initialiseEventHub();
		break;
	}

	azureInitialised = true;
}

void IoesptAzure::initialiseIotHub() {
	String url = urlEncode(cloud.host) + urlEncode(TARGET_URL) + (String)cloud.id;
	cloud.endPoint = (String)TARGET_URL + (String)cloud.id + (String)IOT_HUB_END_POINT;
	cloud.fullSas = createIotHubSas(cloud.key, url);
}

void IoesptAzure::initialiseEventHub() {
	String url = urlEncode("https://") + urlEncode(cloud.host) + urlEncode(EVENT_HUB_END_POINT);
	cloud.endPoint = EVENT_HUB_END_POINT;
	cloud.fullSas = createEventHubSas(cloud.key, url);
}

void IoesptAzure::connectToAzure() {
	delay(500); // give network connection a moment to settle
	DEBUG_WM(cloud.id);
	DEBUG_WM(" connecting to ");
	DEBUG_WM(cloud.host);
	if (WiFi.status() != WL_CONNECTED) { return; }
	if (!tlsClient.connect(cloud.host, 443)) {      // Use WiFiClientSecure class to create TLS connection
		DEBUG_WM("Host connection failed.  WiFi IP Address: ");
		DEBUG_WM(WiFi.localIP());

		delay(2000);
	}
	else {
		DEBUG_WM("Host connected");
		yield(); // give firmware some time 
				 //    delay(250); // give network connection a moment to settle
	}
}

String IoesptAzure::createIotHubSas(char *key, String url) {
	String stringToSign = url + "\n" + cloud.sasExpiryDate;

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
	return "sr=" + url + "&sig=" + urlEncode(encodedSign) + "&se=" + cloud.sasExpiryDate;
	// END: create SAS  
}

String IoesptAzure::createEventHubSas(char *key, String url) {
	// START: Create SAS  
	// https://azure.microsoft.com/en-us/documentation/articles/service-bus-sas-overview/
	// Where to get seconds since the epoch: local service, SNTP, RTC

	String stringToSign = url + "\n" + cloud.sasExpiryDate;

	// START: Create signature
	Sha256.initHmac((const uint8_t*)key, 44);
	Sha256.print(stringToSign);

	char* sign = (char*)Sha256.resultHmac();
	int signLen = 32;
	// END: Create signature

	// START: Get base64 of signature
	int encodedSignLen = base64_enc_len(signLen);
	char encodedSign[encodedSignLen];
	base64_encode(encodedSign, sign, signLen);
	// END: Get base64 of signature

	// SharedAccessSignature
	return "sr=" + url + "&sig=" + urlEncode(encodedSign) + "&se=" + cloud.sasExpiryDate + "&skn=" + cloud.id;
	// END: create SAS
}

String IoesptAzure::buildHttpRequest(String data) {
	return "POST " + cloud.endPoint + " HTTP/1.1\r\n" +
		"Host: " + cloud.host + "\r\n" +
		"Authorization: SharedAccessSignature " + cloud.fullSas + "\r\n" +
		"Content-Type: application/atom+xml;type=entry;charset=utf-8\r\n" +
		"Content-Length: " + data.length() + "\r\n\r\n" + data;
}

void IoesptAzure::publishToAzure(String data) {
	int bytesWritten = 0;

	// https://msdn.microsoft.com/en-us/library/azure/dn790664.aspx  

	initialiseAzure();

	if (!tlsClient.connected()) { connectToAzure(); }
	if (!tlsClient.connected()) { return; }

	tlsClient.flush();

	bytesWritten = tlsClient.print(buildHttpRequest(data));

	String response = "";
	String chunk = "";
	int limit = 1;

	do {
		if (tlsClient.connected()) {
			yield();
			chunk = tlsClient.readStringUntil('\n');
			response += chunk;
		}
	} while (chunk.length() > 0 && ++limit < 100);

	DEBUG_WM("Bytes sent ");
	DEBUG_WM(bytesWritten);
	DEBUG_WM(", Memory ");
	DEBUG_WM(ESP.getFreeHeap());
	DEBUG_WM(" Message ");
	DEBUG_WM(sendCount);
	DEBUG_WM(", Response chunks ");
	DEBUG_WM(limit);
	DEBUG_WM(", Response code: ");

	if (response.length() > 12) { DEBUG_WM(response.substring(9, 12)); }
	else { DEBUG_WM("unknown"); }
}

///////////////////////////
// Diagnostics

template <typename Generic>
void IoesptAzure::DEBUG_WM(Generic text) {
	Serial.print("*IOESP-Azure: ");
	Serial.println(text);
}
