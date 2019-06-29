﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Threading;
using System.Globalization;
using System.Reflection;


namespace importSTL
{
    public class DataReader
    {
        public string path;
        private enum FileType { NONE, BINARY, ASCII };
        private bool processError;

        // Convert bytes to double
        private static double ConvertByteToDouble(byte[] b)
        {
            return BitConverter.ToDouble(b, 0);
            
        }

        public void ReadBinaryFile(string stlPath)
        {
            DataModel.DataStructure dm = new DataModel.DataStructure();
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");

            byte[] binaryParts = File.ReadAllBytes(stlPath); // Opens the STL Binary file 

            DataModel.Normal normal = null;
            int byteIdx = 0;

            int nr = BitConverter.ToInt32(binaryParts, 81); // Indicating the Number of Triangles
            byteIdx = 84;

            // The process starts here
            for (int i = 0; i < binaryParts.Length; i++)
            {
                int start = 50 * i + 81 + 4;

                try
                {
                    // Showing face normal
                    DataModel.Point n = new DataModel.Point(BitConverter.ToSingle(binaryParts, start), BitConverter.ToSingle(binaryParts, start + 4), BitConverter.ToSingle(binaryParts, start + 8));
                    normal = new DataModel.Normal(n.X, n.Y, n.Z);
                    byteIdx = start + 12;

                    // Vertex 1
                    int point1 = start + 12;
                    DataModel.Point v1 = new DataModel.Point(BitConverter.ToSingle(binaryParts, point1), BitConverter.ToSingle(binaryParts, point1 + 4), BitConverter.ToSingle(binaryParts, point1 + 8));
                    byteIdx = point1 + 12;

                    // Vertex 2
                    int point2 = point1 + 12;
                    DataModel.Point v2 = new DataModel.Point(BitConverter.ToSingle(binaryParts, point2), BitConverter.ToSingle(binaryParts, point2 + 4), BitConverter.ToSingle(binaryParts, point2 + 8));
                    byteIdx = point2 + 12;

                    // Vertex 3
                    int point3 = point2 + 12;
                    DataModel.Point v3 = new DataModel.Point(BitConverter.ToSingle(binaryParts, point3), BitConverter.ToSingle(binaryParts, point3 + 4), BitConverter.ToSingle(binaryParts, point3 + 8));
                    byteIdx = point3 + 12;

                    //Punkte in pointlist, face erzeugen
                    int p1 = dm.points.AddOrGetPoint(v1);
                    int p2 = dm.points.AddOrGetPoint(v2);
                    int p3 = dm.points.AddOrGetPoint(v3);

                    DataModel.Edge e1 = new DataModel.Edge(p1, p2, dm);
                    DataModel.Edge e2 = new DataModel.Edge(p1, p3, dm);
                    DataModel.Edge e3 = new DataModel.Edge(p2, p3, dm);

                    int ei1 = dm.edges.AddOrGetEdge(e1);
                    int ei2 = dm.edges.AddOrGetEdge(e2);
                    int ei3 = dm.edges.AddOrGetEdge(e3);

                    dm.AddFace(ei1, ei2, ei3, normal);

                    byteIdx = 134;
                }

                catch
                {
                    processError = true;
                    break;
                }


            }

            int abc = BitConverter.ToUInt16(binaryParts, 133); // Attribute byte count
            byteIdx = 0;

            Thread.CurrentThread.CurrentCulture = ci;


        }


        // Register points from text (string)
        private DataModel.Point FromStrings(string s1, string s2, string s3)
        {
            if (double.TryParse(s1, out double d1))
                if (double.TryParse(s2, out double d2))
                    if (double.TryParse(s3, out double d3))
                    {
                        return new DataModel.Point(d1, d2, d3);
                    }
            return null;

        }

        public void ReadASCIIFile(string stlPath)
        {
            DataModel.DataStructure dm = new DataModel.DataStructure();
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");

            string[] lines = File.ReadAllLines(stlPath); // Opens the STL ASCII file and read all lines of the file

            // defining all the components for the data structure
            DataModel.Normal normal = null;
            DataModel.Point[] points = new DataModel.Point[3];
            int idxPoint = -1;

            // Process of reading ASCII Data starts here
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                string[] parts = line.Split(' '); // each word in ASCII file structure will be represented as part
                if (parts.Length == 0) continue;

                try
                {
                    switch (parts[0])
                    {
                        case "facet":
                            // invalid if the line 'facet' doesn't consist of 5 parts (words)
                            if (parts.Length != 5)
                            {
                                processError = true;
                            }
                            // if it's valid, normal will be added
                            DataModel.Point n = FromStrings(parts[2], parts[3], parts[4]);
                            normal = new DataModel.Normal(n.X, n.Y, n.Z);
                            idxPoint = 0;
                            break;
                        case "vertex":
                            // invalid if the line 'vertex' doesn't consist of 4 parts (words)
                            if (parts.Length != 4)
                            {
                                processError = true;
                            }
                            // invalid if it's out of range
                            if (idxPoint > 2 || idxPoint == -1)
                            {
                                processError = true;
                            }
                            //if it's valid, points will be registered from the first vertex and continues until the 3rd vertex
                            points[idxPoint++] = FromStrings(parts[1], parts[2], parts[3]);
                            break;

                        case "endfacet":
                            if (idxPoint != 3)
                            { }

                            //Punkte in pointlist, face erzeugen
                            int p1 = dm.points.AddOrGetPoint(points[0]);
                            int p2 = dm.points.AddOrGetPoint(points[1]);
                            int p3 = dm.points.AddOrGetPoint(points[2]);

                            DataModel.Edge e1 = new DataModel.Edge(p1, p2, dm);
                            DataModel.Edge e2 = new DataModel.Edge(p1, p3, dm);
                            DataModel.Edge e3 = new DataModel.Edge(p2, p3, dm);

                            int ei1 = dm.edges.AddOrGetEdge(e1);
                            int ei2 = dm.edges.AddOrGetEdge(e2);
                            int ei3 = dm.edges.AddOrGetEdge(e3);

                            dm.AddFace(ei1, ei2, ei3, normal);
                            idxPoint = -1;
                            break;

                        default:
                            //no information(?)
                            break;
                    }
                }

                catch
                {
                    processError = true;
                    break;
                }

            }

            Thread.CurrentThread.CurrentCulture = ci;
        }

    }
}