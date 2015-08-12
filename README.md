# FruitHAP (Fruit Home Automation Platform) #

A Home Automation Platform designed to run on a Raspberry Pi with extensibility in mind.

### What is this all about? ###

This version contains a sample setup which is used in my home to control the front doorbell and contains the following components:

* The Engine (the main component)
* A controller for the KlikAanKlikUit (KaKu) system (which is a 433 Mhz based X10 derivative system)
* A contoller for an IP Camera
* Support for the following sensor types: Button, Camera, Switch, Button+Camera, Switch+Camera
* A notifier script that monitors the message queue for alerts and send it through Pushbullet to another device (pushbullet-notifier)

For more in depth info about controllers, sensors, actions and general see my blog

### Quick setup ###

Needed stuff:

* A Raspberry PI (i use a Model B+, it should also work on other models, but i haven't tested it) 
* A network connection for the Raspberry PI (either wired or wireless, I use a wireless dongle from Edimax)
* A serial (USB) 433 Mhz receiver (i use the RFXtrx433E - see [here](http://www.rfxcom.com) )
* A KaKu doorbell set (I use [this one](http://www.klikaanklikuit.nl/shop/nl/producten-1/draadloze-deurbellen/draadloze-deurbel-acdb-7000ac-1/) (unfortunately the site is only in Dutch), which consists of a button and an actual doorbell)
* An IP Camera (I use the Elro C903IP) which is connected to your network
* Raspbian image with the FruitHAP software (See Downloads section)
* A Pushbullet account (see [here](https://www.pushbullet.com/) )
* (Preferably) A PC with a flash card reader in the same network as the RPi

Preparation:

* Download and unzip the latest FruitHAP image (see [here](http://joosten-industries.nl/files/fruithap_images/)) to a temporary directory
* Put the image onto a flash card (i used one with 8GB) as follows (on linux)

  `dd bs=4M if=<path/to/raspbian_image> of=/dev/<flashcarddrive>`
  
* Put the flash card in the RPi
* Connect the 433 Mhz receiver to an USB port of the RPi
* Connect a screen, keyboard, power and depending of your setup a network cable to th RPi
* After boot login with standard username `pi` and standard password `raspberry`
* Note your ip-address by executing `ifconfig`
* Optionally you can now disconnect the screen and keyboard, since you can do all the rest of the configuration through an SSH connection from your PC.

Sensor configuration:

FruitHAP configuration resides in a couple of configuration files. 
First you have to configure your sensors (i.e your doorbell and your ip camera).
For your KaKu doorbell you have to find out which Device Id, Unit code and Command your **BUTTON** generates. If you use a RFXtrx433E like I do, you can use the RFXmngr program which comes with the device.
Just connect your RFX to a USB port of your pc and run the program. Make sure the 'AC' checkbox is checked before connecting to the device (see the RFX user guide for more information for the right settings)
When connected to the device, press you doorbell button once and the RFXmngr program shows the device id, unit code and command of the button.

Then (on the RPi):

* Open the sensor configuration file with your favorite text editor (its location is `/home/pi/fruithap/engine/sensors.json`)
* The first line (with `[{Name: "Doorbell"`) is the configuration for the KaKu doorbell itself
  Fill in the correct DeviceId (in hex), UnitCode (in hex) and Command (in decimal) as found above (keep the double quotes)
* The second line is the configuration of the ip camera
  Fill in the URL of your camera that returns a **single** image and optionally a username and password. (also keep the double quotes)
* Save the file

Pushbullet notifier configuration:

* Open the Pushbullet notifier configuration file with your favorite text editor (its location is `/home/pi/fruithap/pushbullet-notifier/config.json`)
* In the `pushbullet` section enter your Pushbullet API key (this can be found on your Pushbullet account page), an optional channel (which you can create through your Pushbullet account as well) to which followers can subscribe, 
  an optional title and text for your message that is shown in your notification.

Final steps:

* Reboot your RPi
* Login to your RPi
* At the prompt type `sudo service fruithap-pushbullet-notifier start` and press enter
  This is actually a workaround for a bug that the notifier service doesn't get started at boot (help needed). Everytime when you reboot the RPi you have to do this step
* Install Pushbullet to your phone, browser etc and watch the magic happen when someons rings your doorbell :)

Good luck!
  

















### How to setup a development environment ###

TODO