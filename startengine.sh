#!/bin/bash
sudo service supervisord stop
cd Engine/FruitHAP.Startup/bin/Debug/
mono FruitHAP.Startup.exe

