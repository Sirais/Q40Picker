using System;
using System.Collections.Generic;
using Druzil.Poe.Libs;

namespace Subsettest
{
    class Program
    {

        static void Main(string[] args)
        {
            List<setData> dta = getdata();
            Console.ReadKey();
            SetFinder Sets = new SetFinder(dta, 40);
            Console.WriteLine("found set : ");
            int i = 1;
            foreach (QualityGem g in Sets.BestSet.Values)
            {
                Console.WriteLine($"{i:2} : {g.ToString()} - Q{g.getValue()} ");
                i++;
            }
            Console.ReadKey();
        }

        private static List<setData> getdata()
        {
            List<setData> res = new List<setData>();
            Random rnd = new Random();
            for (int i=1;i<=400; i++)
            {
                QualityGem q = new QualityGem(i.ToString(), rnd.Next(19) + 1);
                res.Add(q);
                Console.WriteLine(q.ToString());
            }
            return res;

        }

        /// <summary>
        /// test-Class for Subset
        /// </summary>
        public class QualityGem : setData
        {
            public String Name { get; set; }
            public int Quality { get; set; }
            public QualityGem(String name, int quality)
            {
                Name = name;
                Quality = quality;
            }

            public int getValue()
            {
                return Quality; // Gem.GetComponent<Quality>().ItemQuality;
            }

            public override string ToString()
            {
                return Name + " : Q"+Quality.ToString();
            }

        }

    }
}
