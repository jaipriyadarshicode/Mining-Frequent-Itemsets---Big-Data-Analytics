using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APrioriPCY
{
    public class PCY
    {
        Dictionary<Tuple<int, int>, int> itemCount = null;//stores item count when scanning the file
        Dictionary<int, int> hashBuckets = null;//hash bucket concept data structure of PCY
        List<int> frequentItemSets = null;//stores single frequent item sets
        int numberOfHashBuckets = 0;//bucket count
        System.Diagnostics.Stopwatch timeTaken = null;
        public PCY (List<List<int>> Baskets, int S, int supportThreshold, double chunckSize, int chunk)//baskets to analyse inaccordance to support S
        {
            itemCount = new Dictionary<Tuple<int, int>, int>();
            hashBuckets = new Dictionary<int, int>();
            frequentItemSets = new List<int>();
            timeTaken = System.Diagnostics.Stopwatch.StartNew();
            Pass1(Baskets, hashBuckets, supportThreshold);
            timeTaken.Stop();
            System.Diagnostics.Debug.WriteLine("Time taken: PCY - " + timeTaken.ElapsedMilliseconds + " Support Threshold = " + S + " chunckSize = "+ chunckSize + " chunk = " + chunk);
        }
        
        //Pass 1 calculates both singleton frequent item set count and hashes pair count with conversion to bitmap
        //Uses memory idle in apriori algorithm
        public void Pass1(List<List<int>> Baskets, Dictionary<int, int> hashBuckets, int supportThreshold)
        {
            //Finding the count of unique items
            Baskets
                .ForEach(itemset => UniqueItemCount(itemset));

            //frequent singleton item sets
            foreach (var item in itemCount)
                if (item.Value >= supportThreshold) frequentItemSets.Add(item.Key.Item1);
            itemCount.Clear();

            //find pairs of items in itemset or basket
            FindPairs(frequentItemSets, itemCount);

            //Keep the count for the pairs in bucket. We do not hash the actual pair into the hashBucket
            //Number of buckets = 10
            Baskets
                .ForEach(itemset => hashPairCount(itemset));

            //Convert hashBucket to a Bitmap----> frequent bucket value = 1 & infrequent bucket value = 0
            for (int hashBucket = 0; hashBucket < numberOfHashBuckets; hashBucket++)
                if (hashBuckets[hashBucket] >= supportThreshold)
                    hashBuckets[hashBucket] = 1;
                else
                    hashBuckets[hashBucket] = 0;
            itemCount.Clear();

            //call pass 2
            Pass2(frequentItemSets, hashBuckets);

        }
        //Pass 2 of PCY counts the candidate pairs both frequent as single item and in frequent bucket
        public void Pass2(List<int> singleFrequentItems, Dictionary<int, int> bitmap)
        {
            for (int i = 0; i < singleFrequentItems.Count; i++)
                for (int j = i + 1; j < singleFrequentItems.Count; j++)
                {
                    var bucketValue = (singleFrequentItems.ElementAt(i) + singleFrequentItems.ElementAt(j)) %numberOfHashBuckets;//bucket number
                    if (bitmap[bucketValue] == 1);//checking if the pair belongs to the frequent bucket or not and then print it
                        //System.Diagnostics.Debug.WriteLine("{" + singleFrequentItem_1 + " , " + singleFrequentItem_2 + "}");
                }
        }

        //Finds unique singleton frequent item sets and its count
        public void UniqueItemCount(List<int> Itemset)
        {   Itemset
                .ForEach(item =>
                {
                    var singleitemset = new Tuple<int, int>(item, 0);
                    if (!itemCount.ContainsKey(singleitemset)) itemCount.Add(singleitemset, 1);
                    else itemCount[singleitemset] = itemCount[singleitemset] + 1;
                });
        }

        //Finds pairs in the given retail file
        public void FindPairs(List<int> frequentItems, Dictionary<Tuple<int, int>, int> pairCount)
        {
            for (int i = 0; i < frequentItems.Count; i++)
                for (int j = i + 1; j < frequentItems.Count; j++)
                    pairCount.Add(new Tuple<int, int>(frequentItems.ElementAt(i), frequentItems.ElementAt(j)), 0);
            numberOfHashBuckets = pairCount.Count/2;
            for (int i = 0; i < numberOfHashBuckets; i++)//creating hash buckets
                hashBuckets.Add(i, 0);
        }

        //Hashes pair count and not pairs
        public void hashPairCount(List<int> itemset)
        {
            foreach (var pair in itemCount)//just accessing all pairs in hash table (Dictionary in C#)
            {
                if (itemset.Contains(pair.Key.Item1) && itemset.Contains(pair.Key.Item2))
                {
                    var bucketNumber = (pair.Key.Item1 + pair.Key.Item2) % numberOfHashBuckets;
                    hashBuckets[bucketNumber] = hashBuckets[bucketNumber] + 1;
                }
            }
        }
    }
}