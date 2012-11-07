using System;
using System.Collections.Generic;
using GestureFramework;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Samples.Kinect.WpfViewers;

namespace Controller_Core
{

    public class ViCharController
    {
        #region Properties
        public bool SensorActive
        {
            get
            {
                return (sensorChooser.Kinect != null);
            }
        }

        public int Moving
        {
            get
            {
                return moving.Value;
            }
        }

        public int Turning
        {
            get
            {
                return turning.Value;
            }
        }

        public bool Jumping
        {
            get
            {
                return jumping.isActive();
            }
        }
        #endregion

        #region Fields
        private KinectSensorChooser sensorChooser;
        private KinectSensorManager sensorManager;

        private GestureMap gestureMap;
        private Dictionary<int, GestureMapState> gestureMaps;
        private List<Player> allPlayers;

        private ControllerState moving;
        private ControllerState turning;
        private ControllerState jumping;
        #endregion

        //Creates the ViChar Controller, setting all Controller State Activation Durations to the same value
        public ViCharController(int activationDuration=1000)
        {
            ManageSensor();

            gestureMap = new GestureMap();
            gestureMaps = new Dictionary<int, GestureMapState>();
            allPlayers = new List<Player>();

            //Any Controller states must be registered (See registerControllerStates())
            moving = new ControllerState(activationDuration, GestureType.Moving);
            turning = new ControllerState(activationDuration, GestureType.Turning);
            jumping = new ControllerState(activationDuration, GestureType.Jumping);
        }

        //Creates the ViChar Controller, setting all Controller State Activation Durations to a particular value
        public ViCharController(int[] activationDurations)
        {
            gestureMap = new GestureMap();
            gestureMaps = new Dictionary<int, GestureMapState>();
            ManageSensor();
            //Any Controller states must be registered (See registerControllerStates())
            moving = new ControllerState(activationDurations[0], GestureType.Moving);
            turning = new ControllerState(activationDurations[1], GestureType.Turning);
            jumping = new ControllerState(activationDurations[2], GestureType.Jumping);
        }

        //Manages establishing a connection to the Kinect Sensor
        private void ManageSensor()
        {
            //An object which finds any connected Kinects (it's hard to not let those spellings overlap...)
            //and selects one of them for the application. Once selected, it locks that Kinect to our process.
            sensorChooser = new KinectSensorChooser();
            sensorChooser.Start();

            //An object which accesses the underlying data available from the Kinect, allowing us
            // to specify particular settings we desire from it.
            sensorManager = new KinectSensorManager();
            sensorManager.KinectSensorChanged += this.KinectSensorChanged;

            sensorManager.KinectSensor = sensorChooser.Kinect;
        }

        //Event Handler: If the Kinect is disconnected/reconnected, handles that process.
        private void KinectSensorChanged(object sender, KinectSensorManagerEventArgs<KinectSensor> args)
        {
            if (null != args.OldValue)
            {
                UninitializeKinectServices(args.OldValue);
            }

            if (null != args.NewValue)
            {
                InitializeKinectServices(args.NewValue);
            }
        }

        //Enables all the appropriate streams, and sets the smoothing for skeleton data
        private void InitializeKinectServices(KinectSensor sensor)
        {
            sensorManager.ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;
            sensorManager.ColorStreamEnabled = true;

            sensor.SkeletonFrameReady += this.SkeletonsReady;
            sensorManager.TransformSmoothParameters = new TransformSmoothParameters
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            };
            sensorManager.SkeletonStreamEnabled = true;
            sensorManager.KinectSensorEnabled = true;
        }

        //Disconnects the old Kinect from the Controller
        private void UninitializeKinectServices(KinectSensor sensor)
        {
            sensor.SkeletonFrameReady -= this.SkeletonsReady;
        }

        //Event Handler: Whenever a SkeletonFrame is ready from the Kinect, it is passed here to be inspected
        //for Gesture Components.
        private void SkeletonsReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                //If there is no frame, we're done.
                if (skeletonFrameData == null)
                {
                    return;
                }

                var allSkeletons = new Skeleton[skeletonFrameData.SkeletonArrayLength];
                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                foreach (Skeleton sd in allSkeletons)
                {
                    // If this skeleton is no longer being tracked, skip it
                    if (sd.TrackingState != SkeletonTrackingState.Tracked)
                    {
                        continue;
                    }

                    //If there is not already a gesture state map for this skeleton, then create one
                    //Additionally, register its Controller States to the GestureMapState
                    if (!gestureMaps.ContainsKey(sd.TrackingId))
                    {
                        Console.WriteLine("New Skeleton Detected: " + sd.TrackingId + " - out of " + allSkeletons.Length);
                        var mapstate = new GestureMapState(gestureMap);
                        registerControllerStates(mapstate);
                        gestureMaps.Add(sd.TrackingId, mapstate);
                    }

                    //Check for a gesture. If one is found to be completed, an event will be fired and handled elsewhere.
                    bool gestureComplete = gestureMaps[sd.TrackingId].Evaluate(sd, 640, 480);
                    if (gestureComplete)
                    {
                        gestureMaps[sd.TrackingId].ResetAll(sd);
                    }
                }
            }
        }

        //If you add a controller state, it must be added to this Registration list. If not, it will never activate. 
        private void registerControllerStates(GestureMapState state)
        {
            Action<GestureType, int> movingActivate = moving.Activate;
            state.RegisterGestureResult(movingActivate);
        }

    }
}
