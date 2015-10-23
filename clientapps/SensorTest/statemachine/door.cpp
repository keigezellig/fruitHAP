#include "door.h"
#include <QDebug>


Door::Door(QObject *parent): QObject(parent), m_doorMachine(*this)
{    
    m_approvalTimer = new QTimer(this);
    m_unlockedTimer = new QTimer(this);
    m_approvalTimer->setInterval(5000);
    m_unlockedTimer->setInterval(7000);
    connect(m_approvalTimer, &QTimer::timeout, this, &Door::approvalTimerExpired);
    connect(m_unlockedTimer, &QTimer::timeout, this, &Door::unlockedTimerExpired);

}
void Door::init()
{
    m_doorMachine.enterStartState();
}

Door::~Door()
{
    delete m_approvalTimer;
    delete m_unlockedTimer;
}

void Door::startApprovalTimer()
{
   m_approvalTimer->start();
}

void Door::stopApprovalTimer()
{
   m_approvalTimer->stop();
}

void Door::startUnlockedTimer()
{
   m_unlockedTimer->start();
}

void Door::stopUnlockedTimer()
{
    m_unlockedTimer->stop();
}

void Door::onFaceDetected(const QByteArray &image)
{
    emit imageWithFaceIsAvailable(image);
}

void Door::onApproved()
{
   emit personApproved();
}

void Door::onNotApproved()
{
   emit personNotApproved();
}

void Door::initialAction()
{
   emit initialize();
}

void Door::onApprovalTimeOut()
{
    emit noAnswer();
}

void Door::faceHasBeenDetected(const QByteArray &image)
{
    m_doorMachine.FaceDetected(image);
}

void Door::approve(bool isApproved)
{
    m_doorMachine.Approval(isApproved);
}

void Door::reset()
{
    m_doorMachine.Reset();
}


void Door::approvalTimerExpired()
{
    m_doorMachine.ApprovalTimeOut();
}

void Door::unlockedTimerExpired()
{
    m_doorMachine.UnlockedTimerTimeOut();
}
