namespace GestureFramework
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Microsoft.Kinect;

    public class Player
    {
        public int PlayerID
        {
            get
            {
                return this.id;
            }
        }

        // Uses a nullable type for when a player is not in the screen
        public int? SkeletonID
        {
            get;
            set;
        }

        public readonly List<GestureType> playerGestures;

        private readonly int id;


        // In a better world, I wouldn't have this hardcoded into the Gesture Framework. But, for our purposes it's good enough.
        public Player(int id)
        {
            this.id = id;
            this.SkeletonID = null;
            switch (id)
            {
                case 0:
                    this.playerGestures = new List<GestureType> { GestureType.Moving, GestureType.Jumping };
                    break;
                case 1:
                    this.playerGestures = new List<GestureType> { GestureType.Turning, GestureType.Jumping };
                    break;
                default:
                    this.playerGestures = new List<GestureType> { GestureType.None };
                    break;

            }
        }
    }
}
