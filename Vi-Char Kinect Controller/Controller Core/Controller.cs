using System;
using System.Collections.Generic;
using GestureFramework;
using VoiceFramework;
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

        public bool Moving
        {
            get
            {
                return controllerStates[ViCharGesture.Moving.ToString()].isActive();
            }
        }

        public bool Turning
        {
            get
            {
                return controllerStates[ViCharGesture.Turning.ToString()].isActive();
            }
        }

        public bool Jumping
        {
            get
            {
                return controllerStates[ViCharGesture.Jumping.ToString()].isActive();
            }
        }
        #endregion

        #region Fields
        private KinectSensorChooser sensorChooser;
        private KinectSensorManager sensorManager;

        private Dictionary<int, GestureMapState> gestureMaps;
        private SpeechRecognizer speechManager;
        private List<Player> allPlayers = new List<Player>{new PlayerOne(), new PlayerTwo()};

        private Dictionary<string, ControllerState> controllerStates;
        #endregion

        //Creates the ViChar Controller, setting all Controller State Activation Durations to the same value
        public ViCharController(int activationDuration=1000)
        {
            gestureMaps = new Dictionary<int, GestureMapState>();
            speechManager = SpeechRecognizer.Create();

            controllerStates = new Dictionary<string, ControllerState>();

            //Any Controller states must be registered (See registerControllerStates())
            foreach (ViCharGesture gesture in Enum.GetValues(typeof(ViCharGesture)))
            {
                controllerStates.Add(gesture.ToString(), new ControllerState(activationDuration, gesture));
            }

            foreach (Player p in allPlayers)
            {
                p.mapState = new GestureMapState(p.GetGestureMap());
                registerControllerStates(p.mapState);
            }
            ManageSensor();
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
            
            if (!sensorManager.KinectSensorAppConflict)
            {
                // Start speech recognizer after KinectSensor started successfully.
                speechManager = SpeechRecognizer.Create();

                if (speechManager != null)
                {
                    speechManager.SaidSomething += this.VoiceEvent;
                    speechManager.Start(sensor.AudioSource);
                }
            }
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
                        if(gestureMaps.ContainsKey(sd.TrackingId))
                            gestureMaps.Remove(sd.TrackingId);
                        continue;
                    }

                    //If there is not already a gesture state map for this skeleton, then create one
                    //Additionally, register its Controller States to the GestureMapState
                    if (!gestureMaps.ContainsKey(sd.TrackingId))
                    {
                        Player noSkelPlayer = findInactivePlayer();
                        if (noSkelPlayer != null)
                        {
                            Console.WriteLine("New Skeleton Detected: " + sd.TrackingId + " - added as " + noSkelPlayer.ToString());
                            noSkelPlayer.SkeletonID = sd.TrackingId;
                            gestureMaps.Add(sd.TrackingId, noSkelPlayer.mapState);
                        }
                        
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

        private void VoiceEvent(object sender, SpeechRecognizer.SaidSomethingEventArgs e)
        {
            
        }

        private Player findInactivePlayer()
        {
            foreach (Player p in allPlayers)
            {
                if (p.SkeletonID.HasValue && !gestureMaps.ContainsKey(p.SkeletonID.Value))
                    return p;
            }
            return null;
        }

        //If you add a controller state, it must be added to this Registration list. If not, it will never activate. 
        private void registerControllerStates(GestureMapState state)
        {
            foreach (ControllerState cState in controllerStates.Values)
            {
                state.RegisterGestureResult(x => cState.Activate((ViCharGesture)x));
            }
        }

    }
}
