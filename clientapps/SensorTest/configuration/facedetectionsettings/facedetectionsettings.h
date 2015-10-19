#ifndef FACEDETECTIONSETTINGS_H
#define FACEDETECTIONSETTINGS_H

#include <QString>

class FaceDetectionSettings
{
private:
    QString m_cameraName;
    QString m_switchName;
    bool m_faceDetection;
    bool m_faceRecognition;
public:
    FaceDetectionSettings();
    FaceDetectionSettings(const QString &cameraName, const QString &switchName, const bool faceDetectionEnabled, const bool faceRecognitionEnabled);
    void setCameraName(const QString &name);
    void setSwitchName(const QString &switchName);
    void setFaceDetectionEnabled(const bool isEnabled);
    void setFaceRecognitionEnabled(const bool isEnabled);

    QString getCameraName() const;
    QString getSwitchName() const;
    bool getFaceDetectionEnabled() const;
    bool getFaceRecognitionEnabled() const;
};

#endif // FACEDETECTIONSETTINGS_H
