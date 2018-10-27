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
        public static Random Number = new Random();
        public static int GeneralRandomFunction(int Startindex, int Endindex)
        {
            lock(Number)
            return Number.Next(Startindex, Endindex);
        }
    }
}
