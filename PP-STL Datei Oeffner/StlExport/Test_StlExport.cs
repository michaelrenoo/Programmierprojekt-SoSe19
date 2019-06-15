﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataModel;
using System.Collections;

namespace StlExport
{
    public class Test_StlExport
    {
        // Collect all coordinates from PointList
        DataModel.DataStructure dm = new DataModel.DataStructure();
        List<Point> ListOfPoints;

        public void listCoordinates()
        {
            foreach (Point p in dm.points)
            {
                ListOfPoints.Add(p);
            }
        }

        // Collect all point-normal from Normal
        List<Normal> ListOfNormals;


        public void listNormal()
        {
            foreach (Point pts in ListOfPoints)
            {
                ListOfNormals.Add(Normal.FromPoint(pts));
            }
        }

        // Compile as one STL File
        public void AsFile(List<Point> pts, List<Normal> norms, string filePath)
        {
            //TODO: Add indentation as a method
            

            try
            {
                // Add file name and location to StreamWriter
                StreamWriter txtWriter = new StreamWriter(filePath);

                // Write an opening line of ASCII STL Data
                txtWriter.WriteLine("solid ");

                //Write the body of ASCII STL Data
                txtWriter.WriteLine("facet normal" + ListOfNormals[1]);
                txtWriter.WriteLine();


                //Close the file
                txtWriter.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
        }
    }

    public class ExportTest_DataModel
    {
        public void FillDatamodel(DataStructure dm)
        {
            dm.AddPoint(11, 22, 33);
            dm.AddPoint(20, 10, 30);
            dm.AddPoint(1, 2, 3);
            dm.AddPoint(4, 6, 8);

            dm.AddEdge(0, 1);
            dm.AddEdge(1, 2);
            dm.AddEdge(2, 3);
            dm.AddEdge(3, 0);
            dm.AddEdge(3, 1);

            Normal norm1 = new Normal(1, 1, 1);
            Normal norm2 = new Normal(1, 2, 3);


            dm.AddFace(3, 1, 2, norm1);
            dm.AddFace(0, 1, 2, norm2);
        }
    }
}