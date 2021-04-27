using System;
using System.Collections.Generic;
using System.Text;

namespace DatasetMaliciousInjector.Models
{
    public class WorkerInfo
    {
        public int WorkerID { get; set; }

        public int Class
        {
            get
            {
                return (this.WorkerID <= 70) ? 0 : 1;
            }
        }

        public List<Evaluation> Evaluations { get; set; }

        public WorkerInfo(int workerID)
        {
            this.WorkerID = workerID;
            this.Evaluations = new List<Evaluation>();
        }
    }
}
