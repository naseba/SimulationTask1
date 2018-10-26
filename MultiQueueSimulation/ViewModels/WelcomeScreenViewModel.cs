using MultiQueueModels;
using MultiQueueSimulation.ViewModels.Base;
using System;
using System.Collections.Generic;
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
                return _simulateFromFile ?? (_simulateFromFile = new CommandHandler(async () => await HandleSimulationFromFile(), _canExecute));
            }
        }

        public WelcomeScreenViewModel()
        {
            _canExecute = true;
            ExistingFiles = new List<string>();
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
            throw new NotImplementedException();
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
        /// Populates the Pupblic simulation system in the app class with the data in the file then calls starts simultion
        /// </summary>
        void PopulateSystem()
        {
            //Fill Exsisting Files
            //Nosiba's  function 
            App.SimulationSystem.Servers.ForEach(x =>
            {
                AddCumulativeProbability(x.TimeDistribution);
                AddRange(x.TimeDistribution);
            });
        }

        //*It should be Void with no parameters and Edits ( " App.SimulationSystem " )*

        // you will find all the needed attributes there 

        //public void ReadFromFile()
        //{
        //    FileName += ".txt";
        //    string record;
        //    string[] fields;
        //    List<TimeDistribution> InterarrivalDistribution = new List<TimeDistribution>();

        //    FileStream FS = new FileStream(FileName, FileMode.Open);
        //    StreamReader SR = new StreamReader(FS);
        //    while (SR.Peek() != -1)
        //    {
        //        if (FileName == SR.ReadLine())
        //        {
        //            record = SR.ReadLine();
        //            while (record != "")
        //            {

        //                fields = record.Split(',');

        //                InterarrivalDistribution.Add(new TimeDistribution()
        //                {
        //                    Time = int.Parse(fields[0]),
        //                    Probability = Convert.ToDecimal(fields[1])
        //                });
        //                record = SR.ReadLine();
        //            }
        //            break;
        //        }
        //    }
        //    SR.Close();

        //    return InterarrivalDistribution;
        //} 

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
