#ifndef QSWITCHWIDGET_H
#define QSWITCHWIDGET_H

#include <QWidget>
#include <sensor/switch/definitions.h>
#include <QDateTime>

namespace Ui {
class QSwitchWidget;
}

class QSwitchWidget : public QWidget
{
    Q_OBJECT
    void setName(const QString &name);
    QString m_name;

public:
    QSwitchWidget(const QString name, const bool isReadOnly, const bool isPollable, QWidget *parent = 0);
    void setIsReadOnly(bool value);
    ~QSwitchWidget();
signals:
    void turnOn();
    void turnOff();
    void refresh();
public slots:
    void onStateChanged(const QString name, const SwitchState newState, const QDateTime timestamp);
private slots:
    void on_btnOff_clicked();
    void on_btnOn_clicked();
    void on_btnRefresh_clicked();
private:
    Ui::QSwitchWidget *ui;
    void convertEnumToString(const SwitchState &state, QString &string, QString &styleSheet);
    void setPollable(bool value);
};

#endif // QSWITCHWIDGET_H
