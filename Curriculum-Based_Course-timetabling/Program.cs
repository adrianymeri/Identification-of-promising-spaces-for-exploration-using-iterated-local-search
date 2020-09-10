using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Curriculum_Based_Course_timetabling.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.LightGbm;
using Microsoft.ML.Transforms.Text;
using Microsoft.Win32;

namespace Curriculum_Based_Course_timetabling
{

    class DataPoints
    {
        public float Label { get; set; }

        [VectorType(5)]
        public float[] Features { get; set; }
    }

    class MetricResult
    {
        public RegressionMetrics Metric { get; set; }
        public List<AlgorithmModel> AlgorithmModels { get; set; }
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var metricResults = new List<MetricResult>();
            for (int j = 0; j < 5; j++)
            {
                var dataPoints2 = new List<DataPoints>();
                var testDataPoints = new List<DataPoints>();
                var metricResult = new MetricResult();
                metricResult.AlgorithmModels = new List<AlgorithmModel>();


                var random = new Random(0);

                for (int i = 0; i < 20; i++)
                {
                    IO.Read("UP-FME-SS2018");
                    string text = "";
                    Solution solution = new Solution();

                    ILS ils = new ILS(5);
                    solution = ils.FindSolution();
                    foreach (var item in solution.assignments)
                    {
                        text += item.course.Id + " " + item.room.Id.Trim() + " " + item.timeslot.Day + " " + item.timeslot.Period + "\n";
                    }

                    if (i <= 6)
                    {
                        metricResult.AlgorithmModels.AddRange(solution.AlgorithmModels);
                        dataPoints2.Add(new DataPoints()
                        {
                            Features = solution.AlgorithmModels.Select(a => float.Parse(a.Score.ToString())).Take(5).ToArray(),
                            Label = random.Next(1, 4),
                        });

                    }
                    else
                    {
                        testDataPoints.Add(new DataPoints()
                        {
                            Features = solution.AlgorithmModels.Select(a => float.Parse(a.Score.ToString())).Take(5).ToArray(),
                            Label = random.Next(1, 4),
                        });
                    }
                        

                    IO.Write(solution.assignments, solution.GetScore());

                    var allLines = (from modelObj in solution.AlgorithmModels
                                    select new object[]
                                    {
                        modelObj.Iteration,
                        modelObj.ImprovementPercent,
                        modelObj.IterationNoResult,
                        modelObj.HillCliming,
                        modelObj.Swaps,
                        modelObj.ChangeTimeSlot,
                        modelObj.NewHomeBase,
                        modelObj.Score,
                                    }).ToList();

                    // Build the file content
                    var csv = new StringBuilder();

                    csv.AppendLine("Iteration,% improvement percetange,Number of interations without good result,Number of hillcliming iterations (time),Number of  Swap Courses on Perturb,Number of  Change Time Slot on Perturb,New Home Base Type: 1.Greedy 0.Random,Score");

                    allLines.ForEach(line =>
                    {
                        csv.AppendLine(string.Join(",", line));
                    });

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "CSV Dcoument|*.csv";
                    saveFileDialog1.Title = "Save CSV";


                    File.WriteAllText($@"C:\Users\NB\Desktop\tests\{DateTime.Now.Millisecond}.csv", csv.ToString());


                    Instance.Reset();
                }


                var mlContext = new MLContext(seed: 0);



                var pipeline = mlContext.Regression.Trainers.
                LightGbm(
                    labelColumnName: nameof(DataPoints.Label),
                    featureColumnName: nameof(DataPoints.Features));

                // Train the model.
                var trainingData = mlContext.Data.LoadFromEnumerable(dataPoints2);

                // Train the model.
                var model = pipeline.Fit(trainingData);

                var testData = mlContext.Data.LoadFromEnumerable(
                 testDataPoints);

                // Run the model on test data set.
                var transformedTestData = model.Transform(testData);

                // Convert IDataView object to a list.
                var predictions = mlContext.Data.CreateEnumerable<Prediction>(
                    transformedTestData, reuseRowObject: false).ToList();

                // Look at 5 predictions
                foreach (var p in predictions.Take(5))
                    Console.WriteLine($"Label: {p.Label}, " +
                        $"Prediction: {p.Score}");

                // Evaluate the overall metrics
                var metrics = mlContext.Regression.Evaluate(transformedTestData);


                PrintMetrics(metrics);

                metricResult.Metric = metrics;

                metricResults.Add(metricResult);
            }

            var metric = metricResults.OrderBy(a => a.Metric.RootMeanSquaredError)
                .OrderBy(a => a.Metric.MeanAbsoluteError).OrderBy(a => a.Metric.MeanSquaredError).FirstOrDefault();


            var bestModel = metric.AlgorithmModels.OrderBy(a => a.Score).FirstOrDefault();
            IO.Read("UP-FME-SS2018");
            Solution newSolution = new Solution();

            ILS newIls = new ILS(180);
            newSolution = newIls.FindSolution(bestModel);

            var bestModelAfter = newSolution.AlgorithmModels.OrderBy(a => a.Score).FirstOrDefault();
            

            Console.WriteLine($"Best Model Score:{bestModel.Score}");
            Console.WriteLine($"After Best Model Score:{bestModelAfter.Score}");
            Console.ReadLine();
        }


        // Class used to capture predictions.
        private class Prediction
        {
            // Original label.
            public float Label { get; set; }
            // Predicted score from the trainer.
            public float Score { get; set; }
        }

        // Print some evaluation metrics to regression problems.
        private static void PrintMetrics(RegressionMetrics metrics)
        {
            Console.WriteLine("Mean Absolute Error: " + metrics.MeanAbsoluteError);
            Console.WriteLine("Mean Squared Error: " + metrics.MeanSquaredError);
            Console.WriteLine(
                "Root Mean Squared Error: " + metrics.RootMeanSquaredError);

            Console.WriteLine("RSquared: " + metrics.RSquared);
        }
    }
}
