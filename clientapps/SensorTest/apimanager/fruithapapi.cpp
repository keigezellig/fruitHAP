#include "fruithapapi.h"


FruitHapApi::FruitHapApi(const QString baseUrl, QObject *parent) : QObject(parent), m_networkManager(this), m_baseUrl(baseUrl)
{
    QObject::connect(&m_networkManager, &QNetworkAccessManager::finished, this, &FruitHapApi::networkResponseReceived);
}

void FruitHapApi::requestConfiguration()
{
    qDebug() << "Sending CONFIG request";
    QUrl url(m_baseUrl + CONFIG_ENDPOINT + "sensors");
    QNetworkRequest request(url);
    m_networkManager.get(request);

}

void FruitHapApi::requestSensorOperation(const QString &sensorName, const QString &operation)
{
    qDebug() << "Sending SENSOR request";
    QUrl url(m_baseUrl + SENSOR_ENDPOINT + sensorName + "/" + operation);
    QNetworkRequest request(url);
    m_networkManager.get(request);
}

void FruitHapApi::networkResponseReceived(QNetworkReply *reply)
{
    if (reply->error() == QNetworkReply::NoError){
        QByteArray response(reply->readAll());
        QJsonDocument decodedMessage = QJsonDocument::fromJson(response);

        if (reply->request().url().toString().contains(CONFIG_ENDPOINT)) {
             emit configResponseReceived(decodedMessage);
             return;
         }
         else if (reply->request().url().toString().contains(SENSOR_ENDPOINT)) {
            emit sensorResponseReceived(decodedMessage);
         }
    }
     else {
         qWarning() <<"ErrorNo: "<< reply->error() << "for url: " << reply->url().toString();
         qDebug() << "Request failed, " << reply->errorString();
         qDebug() << "Headers:"<<  reply->rawHeaderList()<< "content:" << reply->readAll();
         emit networkErrorOccured(reply->errorString());
     }

}
