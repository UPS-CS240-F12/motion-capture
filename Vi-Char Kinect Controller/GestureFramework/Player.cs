namespace GestureFramework
{
    using System.Collections.Generic;

    public abstract class Player
    {
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

        // Actual Gestures must be specified by the controller using the Gesture Framework
        public abstract GestureMap GetGestureMap();

        public Player()
        {
            this.SkeletonID = null;
        }
    }
}
