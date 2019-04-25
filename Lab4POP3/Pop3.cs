using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            long result = 1;
            for (int i = 261; i < 301; i++)
                result *= i;
            for (int i = 1; i < 60; i++)
                result /= i;
            Console.WriteLine(result.ToString());
            Console.ReadKey();
        }
    }
}
