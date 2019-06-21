﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataModel;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using StlExportDataModel;
using DataModel;

namespace StlExport
{
    public class DataWriter
    {
        // Collect all coordinates from PointList in test environment (from View)
        //DataStructure dm = new DataStructure();
        DataStructure testDM = new DataStructure();

        // Indentation as strings
        public string indent = String.Join("    ", new String[4]);
        public string indent2 = String.Join("    ", new String[8]);

        // Compile as one STL File
        // This one is as ASCII file
        public void AsAsciiFile(string File, DataStructure dataStructure) //TODO: Data Model as parameter and Exception as return type?
        {
            StreamWriter txtWriter = null;
            try
            {
                // Add file name and location to StreamWriter
                txtWriter = new StreamWriter(File, false);

                // Starting to write the data from here
                // Write an opening line of ASCII STL Data
                txtWriter.WriteLine("solid ");

                // Setting the culture info to make sure the exponents are the same
                CultureInfo current = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = current;
                Thread.CurrentThread.CurrentUICulture = current;

                //foreach (var dictionaries in dataStructure.points.m_int2pt)
                //{
                //    Point pPlus1 = dataStructure.points.GetPoint(dictionaries.Key + 1);
                //    Point pPlus2 = dataStructure.points.GetPoint(dictionaries.Key + 2);

                //    string normX = dictionaries.Value.X.ToString("E");
                //    string normY = dictionaries.Value.Y.ToString("E");
                //    string normZ = dictionaries.Value.Z.ToString("E");

                //    string iXasE = dictionaries.Value.X.ToString("E"); // for i
                //    string iYasE = dictionaries.Value.Y.ToString("E");
                //    string iZasE = dictionaries.Value.Z.ToString("E");

                //    string i1XasE = pPlus1.X.ToString("E"); // for i + 1
                //    string i1YasE = pPlus1.Y.ToString("E");
                //    string i1ZasE = pPlus1.Z.ToString("E");

                //    string i2XasE = pPlus2.X.ToString("E"); // for i + 2
                //    string i2YasE = pPlus2.Y.ToString("E");
                //    string i2ZasE = pPlus2.Z.ToString("E");


                //    //Write the body of ASCII STL Data
                //    txtWriter.WriteLine($"facet normal {normX} {normY} {normZ}");
                //    txtWriter.WriteLine(indent + "outer loop");
                //    txtWriter.WriteLine($"{indent2} vertex {iXasE} {iYasE} {iZasE}");       //coordinates point1
                //    txtWriter.WriteLine($"{indent2} vertex {i1XasE} {i1YasE} {i1ZasE}");    //coordinates point2
                //    txtWriter.WriteLine($"{indent2} vertex {i2XasE} {i2YasE} {i2ZasE}");    //coordinates point3
                //    txtWriter.WriteLine(indent + "endloop");
                //    txtWriter.WriteLine("endfacet");
                //}

                for (int i = 0; i < dataStructure.points.int2pts.Count; i = i + 3)
                {
                    //All normal and points as e-sign exponent format
                    Face newFace = dataStructure.faces.Int2Faces[i];
                    Normal norm = newFace.NormFromFace(dataStructure.faces.Int2Faces[i]);

                    string nXasE = norm.X.ToString("E"); // Normals can also be {0; 0; 0}
                    string nYasE = norm.Y.ToString("E");
                    string nZasE = norm.Z.ToString("E");

                    string iXasE = dataStructure.points.int2pts[i].X.ToString("E"); // for i
                    string iYasE = dataStructure.points.int2pts[i].Y.ToString("E");
                    string iZasE = dataStructure.points.int2pts[i].Z.ToString("E");

                    string i1XasE = dataStructure.points.int2pts[i + 1].X.ToString("E"); // for i + 1
                    string i1YasE = dataStructure.points.int2pts[i + 1].Y.ToString("E");
                    string i1ZasE = dataStructure.points.int2pts[i + 1].Z.ToString("E");

                    string i2XasE = dataStructure.points.int2pts[i + 2].X.ToString("E"); // for i + 2
                    string i2YasE = dataStructure.points.int2pts[i + 2].Y.ToString("E");
                    string i2ZasE = dataStructure.points.int2pts[i + 2].Z.ToString("E");

                    //Write the body of ASCII STL Data
                    txtWriter.WriteLine($"facet normal {nXasE} {nYasE} {nZasE}");
                    txtWriter.WriteLine(indent + "outer loop");
                    txtWriter.WriteLine($"{indent2} vertex {iXasE} {iYasE} {iZasE}");       //coordinates point1
                    txtWriter.WriteLine($"{indent2} vertex {i1XasE} {i1YasE} {i1ZasE}");    //coordinates point2
                    txtWriter.WriteLine($"{indent2} vertex {i2XasE} {i2YasE} {i2ZasE}");    //coordinates point3
                    txtWriter.WriteLine(indent + "endloop");
                    txtWriter.WriteLine("endfacet");
                }

                // Finish the file
                txtWriter.Write("endsolid");

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                //Close the file
                if (txtWriter != null) txtWriter.Close();
            }
        }

