#include "qopencvfaceverifier.h"
#include <opencv2/core/mat.hpp>
#include <QDebug>

QOpenCvFaceVerifier::QOpenCvFaceVerifier(double scale, bool tryFlip, QObject *parent) :
    FaceVerifier(parent),
    m_cascadeName("haarcascade_frontalface_default.xml"),
    m_nestedCascadeName(""),//haarcascade_eye_tree_eyeglasses.xml"),
    m_scale(scale),
    m_tryFlip(tryFlip)
{
    m_cascadeList.push_back("haarcascade_frontalface_default.xml");
    m_cascadeList.push_back("haarcascade_frontalface_alt.xml");
    m_cascadeList.push_back("haarcascade_frontalface_alt2.xml");
    m_cascadeList.push_back("haarcascade_profileface.xml");
}

int QOpenCvFaceVerifier::DetectFacesInImage(const QByteArray &image, QByteArray &destImage)
{
     try
    {
     vector<unsigned char> imageData(image.begin(), image.end());
     Mat img = imdecode(imageData,1);
     if (img.empty())
     {
         qCritical() << "Cannot decode input image";
         return -1;
     }

     CascadeClassifier cascade;
     CascadeClassifier nestedcascade;

     for(std::string &c : m_cascadeList)
     {
         cascade.load(c);

         if (cascade.empty())
         {
             qCritical() << "Cannot load classifier" << QString::fromStdString(c) << "put them in the right folder";
             continue;
         }

         qDebug() << "Detecting face with: " << QString::fromStdString(c);
         int facesDetected = detectAndDraw(img,cascade,nestedcascade,m_scale,m_tryFlip);
         if (facesDetected > 0)
         {
             qDebug() << "Found face with: " << QString::fromStdString(c);
             return facesDetected;
         }
     }

     return -1;
    }
    catch (cv::Exception ex)
    {
        qCritical() << "Something went wrong with opencv: " << QString::fromStdString(ex.msg);
        return -1;
    }


}


int QOpenCvFaceVerifier::detectAndDraw( Mat& img, CascadeClassifier& cascade,
                    CascadeClassifier& nestedCascade,
                    double scale, bool tryflip )
{
    int i = 0;
    double t = 0;
    vector<Rect> faces, faces2;
    const static Scalar colors[] =  { CV_RGB(0,0,255),
        CV_RGB(0,128,255),
        CV_RGB(0,255,255),
        CV_RGB(0,255,0),
        CV_RGB(255,128,0),
        CV_RGB(255,255,0),
        CV_RGB(255,0,0),
        CV_RGB(255,0,255)} ;
    Mat gray, smallImg( cvRound (img.rows/scale), cvRound(img.cols/scale), CV_8UC1 );

    cvtColor( img, gray, COLOR_BGR2GRAY );
    resize( gray, smallImg, smallImg.size(), 0, 0, INTER_LINEAR );
    equalizeHist( smallImg, smallImg );

    t = (double)cvGetTickCount();
    cascade.detectMultiScale( smallImg, faces,
        1.1, 2, 0
        //|CASCADE_FIND_BIGGEST_OBJECT
        //|CASCADE_DO_ROUGH_SEARCH
        |CASCADE_SCALE_IMAGE
        ,
        Size(30, 30) );
    if( tryflip )
    {
        flip(smallImg, smallImg, 1);
        cascade.detectMultiScale( smallImg, faces2,
                                 1.1, 2, 0
                                 //|CASCADE_FIND_BIGGEST_OBJECT
                                 //|CASCADE_DO_ROUGH_SEARCH
                                 |CASCADE_SCALE_IMAGE
                                 ,
                                 Size(30, 30) );
        for( vector<Rect>::const_iterator r = faces2.begin(); r != faces2.end(); r++ )
        {
            faces.push_back(Rect(smallImg.cols - r->x - r->width, r->y, r->width, r->height));
        }
    }
    t = (double)cvGetTickCount() - t;
    printf( "detection time = %g ms\n", t/((double)cvGetTickFrequency()*1000.) );
    int numFaces = 0;
    for( vector<Rect>::const_iterator r = faces.begin(); r != faces.end(); r++, i++ )
    {
        Mat smallImgROI;
        vector<Rect> nestedObjects;
        Point center;
        Scalar color = colors[i%8];
        int radius;

        double aspect_ratio = (double)r->width/r->height;
        if( 0.75 < aspect_ratio && aspect_ratio < 1.3 )
        {
            center.x = cvRound((r->x + r->width*0.5)*scale);
            center.y = cvRound((r->y + r->height*0.5)*scale);
            radius = cvRound((r->width + r->height)*0.25*scale);
            circle( img, center, radius, color, 3, 8, 0 );
        }
        else
            rectangle( img, cvPoint(cvRound(r->x*scale), cvRound(r->y*scale)),
                       cvPoint(cvRound((r->x + r->width-1)*scale), cvRound((r->y + r->height-1)*scale)),
                       color, 3, 8, 0);
        if( nestedCascade.empty() )
            continue;
        smallImgROI = smallImg(*r);
        nestedCascade.detectMultiScale( smallImgROI, nestedObjects,
            1.1, 2, 0
            //|CASCADE_FIND_BIGGEST_OBJECT
            //|CASCADE_DO_ROUGH_SEARCH
            //|CASCADE_DO_CANNY_PRUNING
            |CASCADE_SCALE_IMAGE
            ,
            Size(30, 30) );
        for( vector<Rect>::const_iterator nr = nestedObjects.begin(); nr != nestedObjects.end(); nr++ )
        {
            center.x = cvRound((r->x + nr->x + nr->width*0.5)*scale);
            center.y = cvRound((r->y + nr->y + nr->height*0.5)*scale);
            radius = cvRound((nr->width + nr->height)*0.25*scale);
            circle( img, center, radius, color, 3, 8, 0 );
        }




    }

    return faces.size();
}

