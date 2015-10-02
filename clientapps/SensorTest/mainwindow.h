#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include "switch/qswitchcontrol.h"

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
    QFruitHapClient m_client;
    QSwitchControl m_switchControl;


private slots:
    void loadSwitches();
    void on_btnOn_clicked();
    void on_btnOff_clicked();
    void on_cmbSwitchList_currentIndexChanged(int index);
    void onSwitchStateReceived(const QString name, SwitchState state);
    void onSwitchListReceived(const QStringList list);
    void on_actionConnect_triggered();

    void connectToMQ(const QStringList &bindingKeys, const QString &uri);
    void onConnected();
    void on_actionDisconnect_triggered();
    void onDisconnected();
    void onRpcQueueReady();
};

#endif // MAINWINDOW_H
