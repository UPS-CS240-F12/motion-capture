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
            get { return this.activationDuration; }
            set { this.activationDuration = value; }
        }

        //To be implemented...
        public int Magnitude
        {
            get { return this.magnitude; }
            set { this.magnitude = value; }
        }

        private DateTime lastActivation;
        private TimeSpan activationDuration;
        private int magnitude;
        private ViCharGestures activatingGesture;

        public ControllerState(int activationDuration, ViCharGestures activatingGesture)
        {
            this.lastActivation = DateTime.MinValue;
            this.activationDuration = TimeSpan.FromMilliseconds(activationDuration);
            this.magnitude = 0;
            this.activatingGesture = activatingGesture;
        }

        //Activates the controller state
        public void Activate(ViCharGestures type)
        {
            if (type == activatingGesture)
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
