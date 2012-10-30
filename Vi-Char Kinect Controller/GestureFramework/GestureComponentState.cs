using Coding4Fun.Kinect.WinForm;
using Microsoft.Kinect;

namespace GestureFramework
{

    public class GestureComponentState
    {
        private readonly GestureComponent _component;
        private bool _beginningRelationshipSatisfied;
        private bool _endingRelationshipSatisfied;


        public bool BeginningRelationshipSatisfied
        {
            get
            {
                if (_component.BeginningRelationship == JointRelationship.None)
                    _beginningRelationshipSatisfied = true;
                return _beginningRelationshipSatisfied;
            }
        }


        public bool EndingRelationshipSatisfied
        {
            get
            {
                if (_component.EndingRelationship == JointRelationship.None)
                    _endingRelationshipSatisfied = true;
                return _endingRelationshipSatisfied;
            }
        }

        public GestureComponent Component
        {
            get
            {
                return _component;
            }
        }


        public GestureComponentState(GestureComponent component)
        {
            _component = component;
            Reset();
        }


        public void Reset()
        {
            _beginningRelationshipSatisfied = false;
            _endingRelationshipSatisfied = false;
        }

        //Checks to see if the two joints are in either of the two states. Unless the beginning relationship has
        //been satisifed, it will not check the ending relationship.
        public bool Evaluate(Skeleton skeleton, int xScale, int yScale)
        {
            var sjoint1 = skeleton.Joints[_component.Joint1].ScaleTo(xScale, yScale);
            var sjoint2 = skeleton.Joints[_component.Joint2].ScaleTo(xScale, yScale);

            if (!BeginningRelationshipSatisfied)
            {
                var goodtogo = CompareJointRelationship(sjoint1, sjoint2, _component.BeginningRelationship);
                if (goodtogo)
                {
                    _beginningRelationshipSatisfied = true;
                }
                else
                {
                    return false;
                }
            }


            if (!EndingRelationshipSatisfied)
            {
                var goodtogo = CompareJointRelationship(sjoint1, sjoint2, _component.EndingRelationship);
                if (goodtogo)
                {
                    return _endingRelationshipSatisfied = true;
                }
                return false;
            }

            return true;
        }


        private bool CompareJointRelationship(Joint inJoint1, Joint inJoint2, JointRelationship relation)
        {
            switch (relation)
            {
                case JointRelationship.None:
                    return true;

                case JointRelationship.Below:
                    return inJoint1.Position.Y > inJoint2.Position.Y;
                case JointRelationship.Above:
                    return inJoint1.Position.Y < inJoint2.Position.Y;
                case JointRelationship.LeftOf:
                    return inJoint1.Position.X < inJoint2.Position.X;
                case JointRelationship.RightOf:
                    return inJoint1.Position.X > inJoint2.Position.X;
                case JointRelationship.Front:
                    return inJoint1.Position.Z < inJoint2.Position.Z;
                case JointRelationship.Behind:
                    return inJoint1.Position.Z > inJoint2.Position.Z;

                case JointRelationship.AboveAndLeft:
                    return ((inJoint1.Position.X < inJoint2.Position.X) && (inJoint1.Position.Y < inJoint2.Position.Y));
                case JointRelationship.AboveAndRight:
                    return ((inJoint1.Position.X > inJoint2.Position.X) && (inJoint1.Position.Y < inJoint2.Position.Y));
                case JointRelationship.BelowAndLeft:
                    return ((inJoint1.Position.X < inJoint2.Position.X) && (inJoint1.Position.Y > inJoint2.Position.Y));
                case JointRelationship.BelowAndRight:
                    return ((inJoint1.Position.X > inJoint2.Position.X) && (inJoint1.Position.Y > inJoint2.Position.Y));

                case JointRelationship.FrontAbove:
                    return ((inJoint1.Position.Z < inJoint2.Position.Z) && (inJoint1.Position.Y < inJoint2.Position.Y));
                case JointRelationship.FrontBelow:
                    return ((inJoint1.Position.Z < inJoint2.Position.Z) && (inJoint1.Position.Y > inJoint2.Position.Y));
                case JointRelationship.FrontLeftOf:
                    return ((inJoint1.Position.Z < inJoint2.Position.Z) && (inJoint1.Position.X < inJoint2.Position.X));
                case JointRelationship.FrontRightOf:
                    return ((inJoint1.Position.Z < inJoint2.Position.Z) && (inJoint1.Position.X > inJoint2.Position.X));

                case JointRelationship.FrontAboveAndRight:
                    return ((inJoint1.Position.Z < inJoint2.Position.Z) && (inJoint1.Position.Y < inJoint2.Position.Y) && (inJoint1.Position.X > inJoint2.Position.X));
                case JointRelationship.FrontBelowAndRight:
                    return ((inJoint1.Position.Z < inJoint2.Position.Z) && (inJoint1.Position.Y > inJoint2.Position.Y) && (inJoint1.Position.X > inJoint2.Position.X));
                case JointRelationship.FrontAboveAndLeft:
                    return ((inJoint1.Position.Z < inJoint2.Position.Z) && (inJoint1.Position.Y < inJoint2.Position.Y) && (inJoint1.Position.X < inJoint2.Position.X));
                case JointRelationship.FrontBelowAndLeft:
                    return ((inJoint1.Position.Z < inJoint2.Position.Z) && (inJoint1.Position.Y > inJoint2.Position.Y) && (inJoint1.Position.X < inJoint2.Position.X));

                case JointRelationship.BehindAbove:
                    return ((inJoint1.Position.Z > inJoint2.Position.Z) && (inJoint1.Position.Y < inJoint2.Position.Y));
                case JointRelationship.BehindBelow:
                    return ((inJoint1.Position.Z > inJoint2.Position.Z) && (inJoint1.Position.Y > inJoint2.Position.Y));
                case JointRelationship.BehindLeftOf:
                    return ((inJoint1.Position.Z > inJoint2.Position.Z) && (inJoint1.Position.X < inJoint2.Position.X));
                case JointRelationship.BehindRightOf:
                    return ((inJoint1.Position.Z > inJoint2.Position.Z) && (inJoint1.Position.X > inJoint2.Position.X));

                case JointRelationship.BehindAboveAndRight:
                    return ((inJoint1.Position.Z > inJoint2.Position.Z) && (inJoint1.Position.Y < inJoint2.Position.Y) && (inJoint1.Position.X > inJoint2.Position.X));
                case JointRelationship.BehindBelowAndRight:
                    return ((inJoint1.Position.Z > inJoint2.Position.Z) && (inJoint1.Position.Y > inJoint2.Position.Y) && (inJoint1.Position.X > inJoint2.Position.X));
                case JointRelationship.BehindAboveAndLeft:
                    return ((inJoint1.Position.Z > inJoint2.Position.Z) && (inJoint1.Position.Y < inJoint2.Position.Y) && (inJoint1.Position.X < inJoint2.Position.X));
                case JointRelationship.BehindBelowAndLeft:
                    return ((inJoint1.Position.Z > inJoint2.Position.Z) && (inJoint1.Position.Y > inJoint2.Position.Y) && (inJoint1.Position.X < inJoint2.Position.X));
            }
            return false;
        }

    }
}