        // This one is as binary file
        public void AsBinaryFile(string File, DataStructure dataStructure)
        {
            // Create new BinaryWriter
            using (var txtWriter = new BinaryWriter(System.IO.File.OpenWrite(File), Encoding.ASCII))
            {
                try
                {
                    // Setting the culture info to make sure the exponents are the same
                    CultureInfo current = new CultureInfo("en-US");
                    Thread.CurrentThread.CurrentCulture = current;
                    Thread.CurrentThread.CurrentUICulture = current;

                    // Encode the header of the binary file as ASCII and set the buffer to 80 bytes
                    string HeaderAsString = File;
                    byte[] Header = new byte[80];
                    Encoding.ASCII.GetBytes(HeaderAsString, 0, HeaderAsString.Length, Header, 0);
                    txtWriter.Write(Header);

                    // UINT32 – Number of triangles
                    uint totalTriangles = ((UInt32) dataStructure.points.int2pts.Count / 3); // A triangle consists of 3 points
                    txtWriter.Write(totalTriangles.ToString("E"));

                    // foreach triangle
                    // for - loop is used to get a better iteration over the points
                    for (int i = 0; i < dataStructure.points.int2pts.Count; i = i + 3)
                    {
                        //All normal and points as e-sign exponent format
                        Face newFace = dataStructure.faces.Int2Faces[i];
                        Normal norm = newFace.NormFromFace(dataStructure.faces.Int2Faces[i]);

                        uint nX = (UInt32) norm.X;               // Normals can also be {0; 0; 0}
                        uint nY = (UInt32)norm.Y;
                        uint nZ = (UInt32)norm.Z;
                        string nXasE = nX.ToString("E2");
                        string nYasE = nY.ToString("E2");
                        string nZasE = nZ.ToString("E2");

                        uint iX = (UInt32)dataStructure.points.int2pts[i].X; // for i
                        uint iY = (UInt32)dataStructure.points.int2pts[i].Y;
                        uint iZ = (UInt32)dataStructure.points.int2pts[i].Z;
                        string iXasE = iX.ToString("E2");
                        string iYasE = iY.ToString("E2");
                        string iZasE = iZ.ToString("E2");

                        uint i1X = (UInt32)dataStructure.points.int2pts[i + 1].X; // for i + 1
                        uint i1Y = (UInt32)dataStructure.points.int2pts[i + 1].Y;
                        uint i1Z = (UInt32)dataStructure.points.int2pts[i + 1].Z;
                        string i1XasE = i1X.ToString("E2");
                        string i1YasE = i1Y.ToString("E2");
                        string i1ZasE = i1Z.ToString("E2");

                        uint i2X = (UInt32)dataStructure.points.int2pts[i + 2].X;
                        uint i2Y = (UInt32)dataStructure.points.int2pts[i + 2].Y;
                        uint i2Z = (UInt32)dataStructure.points.int2pts[i + 2].Z;
                        string i2XasE = i2X.ToString("E2"); // for i + 2
                        string i2YasE = i2Y.ToString("E2");
                        string i2ZasE = i2Z.ToString("E2");

                        //Write the body of binary STL Data
                        // REAL32[3] four times - normal, vertex 1, vertex 2, vertex 3
                        txtWriter.Write("{nXasE} {nYasE} {nZasE} {iXasE} {iYasE} {iZasE} {i1XasE} {i1YasE} {i1ZasE} {i2XasE} {i2YasE} {i2ZasE} 0 ");

                        //// REAL32[3] – Normal vector
                        //txtWriter.Write($"{nXasE} {nYasE} {nZasE} ");
                        //// REAL32[3] – Vertex 1
                        //txtWriter.Write($"{iXasE} {iYasE} {iZasE} ");
                        //// REAL32[3] – Vertex 2
                        //txtWriter.Write($"{i1XasE} {i1YasE} {i1ZasE} ");
                        //// REAL32[3] – Vertex 3
                        //txtWriter.Write($"{i2XasE} {i2YasE} {i2ZasE} ");
                        //// UINT16 – Attribute byte count = normally 0
                        //txtWriter.Write(0);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}