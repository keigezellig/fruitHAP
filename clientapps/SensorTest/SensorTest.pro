#-------------------------------------------------
#
# Project created by QtCreator 2015-09-21T20:31:29
#
#-------------------------------------------------

QT       += core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = SensorTest
TEMPLATE = app
CONFIG += c++11


SOURCES += main.cpp\
        mainwindow.cpp \
    switch/switch.cpp \
    switch/switch_control.cpp

HEADERS  += mainwindow.h \
    switch/switch_control.h \
    switch/switch.h

FORMS    += mainwindow.ui
