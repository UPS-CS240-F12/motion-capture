using System;

namespace GestureFramework
{
    //Used for passing information about Gesture Events
    public class GestureEventArgs : EventArgs
    {

        public GestureType Type
        {
            get
            {
                return type;
            }
        }

        private readonly GestureType type;

        public GestureEventArgs(GestureType type)
        {
            this.type = type;
        }
    }
}
