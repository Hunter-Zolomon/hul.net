using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HUL
{
    /* Implement Priority Queue (Min/Max)*/

    public static class Heap
    {
        private static int Parent(int i) { return (int)(Math.Floor((decimal)(i / 2))); }

        private static int Left(int i) { return ((2 * i) + 1); }

        private static int Right(int i) { return ((2 * i) + 2); }

        public static void Swap(int[]array, int indexa, int indexb)
        {
            int temp = array[indexa];
            array[indexa] = array[indexb];
            array[indexb] = temp;
        }

        public static void Swap(double[] array, int indexa, int indexb)
        {
            double temp = array[indexa];
            array[indexa] = array[indexb];
            array[indexb] = temp;
        }

        public static void MaxHeapify(int[] array, int startindex, int heapsize)
        {
            int heapSize = heapsize;
            int left = Left(startindex);
            int right = Right(startindex);
            int largest;

            if ((left < heapSize) && (array[left] > array[startindex])) { largest = left; }
            else { largest = startindex; }
            if ((right < heapSize) && (array[right] > array[largest])) { largest = right; }

            if (largest != startindex)
            {
                Swap(array, startindex, largest);
                MaxHeapify(array, largest, heapsize);
            }
        }

        public static void MaxHeapify(double[] array, int startindex, int heapsize)
        {
            int heapSize = heapsize;
            int left = Left(startindex);
            int right = Right(startindex);
            int largest;

            if ((left < heapSize) && (array[left] > array[startindex])) { largest = left; }
            else { largest = startindex; }
            if ((right < heapSize) && (array[right] > array[largest])) { largest = right; }

            if (largest != startindex)
            {
                Swap(array, startindex, largest);
                MaxHeapify(array, largest, heapsize);
            }
        }

        public static void MinHeapify(int[] array, int startindex, int heapsize)
        {
            int heapSize = heapsize;
            int left = Left(startindex);
            int right = Right(startindex);
            int smallest;

            if ((left < heapSize) && (array[left] < array[startindex])) { smallest = left; }
            else { smallest = startindex; }
            if ((right < heapSize) && (array[right] < array[smallest])) { smallest = right; }

            if (smallest != startindex)
            {
                Swap(array, startindex, smallest);
                MinHeapify(array, smallest, heapsize);
            }
        }

        public static void MinHeapify(double[] array, int startindex, int heapsize)
        {
            int heapSize = heapsize;
            int left = Left(startindex);
            int right = Right(startindex);
            int smallest;

            if ((left < heapSize) && (array[left] < array[startindex])) { smallest = left; }
            else { smallest = startindex; }
            if ((right < heapSize) && (array[right] < array[smallest])) { smallest = right; }

            if (smallest != startindex)
            {
                Swap(array, startindex, smallest);
                MinHeapify(array, smallest, heapsize);
            }
        }

        public static void BuildMaxHeap(int[] array)
        {
            int heapSize = array.Length;
            for (int i = (int)(Math.Floor((decimal)((heapSize / 2) - 1))); i >= 0; i--) { MaxHeapify(array, i, heapSize); }
        }

        public static void BuildMaxHeap(double[] array)
        {
            int heapSize = array.Length;
            for (int i = (int)(Math.Floor((decimal)((heapSize / 2) - 1))); i >= 0; i--) { MaxHeapify(array, i, heapSize); }
        }

        public static void BuildMinHeap(int[] array)
        {
            int heapSize = array.Length;
            for (int i = (int)(Math.Floor((decimal)((heapSize / 2) - 1))); i >= 0; i--) { MinHeapify(array, i, heapSize);}
        }

        public static void BuildMinHeap(double[] array)
        {
            int heapSize = array.Length;
            for (int i = (int)(Math.Floor((decimal)((heapSize / 2) - 1))); i >= 0; i--) { MinHeapify(array, i, heapSize); }
        }

        public static void HeapSort(int[] array)
        {
            BuildMaxHeap(array);
            int heapSize = array.Length;
            for (int i = array.Length; i > 0; i--)
            {
                Swap(array, 0, i - 1);
                heapSize = heapSize - 1;
                MaxHeapify(array, 0, heapSize);
            }
        }

        public static void HeapSort(double[] array)
        {
            BuildMaxHeap(array);
            int heapSize = array.Length;
            for (int i = array.Length; i > 0; i--)
            {
                Swap(array, 0, i - 1);
                heapSize = heapSize - 1;
                MaxHeapify(array, 0, heapSize);
            }
        }
    }

    public static class QuickSort
    {
        public static void NormalQuickSort(int[] array, int p, int r)
        {
            if (p < r)
            {
                int q = Partition(array, p, r);
                NormalQuickSort(array, p, q - 1);
                NormalQuickSort(array, q + 1, r);
            }
        }

        public static void MTNormalQuickSort(int[] array, int p, int r)
        {
            if (r - p < 2000) { NormalQuickSort(array, p, r); }
            else
            {
                int q = Partition(array, p, r);
                Parallel.Invoke(
                    () => MTNormalQuickSort(array, p, q - 1),
                    () => MTNormalQuickSort(array, q + 1, r)
                    );
            }
        }

        private static int Partition(int[] array, int p, int r)
        {
            int x = array[r];
            int i = p - 1;
            for (int j = p; j < r; j++) { if (array[j] <= x) { i++; Heap.Swap(array, i, j); } }
            Heap.Swap(array, i + 1, r);
            return i + 1;
        }

        public static void RandomizedQuickSort(int[] array, int p, int r)
        {
            if (p < r)
            {
                int q = RandomizedPartition(array, p, r);
                RandomizedQuickSort(array, p, q - 1);
                RandomizedQuickSort(array, q + 1, r);
            }
        }

        public static void MTRandomizedQuickSort(int[] array, int p, int r)
        {
            if (r - p < 2000) { NormalQuickSort(array, p, r); }
            else
            {
                int q = RandomizedPartition(array, p, r);
                Parallel.Invoke(
                    () => MTRandomizedQuickSort(array, p, q - 1),
                    () => MTRandomizedQuickSort(array, q + 1, r)
                );
            }
        }

        private static int RandomizedPartition(int[] array, int p, int r)
        {
            int i = new Random().Next(p, r);
            Heap.Swap(array, r, i);
            return Partition(array, p, r);
        }
    }

    public static class LinearSorting
    {
        private static void BInitializeBuckets(List<List<double>> buckets)
        {
            for (int i = 0; i < 10; i++)
            {
                List<double> a = new List<double>();
                buckets.Add(a);
            }
        }

        private static int BGetBucketNumber(double value)
        {
            double val = value * 10;
            int bucketNumber = (int)Math.Floor(val);
            return bucketNumber;
        }

        private static void BScatter(double[] array, List<List<double>> buckets)
        {
            foreach (double value in array)
            {
                int bucketNumber = BGetBucketNumber(value);
                buckets[bucketNumber].Add(value);
            }
        }

        private static void InsertionSort(double[] array)
        {
            int j;
            double temp;

            for (int i = 1; i < array.Length; i++)
            {
                j = i;
                while (j > 0 && array[j] < array[j - 1])
                {
                    temp = array[j];
                    array[j] = array[j - 1];
                    array[j - 1] = temp;
                    j--;
                }
            }
        }

        public static double[] BSort(double[] array)
        {
            List<List<double>> buckets = new List<List<double>>();
            BInitializeBuckets(buckets);
            BScatter(array, buckets);

            int i = 0;
            foreach (List<double> bucket in buckets)
            {
                double[] auxiliary = bucket.ToArray();
                Heap.HeapSort(auxiliary);
                foreach (double d in auxiliary) { array[i++] = d; }
            }

            return array;
        }

        private static void RInitializeBuckets(List<Queue<int>> buckets)
        {
            for (int i = 0; i < 10; i++)
            {
                Queue<int> q = new Queue<int>();
                buckets.Add(q);
            }
        }

        private static int RGetBucketNumber(int value, int digitPosition)
        {
            int bucketNumber = (value / (int)Math.Pow(10, digitPosition)) % 10;
            return bucketNumber;
        }

        private static void RScatter(int[] array, List<Queue<int>> buckets, ref bool finished, int digitPosition)
        {
            foreach (int value in array)
            {
                int bucketNumber = RGetBucketNumber(value, digitPosition);
                if (bucketNumber > 0) { finished = false; }
                buckets[bucketNumber].Enqueue(value);
            }
        }

        public static int[] RSort(int[] array)
        {
            bool Finished = false;
            int digitPosition = 0;

            List<Queue<int>> buckets = new List<Queue<int>>();
            RInitializeBuckets(buckets);

            while (!Finished)
            {
                Finished = true;
                RScatter(array, buckets, ref Finished, digitPosition);

                int i = 0;
                foreach (Queue<int> bucket in buckets)
                {
                    while (bucket.Count > 0) { array[i] = bucket.Dequeue(); i++; }
                }

                digitPosition++;
            }

            return array;
        }

        public static int[] CSort(int[] array)
        {
            int[] auxiliary = new int[array.Max() + 1];
            int[] returnarray = new int[array.Length];
            for (int i = 0; i < auxiliary.Length; i++) { auxiliary[i] = 0; }
            for (int j = 0; j < array.Length; j++) { auxiliary[array[j]] = auxiliary[array[j]] + 1; }
            for (int k = 1; k < auxiliary.Length; k++) { auxiliary[k] = auxiliary[k] + auxiliary[k - 1]; }
            for (int o = 0; o < auxiliary.Length; o++) { auxiliary[o] = auxiliary[o] - 1; }

            for (int w = array.Length - 1; w > -1; w--)
            {
                returnarray[auxiliary[array[w]]] = array[w];
                auxiliary[array[w]] = auxiliary[array[w]] - 1;
            }

            return returnarray;
        }

        //public static int[] MTCSort(int[] array)
        //{
        //    int divider = (int) Math.Floor((double) array.Length / 8);
        //    int remainder = array.Length % 8;
        //    int[] array1 = array.Skip(0).Take(divider).ToArray();
        //    int[] array2 = array.Skip(1 * divider).Take(divider).ToArray();
        //    int[] array3 = array.Skip(2 * divider).Take(divider).ToArray();
        //    int[] array4 = array.Skip(3 * divider).Take(divider).ToArray();
        //    int[] array5 = array.Skip(4 * divider).Take(divider).ToArray();
        //    int[] array6 = array.Skip(5 * divider).Take(divider).ToArray();
        //    int[] array7 = array.Skip(6 * divider).Take(divider).ToArray();
        //    int[] array8 = array.Skip(7 * divider).Take(divider + remainder).ToArray();
        //    Parallel.Invoke(
        //        () => array1 = CSort(array1),
        //        () => array2 = CSort(array2),
        //        () => array3 = CSort(array3),
        //        () => array4 = CSort(array4),
        //        () => array5 = CSort(array5),
        //        () => array6 = CSort(array6),
        //        () => array7 = CSort(array7),
        //        () => array8 = CSort(array8)
        //        );
        //    array = array1.Concat(array2.Concat(array3.Concat(array4.Concat(array5.Concat(array6.Concat(array7.Concat(array8))))))).ToArray();
        //    return CSort(array);
        //}
    }
}
