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


INCLUDEPATH += /usr/local/include/opencv
LIBS += -L/usr/local/lib -lopencv_core -lopencv_imgcodecs -lopencv_imgproc -lopencv_highgui -lopencv_objdetect

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
    main.cpp \
    mainwindow.cpp \
    sensor/camera/qcamera.cpp \
    sensor/switch/qswitch.cpp \
    sensor/qfruithapsensor.cpp \
    widgets/qswitchwidget.cpp \
    widgets/qcamerawidget.cpp \
    faceverifier/qopencvfaceverifier.cpp \
    faceverifier/faceverifier.cpp \
    configuration/facedetectionsettings/facedetectionsettings.cpp


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
    mainwindow.h \
    sensor/switch/definitions.h \
    sensor/switch/qswitch.h \
    sensor/camera/qcamera.h \
    sensor/qfruithapsensor.h \
    widgets/qswitchwidget.h \
    widgets/qcamerawidget.h \
    faceverifier/qopencvfaceverifier.h \
    faceverifier/faceverifier.h \
    configuration/facedetectionsettings/facedetectionsettings.h

FORMS += \
    mainwindow.ui \
    widgets/qswitchwidget.ui \
    widgets/qcamerawidget.ui
