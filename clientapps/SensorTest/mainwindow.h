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
    void on_btnGetSwitchList_clicked();
    void on_btnOn_clicked();
    void on_btnOff_clicked();
    void on_cmbSwitchList_currentIndexChanged(int index);
    void on_btnGetState_clicked();
    void onSwitchStateReceived(const QString name, SwitchState state);

};

#endif // MAINWINDOW_H
