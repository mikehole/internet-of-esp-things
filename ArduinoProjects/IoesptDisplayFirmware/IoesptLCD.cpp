// 
// 
// 

#include "IoesptLCD.h"

void gotoXY(int x, int y) {
	LCDWrite(0, 0x80 | x);  // Column  x - range: 0 to 84
	LCDWrite(0, 0x40 | y);  // Row     y - range: 0 to 5
}

//This takes a large array of bits and sends them to the LCD
void LCDBitmap(char my_array[]) {
	for (int index = 0; index < (LCD_X * LCD_Y / 8); index++)
		LCDWrite(LCD_DATA, my_array[index]);
}

//This function takes in a character, looks it up in the font table/array
//And writes it to the screen
//Each character is 8 bits tall and 5 bits wide. We pad one blank column of
//pixels on each side of the character for readability.
void LCDCharacter(char character) {
	LCDWrite(LCD_DATA, 0x00); //Blank vertical line padding

	for (int index = 0; index < 5; index++)
		LCDWrite(LCD_DATA, ASCII[character - 0x20][index]);
	//0x20 is the ASCII character for Space (' '). The font table starts with this character

	LCDWrite(LCD_DATA, 0x00); //Blank vertical line padding
}

//Given a string of characters, one by one is passed to the LCD
void LCDString(char *characters) {
	while (*characters)
		LCDCharacter(*characters++);
}

//Clears the LCD by writing zeros to the entire screen
void LCDClear(void) {
	for (int index = 0; index < (LCD_X * LCD_Y / 8); index++)
		LCDWrite(LCD_DATA, 0x00);

	gotoXY(0, 0); //After we clear the display, return to the home position
}

//This sends the magical commands to the PCD8544
void LCDInit(void) {

	//Configure control pins
	pinMode(PIN_SCE, OUTPUT);
	pinMode(PIN_RESET, OUTPUT);
	pinMode(PIN_DC, OUTPUT);
	pinMode(PIN_SDIN, OUTPUT);
	pinMode(PIN_SCLK, OUTPUT);

	//Reset the LCD to a known state
	digitalWrite(PIN_RESET, LOW);
	digitalWrite(PIN_RESET, HIGH);

	LCDWrite(LCD_COMMAND, 0x21); //Tell LCD that extended commands follow
	LCDWrite(LCD_COMMAND, 0xBB); //Set LCD Vop (Contrast): Try 0xB1(good @ 3.3V) or 0xBF if your display is too dark
	LCDWrite(LCD_COMMAND, 0x04); //Set Temp coefficent
	LCDWrite(LCD_COMMAND, 0x14); //LCD bias mode 1:48: Try 0x13 or 0x14

	LCDWrite(LCD_COMMAND, 0x20); //We must send 0x20 before modifying the display control mode
	LCDWrite(LCD_COMMAND, 0x0C); //Set display control, normal mode. 0x0D for inverse
}

//There are two memory banks in the LCD, data/RAM and commands. This 
//function sets the DC pin high or low depending, and then sends
//the data byte
void LCDWrite(byte data_or_command, byte data) {
	digitalWrite(PIN_DC, data_or_command); //Tell the LCD that we are writing either to data or a command

										   //Send the data
	digitalWrite(PIN_SCE, LOW);
	shiftOut(PIN_SDIN, PIN_SCLK, MSBFIRST, data);
	digitalWrite(PIN_SCE, HIGH);
}

