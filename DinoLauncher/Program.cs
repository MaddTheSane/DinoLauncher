﻿using Eto.Forms;
using System;

namespace DinoLauncher
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            new Application(Eto.Platform.Detect).Run(new MainForm());
        }
    }
}