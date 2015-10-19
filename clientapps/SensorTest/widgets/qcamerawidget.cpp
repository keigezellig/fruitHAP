#include "qcamerawidget.h"
#include "ui_qcamerawidget.h"
#include <QDebug>
#include <QTimer>

QCameraWidget::QCameraWidget(const QString name, bool isPollable, QWidget *parent) :
    QWidget(parent),
    ui(new Ui::QCameraWidget),
    m_name(name),
    m_isPollable(isPollable),    
    m_drawing(0)
{
    ui->setupUi(this);

    if (m_isPollable)
    {
       m_timer = new QTimer(this);
       connect(m_timer,&QTimer::timeout, this, &QCameraWidget::on_btnRefresh_clicked);
       ui->btnRefresh->setVisible(true);
       ui->chkAutoRefresh->setVisible(true);
       ui->dialRefreshInterval->setVisible(true);
       ui->lcdRefreshInterval->setVisible(true);
       ui->label->setVisible(true);
    }
    else
    {
        ui->btnRefresh->setVisible(false);
        ui->chkAutoRefresh->setVisible(false);
        ui->dialRefreshInterval->setVisible(false);
        ui->lcdRefreshInterval->setVisible(false);
        ui->label->setVisible(false);
    }

}

QCameraWidget::~QCameraWidget()
{
    delete ui;
}


void QCameraWidget::on_btnRefresh_clicked()
{
    qDebug() << "Refresh";
    emit refresh();
}

void QCameraWidget::updateAutoRefreshWidgets(const int isEnabled, const int intervalInSec)
{
    m_timer->stop();

    if (!isEnabled || intervalInSec == 0)
    {
        ui->btnRefresh->setEnabled(true);
    }
    if (isEnabled == Qt::Checked && intervalInSec > 0)
    {
        ui->btnRefresh->setEnabled(false);
        m_timer->start(intervalInSec * 1000);
        return;
    }

    if (!isEnabled && intervalInSec > 0)
    {
        ui->chkAutoRefresh->setChecked(true);
        return;
    }

    if (isEnabled && intervalInSec == 0)
    {
        ui->chkAutoRefresh->setChecked(false);
        return;
    }




}

void QCameraWidget::on_chkAutoRefresh_stateChanged(int state)
{
    updateAutoRefreshWidgets(state,ui->dialRefreshInterval->value());


}

void QCameraWidget::on_dialRefreshInterval_valueChanged(int value)
{
   updateAutoRefreshWidgets(ui->chkAutoRefresh->checkState(),value);

}

void QCameraWidget::onImageReceived(const QString name, const QByteArray imageData, QDateTime timeStamp)
{
    if (name == m_name)
    {

       if (m_drawing != nullptr)
       {
           delete m_drawing;
           m_drawing = nullptr;
       }

        QImage image = QImage::fromData(imageData);
        m_drawing = new QGraphicsScene(this);
        QPixmap pixmap = QPixmap::fromImage(image);
        QString text(m_name + " " + timeStamp.toString());



        m_drawing->addPixmap(pixmap.scaled(ui->graphicsView->width() - 5,ui->graphicsView->height() - 5));
        m_drawing->addText(text);
        ui->graphicsView->setScene(m_drawing);
    }
}
