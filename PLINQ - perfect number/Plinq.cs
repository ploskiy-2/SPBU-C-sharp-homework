using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLINQ___perfect_number
{
    internal class Program
    {
        public static bool Is_perfect_withLINQ(int num)
        {
            //this is first version     -----     I guess it is slowly because there are a lot of function .ToList() and .Sum()
            //create to list with divisiors ( div_num_1 - list with div before sqrt  and  div_num_2 after sqrt ) 
            var div_num_1 = (from n in Enumerable.Range(1,(int) Math.Sqrt(num)).AsParallel() where (num%n==0) select (n)).ToList();
            var div_num_2 = (from n in div_num_1.AsParallel() where (int)num/n != n select (num/n)).ToList();
            return (div_num_1.Sum() + div_num_2.Sum() - num)==num;

        }

        public static bool Is_perfect_withoutLINQ (int num)
        {
            
            //this is second version without LINQ 
            int sum_div = 0;
            foreach (var n in (Enumerable.Range(1, (int)Math.Sqrt(num))).AsParallel())
            {
                if (num % n == 0)
                {
                    sum_div += n;
                    if (num / n != n) { sum_div += num/n; }
                }
            }
            return sum_div == 2 * num;
        }
        
        public static void Main(string[] args)
        {
            //WITH ASPARALLEL AND WITHOUT LINQ IN FUNCTION 
            {
                //want to calculate time 
                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();

                //to create our range, and using PLINQ with AsParallel() 
                IEnumerable<int> numbers = Enumerable.Range(2, 10000000);
                var perfect_numbers_PLINQ = (from n in numbers.AsParallel() where Is_perfect_withoutLINQ(n) select n).ToList();
                stopwatch1.Stop();


                foreach (var n in perfect_numbers_PLINQ) { Console.WriteLine(n); }
                Console.WriteLine();
                Console.WriteLine("Time with AsParallel and without LINQ: " + stopwatch1.ElapsedMilliseconds + " мс");
            }

            //WITH ASPARALLEL AND WITH LINQ IN FUNCTION 
            {
                //want to calculate time 
                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();

                //to create our range, and using PLINQ with AsParallel() 
                IEnumerable<int> numbers = Enumerable.Range(1, 10000000);
                var perfect_numbers_PLINQ = (from n in numbers.AsParallel() where (from j in Enumerable.Range(1, (int)(Math.Sqrt(n))) where (n % j == 0 && (n / j) != j) select (n / j + j)).Sum() == n select n).ToList()        ;
                stopwatch2.Stop();


                Console.WriteLine("Time with AsParallel and with LINQ: " + stopwatch2.ElapsedMilliseconds + " мс");
            }

            //WITHOUT ASPARALLEL
            {
                //want to calculate time 
                Stopwatch stopwatch3 = new Stopwatch();
                stopwatch3.Start();

                //to create our range, and using PLINQ with AsParallel() 
                IEnumerable<int> numbers = Enumerable.Range(2, 10000000);
                var perfect_numbers_PLINQ = (from n in numbers where (from j in Enumerable.Range(1, (int)(Math.Sqrt(n))) 
                                                                      where (n % j == 0 && (n / j) != j) select (n / j + j)).Sum() == n select n).ToList();
                stopwatch3.Stop();

                Console.WriteLine("Time without AsParallel: " + stopwatch3.ElapsedMilliseconds + " мс");
            }


            //results:
            //Time with AsParallel and without LINQ: 9707 мс
            //Time with AsParallel and with LINQ: 193878 мс
            //Time without AsParallel: 74864 мс


        }
    }
}
