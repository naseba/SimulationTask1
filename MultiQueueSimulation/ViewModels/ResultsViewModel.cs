using MultiQueueModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MultiQueueSimulation.ViewModels
{
    class ResultsViewModel
    {
        #region Properties
        public ObservableCollection<SimulationCase> VisualSimulationTable { get; set; }
        public int TotalNumberOfCustomers { get; set; }
        public int TotalRunTime { get; set; }
        public int TotalIdleTime { get; set; }
        public int TotalServiceTime { get; set; }
        public int TotalCustomerWaitTime { get; set;}
        public int CustomersWhoWaited { get; set; }
        #endregion
        public ResultsViewModel()
        {
            VisualSimulationTable = new ObservableCollection<SimulationCase>(App.SimulationSystem.SimulationTable as List<SimulationCase>);
            TotalNumberOfCustomers = App.SimulationSystem.StoppingNumber;
            TotalRunTime = App.SimulationSystem.SimulationTable.ElementAt(TotalNumberOfCustomers).EndTime;
            ComputeResults();
            TotalIdleTime = TotalRunTime - TotalServiceTime;
        }
        private void ComputeResults()
        {
            //TotalServiceTime = ComputeTotalServiceTime();
            TotalCustomerWaitTime = ComputeTotalCustomerWaitTime();
            CustomersWhoWaited = ComputeNumberOfCustomersWhoWaited();
        }
        void ComputeTotalServiceTime()
        {
            int TotalNumberOfServers = App.SimulationSystem.NumberOfServers;
            for (int i = 0; i < TotalNumberOfServers; i++)
            {
                TotalServiceTime = App.SimulationSystem.Servers.ElementAt(i).TotalWorkingTime;
                App.SimulationSystem.Servers.ElementAt(i).Utilization = TotalServiceTime / TotalRunTime;
                App.SimulationSystem.Servers.ElementAt(i).AverageServiceTime = TotalServiceTime / TotalNumberOfCustomers;
                App.SimulationSystem.Servers.ElementAt(i).IdleProbability= TotalIdleTime / TotalRunTime;
            }
        }
        int ComputeTotalCustomerWaitTime()
        {
            int TimeWaited = 0;
            for (int i = 0; i < TotalNumberOfCustomers; i++)
            {
                TimeWaited += App.SimulationSystem.SimulationTable.ElementAt(i).TimeInQueue;
            }
            return TimeWaited;
        }
         int ComputeNumberOfCustomersWhoWaited()
        {
            int CustomersWaited = 0;
            for (int i = 0; i < TotalNumberOfCustomers; i++)
            {
                if ((App.SimulationSystem.SimulationTable.ElementAt(i).TimeInQueue) != 0)
                    CustomersWaited++;
            }
            return CustomersWaited;
        }

        /////////Performance Measures per server////////
        //public float ProbabilityOfIdleServer = TotalIdleTime / TotalRunTime;
        //public float AverageServiceTime = TotalServiceTime() / TotalNumberOfCustomers;
        //public float Utilization = TotalServiceTime() / TotalRunTime;

        ///////////System OutPut Performance Measures//////
        public float AverageWaitingTime() {
            return  TotalCustomerWaitTime / TotalNumberOfCustomers;
        }
        public float PrababilityOfWaiting() {
            return CustomersWhoWaited / TotalNumberOfCustomers; 
        } 

    }
}
