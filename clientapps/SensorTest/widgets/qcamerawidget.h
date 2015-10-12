#ifndef QCAMERAWIDGET_H
#define QCAMERAWIDGET_H

#include <QWidget>

namespace Ui {
class QCameraWidget;
}

class QCameraWidget : public QWidget
{
    Q_OBJECT

public:
    explicit QCameraWidget(QWidget *parent = 0);
    ~QCameraWidget();

private:
    Ui::QCameraWidget *ui;
};

#endif // QCAMERAWIDGET_H
