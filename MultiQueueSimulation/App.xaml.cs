using MultiQueueModels;
using MultiQueueTesting;
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
        public static List<TimeDistribution> InterarrivalDistribution = new List<TimeDistribution>();

    }
}
