using System.Collections.Generic;
using Microsoft.Kinect;


namespace GestureFramework
{
    public class GestureMap
    {
        public List<Gesture> Items;
        public int GestureResetTimeout;


        public GestureMap(Player p)
        {
            Items = new List<Gesture>();
            InitializeGestures(p);
        }

        private void InitializeGestures(Player p)
        {
            //Gesture template = new Gesture("gesture_name", Tiemout_Value, GestureType.Type_Of_Gesture);
            //template.Components.Add(new GestureComponent(JointType.FirstJoint, JointType.SecondJoint, 
            //    JointRelationship.EndingRelationship, JointRelationship.BeginningRelationship));
            //Items.Add(template);

        }

        
    }
}
