﻿using System;
using System.Collections.Generic;
using GestureFramework;
using VoiceFramework;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Samples.Kinect.WpfViewers;
using Microsoft.Speech.Recognition;

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
        private List<Player> allPlayers = new List<Player> { new PlayerOne(), new PlayerTwo() };

        private Dictionary<string, ControllerState> controllerStates;
        #endregion

        //Creates the ViChar Controller, setting all Controller State Activation Durations to the same value
        public ViCharController(int activationDuration = 1000)
        {
            gestureMaps = new Dictionary<int, GestureMapState>();
            controllerStates = new Dictionary<string, ControllerState>();

            //Any Controller states must be registered (See registerControllerStates())
            foreach (ViCharGesture gesture in Enum.GetValues(typeof(ViCharGesture)))
            {
                controllerStates.Add(gesture.ToString(), new ControllerState(activationDuration, gesture, ViCharVoiceAction.None));
            }

            //foreach (ViCharVoiceAction voiceAction in Enum.GetValues(typeof(ViCharVoiceAction)))
            //{
            //    controllerStates.Add(voiceAction.ToString(), new ControllerState(activationDuration, ViCharGesture.None, voiceAction));
            //}

            foreach (Player p in allPlayers)
            {
                p.mapState = new GestureMapState(p.GetGestureMap());
                registerGestureControllerStates(p.mapState);
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
                //speechManager = SpeechRecognizer.Create();

                //if (speechManager != null)
                //{
                //    registerVoiceControllerStates(speechManager);
                //    speechManager.Start(sensor.AudioSource);
                //}
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
                removeInactivePlayers(allSkeletons);

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
                        Player noSkelPlayer = findInactivePlayer();
                        Console.WriteLine("Adding new skeleton as Player");
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

        private bool isActivePlayer(int id)
        {
            return allPlayers.Exists((x => x.SkeletonID == id));
        }



        //if (gestureMaps.ContainsKey(sd.TrackingId))
        //    gestureMaps.Remove(sd.TrackingId);

        private void removeInactivePlayers(Skeleton[] detectedSkeletons)
        {
            List<int> detectedIDs = new List<int>();
            foreach (Skeleton s in detectedSkeletons)
                detectedIDs.Add(s.TrackingId);
            foreach (Player p in allPlayers)
            {
                if (p.SkeletonID.HasValue && !detectedIDs.Contains(p.SkeletonID.Value))
                {
                    Console.WriteLine("Removing Inactive Player: " + p.ToString() + p.SkeletonID.Value);
                    if (gestureMaps.ContainsKey(p.SkeletonID.Value))
                        gestureMaps.Remove(p.SkeletonID.Value);
                    p.SkeletonID = null;
                }
            }
        }

        private Player findInactivePlayer()
        {
            foreach (Player p in allPlayers)
            {
                if (p.SkeletonID.HasValue)
                {
                    if (!gestureMaps.ContainsKey(p.SkeletonID.Value))
                        return p;
                }
                else
                {
                    return p;
                }

            }
            return null;
        }

        //If you add a controller state, it must be added to this Registration list. If not, it will never activate. 
        private void registerGestureControllerStates(GestureMapState state)
        {
            foreach (ControllerState cState in controllerStates.Values)
            {
                // Necessary because of how lambda statements are interpreted
                // http://stackoverflow.com/questions/4945486/multiple-anonymous-event-handlers-but-only-last-one-is-called
                ControllerState copy = cState;
                if (copy.IsGestureActivated())
                {
                    state.RegisterGestureResult(x => copy.ActivateGesture((ViCharGesture)x));
                }
            }
        }

        private void registerVoiceControllerStates(SpeechRecognizer speechEngine)
        {
            foreach (ControllerState cState in controllerStates.Values)
            {
                if (cState.IsVoiceActivated())
                {
                    speechEngine.RegisterVoiceActionResult(x => cState.ActivateVoice((ViCharVoiceAction)x));
                }
            }
        }



    }
}
