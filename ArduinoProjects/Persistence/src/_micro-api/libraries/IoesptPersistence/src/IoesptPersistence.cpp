// 
// 
// 

#include "IoesptPersistence.h"

IoesptPersistance::IoesptPersistance()
{
}

void IoesptPersistance::loadSettings(LoadCallbackType callback)
{
	StaticJsonBuffer<1000> jsonBuffer;

	length = word(EEPROM.read(0), EEPROM.read(1));

	int address = 2;

	for (address = 2; address < length + 2; address++) {
		buffer[address - 2] = EEPROM.read(address);
	}

	buffer[address - 2] = '\0';
	
	Serial.println(buffer);

	JsonObject& root = jsonBuffer.parseObject(buffer);

	callback(root);
}

void IoesptPersistance::saveSettings(SaveCallbackType callback)
{
	StaticJsonBuffer<1000> jsonBuffer;

	JsonObject& root = jsonBuffer.createObject();
	
	callback(root);

	root.prettyPrintTo(Serial);
	
	EEPROM.begin(BufferLen+2);

	length = root.printTo(buffer, BufferLen);
	
	root.prettyPrintTo(Serial);

	EEPROM.write(0, highByte(length));
	EEPROM.write(1, lowByte(length));
	for (int address = 2; address < length + 2; address++) {
		EEPROM.write(address, buffer[address - 2]);
	}
	EEPROM.commit();
}

///////////////////////////
// Diagnostics

template <typename Generic>
void IoesptPersistance::DEBUG_WMSL(Generic text) {
	Serial.print("*IOESPT-Settings: ");
	Serial.println(text);
}

template <typename Generic>
void IoesptPersistance::DEBUG_WMS(Generic text) {
	Serial.print("*IOESPT-Settings: ");
	Serial.print(text);
}

template <typename Generic>
void IoesptPersistance::DEBUG_WMC(Generic text) {
	Serial.print(text);
}

template <typename Generic>
void IoesptPersistance::DEBUG_WMF(Generic text) {
	Serial.println(text);
}
