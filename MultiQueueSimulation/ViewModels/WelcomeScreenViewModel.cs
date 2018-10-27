using MultiQueueModels;
using MultiQueueSimulation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace MultiQueueSimulation.ViewModels
{
    public class WelcomeScreenViewModel
    {
        private bool _canExecute;

        private ICommand _simulateFromFile;
        public ICommand SimulateFromFile
        {
            get
            {
                return _simulateFromFile ?? (_simulateFromFile = new CommandHandler(() => HandleSimulationFromFile(), _canExecute));
            }
        }

        public WelcomeScreenViewModel()
        {
            _canExecute = true;
        }

        private void HandleSimulationFromFile()
        {
            PopulateSystem();
            if (App.SimulationSystem.SelectionMethod == Enums.SelectionMethod.HighestPriority)
            {
                StartHighestPrioritySimulation();
            }
            else if (App.SimulationSystem.SelectionMethod == Enums.SelectionMethod.LeastUtilization)
            {
                StartLeastUtilizationSimulation();
            }
            else if (App.SimulationSystem.SelectionMethod == Enums.SelectionMethod.Random)
            {
                StartRandomSimulation();
            }
        }

        #region Simulations 
        private void StartRandomSimulation()
        {
            if (App.SimulationSystem.StoppingCriteria == Enums.StoppingCriteria.NumberOfCustomers)
            {
                for (int i = 0; i < App.SimulationSystem.StoppingNumber; i++)
                {
                    SimulationCase temp = new SimulationCase();

                    temp.CustomerNumber = i + 1;
                    if (i == 0)
                    {
                        temp.InterArrival = 0;
                        temp.ArrivalTime = 0;
                    }
                    else
                    {
                        temp.RandomInterArrival = App.GeneralRandomFunction(1, 100);
                        temp.InterArrival = GetValueFromDistribution(temp.RandomInterArrival, App.SimulationSystem.InterarrivalDistribution);
                        temp.ArrivalTime += temp.InterArrival + App.SimulationSystem.SimulationTable[i - 1].ArrivalTime;
                    }
                    foreach (var item in App.SimulationSystem.Servers)
                    {
                        if (item.FinishTime > temp.ArrivalTime)
                        {
                            item.IsAvailable = false;
                        }
                        else if (item.FinishTime <= temp.ArrivalTime)
                        {
                            item.IsAvailable = true;
                        }
                    }
                    temp.RandomService = App.GeneralRandomFunction(1, 100);
                    Server AssignedServer = RandomServerSelect(App.SimulationSystem.Servers);
                    if (AssignedServer == null)
                    {
                        int min = App.SimulationSystem.Servers[0].FinishTime;
                        int ind = 0;
                        for (int q = 0; q < App.SimulationSystem.Servers.Count; q++)
                        {
                            if (App.SimulationSystem.Servers[q].FinishTime < min)
                            {
                                min = App.SimulationSystem.Servers[q].FinishTime;
                                ind = q;
                            }
                        }
                        temp.TimeInQueue = min - temp.ArrivalTime;
                        temp.AssignedServer = App.SimulationSystem.Servers[ind];
                    }
                    else
                    {
                        int index = App.SimulationSystem.Servers.IndexOf(AssignedServer);
                        temp.AssignedServer = App.SimulationSystem.Servers[index];
                        temp.TimeInQueue = 0;
                    }
                    temp.ServiceTime = GetValueFromDistribution(temp.RandomService, temp.AssignedServer.TimeDistribution);

                    temp.StartTime = temp.ArrivalTime + temp.TimeInQueue;
                    temp.EndTime = temp.ArrivalTime + temp.ServiceTime;
                    temp.AssignedServer.FinishTime = temp.EndTime;

                    App.SimulationSystem.SimulationTable.Add(temp);
                }
            }
            else if (App.SimulationSystem.StoppingCriteria == Enums.StoppingCriteria.SimulationEndTime)
            {

            }
        }

        private void StartLeastUtilizationSimulation()
        {
            if (App.SimulationSystem.StoppingCriteria == Enums.StoppingCriteria.NumberOfCustomers)
            {
                for (int i = 0; i < App.SimulationSystem.StoppingNumber; i++)
                {
                    SimulationCase temp = new SimulationCase();

                    temp.CustomerNumber = i + 1;
                    if (i == 0)
                    {
                        temp.InterArrival = 0;
                        temp.ArrivalTime = 0;
                    }
                    else
                    {
                        temp.RandomInterArrival = App.GeneralRandomFunction(1, 100);
                        temp.InterArrival = GetValueFromDistribution(temp.RandomInterArrival, App.SimulationSystem.InterarrivalDistribution);
                        temp.ArrivalTime += temp.InterArrival + App.SimulationSystem.SimulationTable[i - 1].ArrivalTime;
                    }

                    foreach (var item in App.SimulationSystem.Servers)
                    {
                        if (item.FinishTime > temp.ArrivalTime)
                        {
                            item.IsAvailable = false;
                        }
                        else if (item.FinishTime <= temp.ArrivalTime)
                        {
                            item.IsAvailable = true;
                        }
                    }

                    temp.RandomService = App.GeneralRandomFunction(1, 100);

                    int index = LeastUtilizationServerSelect(App.SimulationSystem.Servers);

                    if (index == -1)
                    {
                        int min = App.SimulationSystem.Servers[0].FinishTime;
                        int ind = 0;
                        for (int q = 0; q < App.SimulationSystem.Servers.Count; q++)
                        {
                            if (App.SimulationSystem.Servers[q].FinishTime < min)
                            {
                                min = App.SimulationSystem.Servers[q].FinishTime;
                                ind = q;
                            }
                        }
                        temp.TimeInQueue = min - temp.ArrivalTime;
                        temp.AssignedServer = App.SimulationSystem.Servers[ind];
                    }
                    else
                    {
                        temp.AssignedServer = App.SimulationSystem.Servers[index];
                        temp.TimeInQueue = 0;
                    }

                    temp.ServiceTime = GetValueFromDistribution(temp.RandomService, temp.AssignedServer.TimeDistribution);

                    temp.StartTime = temp.ArrivalTime + temp.TimeInQueue;
                    temp.EndTime = temp.ArrivalTime + temp.ServiceTime;
                    temp.AssignedServer.FinishTime = temp.EndTime;
                    temp.AssignedServer.TotalWorkingTime += temp.ServiceTime;

                    App.SimulationSystem.SimulationTable.Add(temp);
                }
            }
            else if (App.SimulationSystem.StoppingCriteria == Enums.StoppingCriteria.SimulationEndTime)
            {

            }
        }

        private void StartHighestPrioritySimulation()
        {
            if (App.SimulationSystem.StoppingCriteria == Enums.StoppingCriteria.NumberOfCustomers)
            {
                for (int i = 0; i < App.SimulationSystem.StoppingNumber; i++)
                {
                    SimulationCase temp = new SimulationCase();

                    temp.CustomerNumber = i + 1;
                    if (i == 0)
                    {
                        temp.InterArrival = 0;
                        temp.ArrivalTime = 0;
                    }
                    else
                    {
                        temp.RandomInterArrival = App.GeneralRandomFunction(1, 100);
                        temp.InterArrival = GetValueFromDistribution(temp.RandomInterArrival, App.SimulationSystem.InterarrivalDistribution);
                        temp.ArrivalTime += temp.InterArrival + App.SimulationSystem.SimulationTable[i - 1].ArrivalTime;
                    }
                    foreach (var item in App.SimulationSystem.Servers)
                    {
                        if (item.FinishTime > temp.ArrivalTime)
                        {
                            item.IsAvailable = false;
                        }
                        else if (item.FinishTime <= temp.ArrivalTime)
                        {
                            item.IsAvailable = true;
                        }
                    }
                    temp.RandomService = App.GeneralRandomFunction(1, 100);
                    int index = HighestPriorityServerSelect(App.SimulationSystem.Servers);
                    if (index == -1)
                    {
                        int min = App.SimulationSystem.Servers[0].FinishTime;
                        int ind = 0;
                        for (int q = 0; q < App.SimulationSystem.Servers.Count; q++)
                        {
                            if (App.SimulationSystem.Servers[q].FinishTime < min)
                            {
                                min = App.SimulationSystem.Servers[q].FinishTime;
                                ind = q;
                            }
                        }
                        temp.TimeInQueue = min - temp.ArrivalTime;
                        temp.AssignedServer = App.SimulationSystem.Servers[ind];
                    }
                    else
                    {
                        temp.AssignedServer = App.SimulationSystem.Servers[index];
                        temp.TimeInQueue = 0;
                    }
                    temp.ServiceTime = GetValueFromDistribution(temp.RandomService, temp.AssignedServer.TimeDistribution);

                    temp.StartTime = temp.ArrivalTime + temp.TimeInQueue;
                    temp.EndTime = temp.ArrivalTime + temp.ServiceTime;
                    temp.AssignedServer.FinishTime = temp.EndTime;
                    temp.AssignedServer.TotalWorkingTime += temp.ServiceTime;

                    App.SimulationSystem.SimulationTable.Add(temp);
                }
            }
            else if (App.SimulationSystem.StoppingCriteria == Enums.StoppingCriteria.SimulationEndTime)
            {

            }
        }
        #endregion

        #region Populating System 
        /// <summary>
        /// Populates the Public simulation system in the app class with the data in the file then calls starts simultion
        /// </summary>
        void PopulateSystem()
        {
            ReadFromFile();

            AddCumulativeProbability(App.SimulationSystem.InterarrivalDistribution);
            AddRange(App.SimulationSystem.InterarrivalDistribution);

            App.SimulationSystem.Servers.ForEach(x =>
            {
                AddCumulativeProbability(x.TimeDistribution);
                AddRange(x.TimeDistribution);
            });

            Thread t = new Thread(new ThreadStart(Populateroc));
            t.Start();
            Thread.Sleep(150);
            if (t.IsAlive)
                t.Abort();
        }

        private void Populateroc()
        {
            MessageBox.Show("System object Ready...Starting Simulation");
        }

        public void ReadFromFile()
        {
            string FileContent = File.ReadAllText("C:\\Users\\Nour\\Desktop\\MultiQueue System\\SimulationTask1\\MultiQueueSimulation\\TestCases\\TestCase1.txt");
            string[] Lines = FileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            App.SimulationSystem.NumberOfServers = Convert.ToInt32(Lines[1]);
            for (int i = 0; i < App.SimulationSystem.NumberOfServers; i++)
            {
                Server Temp = new Server() { ID = i + 1 };
                App.SimulationSystem.Servers.Add(Temp);
            }
            App.SimulationSystem.StoppingNumber = Convert.ToInt32(Lines[4]);
            App.SimulationSystem.StoppingCriteria = (Enums.StoppingCriteria)Enum.ToObject(typeof(Enums.StoppingCriteria), Convert.ToInt32(Lines[7]));
            App.SimulationSystem.SelectionMethod = (Enums.SelectionMethod)Enum.ToObject(typeof(Enums.SelectionMethod), Convert.ToInt32(Lines[10]));
            int index = 13;
            while (!String.IsNullOrWhiteSpace(Lines[index]))
            {
                string[] halfline = Lines[index].Split(',');
                TimeDistribution temp = new TimeDistribution
                {
                    Time = Convert.ToInt32(halfline[0]),
                    Probability = Convert.ToDecimal(halfline[1]),
                };
                App.SimulationSystem.InterarrivalDistribution.Add(temp);
                index++;
            }

            for (int q = 0; q < App.SimulationSystem.NumberOfServers; q++)
            {
                index += 2;
                while (!String.IsNullOrWhiteSpace(Lines[index]))
                {
                    string[] halfline = Lines[index].Split(',');
                    TimeDistribution temp = new TimeDistribution
                    {
                        Time = Convert.ToInt32(halfline[0]),
                        Probability = Convert.ToDecimal(halfline[1]),
                    };
                    App.SimulationSystem.Servers[q].TimeDistribution.Add(temp);
                    index++;
                    if (index == Lines.Length)
                    {
                        break;
                    }
                }
            }
            Thread t = new Thread(new ThreadStart(FileProc));
            t.Start();
            Thread.Sleep(150);
            if (t.IsAlive)
                t.Abort();
        }

        private void FileProc()
        {
            MessageBox.Show("File Imported Successfully...Preparing For Simulation");
        }

        public void AddCumulativeProbability(List<TimeDistribution> TimeDistribution)
        {
            TimeDistribution[0].CummProbability = TimeDistribution[0].Probability;

            for (int i = 1; i < TimeDistribution.Count; i++)
            {
                TimeDistribution[i].CummProbability = TimeDistribution[i].Probability + TimeDistribution[i - 1].CummProbability;
            }
        }

        public void AddRange(List<TimeDistribution> TimeDistribution)
        {
            TimeDistribution[0].MinRange = 1;

            for (int i = 0; i < TimeDistribution.Count; i++)
            {
                TimeDistribution[i].MaxRange = (int)((TimeDistribution[i].CummProbability) * 100);
            }
            for (int i = 1; i < TimeDistribution.Count; i++)
            {
                TimeDistribution[i].MinRange = (TimeDistribution[i - 1].MaxRange + 1);
            }
        }
        #endregion

        #region Server Selction
        Server RandomServerSelect(List<Server> Servers)
        {
            List<Server> AvilableServers = new List<Server>();
            AvilableServers = Servers;
            Random RandomIndex = new Random();
            int RandomNumber;
            do
            {
                RandomNumber = RandomIndex.Next(0, Servers.Count);
                if (Servers[RandomNumber].IsAvailable == false)
                {
                    AvilableServers.RemoveAt(RandomNumber);
                }
                else
                    return AvilableServers[RandomNumber];
            } while (AvilableServers.Count != 0);
            return null;
        }
        int HighestPriorityServerSelect(List<Server> servers)
        {
            int serverIndex = -1;
            for (int i = 0; i < servers.Count; i++)
            {
                if (servers[i].IsAvailable)
                {
                    serverIndex = i;
                    servers[i].IsAvailable = false;
                    break;
                }
            }
            return serverIndex;
        }
        int LeastUtilizationServerSelect(List<Server> servers)
        {
            int Leastindex = 0;
            bool found = false;

            for (int i = 1; i < servers.Count; i++)
            {
                if (servers[Leastindex].TotalWorkingTime > servers[i].TotalWorkingTime)
                {
                    Leastindex = i;
                    found = true;
                }
            }
            if (found)
            {
                return Leastindex;
            }
            else
            {
                return -1;
            }
        }
        #endregion

        /// <summary>
        /// Gets the required value from distribution
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="Distribution"></param>
        int GetValueFromDistribution(int rand, List<TimeDistribution> Distribution)
        {
            foreach (var item in Distribution)
            {
                if (rand >= item.MinRange && rand <= item.MaxRange)
                {
                    return item.Time;
                }
            }
            return 0;
        }
    }
}
