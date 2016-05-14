#ifndef QCAMERAWIDGET_H
#define QCAMERAWIDGET_H

#include <QWidget>
#include <QTimer>
#include <QByteArray>
#include <QGraphicsScene>
#include <QDateTime>

namespace Ui {
class QCameraWidget;
}

class QCameraWidget : public QWidget
{
    Q_OBJECT
    QTimer *m_timer;
    QString m_name;
    bool m_isPollable;
    bool m_isFaceDetectionEnabled;
    QGraphicsScene *m_drawing;
    QStringList m_switchListForFaceDetection;


public:
    QCameraWidget(const QString name, bool isPollable, bool isFaceDetectionEnabled, QWidget *parent = 0);
    ~QCameraWidget();

signals:
    void refresh();

public slots:
    void onImageReceived(const QString name, const QByteArray imageData, QDateTime timeStamp);
    void clear();
private slots:

    void on_btnRefresh_clicked();

    void on_chkAutoRefresh_stateChanged(int state);

    void on_dialRefreshInterval_valueChanged(int value);


private:
    Ui::QCameraWidget *ui;
    void updateAutoRefreshWidgets(const int isEnabled, const int intervalInSec);
};

#endif // QCAMERAWIDGET_H
