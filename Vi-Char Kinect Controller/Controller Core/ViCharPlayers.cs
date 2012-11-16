using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestureFramework;
using Microsoft.Kinect;

namespace Controller_Core
{   
    public class PlayerOne : Player
    {
        public PlayerOne()
        {
            this.playerGestures = PlayerHelpers.ProcessGestureList(new List<ViCharGestures> { ViCharGestures.Moving, ViCharGestures.Jumping });
        }

        public override GestureMap GetGestureMap()
        {
            GestureMap map = new GestureMap(this);

            Gesture moving = new Gesture("moving", 1000, (int)ViCharGestures.Moving);
            moving.Components.Add(new GestureComponent(JointType.HandRight, JointType.ElbowRight,
                JointRelationship.AboveAndLeft, JointRelationship.AboveAndRight));
            map.Items.Add(moving);

            Gesture jumping = new Gesture("jumping", 1000, (int)ViCharGestures.Jumping);
            jumping.Components.Add(new GestureComponent(JointType.HandRight, JointType.ShoulderRight,
                JointRelationship.Below, JointRelationship.Above));
            jumping.Components.Add(new GestureComponent(JointType.HandLeft, JointType.ShoulderLeft,
                JointRelationship.Below, JointRelationship.Above));
            map.Items.Add(jumping);

            return map;
        }
    }

    public class PlayerTwo : Player
    {
        public PlayerTwo()
        {
            this.playerGestures = PlayerHelpers.ProcessGestureList(new List<ViCharGestures> { ViCharGestures.Turning, ViCharGestures.Jumping });
        }

        public override GestureMap GetGestureMap()
        {
            GestureMap map = new GestureMap(this);

            Gesture jumping = new Gesture("jumping", 1000, (int)ViCharGestures.Jumping);
            jumping.Components.Add(new GestureComponent(JointType.HandRight, JointType.ShoulderRight,
                JointRelationship.Below, JointRelationship.Above));
            jumping.Components.Add(new GestureComponent(JointType.HandLeft, JointType.ShoulderLeft,
                JointRelationship.Below, JointRelationship.Above));
            map.Items.Add(jumping);

            Gesture turning = new Gesture("turning", 1000, (int)ViCharGestures.Turning);
            turning.Components.Add(new GestureComponent(JointType.HandLeft, JointType.ShoulderLeft,
                JointRelationship.BelowAndLeft, JointRelationship.BelowAndRight));
            map.Items.Add(turning);

            return map;
        }
    }

    public class PlayerHelpers
    {
        public static List<int> ProcessGestureList(List<ViCharGestures> gestures)
        {
            List<int> gestureIDs = new List<int>();
            foreach (ViCharGestures gesture in gestures)
            {
                gestureIDs.Add((int)gesture);
            }
            return gestureIDs;
        }
    }
}
