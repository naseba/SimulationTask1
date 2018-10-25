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
                return _simulateFromFile ?? (_simulateFromFile = new CommandHandler(async () => await check(), _canExecute));
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
        }

        #region Creating  Distributions 
        //public List<TimeDistribution> ReadFromFile(string FileName)
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
        public void AddCumulativeProbability()
        {
            App.InterarrivalDistribution[0].CummProbability = App.InterarrivalDistribution[0].Probability;

            for (int i = 1; i < App.InterarrivalDistribution.Count; i++)
            {
                App.InterarrivalDistribution[i].CummProbability = App.InterarrivalDistribution[i].Probability + App.InterarrivalDistribution[i - 1].CummProbability;
            }
        }
        public void AddRange()
        {
            App.InterarrivalDistribution[0].MinRange = 1;

            for (int i = 0; i < App.InterarrivalDistribution.Count; i++)
            {
                App.InterarrivalDistribution[i].MaxRange = (int)((App.InterarrivalDistribution[i].CummProbability) * 100);
            }
            for (int i = 1; i < App.InterarrivalDistribution.Count; i++)
            {
                App.InterarrivalDistribution[i].MinRange = (App.InterarrivalDistribution[i - 1].MaxRange + 1);
            }
        }
        #endregion





    }
}
