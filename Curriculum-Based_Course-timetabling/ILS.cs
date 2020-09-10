using Curriculum_Based_Course_timetabling.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curriculum_Based_Course_timetabling
{
    class ILS
    {
        readonly int iterations;
        readonly int seconds;

        public ILS(int seconds = 60, int iterations = 1000)
        {
            this.iterations = iterations;
            this.seconds = seconds;
        }

        public Solution FindSolution(AlgorithmModel model = null)
        {
            var algorithmList = new List<AlgorithmModel>();
            DateTime startTime = DateTime.Now;
            Random rnd = new Random();
            int iterations_count = 0;
            List<int> T = new List<int>() { 15, 11, 12, 100, 14, 15 };
            Solution S = new Solution();
            S.assignments = S.GenerateSolution();
            //Console.WriteLine("Score ne fillim : {0}",S.GetScore());
            Solution H = new Solution();
            H = H.Copy(S.assignments);
            Solution Best = new Solution();
            Best = Best.Copy(S.assignments);
            var R = new Solution();
            algorithmList.Add(new AlgorithmModel()
            {
                ChangeTimeSlot = 0,
                HillCliming = 0,
                ImprovementPercent = 0,
                Iteration = iterations_count + 1,
                IterationNoResult = 0,
                NewHomeBase = 0,
                Swaps = 0,
                Score = S.GetScore()
            });

            Stopwatch s = new Stopwatch();
            s.Start();
            while (iterations_count < iterations && s.Elapsed < TimeSpan.FromSeconds(seconds))
            {
                int climb_iterations = 0;
                int time = T[rnd.Next(T.Count)];

                while (iterations_count < iterations && climb_iterations < time && s.Elapsed < TimeSpan.FromSeconds(seconds))
                {
                    if (model != null && model.HillCliming == climb_iterations)
                        break;

                    R = R.Copy(S.assignments);
                    R.Tweak();

                    var random = rnd.Next(1, 70);
                    if (R.GetScore() < S.GetScore())
                    {
                        S = S.Copy(R.assignments);
                    }

                    climb_iterations++;
                }

                int noResult = 1;
                double improvementPercent = 0;

                if (S.GetScore() < Best.GetScore())
                {
                    double decrease = Best.GetScore() - S.GetScore();

                    improvementPercent = decrease / Best.GetScore() * 100;

                    noResult = 0;
                    Best = Best.Copy(S.assignments);
                }

                int newHomeBase = 0, swaps = 0, changeTimeSlots = 0;

                H = H.Copy(NewHomeBase(H, S, out newHomeBase, model?.NewHomeBase).assignments);
                S = S.Copy(H.assignments);

                S.Perturb(out swaps, out changeTimeSlots, model?.Swaps, model?.ChangeTimeSlot);
                iterations_count++;

                algorithmList.Add(new AlgorithmModel()
                {
                    ChangeTimeSlot = changeTimeSlots,
                    HillCliming = climb_iterations,
                    ImprovementPercent = improvementPercent,
                    Iteration = iterations_count + 1,
                    IterationNoResult = noResult,
                    NewHomeBase = newHomeBase,
                    Swaps = swaps,
                    Score = Best.GetScore()
                });

                // Console.WriteLine(Best.GetScore());
                if (Best.GetScore() == 0)
                {
                    break;
                }
            }

            s.Stop();

            Best.AlgorithmModels = algorithmList;
            return Best;
        }

        public Solution NewHomeBase(Solution H, Solution S, out int newHomeBase, int? oldHomeBase = 0)
        {
            var r = new Random();

            int nr = r.Next();

            if ((oldHomeBase.HasValue && oldHomeBase == 1) || nr % 2 == 0)
            {
                newHomeBase = 1;
                if (S.GetScore() < H.GetScore())
                {
                    return S;
                }

                return H;
            }

            newHomeBase = 0;
            return H;
        }

        private bool TimeDifferenceReached(DateTime startTime, int total_execution_seconds)
        {
            bool result = (int)DateTime.Now.Subtract(startTime).TotalSeconds > total_execution_seconds ? true : false;
            return result;
        }
    }
}
