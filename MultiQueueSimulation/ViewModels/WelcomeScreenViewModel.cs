using MultiQueueModels;
using MultiQueueSimulation.ViewModels.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public List<string> ExistingFiles { get; set; }

        public WelcomeScreenViewModel()
        {
            _canExecute = true;
            ExistingFiles = new List<string>();
        }

        async Task HandleSimulationFromFile()
        {
            //Fill Exsisting Files
            //App.InterarrivalDistribution = ReadFromFile(ExistingFiles[0]);
            App.SimulationSystem.Servers.ForEach(x =>
            {
                AddCumulativeProbability(x.TimeDistribution);
                AddRange(x.TimeDistribution);
            });
        }
        async Task StartSimulation()
        {
            int LowestNumber = App.SimulationSystem.InterarrivalDistribution[0].MinRange;
            int HighestNumber = App.SimulationSystem.InterarrivalDistribution[App.SimulationSystem.InterarrivalDistribution.Count-1].MaxRange;
            int rand = App.GeneralRandomFunction(LowestNumber,HighestNumber);
            
        }
        int GetArrivalTime(int rand)
        {
            foreach (var item in App.InterarrivalDistribution)
            {
                if (rand >= item.MinRange && rand <= item.MaxRange)
                {
                    return item.Time;
                }
            }
            return 0;
        }

        //async Task PopulateSystem()
        //{
        //    //ReadFromFile();
        //    foreach (var item in App.SimulationSystem.InterarrivalDistribution)
        //    {
        //        AddCumulativeProbability(item);
        //        AddRange();
        //    }
        //}
        #region Creating  Distributions 

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





    }
}
