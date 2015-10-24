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
#include "widgets/qcamerawidget.h"

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT
    const QString DOORCAMERA_NAME = QString("IpCamMotionDetect");
    const QString ALARMSWITCH_NAME = QString("RedLight");
    const QString APROVESWITCH_NAME = QString("BlueLight");

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

    QCameraWidget* m_doorCameraWidget;

    void loadSwitchboard();
    void loadCameraView();
    QFruitHapSensor* getSensorByName(const QString& name) const;
    void loadFaceDetectionSettings();

    void delay(int seconds);
private slots:
    void loadSensors();    
    void initDoorSetup();
    void on_actionConnect_triggered();
    void on_actionDisconnect_triggered();    
    void on_btnApprove_clicked();
    void on_btnNotApprove_clicked();
    void on_btnReset_clicked();

    void connectToMQ(const QStringList &bindingKeys, const QString &uri);
    void onConnected(const QString uri);
    void onSensorListLoaded();

    void onErrorReceived(const QString name, const QString errorMessage);
    void onDisconnected();    
    void onFaceDetected(const QString name, const QByteArray imageData, const QDateTime timestamp);
    void onDoorInitialize();
    void onDoorImageWithFaceIsAvailable(const QByteArray image);
    void onDoorAccessGranted();
    void onDoorAccessDenied();
    void onDoorNoAnswer();

};

#endif // MAINWINDOW_H
