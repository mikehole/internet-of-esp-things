///////////////////////////////
// NOTE WORK IN PROGRESS

#include <OneWire.h>
#include "DallasTemperature\DallasTemperature.h"

#define ONE_WIRE_BUS 2  // DS18B20 pin

OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature DS18B20(&oneWire);

void setup()
{
	Serial.begin(115200);
	Serial.println(""); //Lets move away from the ugly stuff that gets sent on boot
	Serial.println("*IOESP-Slave Firmware Start - Hello world.");

}

void loop()
{
	float temp;

	do {
		DS18B20.requestTemperatures();
		temp = DS18B20.getTempCByIndex(0);
		Serial.print("Temperature: ");
		Serial.println(temp);
	} while (temp == 85.0 || temp == (-127.0));
}
