using MultiQueueModels;
using MultiQueueSimulation.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MultiQueueSimulation.ViewModels
{
    public class WelcomeScreenViewModel 
    {
        public ICommand ButtonCommand { get; set; }

        public WelcomeScreenViewModel()
        {
            ButtonCommand = new Command(o => MainButtonClick("MainButton"));
        }

        private void MainButtonClick(object sender)
        {
            MessageBox.Show("Working");
        }

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
    }
}
