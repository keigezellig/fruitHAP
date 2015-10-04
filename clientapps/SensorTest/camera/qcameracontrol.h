#ifndef QCAMERACONTROL_H
#define QCAMERACONTROL_H

#include <QObject>
#include <QByteArray>
#include <QTimer>
#include "qfruithapclient.h"

class QCameraControl : public QObject
{
    Q_OBJECT
public:
    QCameraControl(QString category, QFruitHapClient &client, QObject *parent = 0);
    void getImage (const QString &name);

private:
    bool m_isBusy;
    QString m_currentName;
    QString m_category;
    QFruitHapClient &m_client;
    QTimer *m_requestTimer;
    void handleSensorMessage(QJsonObject responseObject);
signals:
    void imageDataReceived(const QString name, const QByteArray imageData);
private slots:
    void onClientResponseReceived(const QJsonDocument response, const QString messageType);
    void requestTimeout();
};

#endif // QCAMERACONTROL_H
