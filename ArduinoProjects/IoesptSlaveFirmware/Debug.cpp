// 
// 
// 

#include "Debug.h"

template <typename Generic>
void Dbg::DEBUG_WMSL(Generic text) {
	Serial.print("*IOESPT-Slave Firmware: ");
	Serial.println(text);
}

template <typename Generic>
void Dbg::DEBUG_WMS(Generic text) {
	Serial.print("*IOESPT-Slave Firmware: ");
	Serial.print(text);
}

template <typename Generic>
void Dbg::DEBUG_WMC(Generic text) {
	Serial.print(text);
}

template <typename Generic>
void Dbg::DEBUG_WMF(Generic text) {
	Serial.println(text);
}

