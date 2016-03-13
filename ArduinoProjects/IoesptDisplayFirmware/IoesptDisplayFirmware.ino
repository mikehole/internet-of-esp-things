/*
* Connecting ESP8266 (ESP-01) and Nokia 5110 LCD
* www.KendrickTabi.com
* http://www.kendricktabi.com/2015/08/esp8266-and-nokia-5110-lcd.html
*/

#include "IoesptLCD.h"
#include <ESP8266SSDP.h>
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <ArduinoJson.h>

// Hex values that represent the IOESPT logo
char DisplayDevice[] = {
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0xF0, 0xF0, 0xF0, 0x00, 0x00, 0x00, 0x00, 0xF0, 0xF0, 0xF0, 0x00, 0x00, 0x00, 0x00, 0xF0,
	0xF0, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0xF8,
	0xFC, 0xFE, 0x0E, 0x0E, 0x0E, 0x0F, 0x0F, 0x0F, 0x0E, 0x0E, 0x0E, 0x0E, 0x0F, 0x0F, 0x0F, 0x8E,
	0x8E, 0x8E, 0x8E, 0x8F, 0x8F, 0x8F, 0x8E, 0x8E, 0x0E, 0x0E, 0xFE, 0xFC, 0xF8, 0x80, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C, 0x1C, 0x1C, 0x1C,
	0x0E, 0x87, 0xC7, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0xFF, 0xFF,
	0xFF, 0xC7, 0x87, 0x0E, 0x1C, 0x1C, 0x1C, 0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x0E, 0x0E, 0x0E, 0x0E, 0x87, 0xC3, 0xE3, 0xFF, 0xFF, 0xFF, 0x00, 0xF0, 0x90, 0x90, 0x90, 0x00,
	0x20, 0x50, 0x90, 0x10, 0x00, 0x00, 0xF0, 0x90, 0x90, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xE3, 0xC3, 0x87, 0x0E, 0x0E, 0x0E, 0x0E, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x07, 0x07, 0x07, 0x07, 0x03, 0x01, 0x01, 0x3F, 0x7F, 0xFF, 0xE0, 0xE7,
	0xE4, 0xE4, 0xE4, 0xE0, 0xE4, 0xE4, 0xE4, 0xE3, 0xE0, 0xE0, 0xE7, 0xE0, 0xE0, 0xE0, 0xE0, 0xE0,
	0xE0, 0xE0, 0xE0, 0xE0, 0xE0, 0xE0, 0xFF, 0x7F, 0x3F, 0x01, 0x01, 0x03, 0x07, 0x07, 0x07, 0x07,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x00,
	0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
};

ESP8266WebServer HTTP(80);

const char *ssid = "edge";
const char *password = "P3n4rth#";

bool SetupSSDP()
{
	SSDP.setHTTPPort(80);
	SSDP.setManufacturer(F("IOESPT"));

	String deviceName = "IOESPT-Display-" + String(ESP.getChipId());

	SSDP.setName(deviceName);
	SSDP.setSerialNumber(String(ESP.getChipId()));
	SSDP.setModelName(F("IOESPT-Display"));
	SSDP.setModelNumber("0.0.1");

	SSDP.setModelURL(F("https://github.com/mikehole/internet-of-esp-things"));
	SSDP.setManufacturerURL(F("https://github.com/mikehole/internet-of-esp-things"));

	SSDP.setURL(F("index.html"));
	SSDP.setSchemaURL(F("description.xml"));
	
	return SSDP.begin();
}


void setup() {
	LCDInit(); //Init the LCD

	LCDClear();
	LCDBitmap(DisplayDevice); // display icon
	delay(2000);

	WiFi.begin(ssid, password);
	
	// Wait for connection
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		LCDClear();
		delay(500);
		gotoXY(0, 0);
		LCDString("Connecting..");
	}

	HTTP.on("/index.html", HTTP_GET, []() {
		HTTP.send(200, "text/plain", "Hello World!");
	});
	HTTP.on("/description.xml", HTTP_GET, []() {
		SSDP.schema(HTTP.client());
	});

	HTTP.on("/setbitmap", HTTP_PUT, []() {
		HTTP.arg("plain").toCharArray(DisplayDevice, sizeof(DisplayDevice)+1);
		LCDClear();
		LCDBitmap(DisplayDevice);
		HTTP.send(200);
	});

	HTTP.on("/setcontrast", HTTP_PUT, []() {
		LCDWrite(LCD_COMMAND, 0x21); //Tell LCD that extended commands follow
		LCDWrite(LCD_COMMAND, HTTP.arg("plain").charAt(0)); //Set LCD Vop (Contrast): Try 0xB1(good @ 3.3V) or 0xBF if your display is too dark
		LCDWrite(LCD_COMMAND, 0x04); //Set Temp coefficent
		LCDWrite(LCD_COMMAND, 0x14); //LCD bias mode 1:48: Try 0x13 or 0x14
		HTTP.send(200);
	});

	HTTP.begin();

	SetupSSDP();
}

void loop() {
	
	HTTP.handleClient();
	delay(1);
}
