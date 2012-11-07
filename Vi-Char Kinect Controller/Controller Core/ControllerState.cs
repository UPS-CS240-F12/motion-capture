using System;
using GestureFramework;

namespace Controller_Core
{
    public class ControllerState
    {
        public DateTime LastActivation
        {
            get;
            set;
        }

        public TimeSpan ActivationDuration
        {
            get;
            set;
        }

        //To be implemented...
        public int Value
        {
            get;
            set;
        }

        public Player Player
        {
            get;
            set;
        }

        private DateTime lastActivation;
        private TimeSpan activationDuration;
        private int value;
        private GestureType activatingGesture;
        private Player activatingPlayer;

        public ControllerState(int activationDuration, GestureType activatingGesture)
        {
            this.lastActivation = DateTime.MinValue;
            this.activationDuration = TimeSpan.FromMilliseconds(activationDuration);
            this.value = 0;
            this.activatingGesture = activatingGesture;
        }

        //Activates the controller state
        public void Activate(GestureType type, int activatingSkeletonID)
        {
            if (type == activatingGesture && activatingPlayer.GetId() == activatingSkeletonID)
            {
                lastActivation = DateTime.Now;
                Console.WriteLine("Activating Gesture: " + type.ToString() + "@" + lastActivation);
            }
        }

        //Checks if the state is still active. If the time elapsed since last activation exceeds the activationDuration, then
        // the controller state has timed out
        public bool isActive()
        {
            return (DateTime.Now - lastActivation) < activationDuration;
        }
    }
}
