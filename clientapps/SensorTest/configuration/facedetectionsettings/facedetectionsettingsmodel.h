#ifndef FACEDETECTIONSETTINGSMODEL_H
#define FACEDETECTIONSETTINGSMODEL_H

#include <QAbstractTableModel>
#include "facedetectionsettings.h"

class FaceDetectionSettingsModel
{   
public:    
    void LoadSettingsFromFile(const QString& filename, QList<FaceDetectionSetting> &settings) const;
    void SaveSettingsToFile(const QString& filename, const QList<FaceDetectionSetting> &settings);
};

#endif // FACEDETECTIONSETTINGSMODEL_H
