#ifndef IFACEVERIFIER_H
#define IFACEVERIFIER_H

#include <QByteArray>

class IFaceVerifier
{
public:
    virtual int DetectFacesInImage(const QByteArray &image, QByteArray &destImage) = 0;
protected:
    IFaceVerifier();

};

#endif // IFACEVERIFIER_H
