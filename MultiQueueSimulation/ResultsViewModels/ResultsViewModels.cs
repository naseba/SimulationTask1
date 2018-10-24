using MultiQueueModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueSimulation.ResultsViewModels
{
     public class ResultsViewModels
    {
        // return the index of the highest prioroty server, return 0 if all servres are unavailabel
        static public int Highest_Priority(List<Server> servers)
        {
            int serverIndex = 0;
            for(int i=0; i<servers.Capacity;i++)
            {
                if(servers[i].availabel)
                {
                    serverIndex = i;
                    servers[i].availabel = false;
                    break;
                }
            }
            return serverIndex;
        }
    }
}
