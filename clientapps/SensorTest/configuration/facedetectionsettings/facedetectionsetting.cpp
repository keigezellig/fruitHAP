#include "facedetectionsettings.h"

FaceDetectionSetting::FaceDetectionSetting()
{

}

FaceDetectionSetting::FaceDetectionSetting(const QString cameraName, const QString switchName, const bool faceDetectionEnabled, const bool faceRecognitionEnabled):
    m_cameraName(cameraName),
    m_switchName(switchName),
    m_faceDetection(faceDetectionEnabled),
    m_faceRecognition(faceRecognitionEnabled)

{

}

void FaceDetectionSetting::setCameraName(const QString &name)
{
   m_cameraName = name;
}

void FaceDetectionSetting::setSwitchName(const QString &switchName)
{
    m_switchName = switchName;
}

void FaceDetectionSetting::setFaceDetectionEnabled(const bool isEnabled)
{
    m_faceDetection = isEnabled;
}

void FaceDetectionSetting::setFaceRecognitionEnabled(const bool isEnabled)
{
    m_faceRecognition = isEnabled;
}

QString FaceDetectionSetting::getCameraName() const
{
    return m_cameraName;
}

QString FaceDetectionSetting::getSwitchName() const
{
    return m_switchName;
}

bool FaceDetectionSetting::isFaceDetectionEnabled() const
{
    return m_faceDetection;
}

bool FaceDetectionSetting::getFaceRecognitionEnabled() const
{
    return m_faceRecognition;
}

