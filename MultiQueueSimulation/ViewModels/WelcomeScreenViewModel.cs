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
                return _simulateFromFile ?? (_simulateFromFile = new CommandHandler(() => PopulateSystem(), _canExecute));
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

        private void StartRandomSimulation()
        {
            if (App.SimulationSystem.StoppingCriteria == Enums.StoppingCriteria.NumberOfCustomers)
            {
                for (int i = 0; i < App.SimulationSystem.StoppingNumber; i++)
                {
                    SimulationCase temp = new SimulationCase();

                    temp.CustomerNumber = i;
                    temp.RandomInterArrival = App.GeneralRandomFunction(1, 100);
                    if (i == 1)
                    {
                        temp.InterArrival = 0;
                        temp.ArrivalTime = 0;
                    }
                    else
                    {
                        temp.ArrivalTime += temp.InterArrival;
                        temp.InterArrival = GetValueFromDistribution(temp.RandomInterArrival, App.SimulationSystem.InterarrivalDistribution);
                    }
                    foreach (var item in App.SimulationSystem.Servers)
                    {
                        if (item.FinishTime > temp.ArrivalTime)
                        {
                            item.availabel = false;
                        }
                    }
                    temp.RandomService = App.GeneralRandomFunction(1, 100);
                    Server AssignedServer = RandomServerSelect(App.SimulationSystem.Servers);
                    if (AssignedServer == null)
                    {
                        int min = App.SimulationSystem.Servers[0].FinishTime;
                        int index = 0;
                        for (int q = 0; q < App.SimulationSystem.Servers.Count; q++)
                        {
                            if (App.SimulationSystem.Servers[q].FinishTime < min)
                            {
                                min = App.SimulationSystem.Servers[q].FinishTime;
                                index = q;
                            }
                        }
                        temp.TimeInQueue = min - temp.ArrivalTime;
                        temp.AssignedServer = App.SimulationSystem.Servers[index];
                    }
                    else
                    {
                        int index = App.SimulationSystem.Servers.IndexOf(AssignedServer);
                        temp.AssignedServer = App.SimulationSystem.Servers[index];
                        temp.TimeInQueue = 0;
                    }
                    temp.ServiceTime = GetValueFromDistribution(temp.RandomService, AssignedServer.TimeDistribution);
                    if (App.Que.Count == 0)
                    {
                        temp.AssignedServer = AssignedServer;
                        temp.StartTime = temp.ArrivalTime;
                        temp.EndTime = temp.ArrivalTime + temp.ServiceTime;
                        temp.AssignedServer.FinishTime = temp.EndTime;
                    }
                    App.SimulationSystem.SimulationTable.Add(temp);
                }
            }
            else if (App.SimulationSystem.StoppingCriteria == Enums.StoppingCriteria.SimulationEndTime)
            {

            }
        }

        private void StartLeastUtilizationSimulation()
        {
            throw new NotImplementedException();
        }

        private void StartHighestPrioritySimulation()
        {
            throw new NotImplementedException();
        }

        #region Populating System 

        /// <summary>
        /// Populates the Public simulation system in the app class with the data in the file then calls starts simultion
        /// </summary>
        void PopulateSystem()
        {
            ReadFromFile();
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
            string FileContent = File.ReadAllText("C:\\Storage\\College\\Task 1\\SimulationTask1\\MultiQueueSimulation\\TestCases\\TestCase2.txt");
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
                if (Servers[RandomNumber].availabel == false)
                {
                    AvilableServers.RemoveAt(RandomNumber);
                }
                else
                    return AvilableServers[RandomNumber];
            } while (AvilableServers.Count != 0);
            return null;
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
