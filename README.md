# FruitHAP (Fruit Home Automation Platform) #

A Home Automation Platform designed to run on a Raspberry Pi with extensibility in mind.

### What is this all about? ###

This first version (1.0) contains a sample setup which is used in my home to control the front doorbell and contains the following components:

* The Engine (the main component)
* A module for the KlikAanKlikUit (KaKu) system (which is a 433 Mhz based X10 derivative system)
* A module for an IP Camera
* Support for the following sensor types: Button, Camera
* Sample sensor configuration that defines one ip camera and one button (which are placed above my front door)
* Sample action that sends an alert to the message queue when the button is pressed (when someone is at the door) containing a snapshot of the camera
* A notifier script that monitors the message queue for alerts and send it through Pushbullet to another device

For more info about modules, sensors, actions and notifiers and general architecture see the Wiki

### How do I get set up? ###

To get up and running with my sample setup, you need the following:

* A Raspberry PI (i use a Model B+, it should also work on other models, but i haven't tested it)
* A 433 Mhz receiver (i use the RFXtrx433E - see [here](http://www.rfxcom.com) )
* A KaKu doorbell set (I use [this one](http://www.klikaanklikuit.nl/shop/nl/producten-1/draadloze-deurbellen/draadloze-deurbel-acdb-7000ac-1/) (unfortunately the site is only in Dutch), which consists of a button and an actual doorbell)
* Raspbian image with the FruitHAP software (See Downloads section)

TODO: setup intructions




### How to setup a development environment ###

TODO