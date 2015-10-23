#ifndef DOOR_H
#define DOOR_H

#include "DoorMachine_sm.h"
#include <QByteArray>
#include <QTimer>
#include <QObject>

class Door : public QObject
{
    Q_OBJECT
private:
    DoorMachineContext m_doorMachine;
    QTimer *m_approvalTimer;
    QTimer *m_unlockedTimer;

public:
    explicit Door(QObject *parent = 0);
    ~Door();

//*** Action methods which are called from FSM. Do not call directl!
    void startApprovalTimer();
    void stopApprovalTimer();
    void startUnlockedTimer();
    void stopUnlockedTimer();
    void onFaceDetected(const QByteArray &image);
    void onApproved();
    void onNotApproved();
    void initialAction();
    void onApprovalTimeOut();
// ***

    void faceHasBeenDetected(const QByteArray &image);
    void approve(bool isApproved);
    void reset();

    void init();
signals:
    void imageWithFaceIsAvailable(const QByteArray image);
    void personApproved();
    void personNotApproved();
    void initialize();
    void noAnswer();

private slots:
    void approvalTimerExpired();
    void unlockedTimerExpired();
};

#endif // DOOR_H
