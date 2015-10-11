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
    QGraphicsScene *m_drawing;
    QTimer *m_timer;
    QLabel *m_statusBarLabel;
    QFruitHapClient *m_client;
    QConfigurationControl m_configControl;
    QString m_uri;
    QList<QFruitHapSensor*> m_eventedSensors;
    QFruitHapSensor* m_selectedSensor;
    QFruitHapSensor* getSensorByName(const QString name) const;
    QWidget* m_switchBoard;
    void loadSwitchboard();
private slots:
    void loadSensors();
    void on_btnOn_clicked();
    void on_btnOff_clicked();
    void on_cmbSensorList_currentIndexChanged(int index);
    void on_actionConnect_triggered();
    void on_actionDisconnect_triggered();
    void on_dialRefreshrate_valueChanged(int value);
    void on_btnGetValue_clicked();

    void connectToMQ(const QStringList &bindingKeys, const QString &uri);
    void onConnected();
    void onSensorListReceived(const QList<SensorData> list);
    void onSwitchStateReceived(const QString name, SwitchState state);
    void onErrorReceived(const QString name, const QString errorMessage);
    void onImageDataReceived(const QString name, const QByteArray imageData);
    void onDisconnected();
    void updateImage(QFruitHapSensor *cameraSensor);
};

#endif // MAINWINDOW_H
