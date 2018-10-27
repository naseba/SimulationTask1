using MultiQueueModels;
using System;
using System.Collections.Generic;
using System.Windows;

namespace MultiQueueSimulation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SimulationSystem SimulationSystem = new SimulationSystem();
        public static Queue<char> Que = new Queue<char>();
        public static int GeneralRandomFunction(int Startindex, int Endindex)
        {
            Random Number = new Random();
            return Number.Next(Startindex, Endindex);
        }
    }
}
