using System;
using System.Collections.Generic;

namespace GestureFramework
{
    public class Gesture
    {
        private readonly Guid id;
        private readonly List<GestureComponent> components;
        private readonly String description;
        private readonly GestureType type;


        public Gesture(string name, int maxExecutionTime, GestureType type)
        {
            id = Guid.NewGuid();
            this.description = name;
            components = new List<GestureComponent>();
            this.type = type;
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

        public GestureType Type
        {
            get
            {
                return type;
            }
        }
    }
}

