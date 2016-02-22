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

	device.boardType = Other;            // BoardType enumeration: NodeMCU, WeMos, SparkfunThing, Other (defaults to Other). This determines pin number of the onboard LED for wifi and publish status. Other means no LED status  
	device.deepSleepSeconds = 0;         // if greater than zero with call ESP8266 deep sleep (default is 0 disabled). GPIO16 needs to be tied to RST to wake from deepSleep. Causes a reset, execution restarts from beginning of sketch 
	
	cloud.cloudMode = IoTHub;            // CloudMode enumeration: IoTHub and EventHub (default is IoTHub) 
	cloud.publishRateInSeconds = 90;     // limits publishing rate to specified seconds (default is 90 seconds).  Connectivity problems may result if number too small eg 2 
	cloud.sasExpiryDate = 1737504000;    // Expires Wed, 22 Jan 2025 00:00:00 GMT (defaults to Expires Wed, 22 Jan 2025 00:00:00 GMT) 

	cloud.host = "MikeHoleHome.azure-devices.net";
	cloud.key = "ExnLPZH9oJAhW2BQf3IMgmJaYuMjep0t52Cfs7+m1rs=";
	cloud.id = "TestDevice";
}

void IoesptAzure::start()
{
	server.reset(new ESP8266WebServer(8080));

	server->on("/", std::bind(&IoesptAzure::handleRoot, this));

	server->begin();
	DEBUG_WMSL("HTTP server started");
}

void IoesptAzure::processRequests()
{
	server->handleClient();
}


///////////////////////////
// Server Handlers

void IoesptAzure::handleRoot() {

	DEBUG_WMSL(F("Handle root"));

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
	DEBUG_WMS(cloud.id);
	DEBUG_WMC(" connecting to ");
	DEBUG_WMF(cloud.host);
	if (WiFi.status() != WL_CONNECTED) { return; }
	if (!tlsClient.connect(cloud.host, 443)) {      // Use WiFiClientSecure class to create TLS connection
		DEBUG_WMS("Host connection failed.  WiFi IP Address: ");
		DEBUG_WMF(WiFi.localIP());

		delay(2000);
	}
	else {
		DEBUG_WMSL("Host connected");
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

	DEBUG_WMS("Bytes sent ");
	DEBUG_WMC(bytesWritten);
	DEBUG_WMC(", Memory ");
	DEBUG_WMC(ESP.getFreeHeap());
	DEBUG_WMC(" Message ");
	DEBUG_WMC(sendCount);
	DEBUG_WMC(", Response chunks ");
	DEBUG_WMC(limit);
	DEBUG_WMC(", Response code: ");

	if (response.length() > 12) { DEBUG_WMF(response.substring(9, 12)); }
	else { DEBUG_WMF("unknown"); }
}

///////////////////////////
// Diagnostics

template <typename Generic>
void IoesptAzure::DEBUG_WMSL(Generic text) {
	Serial.print("*IOESPT-Azure: ");
	Serial.println(text);
}

template <typename Generic>
void IoesptAzure::DEBUG_WMS(Generic text) {
	Serial.print("*IOESPT-Azure: ");
	Serial.print(text);
}

template <typename Generic>
void IoesptAzure::DEBUG_WMC(Generic text) {
	Serial.print(text);
}

template <typename Generic>
void IoesptAzure::DEBUG_WMF(Generic text) {
	Serial.println(text);
}
