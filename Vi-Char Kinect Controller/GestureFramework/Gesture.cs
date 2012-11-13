using System;
using System.Collections.Generic;

namespace GestureFramework
{
    public class Gesture
    {
        private readonly Guid id;
        private readonly List<GestureComponent> components;
        private readonly String description;
        private readonly int gesture_id;


        public Gesture(string name, int maxExecutionTime, int gesture_id)
        {
            id = Guid.NewGuid();
            this.description = name;
            components = new List<GestureComponent>();
            this.gesture_id = gesture_id;
            MaximumExecutionTime = maxExecutionTime;
        }


        public Gesture()
        {
            id = Guid.NewGuid();
            components = new List<GestureComponent>();
        }


        public Guid Id
        {
            get
            {
                return id;
            }
        }

        public String Description
        {
            get
            {
                return description;
            }
        }

        public int MaximumExecutionTime
        {
            get;
            set;
        }

        public List<GestureComponent> Components
        {
            get
            {
                return components;
            }
        }

        public int GestureID
        {
            get
            {
                return gesture_id;
            }
        }
    }
}

