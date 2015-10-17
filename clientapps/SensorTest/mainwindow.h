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

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT

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
    QList<QFruitHapSensor*> m_eventedSensors;    
    QWidget* m_switchBoard;
    void loadSwitchboard();
    void loadCameraView();
    QFruitHapSensor* getSensorByName(const QString& name) const;
private slots:
    void loadSensors();    
    void on_actionConnect_triggered();
    void on_actionDisconnect_triggered();    
    void connectToMQ(const QStringList &bindingKeys, const QString &uri);
    void onConnected(const QString uri);
    void onSensorListReceived(const QList<SensorData> list);
    void onErrorReceived(const QString name, const QString errorMessage);
    void onDisconnected();    
    void onFaceDetected(const QString name, const QDateTime timestamp);
};

#endif // MAINWINDOW_H
