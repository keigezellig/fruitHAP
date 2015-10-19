#include "facedetectionsettings.h"

FaceDetectionSettings::FaceDetectionSettings()
{

}

FaceDetectionSettings::FaceDetectionSettings(const QString &cameraName, const QString &switchName, const bool faceDetectionEnabled, const bool faceRecognitionEnabled):
    m_cameraName(cameraName),
    m_switchName(switchName),
    m_faceDetection(faceDetectionEnabled),
    m_faceRecognition(faceRecognitionEnabled)

{

}

void FaceDetectionSettings::setCameraName(const QString &name)
{

}

void FaceDetectionSettings::setSwitchName(const QString &switchName)
{

}

void FaceDetectionSettings::setFaceDetectionEnabled(const bool isEnabled)
{

}

void FaceDetectionSettings::setFaceRecognitionEnabled(const bool isEnabled)
{

}

QString FaceDetectionSettings::getCameraName() const
{

}

QString FaceDetectionSettings::getSwitchName() const
{

}

bool FaceDetectionSettings::getFaceDetectionEnabled() const
{

}

bool FaceDetectionSettings::getFaceRecognitionEnabled() const
{

}

