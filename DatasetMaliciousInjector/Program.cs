using DatasetMaliciousInjector.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace DatasetMaliciousInjector
{
    class Program
    {
        static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string debugPath = Directory.GetCurrentDirectory();
            var excelDirectory = Directory.GetParent(debugPath).Parent.Parent;
            Console.WriteLine("Load the text file...");
            var excelFilePath = new DirectoryInfo(String.Concat(excelDirectory, "/", ExcelConstants.NameOfExcelFile));
            FileInfo existingFile = new FileInfo(excelFilePath.ToString());
            Console.WriteLine("File Loaded.");
            var listOfOriginalWorkers = new List<WorkerInfo>();
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet evaluations = package.Workbook.Worksheets[0];
                for (int workerEvaluationStep = 2; workerEvaluationStep <= 71; workerEvaluationStep++)
                {
                    var originalWorker = new WorkerInfo(workerEvaluationStep - 1);
                    for (int taskEvaluationStep = 2; taskEvaluationStep <= 133; taskEvaluationStep++)
                    {
                        var workerEvaluation = (double)evaluations.Cells[workerEvaluationStep, taskEvaluationStep].Value;
                        originalWorker.Evaluations.Add(new Evaluation(taskEvaluationStep - 1, workerEvaluation));
                    }
                    listOfOriginalWorkers.Add(originalWorker);
                }
                Console.WriteLine("Original workers loaded!");
            }


            for (int i = 30; i <= 70; i += 10)
            {
                //inject malicious workers
                var listOfOriginalAndMaliciousWorkers = new List<WorkerInfo>();
                listOfOriginalAndMaliciousWorkers.AddRange(listOfOriginalWorkers);

                for (int j = 0; j < i; j++)
                {
                    var maliciousWorker = new WorkerInfo(ExcelConstants.CountOfWorkers + j + 1);
                    for (int taskEvaluationStep = 2; taskEvaluationStep <= 133; taskEvaluationStep++)
                    {

                        Random r = new Random();
                        maliciousWorker.Evaluations.Add(new Evaluation(taskEvaluationStep - 1, r.Next(1, 6)));
                    }
                    listOfOriginalAndMaliciousWorkers.Add(maliciousWorker);
                }

                //Randomize list 
                var randomizedListOfOriginalAndMaliciousWorkers = new List<WorkerInfo>();
                var rnd = new Random();
                while (listOfOriginalAndMaliciousWorkers.Count != 0)
                {
                    var index = rnd.Next(0, listOfOriginalAndMaliciousWorkers.Count);
                    randomizedListOfOriginalAndMaliciousWorkers.Add(listOfOriginalAndMaliciousWorkers[index]);
                    listOfOriginalAndMaliciousWorkers.RemoveAt(index);
                }


                //create excel file
                int totalCountOfWorkers = ExcelConstants.CountOfWorkers + i;
                string nameOfExcelFile = "dataset" + (totalCountOfWorkers).ToString() + ".xlsx";
                excelFilePath = new DirectoryInfo(String.Concat(excelDirectory, "/", nameOfExcelFile));
                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Worksheets.Add("Worksheet1");
                    FileInfo excelFile = new FileInfo(excelFilePath.ToString());
                    excel.SaveAs(excelFile);
                }
                var createdExcelFile = new FileInfo(excelFilePath.ToString());
                using (ExcelPackage package = new ExcelPackage(createdExcelFile))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];

                    //create columns and rows
                    for (int workerEvaluationStep = 2; workerEvaluationStep <= totalCountOfWorkers + 1; workerEvaluationStep++)
                    {
                        workSheet.Cells[workerEvaluationStep, 1].Value = "w" + (workerEvaluationStep - 1);
                    }
                    for (int taskEvaluationStep = 2; taskEvaluationStep <= 133; taskEvaluationStep++)
                    {
                        workSheet.Cells[1, taskEvaluationStep].Value = "t" + (taskEvaluationStep - 1);
                    }
                    workSheet.Cells[1, 134].Value = "class";

                    for (int workerEvaluationStep = 2; workerEvaluationStep <= totalCountOfWorkers + 1; workerEvaluationStep++)
                    {
                        var worker = randomizedListOfOriginalAndMaliciousWorkers[workerEvaluationStep - 2];
                        for (int taskEvaluationStep = 2; taskEvaluationStep <= 133; taskEvaluationStep++)
                        {
                            workSheet.Cells[workerEvaluationStep, taskEvaluationStep].Value = worker.Evaluations[taskEvaluationStep - 2].WorkerEvaluation;
                        }
                        workSheet.Cells[workerEvaluationStep, 134].Value = worker.Class;
                    }
                    package.Save();
                }
            }

            //save workbook
            Console.WriteLine("Datasets Generated :-)");
        }
    }
}
