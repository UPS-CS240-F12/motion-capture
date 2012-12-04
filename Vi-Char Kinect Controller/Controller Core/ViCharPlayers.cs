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
            this.playerGestures = PlayerHelpers.ProcessGestureList(new List<ViCharGesture> { ViCharGesture.Moving, ViCharGesture.Jumping });
        }

        public override GestureMap GetGestureMap()
        {
            GestureMap map = new GestureMap();

            Gesture moving = new Gesture("moving", 1000, (int)ViCharGesture.Moving);
            moving.Components.Add(new GestureComponent(JointType.HandRight, JointType.ElbowRight,
                JointRelationship.AboveAndLeft, JointRelationship.AboveAndRight));
            map.Items.Add(moving);

            Gesture jumping = new Gesture("jumping", 1000, (int)ViCharGesture.Jumping);
            jumping.Components.Add(new GestureComponent(JointType.HandRight, JointType.ShoulderRight,
                JointRelationship.Below, JointRelationship.Above));
            jumping.Components.Add(new GestureComponent(JointType.HandLeft, JointType.ShoulderLeft,
                JointRelationship.Below, JointRelationship.Above));
            map.Items.Add(jumping);

            return map;
        }

        public override string ToString()
        {
            return "Player One";
        }
    }

    public class PlayerTwo : Player
    {
        public PlayerTwo()
        {
            this.playerGestures = PlayerHelpers.ProcessGestureList(new List<ViCharGesture> { ViCharGesture.TurningLeft, ViCharGesture.TurningRight, ViCharGesture.Jumping });
        }

        public override GestureMap GetGestureMap()
        {
            GestureMap map = new GestureMap();

            Gesture jumping = new Gesture("jumping", 1000, (int)ViCharGesture.Jumping);
            jumping.Components.Add(new GestureComponent(JointType.HandRight, JointType.ShoulderRight,
                JointRelationship.Below, JointRelationship.Above));
            jumping.Components.Add(new GestureComponent(JointType.HandLeft, JointType.ShoulderLeft,
                JointRelationship.Below, JointRelationship.Above));
            map.Items.Add(jumping);

            Gesture turningLeft = new Gesture("turningLeft", 1000, (int)ViCharGesture.TurningLeft);
            turningLeft.Components.Add(new GestureComponent(JointType.KneeRight, JointType.KneeLeft,
                JointRelationship.AboveAndLeft));
            map.Items.Add(turningLeft);

            Gesture turningRight = new Gesture("turningRight", 1000, (int)ViCharGesture.TurningRight);
            turningRight.Components.Add(new GestureComponent(JointType.KneeLeft, JointType.KneeRight,
                JointRelationship.AboveAndRight));
            map.Items.Add(turningRight);

            return map;
        }

        public override string ToString()
        {
            return "Player Two";
        }
    }

    public class PlayerHelpers
    {
        public static List<int> ProcessGestureList(List<ViCharGesture> gestures)
        {
            List<int> gestureIDs = new List<int>();
            foreach (ViCharGesture gesture in gestures)
            {
                gestureIDs.Add((int)gesture);
            }
            return gestureIDs;
        }
    }
}
