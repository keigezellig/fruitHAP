#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QGraphicsScene>
#include <QTimer>
#include "switch/qswitchcontrol.h"
#include "camera/qcameracontrol.h"
#include "qconfigurationcontrol.h"


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
    QFruitHapClient m_client;
    QSwitchControl m_switchControl;
    QCameraControl m_cameraControl;
    QConfigurationControl m_configControl;

private slots:
    void loadSensors();
    void on_btnOn_clicked();
    void on_btnOff_clicked();
    void on_cmbSwitchList_currentIndexChanged(int index);
    void on_cmbCameraList_currentIndexChanged(int index);
    void on_actionConnect_triggered();
    void on_actionDisconnect_triggered();
    void on_dialRefreshrate_valueChanged(int value);

    void connectToMQ(const QStringList &bindingKeys, const QString &uri);
    void onConnected();
    void onSensorListReceived(const QList<SensorData> list);
    void onSwitchStateReceived(const QString name, SwitchState state);
    void onImageDataReceived(const QString name, const QByteArray imageData);
    void onDisconnected();
    void onRpcQueueReady();
    void updateImage();
};

#endif // MAINWINDOW_H
