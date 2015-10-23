#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QGraphicsScene>
#include <QTimer>
#include <QLabel>

#include "configuration/qconfigurationcontrol.h"
#include "sensor/qfruithapsensor.h"
#include "sensor/switch/definitions.h"
#include "faceverifier/faceverifier.h"
#include "statemachine/door.h"

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT
    const QString DOORCAMERA_NAME = QString("DoorCamera");
    const QString ALARMSWITCH_NAME = QString("RedLamp");
    const QString APROVESWITCH_NAME = QString("BlueLamp");

public:
    explicit MainWindow(QWidget *parent = 0);
    ~MainWindow();

private:
    Ui::MainWindow *ui;
    QLabel *m_statusBarLabel;
    QFruitHapClient *m_client;
    FaceVerifier *m_faceVerifier;
    QConfigurationControl m_configControl;
    QString m_uri;    
    QWidget* m_switchBoard;
    Door* m_door;
    void loadSwitchboard();
    void loadCameraView();
    QFruitHapSensor* getSensorByName(const QString& name) const;
    void loadFaceDetectionSettings();

private slots:
    void loadSensors();    
    void initDoorSetup();
    void on_actionConnect_triggered();
    void on_actionDisconnect_triggered();    
    void connectToMQ(const QStringList &bindingKeys, const QString &uri);
    void onConnected(const QString uri);
    void onSensorListLoaded();

    void onErrorReceived(const QString name, const QString errorMessage);
    void onDisconnected();    
    void onFaceDetected(const QString name, const QByteArray imageData, const QDateTime timestamp);
};

#endif // MAINWINDOW_H
