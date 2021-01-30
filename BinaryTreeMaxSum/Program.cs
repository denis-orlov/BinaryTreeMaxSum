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
            var inputValues = ParseInput(_input);

            var cache = BuildCache(inputValues);

            var result = GetPathAndMaxSumFromCache(cache);

            Console.WriteLine($"Max sum: {result.Item2}");
            Console.WriteLine($"Path: {string.Join(", ", result.Item1)}");
            Console.ReadLine();
        }

        private static int[][] ParseInput(string input)
        {
            string[] inputStrings = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            int[][] inputValues = new int[inputStrings.Length][];

            for (int i = 0; i < inputStrings.Length; i++)
            {
                inputValues[i] = inputStrings[i].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(str => int.Parse(str)).ToArray();
            }

            return inputValues;
        }

        private static CacheItem[][] BuildCache(int[][] inputValues)
        {
            CacheItem[][] cache = new CacheItem[inputValues.Length][];
            cache[0] = new CacheItem[1] { new CacheItem(inputValues[0][0]) { LeftParentSum = inputValues[0][0] } };//add cache for root element

            for (int rowIndex = 1; rowIndex < inputValues.Length; rowIndex++)
            {
                cache[rowIndex] = new CacheItem[inputValues[rowIndex].Length];

                for (int index = 0; index < inputValues[rowIndex].Length; index++)
                {
                    int val = inputValues[rowIndex][index];
                    int parentRowIndex = rowIndex - 1;

                    CacheItem cacheItem = new CacheItem(val);

                    if (index > 0)//check if there can be parent at left
                    {
                        int parentValue = inputValues[parentRowIndex][index - 1];
                        var parentCache = cache[parentRowIndex][index - 1];

                        if (CheckOddEven(parentValue, val))
                        {
                            cacheItem.LeftParent = parentCache;
                            cacheItem.LeftParentSum = parentCache.MaxSum + val;
                        }
                    }

                    if (inputValues[rowIndex].Length - index > 1)//check if there can be parent at top
                    {
                        int parentValue = inputValues[parentRowIndex][index];
                        var parentCache = cache[parentRowIndex][index];

                        if (CheckOddEven(parentValue, val))
                        {
                            cacheItem.TopParent = parentCache;
                            cacheItem.TopParentSum = parentCache.MaxSum + val;
                        }
                    }

                    if (inputValues[rowIndex].Length - index > 3)//check if there can be parent at right
                    {
                        int parentValue = inputValues[parentRowIndex][index + 1];
                        var parentCache = cache[parentRowIndex][index + 1];

                        if (CheckOddEven(parentValue, val))
                        {
                            cacheItem.RightParent = parentCache;
                            cacheItem.RightParentSum = parentCache.MaxSum + val;
                        }
                    }

                    cache[rowIndex][index] = cacheItem;
                }
            }

            return cache;
        }

        private static bool CheckOddEven(int parentValue, int currentVal)
        {
            return (currentVal + parentValue) % 2 != 0;
        }

        private static (List<int>, long) GetPathAndMaxSumFromCache(CacheItem[][] cache)
        {
            var bottomLineCache = cache[^1];

            var maxCache = bottomLineCache.OrderByDescending(p => p.MaxSum).First();

            List<int> path = new List<int>();

            CacheItem cacheItem = maxCache;

            while (cacheItem != null)
            {
                path.Add(cacheItem.Value);

                cacheItem = cacheItem.MaxParent;
            }

            path.Reverse();

            return (path, maxCache.MaxSum);
        }

        private class CacheItem
        {
            public long LeftParentSum { get; set; }
            public CacheItem LeftParent { get; set; }


            public long TopParentSum { get; set; }
            public CacheItem TopParent { get; set; }


            public long RightParentSum { get; set; }
            public CacheItem RightParent { get; set; }

            public int Value { get; }

            public CacheItem(int value)
            {
                Value = value;
            }

            public long MaxSum
            {
                get
                {
                    return Math.Max(LeftParentSum, Math.Max(TopParentSum, RightParentSum));
                }
            }

            public CacheItem MaxParent
            {
                get
                {
                    long maxSum = MaxSum;

                    if (maxSum == LeftParentSum)
                    {
                        return LeftParent;
                    }

                    if (maxSum == RightParentSum)
                    {
                        return RightParent;
                    }

                    return TopParent;
                }
            }
        }
    }
}
