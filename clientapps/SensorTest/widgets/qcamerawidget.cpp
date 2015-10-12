#include "qcamerawidget.h"
#include "ui_qcamerawidget.h"

QCameraWidget::QCameraWidget(QWidget *parent) :
    QWidget(parent),
    ui(new Ui::QCameraWidget)
{
    ui->setupUi(this);
}

QCameraWidget::~QCameraWidget()
{
    delete ui;
}
