using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueSimulation.ViewModels
{
    class ResultsViewModel
    {
        public static int TotalNumberOfCustomers = App.SimulationSystem.StoppingNumber;
        public static int TotalServiceTime()
        {
            int Service_Time = 0;
            for (int i = 0; i < TotalNumberOfCustomers; i++)
            {
                Service_Time += App.SimulationSystem.SimulationTable.ElementAt(i).ServiceTime;
            }

            return Service_Time;
        }
        public static int TotalRunTime = App.SimulationSystem.SimulationTable.ElementAt(TotalNumberOfCustomers).EndTime;
        public static int TotalIdleTime = TotalRunTime - TotalServiceTime();

        public static int NumberOfCustomersWaited()
        {
            int CustomersWaited = 0;
            for (int i = 0; i < TotalNumberOfCustomers; i++)
            {
                if ((App.SimulationSystem.SimulationTable.ElementAt(i).TimeInQueue) != 0)
                    CustomersWaited++;
            }
            return CustomersWaited;
        }
        public static int TotalTimeCustomersWaited()
        {
            int TimeWaited = 0;
            for (int i = 0; i < TotalNumberOfCustomers; i++)
            {
                TimeWaited += App.SimulationSystem.SimulationTable.ElementAt(i).TimeInQueue;
            }

            return TimeWaited;
        }
        /////////Performance Measures per server////////
        public float ProbabilityOfIdleServer = TotalIdleTime / TotalRunTime;
        public float AverageServiceTime = TotalServiceTime() / TotalNumberOfCustomers;
        public float Utilization = TotalServiceTime() / TotalRunTime;

        /////////System OutPut Performance Measures//////
        public float AverageWaitingTime = TotalTimeCustomersWaited() / TotalNumberOfCustomers;
        public float PrababilityOfWaiting = NumberOfCustomersWaited() / TotalNumberOfCustomers;

    }
}
