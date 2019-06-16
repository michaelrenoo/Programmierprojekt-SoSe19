﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Edge
    {
        /// <summary>
        /// potentiallyFaulty, faulty and FaceIDs. Implemented by Maximilian
        /// </summary>
        public bool potentiallyFaulty;
        public bool faulty;
        public IList<int> FaceIDs = new List<int>();

        public Edge(int startPoint, int endPoint, DataStructure model)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            this.model = model;
        }

        private DataStructure model;
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        
        public Point P1
        {
            get
            {
                return model.points.GetPoint(StartPoint);
            }
        }
        public Point P2
        {
            get
            {
                return model.points.GetPoint(EndPoint);
            }
        }

        // Doesn't work yet. Still need improvement
        public override int GetHashCode()
        {

            return StartPoint.GetHashCode() ^ EndPoint.GetHashCode() ^ model.GetHashCode();
        }


        // Two edges would not be counted as two edges if they both have similar point IDs
        // Two edges with two almost similar pointIDs (e.g. 1,2 and 2,1) are equal
        public override bool Equals(object obj)
        {
            var edge = obj as Edge;
            if (obj == null) return false;
            if (edge.StartPoint == StartPoint ^ edge.StartPoint == EndPoint && edge.EndPoint == EndPoint ^ edge.EndPoint == StartPoint)
            {
                return edge.StartPoint == StartPoint ^ edge.StartPoint == EndPoint && edge.EndPoint == EndPoint ^ edge.EndPoint == StartPoint;
            }


            return false;
        }

    }
}
