namespace GestureFramework
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Microsoft.Kinect;

    public abstract class Player
    {

        // Uses a nullable type for when a player is not in the screen
        public int? SkeletonID
        {
            get;
            set;
        }

        public List<int> playerGestures
        {
            get;
            set;
        }

        public GestureMapState mapState
        {
            get;
            set;
        }

        public abstract GestureMap GetGestureMap();

        // In a better world, I wouldn't have this hardcoded into the Gesture Framework. But, for our purposes it's good enough.
        public Player()
        {
            this.SkeletonID = null;
        }
    }
}
