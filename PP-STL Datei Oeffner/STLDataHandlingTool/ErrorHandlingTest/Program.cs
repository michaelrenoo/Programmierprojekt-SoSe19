﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace ErrorHandlingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Test_Datamodel test_datamodel1 = new Test_Datamodel();
            test_datamodel1.CreateDatamodel(new DataModel.DataModel());
            
        }
    }
}