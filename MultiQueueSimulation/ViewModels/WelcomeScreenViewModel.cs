using MultiQueueModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueSimulation.ViewModels
{
    public class WelcomeScreenViewModel : INotifyPropertyChanged
    {

        public static void Cumulative_Probability(List<TimeDistribution> Probability)
        {
            Probability[0].CummProbability = Probability[0].Probability;

            for (int i = 1; i < Probability.Count; i++)
            {
                Probability[i].CummProbability = Probability[i].Probability + Probability[i - 1].CummProbability;
            }
        }


        public static void Range(List<TimeDistribution> range)
        {
            range[0].MinRange = 1;

            for (int i = 0; i < range.Count; i++)
            {
                range[i].MaxRange = (int)((range[i].CummProbability) * 100);
            }
            for (int i = 1; i < range.Count; i++)
            {
                range[i].MinRange = (range[i - 1].MaxRange + 1);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
    }
}
