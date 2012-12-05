using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using GestureFramework;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Samples.Kinect.WpfViewers;
using VoiceFramework;

namespace Controller_Core
{
    public class ViCharController : IDisposable
    {
        #region Fields
        private KinectSensorChooser sensorChooser;
        private KinectSensorManager sensorManager;
        private SpeechRecognizer speechManager;

        private NamedPipeServerStream serverPipe;
        private StreamWriter serverPipeWriter;

        private List<Player> allPlayers = new List<Player> { new PlayerOne(), new PlayerTwo() };
        private Dictionary<int, GestureMapState> gestureMaps;
        #endregion

        #region Initialization
        //Creates the ViChar Controller, setting all Controller State Activation Durations to the same value
        public ViCharController()
        {
            gestureMaps = new Dictionary<int, GestureMapState>();

            foreach (Player p in allPlayers)
            {
                p.mapState = new GestureMapState(p.GetGestureMap());
                registerGestureControllerStates(p.mapState);
            }
            
            ManageSensor();
            CreateServerPipe();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);      
        }

        protected virtual void Dispose(bool disposing)
        {
            if (serverPipeWriter != null)
                serverPipeWriter.Dispose();
            if (serverPipe != null)
                serverPipe.Dispose();
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
            if(sensorManager.KinectSensor != null)
                Console.WriteLine("*** Kinect Ready! ***");
        }


        private void registerGestureControllerStates(GestureMapState state)
        {
            state.RegisterGestureResult(x => SendEventThroughPipe(((ViCharGesture)x).ToString()));
        }

        private void registerVoiceControllerStates(SpeechRecognizer speechEngine)
        {
            speechEngine.RegisterVoiceActionResult(x => SendEventThroughPipe(((ViCharVoiceAction)x).ToString()));
        }
        #endregion

        #region Kinect Event Management
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

        //Enables all the appropriate Kinect streams, and sets the smoothing for skeleton data
        // Also sets up the speech recognition engine
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
                speechManager = SpeechRecognizer.Create(ViCharVoiceActionGrammar.Words);

                if (speechManager != null)
                {
                    registerVoiceControllerStates(speechManager);
                    speechManager.Start(sensor.AudioSource);
                }
            }
        }

        //Disconnects the old Kinect from the Controller
        private void UninitializeKinectServices(KinectSensor sensor)
        {
            sensor.SkeletonFrameReady -= this.SkeletonsReady;
            if (speechManager != null)
                speechManager.Dispose();
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
                        if (noSkelPlayer != null)
                        {
                            Console.WriteLine("New Skeleton added as " + noSkelPlayer.ToString());
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
        #endregion

        #region Player To Skeleton Mapping Helper Functions
        // Determines if the skeleton attached to either main player has left the play space
        private void removeInactivePlayers(Skeleton[] detectedSkeletons)
        {
            // Creates a list of all the skeleton IDs presently active
            //  -Made into a list for simpler manipulation/queries
            List<int> detectedIDs = new List<int>();
            foreach (Skeleton s in detectedSkeletons)
                detectedIDs.Add(s.TrackingId);
            
            // Checks to see if the skeleton ID attached to either player is still active. 
            // If not, that Player is set to inactive, and their Gesture Map is discarded
            foreach (Player p in allPlayers)
            {
                if (p.SkeletonID.HasValue && !detectedIDs.Contains(p.SkeletonID.Value))
                {
                    Console.WriteLine(p.ToString() + " detected as inactive - Removing skeleton");
                    if (gestureMaps.ContainsKey(p.SkeletonID.Value))
                        gestureMaps.Remove(p.SkeletonID.Value);
                    p.SkeletonID = null;
                }
            }
        }

        // Determines which (if any) player slot is available to be filled
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
        #endregion

        #region Named Pipes
        // Connects to the controller frontend
        private void CreateServerPipe()
        {
            try
            {
                Console.WriteLine("Creating a new Pipe to Client...");
                if (serverPipe != null)
                {
                    Console.WriteLine("Closing previous Pipe...");
                    serverPipe.Close();
                }
                serverPipe = new NamedPipeServerStream("viCharControllerPipe", PipeDirection.Out);
                serverPipe.WaitForConnection();
                serverPipeWriter = new StreamWriter(serverPipe);
                serverPipeWriter.AutoFlush = true;
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            
        }

        // This method is wired up to the event framework to send controller events to the frontend
        private void SendEventThroughPipe(string type)
        {
            if (serverPipe.IsConnected && serverPipeWriter != null)
            {
                try
                {
                    Console.WriteLine("Sending: " + type);
                    serverPipeWriter.WriteLine(type);
                }
                catch (IOException e) { Console.WriteLine(e.Message); CreateServerPipe(); }
            }
            else
            {
                CreateServerPipe();
            }
        }
        #endregion

    }
}
