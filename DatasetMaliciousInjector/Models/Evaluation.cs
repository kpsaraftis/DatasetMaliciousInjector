using System;
using System.Collections.Generic;
using System.Text;

namespace DatasetMaliciousInjector.Models
{
    public class Evaluation
    {
        public int TaskID { get; set; }
        public double WorkerEvaluation { get; set; }
        
        public Evaluation(int taskID, double workerEvaluation)
        {
            TaskID = taskID;
            WorkerEvaluation = workerEvaluation;
        }
    }
}
