#ifndef QOPENCVFACEVERIFIER_H
#define QOPENCVFACEVERIFIER_H

#include <QObject>
#include "ifaceverifier.h"
#include "opencv2/objdetect.hpp"
#include "opencv2/imgcodecs.hpp"
#include "opencv2/videoio.hpp"
#include "opencv2/highgui.hpp"
#include "opencv2/imgproc.hpp"
#include "opencv2/core/utility.hpp"

#include "opencv2/videoio/videoio_c.h"
#include "opencv2/highgui/highgui_c.h"

#include <QString>

using namespace std;
using namespace cv;


class QOpenCvFaceVerifier : public QObject, IFaceVerifier
{
    Q_OBJECT
    int detectAndDraw(Mat &img, CascadeClassifier &cascade, CascadeClassifier &nestedCascade, double scale, bool tryflip);
    QString m_cascadeName;
    QString m_nestedCascadeName;
    double m_scale;
    bool m_tryFlip;
public:
     QOpenCvFaceVerifier(double scale = 1, bool tryFlip = false, QObject *parent = 0);
     int DetectFacesInImage(const QByteArray &image, QByteArray &destImage) override;

};

#endif // QOPENCVFACEVERIFIER_H
