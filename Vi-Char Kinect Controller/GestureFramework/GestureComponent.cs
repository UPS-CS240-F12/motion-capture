using Microsoft.Kinect;

namespace GestureFramework
{
    /// <summary>
    /// Describes a single relationship between two joints.
    /// The relationship has two joints and two parts;
    /// a beginning relationship and an ending relationship
    /// </summary>
    public class GestureComponent
    {

        public JointType Joint1
        {
            get;
            set;
        }

        public JointType Joint2
        {
            get;
            set;
        }

        public JointRelationship BeginningRelationship
        {
            get;
            set;
        }


        public JointRelationship EndingRelationship
        {
            get;
            set;
        }

        public GestureComponent(JointType joint1, JointType joint2, JointRelationship endingRelationship, JointRelationship beginningRelationship = JointRelationship.None)
        {
            Joint1 = joint1;
            Joint2 = joint2;

            EndingRelationship = endingRelationship;
            BeginningRelationship = beginningRelationship;
        }
    }
}
