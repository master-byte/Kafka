using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class NumberGenerator 
    {             
        int GenerateRandom(Random random, int minNumber)
        {
            return random.Next(minNumber, minNumber + 100);
        }

        public int GenerateElement(int minNumber)
        {
            return GenerateRandom(new Random(), minNumber);
        }        
    }
}
