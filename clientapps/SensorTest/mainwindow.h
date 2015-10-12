#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QGraphicsScene>
#include <QTimer>
#include <QLabel>

#include "configuration/qconfigurationcontrol.h"
#include "sensor/qfruithapsensor.h"
#include "sensor/switch/definitions.h"

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
    QConfigurationControl m_configControl;
    QString m_uri;
    QList<QFruitHapSensor*> m_eventedSensors;    
    //QFruitHapSensor* getSensorByName(const QString name) const;
    QWidget* m_switchBoard;
    void loadSwitchboard();
    void loadCameraView();
private slots:
    void loadSensors();    
    void on_actionConnect_triggered();
    void on_actionDisconnect_triggered();    
    void connectToMQ(const QStringList &bindingKeys, const QString &uri);
    void onConnected(const QString uri);
    void onSensorListReceived(const QList<SensorData> list);
    void onErrorReceived(const QString name, const QString errorMessage);
    void onDisconnected();    
};

#endif // MAINWINDOW_H
