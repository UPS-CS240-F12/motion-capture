using System.Collections.Generic;
using Microsoft.Kinect;


namespace GestureFramework
{
    public class GestureMap
    {
        public List<Gesture> Items;
        public int GestureResetTimeout;
        public GestureType Type;


        public GestureMap()
        {
            Items = new List<Gesture>();
            InitializeGestures();
        }

        private void InitializeGestures()
        {
            //Gesture template = new Gesture("gesture_name", Tiemout_Value, GestureType.Type_Of_Gesture);
            //template.Components.Add(new GestureComponent(JointType.FirstJoint, JointType.SecondJoint, 
            //    JointRelationship.EndingRelationship, JointRelationship.BeginningRelationship));
            //Items.Add(template);
            
            Gesture template = new Gesture("pointing", 1000, GestureType.Pointing);
            template.Components.Add(new GestureComponent(JointType.HandRight, JointType.Head, 
                JointRelationship.FrontBelow, JointRelationship.BehindAbove));
            Items.Add(template);

            Gesture moving = new Gesture("moving", 1000, GestureType.Moving);
            moving.Components.Add(new GestureComponent(JointType.HandRight, JointType.ElbowRight, 
                JointRelationship.AboveAndLeft, JointRelationship.AboveAndRight));
            Items.Add(moving);

            Gesture jumping = new Gesture("jumping", 1000, GestureType.Jumping);
            jumping.Components.Add(new GestureComponent(JointType.HandRight, JointType.ShoulderRight,
                JointRelationship.Below, JointRelationship.Above));
            jumping.Components.Add(new GestureComponent(JointType.HandLeft, JointType.ShoulderLeft,
                JointRelationship.Below, JointRelationship.Above));
            Items.Add(jumping);

            Gesture turning = new Gesture("turning", 1000, GestureType.Turning);
            turning.Components.Add(new GestureComponent(JointType.HandLeft, JointType.ShoulderLeft, 
                JointRelationship.BelowAndLeft, JointRelationship.BelowAndRight));
            Items.Add(turning);
        }

        
    }

    public enum GestureType
    {
        Moving,
        Jumping,
        Turning,
        Pointing,
        None
    }
}
