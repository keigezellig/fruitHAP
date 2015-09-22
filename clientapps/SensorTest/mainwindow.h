#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include "switch/switch_control.h"

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
    SwitchControl m_switchControl;

private slots:
    void on_btnGetSwitchList_clicked();
    void on_btnOn_clicked();
    void on_btnOff_clicked();
    void on_cmbSwitchList_currentIndexChanged(int index);
    void on_switchControl_switchStateChanged(const std::string switchName, const SwitchState newState);
};

#endif // MAINWINDOW_H
