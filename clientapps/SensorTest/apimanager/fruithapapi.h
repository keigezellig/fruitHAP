#ifndef FRUITHAPAPI_H
#define FRUITHAPAPI_H

#include <QObject>
#include <QtNetwork>

class FruitHapApi : public QObject
{
    Q_OBJECT

private:
    QNetworkAccessManager m_networkManager;
    QString m_baseUrl;
    QString CONFIG_ENDPOINT = "api/configuration/";
    QString SENSOR_ENDPOINT = "api/sensor/";

public:
    FruitHapApi(const QString baseUrl, QObject *parent = nullptr);
    void requestConfiguration();
    void requestSensorOperation(const QString &sensorName, const QString &operation);

signals:
    void configResponseReceived(const QJsonDocument configResponse);
    void sensorResponseReceived(const QJsonDocument sensorResponse);
    void networkErrorOccured(const QString errorMessage);

private slots:
    void networkResponseReceived(QNetworkReply *reply);

};

#endif // FRUITHAPAPI_H
