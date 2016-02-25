Please note this code is not ready for use. 

# Internet of ESP Things

The main aim of this repository is to simplify the process of connecting Espressif ESP chips/modules to the main cloud based IoT infrastructures. 

This will be achieved by taking advantage of a set of the best ideas and implementations available for the Espressif ESP modules. Currently the only publicly available chip is the ESP8266. More information found here:  

https://en.wikipedia.org/wiki/ESP8266

This project will take advantage of the community base Arduino development environment support that is available for the ESP8266:

https://github.com/esp8266/Arduino

##Firmware Packages##
The firmware will be delivered as two main packages that can fulfil the requirements of the beginner or expert user.  

###Slave###
The slave firmware is for when the module is used by a connected MCU that communicates via the serial port to the chip. The idea behind this package is that the pre compiled binary can be flashed to the chip without any compilation or configuration changes.     

###Stand Alone###
This package is to allow developers to extend the core functionality to include any custom functionality that falls outside of the given libraries. This could include the interfacing with sensors that are connected to the modules GPIO. 

##Libraries##

Due to Arduino project structures the libraries have to be placed within their own repository that can be found here:

[https://github.com/mikehole/ioespt-libraries](https://github.com/mikehole/ioespt-libraries "ioespt-libraries")

This solution will rely on a core set of libraries that will provide the following functionality:  

####Set-up and Provisioning:####
Normally one of the first things you need to do with an ESP based device is connect to a WiFi network this can be achieved by hard coding the access point into the binary that is uploaded to the chip. There are however much neater solutions that allow the module to provide an out of the box method of connection.

Here is a quick video demonstration:

[![Video](https://i.ytimg.com/vi/nMpJDldm9oA/1.jpg?time=1456394675842)](https://youtu.be/nMpJDldm9oA)

These solutions check for given access point setting held on the device persistent storage and if none are found then it offers up a temporary access point so that the user can connect and provide the desired access point settings. A good example of this can be found here: https://github.com/tzapu/WiFiManager 

####Azure: 
A library allowing the publishing of data to the Azure IoT hub. This will be based around work that has been carried out by Dave Glover that can be found here: https://github.com/gloveboxes/Arduino-ESP8266-Secure-Azure-IoT-Hub-Client

####Persistence: 
Used to save configuration data to the devices EPROM.


####MQTT:
To allow event publishing and device control via the MQTT machine to machine device protocol. There are a few libraries available on git hub. Once a suitable candidate is found full details will be provided.

####Others:
Any other cloud based provider yet to be identified as a worthwhile addition.

##MCU Master Libraries##

###Python###

###C# ###

##Applications##



