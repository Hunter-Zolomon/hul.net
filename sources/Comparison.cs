using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUL
{
    public class Comparison
    {
        public class GenericComparer<T> : IComparer<T> where T : IComparable
        {
            public int Compare(T x, T y)
            {
                if (!typeof(T).IsValueType
                    || (typeof(T).IsGenericType
                        && typeof(T).GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))))
                {
                    if (Object.Equals(x, default(T)))
                        return Object.Equals(y, default(T)) ? 0 : -1;
                    if (Object.Equals(y, default(T)))
                        return -1;
                }
                if (x.GetType() != y.GetType())
                    return -1;
                if (x is IComparable<T> tempComparable)
                    return tempComparable.CompareTo(y);
                return x.CompareTo(y);
            }
        }

        public class GenericEqualityComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                if (!typeof(T).IsValueType
                    || (typeof(T).IsGenericType
                    && typeof(T).GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))))
                {
                    if (Object.Equals(x, default(T)))
                        return Object.Equals(y, default(T));
                    if (Object.Equals(y, default(T)))
                        return false;
                }
                if (x.GetType() != y.GetType())
                    return false;
                if (x is IEnumerable enumerablex && y is IEnumerable enumerabley)
                {
                    var comparer = new GenericEqualityComparer<object>();
                    var xEnumerator = enumerablex.GetEnumerator();
                    var yEnumerator = enumerabley.GetEnumerator();
                    while (true)
                    {
                        bool xFinished = !xEnumerator.MoveNext();
                        bool yFinished = !yEnumerator.MoveNext();
                        if (xFinished || yFinished)
                            return xFinished & yFinished;
                        if (!comparer.Equals(xEnumerator.Current, yEnumerator.Current))
                            return false;
                    }
                }

                if (x is IEqualityComparer<T> tempEquality)
                    return tempEquality.Equals(y);
                if (x is IComparable<T> tempComparable)
                    return tempComparable.CompareTo(y) == 0;
                if (x is IComparable tempComparable2)
                    return tempComparable2.CompareTo(y) == 0;
                return x.Equals(y);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

        public class CustomComparer<T> : IComparer<T> where T : IComparable
        {
            public CustomComparer(Func<T, T, int> comparisonFunction) { ComparisonFunction = comparisonFunction; }

            protected Func<T, T, int> ComparisonFunction { get; set; }

            public int Compare(T x, T y) { return ComparisonFunction(x, y); }
        }

        public class CustomEqualityComparer<T> : IEqualityComparer<T>
        {
            public CustomEqualityComparer(Func<T, T, bool> comparisonFunction, Func<T, int> hashFunction)
            {
                ComparisonFunction = comparisonFunction;
                HashFunction = hashFunction;
            }

            protected Func<T, T, bool> ComparisonFunction { get; set; }

            protected Func<T, int> HashFunction { get; set; }

            public bool Equals(T x, T y) { return ComparisonFunction(x, y); }

            public int GetHashCode(T obj) { return HashFunction(obj); }
        }
    }
}
