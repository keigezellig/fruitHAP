#-------------------------------------------------
#
# Project created by QtCreator 2015-09-21T20:31:29
#
#-------------------------------------------------

QT       += core gui network concurrent

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = SensorTest
TEMPLATE = app
CONFIG += c++11



SOURCES += \
    configuration/qconfigurationcontrol.cpp \
    configuration/sensordata.cpp \
    libs/qamqp/source/qamqpauthenticator.cpp \
    libs/qamqp/source/qamqpchannel.cpp \
    libs/qamqp/source/qamqpchannelhash.cpp \
    libs/qamqp/source/qamqpclient.cpp \
    libs/qamqp/source/qamqpexchange.cpp \
    libs/qamqp/source/qamqpframe.cpp \
    libs/qamqp/source/qamqpmessage.cpp \
    libs/qamqp/source/qamqpqueue.cpp \
    libs/qamqp/source/qamqptable.cpp \
    mqclient/qfruithapclient.cpp \
    sensor/camera/qeventedcamera.cpp \
    sensor/switch/qeventedswitch.cpp \
    sensor/qeventedsensor.cpp \
    main.cpp \
    mainwindow.cpp


HEADERS  += \
    configuration/qconfigurationcontrol.h \
    configuration/sensordata.h \
    libs/qamqp/source/qamqpauthenticator.h \
    libs/qamqp/source/qamqpchannel.h \
    libs/qamqp/source/qamqpchannel_p.h \
    libs/qamqp/source/qamqpchannelhash_p.h \
    libs/qamqp/source/qamqpclient.h \
    libs/qamqp/source/qamqpclient_p.h \
    libs/qamqp/source/qamqpexchange.h \
    libs/qamqp/source/qamqpexchange_p.h \
    libs/qamqp/source/qamqpframe_p.h \
    libs/qamqp/source/qamqpglobal.h \
    libs/qamqp/source/qamqpmessage.h \
    libs/qamqp/source/qamqpmessage_p.h \
    libs/qamqp/source/qamqpqueue.h \
    libs/qamqp/source/qamqpqueue_p.h \
    libs/qamqp/source/qamqptable.h \
    mqclient/qfruithapclient.h \
    sensor/camera/qeventedcamera.h \
    sensor/switch/qeventedswitch.h \    
    sensor/qeventedsensor.h \
    mainwindow.h \
    sensor/switch/definitions.h

FORMS += \
    mainwindow.ui
