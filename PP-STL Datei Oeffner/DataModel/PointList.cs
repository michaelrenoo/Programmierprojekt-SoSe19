﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class PointList
    {
        private int m_idx = 0; // Starting index
        private Dictionary<int, Point> m_int2pt = new Dictionary<int, Point>(); // register point if ID is given
        private Dictionary<Point, int> m_pt2int = new Dictionary<Point, int>(); // register point if point is given

        /* User adds or gets point if a 'point' variable is given */
        public int AddOrGetPoint(Point p)
        {
            /* If a point's already registered in dictionary and contains an ID */
            if (m_pt2int.ContainsKey(p))
                return m_pt2int[p];

            //noch nicht in Liste enthalten...
            m_int2pt[m_idx] = p;
            m_pt2int[p] = m_idx;
            return m_idx++;
        }

        /* User defines a point by giving an ID to this methode as parameter */
        public Point GetPoint(int id)
        {
            if (m_int2pt.ContainsKey(id))
                return m_int2pt[id];
            return null;
        }
    }
}
