using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Samples.Kinect.WpfViewers;

namespace KinectManager
{
    public class KinectManager
    {
        private KinectSensorChooser sensorChooser;
        private KinectSensorManager sensorManager;

        public KinectManager()
        {
            sensorChooser = new KinectSensorChooser();
            sensorManager = new KinectSensorManager();

            KinectSensor sensor = new KinectSensor();

            sensor.SkeletonFrameReady = null;
        }

    }
}
