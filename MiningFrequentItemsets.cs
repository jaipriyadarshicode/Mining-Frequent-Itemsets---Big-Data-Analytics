using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APrioriPCY
{
    class MiningFrequentItemsets
    {
        static void Main(string[] args)
        {
            //calculate for different support threshold and data set size
            List<double> supportThreshold = new List<double>() {1.0, 5.0, 10.0};
            List<int> dataSetSize = new List<int>() { 1, 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            List<List<int>> readDataChunk = new List<List<int>>();
            int readBaskets = 0;
            string[] readDataSet = null;
            APriori aPriori = null;
            PCY pcy = null;

            //Read retail dataset containing anonymized retail market basket data 
            readDataSet = File.ReadAllLines(Path.GetFullPath("retail.txt"));
            foreach(double S in supportThreshold)
            {
                foreach (var size in dataSetSize)
                {
                    var chunk = (readDataSet.Count() * size) / 100;
                    var s = (int)(chunk * (S / 100.0));
                    while (readBaskets < chunk)
                    {
                        var templist = readDataSet[readBaskets].Split(' ').ToList();
                        templist.RemoveAt(templist.Count - 1);
                        var tempIntList = templist.Select(int.Parse).ToList();
                        readDataChunk.Add(tempIntList);
                        readBaskets++;
                    }
                    aPriori = new APriori(readDataChunk, (int)S, s, size, chunk);
                    pcy = new PCY(readDataChunk,(int)S, s, size, chunk);
                    readDataChunk.Clear();
                    readBaskets = 0;
                }
            }
        }
    }
}
