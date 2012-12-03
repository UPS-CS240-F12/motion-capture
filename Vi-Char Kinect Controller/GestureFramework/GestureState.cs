using System;
using System.Collections.Generic;
using Microsoft.Kinect;

namespace GestureFramework
{

    public class GestureState
    {
        private DateTime _beginExecutionTime;
        private readonly Gesture _gesture;


        public Gesture Gesture
        {
            get
            {
                return _gesture;
            }
        }


        public int GestureID
        {
            get
            {
                return _gesture.GestureID;
            }
        }


        public bool IsExecuting
        {
            get;
            private set;
        }

        public bool HasError
        {
            get;
            private set;
        }

        public List<GestureComponentState> ComponentStates
        {
            get;
            set;
        }

        public GestureState(Gesture gesture)
        {
            _gesture = gesture;
            ComponentStates = new List<GestureComponentState>();
            InitializeComponents();
            IsExecuting = false;
            HasError = false;
        }


        public void InitializeComponents()
        {
            foreach (var component in _gesture.Components)
            {
                var state = new GestureComponentState(component);
                ComponentStates.Add(state);
            }
        }


        public void Reset()
        {
            foreach (var component in ComponentStates)
            {
                component.Reset();
            }

            IsExecuting = false;
            _beginExecutionTime = DateTime.MinValue;
        }

        //Determines whether a gesture is complete or has timed out
        public bool Evaluate(Skeleton sd, DateTime currentTime, int xScale, int yScale)
        {

            if (IsExecuting)
            {
                TimeSpan executiontime = currentTime - _beginExecutionTime;
                if (executiontime.TotalMilliseconds > _gesture.MaximumExecutionTime && _gesture.MaximumExecutionTime > 0)
                {
                    HasError = true;
                    Reset();

                    return false;
                }
            }

            foreach (var component in ComponentStates)
            {
                component.Evaluate(sd, xScale, yScale);
            }

            var inflightcount = 0;
            var completecount = 0;

            foreach (var component in ComponentStates)
            {
                if (component.BeginningRelationshipSatisfied)
                    inflightcount++;
                if (component.EndingRelationshipSatisfied)
                    completecount++;
            }


            if (completecount >= ComponentStates.Count && IsExecuting)
            {
                HasError = false;
                Reset();
                return true;
            }

            if (inflightcount >= ComponentStates.Count)
            {
                if (!IsExecuting)
                {
                    IsExecuting = true;
                    HasError = false;
                    _beginExecutionTime = DateTime.Now;
                    return false;
                }
            }

            return false;
        }

    }
}
