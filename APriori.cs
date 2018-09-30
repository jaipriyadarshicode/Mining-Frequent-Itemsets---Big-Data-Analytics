using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APrioriPCY
{
    public class APriori
    {
        Dictionary<Tuple<int, int>, int> ItemCount = null;//stores item count when scanning the file
        List<int> frequentItemSets = null;//stores single frequent item sets
        System.Diagnostics.Stopwatch timeTaken = null;
        public APriori(List<List<int>> Baskets,int S, int supportThreshold, double chunckSize, int chunk)
        {
            ItemCount = new Dictionary<Tuple<int, int>, int>();
            frequentItemSets = new List<int>();
            timeTaken = System.Diagnostics.Stopwatch.StartNew();
            Pass1(Baskets, supportThreshold);
            timeTaken.Stop();
            System.Diagnostics.Debug.WriteLine("Time taken: APriori - " + timeTaken.ElapsedMilliseconds + " Support Threshold = " + S + " chunckSize = " + chunckSize + " chunk = " + chunk);
        }

        //Counts all the occurence of a unique singleton frequent item set
        public void Pass1(List<List<int>> Baskets, int supportThreshold)
        {
            //Finding the count of unique items
            Baskets
                .ForEach(itemset => UniqueItemCount(itemset));
            BetweenThePassesOfAPriori(Baskets,ItemCount, supportThreshold);
        }
        //Finds the singleton frequent item sets inccordance to the support threshold S
        public void BetweenThePassesOfAPriori(List<List<int>> Baskets, Dictionary<Tuple<int, int>, int> itemcount, int supportThreshold)
        {
            //frequenty singleton item sets
            foreach(var item in itemcount)
                if (item.Value >= supportThreshold) frequentItemSets.Add(item.Key.Item1);
            ItemCount.Clear();
            Pass2(frequentItemSets, Baskets, ItemCount, supportThreshold);
        }
        //Finds the frequent pairs item set inccordance to the support threshold S 
        public void Pass2(List<int> frequentItems, List<List<int>> Baskets, Dictionary<Tuple<int, int>, int> pairCount, int supportThreshold)
        {
            FindPairs(frequentItems, pairCount);
            frequentItemSets.Clear();
            Baskets
                .ForEach(itemset => FindPairCount(itemset, pairCount));
            foreach (var item in pairCount)
                if (item.Value >= supportThreshold)
                    //you can print on console the frequent pairs item set
                    //System.Diagnostics.Debug.Write("{" + item.Key.Item1 +" , " + item.Key.Item2 + "}")
                        ;
            pairCount.Clear();
        }

        //Finds unique singleton frequent item sets and its count
        public void UniqueItemCount(List<int> Itemset)
        {
            Itemset.ForEach(item => 
               {
                   if (!ItemCount.ContainsKey(new Tuple<int, int>(item, 0))) ItemCount.Add(new Tuple<int, int>(item, 0), 1);
                   else ItemCount[new Tuple<int, int>(item, 0)] = ItemCount[new Tuple<int, int>(item, 0)] + 1;
               });
        }
        //Finds pairs in the given retail file
        public void FindPairs(List<int> frequentItems, Dictionary<Tuple<int, int>, int> pairCount)
        {
            for(int i = 0; i < frequentItems.Count; i++)
                for(int j = i + 1; j < frequentItems.Count; j++)
                    pairCount.Add(new Tuple<int, int>(frequentItems.ElementAt(i), frequentItems.ElementAt(j)), 0);        
        }
        //Hashes both pair and its occurence and not only the pair count
        public void FindPairCount(List<int> Itemset, Dictionary<Tuple<int,int>, int> pairCount)
        {
            Dictionary<Tuple<int,int>, int> tempPairCount = new Dictionary<Tuple<int, int>, int>();
            foreach (var pair in pairCount)//just accessing all pairs in hash table (Dictionary in C#)
            {
                if(Itemset.Contains(pair.Key.Item1) && Itemset.Contains(pair.Key.Item2))
                  tempPairCount.Add(pair.Key, ItemCount[pair.Key] + 1);
            }
            foreach (var tempPair in tempPairCount)//just to restore array as C# does not allow you to change and iterate the same dictionary data set simoultaneously
                pairCount[tempPair.Key] = tempPair.Value; 
        }
    }
}
