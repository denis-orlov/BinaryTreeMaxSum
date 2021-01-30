using System;
using System.Collections.Generic;
using System.Linq;

namespace BinaryTreeMaxSum
{
    class Program
    {
        private static readonly string _input = @"
215
192 124
117 269 442
218 836 347 235
320 805 522 417 345
229 601 728 835 133 124
248 202 277 433 207 263 257
359 464 504 528 516 716 871 182
461 441 426 656 863 560 380 171 923
381 348 573 533 448 632 387 176 975 449
223 711 445 645 245 543 931 532 937 541 444
330 131 333 928 376 733 017 778 839 168 197 197
131 171 522 137 217 224 291 413 528 520 227 229 928
223 626 034 683 839 052 627 310 713 999 629 817 410 121
924 622 911 233 325 139 721 218 253 223 107 233 230 124 233";

        static void Main(string[] args)
        {
            List<int>[] inputValues = ParseInput(_input);

            var cache = BuildCache(inputValues);

            var result = GetPathAndMaxSumFromCache(cache);

            Console.WriteLine($"Max sum: {result.Item2}");
            Console.WriteLine($"Path: {string.Join(", ", result.Item1)}");
            Console.ReadLine();
        }

        private static List<int>[] ParseInput(string input)
        {
            string[] inputStrings = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            List<int>[] inputValues = new List<int>[inputStrings.Length];

            for (int i = 0; i < inputStrings.Length; i++)
            {
                inputValues[i] = inputStrings[i].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(str => int.Parse(str)).ToList();
            }

            return inputValues;
        }

        private static List<List<CacheItem>>[] BuildCache(List<int>[] inputValues)
        {
            List<List<CacheItem>>[] cache = new List<List<CacheItem>>[inputValues.Length];
            cache[0] = new List<List<CacheItem>> { new List<CacheItem> { new CacheItem(inputValues[0][0], inputValues[0][0], null) } };//add cache for root element

            for (int rowIndex = 1; rowIndex < inputValues.Length; rowIndex++)
            {
                cache[rowIndex] = new List<List<CacheItem>>();

                for (int index = 0; index < inputValues[rowIndex].Count; index++)
                {
                    List<CacheItem> cacheList = new List<CacheItem>();

                    int parentRowIndex = rowIndex - 1;
                    int val = inputValues[rowIndex][index];

                    if (index > 0)//check if there can be parent at left
                    {
                        int parentValue = inputValues[rowIndex - 1][index - 1];
                        var parentCache = cache[parentRowIndex][index - 1];
                        
                        AddMaxSumWithOddEvenCheck(cacheList, parentCache, parentValue, val);
                    }

                    if (inputValues[rowIndex].Count - index > 1)//check if there can be parent at top
                    {
                        int parentValue = inputValues[rowIndex - 1][index];
                        var parentCache = cache[parentRowIndex][index];

                        AddMaxSumWithOddEvenCheck(cacheList, parentCache, parentValue, val);
                    }

                    if (inputValues[rowIndex].Count - index > 3)//check if there can be parent at right
                    {
                        int parentValue = inputValues[rowIndex - 1][index + 1];
                        var parentCache = cache[parentRowIndex][index + 1];

                        AddMaxSumWithOddEvenCheck(cacheList, parentCache, parentValue, val);
                    }

                    cache[rowIndex].Add(cacheList);
                }
            }

            return cache;
        }

        private static void AddMaxSumWithOddEvenCheck(List<CacheItem> list, List<CacheItem> parentCache, int parentValue, int currentVal)
        {
            bool oddEvenOk = (currentVal + parentValue) % 2 != 0;

            if (oddEvenOk)
            {
                CacheItem maxCache = null;

                foreach (CacheItem parent in parentCache)
                {
                    if (maxCache == null || maxCache.Sum < parent.Sum + currentVal)
                    {
                        maxCache = new CacheItem(parent.Sum + currentVal, currentVal, parent);
                    }
                }

                if (maxCache != null)
                {
                    list.Add(maxCache);
                }
            }
        }

        private static (List<int>, long) GetPathAndMaxSumFromCache(List<List<CacheItem>>[] cache)
        {
            var bottomLineCache = cache[^1];

            var maxCache = bottomLineCache.SelectMany(list => list).OrderByDescending(p => p.Sum).First();

            List<int> path = new List<int>();

            CacheItem cacheItem = maxCache;

            while (cacheItem != null)
            {
                path.Add(cacheItem.Value);

                cacheItem = cacheItem.ParentCacheItem;
            }

            path.Reverse();

            return (path, maxCache.Sum);
        }

        private class CacheItem
        {
            public long Sum { get; }

            public int Value { get; }
            
            public CacheItem ParentCacheItem { get; }

            public CacheItem(long sum, int value, CacheItem parentCacheItem)
            {
                Sum = sum;
                Value = value;
                ParentCacheItem = parentCacheItem;
            }
        }
    }
}
