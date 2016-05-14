#ifndef FACEVERIFIER_H
#define FACEVERIFIER_H

#include <QObject>

class FaceVerifier : public QObject
{
    Q_OBJECT
protected:
    explicit FaceVerifier(QObject *parent = 0);
public slots:
    virtual int DetectFacesInImage(const QByteArray &image, QByteArray &destImage) = 0;
};

#endif // FACEVERIFIER_H
