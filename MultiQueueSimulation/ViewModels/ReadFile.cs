using MultiQueueModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueSimulation.ViewModels
{
    class ReadFile : INotifyPropertyChanged
    {
        public static List<TimeDistribution> Read_File(string FileName)
        {
            string record;
            string[] fields;
            List<TimeDistribution> InterarrivalDistribution = new List<TimeDistribution>();

            FileStream FS = new FileStream("TestCase.txt", FileMode.Open);
            StreamReader SR = new StreamReader(FS);
            while (SR.Peek() != -1)
            {
                if (FileName == SR.ReadLine())
                {
                    record = SR.ReadLine();
                    while (record != "")
                    {

                        fields = record.Split(',');

                        InterarrivalDistribution.Add(new TimeDistribution()
                        {
                            Time = int.Parse(fields[0]),
                            Probability = Convert.ToDecimal(fields[1])
                        });
                        record = SR.ReadLine();
                    }
                    break;
                }
            }
            SR.Close();

            return InterarrivalDistribution;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
    }
}
