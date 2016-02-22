// IoesptPersistence.h

#ifndef _IOESPTPERSISTENCE_h
#define _IOESPTPERSISTENCE_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <ArduinoJson.h>
#include <EEPROM.h>

const int BufferLen = 2046;  // max eprom length, minus 2 for data length

typedef void(*SaveCallbackType)(JsonObject&);
typedef void(*LoadCallbackType)(JsonObject&);

class IoesptPersistance
{
public:

	IoesptPersistance();

	void saveSettings(SaveCallbackType callback);
	void loadSettings(LoadCallbackType callback);

private:

	char buffer[BufferLen];
	
	int length;

	//Diagnostics
	template <typename Generic>
	void DEBUG_WMSL(Generic text);

	template <typename Generic>
	void DEBUG_WMS(Generic text);

	template <typename Generic>
	void DEBUG_WMC(Generic text);

	template <typename Generic>
	void DEBUG_WMF(Generic text);
};


#endif

