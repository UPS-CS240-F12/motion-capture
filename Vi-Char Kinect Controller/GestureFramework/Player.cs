//------------------------------------------------------------------------------
// <copyright file="Player.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace GestureFramework
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Microsoft.Kinect;

    public class Player
    {
        private const double BoneSize = 0.01;
        private const double HeadSize = 0.075;
        private const double HandSize = 0.03;

        // Keeping track of all bone segments of interest as well as head, hands and feet
        private readonly Dictionary<Bone, BoneData> segments = new Dictionary<Bone, BoneData>();
        private readonly int id;
        private Rect playerBounds;
        private System.Windows.Point playerCenter;
        private double playerScale;

        public Player(int skeletonSlot)
        {
            this.id = skeletonSlot;
            this.LastUpdated = DateTime.Now;
        }

        public bool IsAlive { get; set; }

        public DateTime LastUpdated { get; set; }

        public Dictionary<Bone, BoneData> Segments
        {
            get
            {
                return this.segments;
            }
        }

        public int GetId()
        {
            return this.id;
        }

        public void SetBounds(Rect r)
        {
            this.playerBounds = r;
            this.playerCenter.X = (this.playerBounds.Left + this.playerBounds.Right) / 2;
            this.playerCenter.Y = (this.playerBounds.Top + this.playerBounds.Bottom) / 2;
            this.playerScale = Math.Min(this.playerBounds.Width, this.playerBounds.Height / 2);
        }

        public void UpdateBonePosition(Microsoft.Kinect.JointCollection joints, JointType j1, JointType j2)
        {
            var seg = new Segment(
                (joints[j1].Position.X * this.playerScale) + this.playerCenter.X,
                this.playerCenter.Y - (joints[j1].Position.Y * this.playerScale),
                (joints[j2].Position.X * this.playerScale) + this.playerCenter.X,
                this.playerCenter.Y - (joints[j2].Position.Y * this.playerScale))
                { Radius = Math.Max(3.0, this.playerBounds.Height * BoneSize) / 2 };
            this.UpdateSegmentPosition(j1, j2, seg);
        }

        public void UpdateJointPosition(Microsoft.Kinect.JointCollection joints, JointType j)
        {
            var seg = new Segment(
                (joints[j].Position.X * this.playerScale) + this.playerCenter.X,
                this.playerCenter.Y - (joints[j].Position.Y * this.playerScale))
                { Radius = this.playerBounds.Height * ((j == JointType.Head) ? HeadSize : HandSize) / 2 };
            this.UpdateSegmentPosition(j, j, seg);
        }

        private void UpdateSegmentPosition(JointType j1, JointType j2, Segment seg)
        {
            var bone = new Bone(j1, j2);
            if (this.segments.ContainsKey(bone))
            {
                BoneData data = this.segments[bone];
                data.UpdateSegment(seg);
                this.segments[bone] = data;
            }
            else
            {
                this.segments.Add(bone, new BoneData(seg));
            }
        }
    }
}
