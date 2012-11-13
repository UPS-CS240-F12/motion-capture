using System;
using System.Collections.Generic;
using Microsoft.Kinect;

namespace GestureFramework
{
    /// <summary>
    /// This class is the topmost gesture state class.  There should only be one of these instantiated
    /// per active user.
    /// </summary>
    public class GestureMapState
    {
        private readonly List<GestureState> _gesturestate;
        public DateTime LastGestureCompletionTime;

        //Event Handling
        public event GestureCompletedEventHandler gestureCompleted;
        public delegate void GestureCompletedEventHandler(GestureType type);

        public GestureMapState(GestureMap map)
        {
            _gesturestate = new List<GestureState>();
            InitializeGestureState(map);
        }

        public void InitializeGestureState(GestureMap map)
        {
            foreach (var gesturemapping in map.Items)
            {
                var state = new GestureState(gesturemapping);
                _gesturestate.Add(state);
            }
        }

        //Called by the Controller implementing the Gesture Framework
        public void RegisterGestureResult(Action<GestureType> gestureHandler)
        {
            gestureCompleted += new GestureCompletedEventHandler(gestureHandler);
        }

        // This method goes through each gesture state for the user and updates it, looking for completed gestures
        public bool Evaluate(Skeleton skeleton, int xScale, int yScale)
        {
            foreach (var state in _gesturestate)
            {
                var iscomplete = state.Evaluate(skeleton, DateTime.Now, xScale, yScale);

                // Skip the rest of this unless the gesture is complete
                if (!iscomplete)
                    continue;

                LastGestureCompletionTime = DateTime.Now;
                
                //Triggers a gesture completed event
                this.gestureCompleted(state.Type);
                return true;
            }
            return false;
        }


        public void ResetAll(Skeleton skeleton)
        {
            foreach (var state in _gesturestate)
            {
                state.Reset();
            }
        }
    }
}
