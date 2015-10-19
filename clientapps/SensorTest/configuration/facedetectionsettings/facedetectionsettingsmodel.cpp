#include "facedetectionsettingsmodel.h"
#include <QJsonDocument>
#include <QJsonArray>
#include <QJsonObject>
#include <QFile>
#include <QDebug>



void FaceDetectionSettingsModel::LoadSettingsFromFile(const QString &filename, QList<FaceDetectionSetting> &settings) const
{
    QFile loadFile(filename);
    if (!loadFile.open(QIODevice::ReadOnly))
    {
        qWarning("Couldn't open save file.");
        return;
    }
    QByteArray contents = loadFile.readAll();
    QJsonDocument jsonDocument = QJsonDocument::fromJson(contents);
    if (jsonDocument.isNull() || jsonDocument.isEmpty())
    {
        qCritical() << "Invalid format of settings file";
    }

    settings.clear();

    QJsonArray settingsArray = jsonDocument.array();
    foreach (auto settingItem, settingsArray)
    {
        QJsonObject settingObject = settingItem.toObject();
        FaceDetectionSetting setting(
                    settingObject["CameraName"].toString(),
                    settingObject["SwitchName"].toString(),
                    settingObject["TriggerOnFaceDetection"].toBool(),
                    settingObject["TriggerOnFaceRecognition"].toBool());

        settings.append(setting);


    }

    qDebug() << "Loaded settings";
}

void FaceDetectionSettingsModel::SaveSettingsToFile(const QString &filename, const QList<FaceDetectionSetting> &settings)
{
    QJsonArray jsonArray;

    foreach (const FaceDetectionSetting &setting, settings)
    {
        QJsonObject dataObject;
        dataObject["CameraName"] = setting.getCameraName();
        dataObject["SwitchName"] = setting.getSwitchName();
        dataObject["TriggerOnFaceDetection"] = setting.getFaceDetectionEnabled();
        dataObject["TriggerOnFaceRecognition"] = setting.getFaceRecognitionEnabled();

        jsonArray.append(dataObject);
    }

    QJsonDocument jsonDocument(jsonArray);
    QFile saveFile(filename);
    if (!saveFile.open(QIODevice::WriteOnly))
    {
        qWarning("Couldn't open save file.");
        return;
    }
    saveFile.write(jsonDocument.toJson());
    saveFile.close();
    qDebug("Saved settings");


}
