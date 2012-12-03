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

        private DateTime lastActivation;
        private TimeSpan activationDuration;
        private ViCharGesture activatingGesture;
        private ViCharVoiceAction activatingVoiceAction;

        public ControllerState(int activationDuration, ViCharGesture activatingGesture, ViCharVoiceAction activatingVoiceAction)
        {
            this.lastActivation = DateTime.MinValue;
            this.activationDuration = TimeSpan.FromMilliseconds(activationDuration);
            this.activatingGesture = activatingGesture;
            this.activatingVoiceAction = activatingVoiceAction;
        }

        //Activates the controller state via a Gesture
        public void ActivateGesture(ViCharGesture type)
        {
            if (type == activatingGesture)
            {
                Console.WriteLine("Action Detected: " + type.ToString());
                lastActivation = DateTime.Now;
            }
        }

        public bool IsGestureActivated()
        {
            return (activatingGesture != null);
        }

        //Activates the controller state via a Voice Action
        public void ActivateVoice(ViCharVoiceAction type)
        {
            if (type == activatingVoiceAction)
            {
                lastActivation = DateTime.Now;
            }
        }

        public bool IsVoiceActivated()
        {
            return (activatingVoiceAction != null);
        }

        //Checks if the state is still active. If the time elapsed since last activation exceeds the activationDuration, then
        //the controller state has timed out
        public bool isActive()
        {
            return (DateTime.Now - lastActivation) < activationDuration;
        }
    }
}
