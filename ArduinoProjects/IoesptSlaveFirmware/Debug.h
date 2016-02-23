// Debug.h

#ifndef _DEBUG_h
#define _DEBUG_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class Dbg
{
public:
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

