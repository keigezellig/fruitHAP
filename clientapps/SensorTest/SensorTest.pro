#-------------------------------------------------
#
# Project created by QtCreator 2015-09-21T20:31:29
#
#-------------------------------------------------

QT       += core gui network

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = SensorTest
TEMPLATE = app
CONFIG += c++11



SOURCES += main.cpp\
        mainwindow.cpp \
    switch/switch.cpp \
    switch/switch_control.cpp \
    qfruithapclient.cpp \
    libs/qamqp/source/qamqpauthenticator.cpp \
    libs/qamqp/source/qamqpchannel.cpp \
    libs/qamqp/source/qamqpchannelhash.cpp \
    libs/qamqp/source/qamqpclient.cpp \
    libs/qamqp/source/qamqpexchange.cpp \
    libs/qamqp/source/qamqpframe.cpp \
    libs/qamqp/source/qamqpmessage.cpp \
    libs/qamqp/source/qamqpqueue.cpp \
    libs/qamqp/source/qamqptable.cpp \
    switch/qswitchcontrol.cpp


HEADERS  += mainwindow.h \
    switch/switch_control.h \
    switch/switch.h \
    qfruithapclient.h \    
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
    switch/qswitchcontrol.h


FORMS    += mainwindow.ui

