using System.Collections.Generic;
using Microsoft.Kinect;


namespace GestureFramework
{
    public class GestureMap
    {
        public List<Gesture> Items;

        public GestureMap()
        {
            Items = new List<Gesture>();
        }
    }
}
