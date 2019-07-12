﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DataModel
{
    public class UserSettings
    {
        public Color BackgroundColor { get; private set; }
        public Color ForegroundColor { get; private set; }
        public Color ErrorColor { get; private set; }

        public UserSettings(Color backgroundColor, Color foregroundColor, Color errorColor)
        {
            BackgroundColor = backgroundColor;
            ForegroundColor = foregroundColor;
            ErrorColor = errorColor;
        }
    }
}
