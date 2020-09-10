using ccbt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ccbt
{
    public class ILS
    {
        readonly int iterations;

        public ILS(int iterations =10000)
        {
            this.iterations = iterations;
        }

        public Solution FindSolution()
        {
            DateTime startTime = DateTime.Now;
            Random rnd = new Random();
            int iterations_count = 0;
            List<int> T = new List<int>() { 150, 111, 120,100, 140,151 };
            Solution S = new Solution();
            S.assignments = S.GenerateSolution();
            Console.WriteLine("Score ne fillim : {0}",S.GetScore());
            Solution H = new Solution();
            H=H.Copy(S.assignments);
            Solution Best = new Solution();
            Best = Best.Copy(S.assignments);
            var R = new Solution();

            while (iterations_count < iterations && !TimeDifferenceReached(startTime))
            {
                int climb_iterations = 0;
                int time = T[rnd.Next(T.Count)];

                while (iterations_count < iterations && climb_iterations < time && !TimeDifferenceReached(startTime))
                {
                    R = R.Copy(S.assignments);
                    R.Tweak();
       
                    var random = rnd.Next(1, 70);
                    if (R.GetScore() < S.GetScore() )
                    {
                        S = S.Copy(R.assignments);
                    }
                    climb_iterations++;
                }

                if (S.GetScore() < Best.GetScore())
                {
                  
                    Best = Best.Copy(S.assignments);
                }

                H =H.Copy( NewHomeBase(H, S).assignments);
                S = S.Copy(H.assignments);

                S.Perturb();
                iterations_count++;

                Console.WriteLine(Best.GetScore());
                if(Best.GetScore()==0)
                {
                    break;
                }
            }
            return Best;
        }

        public Solution NewHomeBase(Solution H, Solution S)
        {
            return S.GetScore() < H.GetScore() ? S : H;
        }

        private bool TimeDifferenceReached(DateTime startTime,double total_execution_minutes = 1)
        {
            bool result = (int)DateTime.Now.Subtract(startTime).TotalMinutes > total_execution_minutes ? true : false;
            return result;
        }
    }
}
