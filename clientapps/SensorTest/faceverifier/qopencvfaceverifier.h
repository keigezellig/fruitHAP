#ifndef QOPENCVFACEVERIFIER_H
#define QOPENCVFACEVERIFIER_H

#include <QObject>
#include "faceverifier.h"
#include "opencv2/objdetect.hpp"
#include "opencv2/imgcodecs.hpp"
#include "opencv2/videoio.hpp"
#include "opencv2/highgui.hpp"
#include "opencv2/imgproc.hpp"
#include "opencv2/core/utility.hpp"

#include "opencv2/videoio/videoio_c.h"
#include "opencv2/highgui/highgui_c.h"

#include <QString>
#include <QObject>

using namespace std;
using namespace cv;


class QOpenCvFaceVerifier : public FaceVerifier
{
    Q_OBJECT
    int detectAndDraw(Mat &img, CascadeClassifier &cascade, CascadeClassifier &nestedCascade, double scale, bool tryflip);
    QString m_cascadeName;
    QString m_nestedCascadeName;
    double m_scale;
    bool m_tryFlip;
    vector<string> m_cascadeList;
public:
     QOpenCvFaceVerifier(double scale = 1, bool tryFlip = false, QObject *parent = 0);
     virtual int DetectFacesInImage(const QByteArray &image, QByteArray &destImage);

};

#endif // QOPENCVFACEVERIFIER_H
