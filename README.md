# FruitHAP (Fruit Home Automation Platform) #

A Home Automation Platform designed to run on a Raspberry Pi with extensibility in mind.

### Features in this version ###

### Backend ###

**Controllers**

* RfxController, support for the RFXtrx433E 433 MHz transceiver
* ImageController, support for retrieving images from a camera (IP or USB)

**Sensor types**

* Klik-aan-klik-uit: KakuButton, KakuSwitch
* Other: Camera, IP or USB
* Other: FruitTempSensor, this supports my custom made Arduino based temperature sensor (see [here](http://joosten-industries.nl/blog/2016/03/12/temperature-sensor-with-arduino/) )
* Aggregated sensors: Button+Camera, Switch+Camera, you can use this sensor type for example to send a image from a camera when a switch is flipped or a button is pressed

**Plugins**

*Core Plugins*

* EventTriggerPlugin: Updates of sensor states are sent to a specified topic in the message queue when a sensor update occurs (e.g. switch is flipped, new temperature reading is available).
* AlertNotificationPlugin: A configurable alert message is sent to a specified topic in the message when a sensor event occurs (e.g. button is pressed externally, switch is flipped externally)
* WebPlugin: The plugin that makes the control and reading of sensors/actuators available through a REST API 


*Example plugins*

* ThermostatPlugin: An example plugin to control two switches depending on the input of a temperature sensor


### Client programs ###

* FruitHapNotifier: An Android application which features a dashboard part to control the sensors and an alert part that notifies the user of alerts. This uses the message queue and the REST API 
* SensorTest: An QT C++ application that also is an example of an dashboard and control part with the extra feature of face detection with OpenCV (OpenCV installation needed) WARNING: MAY NOT WORK CORRECTLY SINCE THIS WAS ONLY A DEMO
  This isn't part of the core installation package as below, but you can still look into it to learn more about the system.

### Installation of the basic backend ###

*Needed stuff (NB this is the BASIC setup, after this you have still to configure your hardware and sensors to get it completely working)*

* A Raspberry Pi (i use a Model 2 B, it should also work on other model Bs, but i haven't tested it thouroughly) 
* A Raspbian distribution based on Jessie (to be found [here](https://downloads.raspberrypi.org) )
* A network connection for the Raspberry PI (either wired or wireless, I use a wireless dongle from Edimax)
* Latest version of [Ansible](https://www.ansible.com/) on the computer from which you do the install
* Latest version of the FruitHAP distribution (see [here](http://joosten-industries.nl/files/fruithap_dist/) )

*Installation:*

* Install the Raspbian distribution on your Raspberry PI and enable SSH access.
* On the computer with Ansible modify the /etc/ansible/hosts in such a way that the IP address or hostname of your RPi is listed. For example :
    
    [raspberrys]
    192.168.1.67

* Unzip the FruitHAP distribution on a computer where Ansible is installed on.
* Go to the directory in which you unzipped FruitHAP
* run `./deploy.sh <ip address of RPi>`
* Ansible will now take care of installing the backend system on your Pi and will start the FruitHAP service afterwards
* Now you can configure your sensors and plugins etc.

*Starting/stopping the FruitHAP service:*

The FruitHAP service is controlled by the excellent [Supervisor](http://supervisord.org/) daemon.

To start/stop/restart:

* SSH into your RPi
* Run 'sudo supervisorctl start|stop|restart fruithap
* Or: in a webbrowser go to `http://<RPI-ip>:9001` (username: `supervisor`, password `supervisor`)

## Installation of the FruitHAPNotifier Android app

In the FruitHAP distribution under `ClientApps/FruitHAPNotifier` there is an APK which you can install on any Android device which has Android 4.2 or later.


Good luck!
  

















### How to setup a development environment ###

TODO