#ifndef QSWITCHWIDGET_H
#define QSWITCHWIDGET_H

#include <QWidget>

namespace Ui {
class QSwitchWidget;
}

class QSwitchWidget : public QWidget
{
    Q_OBJECT
    void setName(const QString &name);

public:
    QSwitchWidget(const QString name, const bool isReadOnly, QWidget *parent = 0);
    ~QSwitchWidget();
    enum class State {Undefined, On, Off};
    void setIsReadOnly(bool value);
signals:
    void turnOn();
    void turnOff();
public slots:
    void onStateChanged(const QSwitchWidget::State newState);
private slots:
    void on_btnOff_clicked();
    void on_btnOn_clicked();
private:
    Ui::QSwitchWidget *ui;
    void convertEnumToString(const State &state, QString &string, QString &styleSheet);
};

#endif // QSWITCHWIDGET_H
