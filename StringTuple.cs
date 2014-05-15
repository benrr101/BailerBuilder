using System;
using System.Collections.Generic;

namespace BailerBuilder
{
    class StringTuple : IComparable<StringTuple>, IEquatable<StringTuple>
    {
        public string First { get; set; }

        public string Second { get; set; }

        public StringTuple(string first, string second)
        {
            First = first;
            Second = second;
        }

        public int CompareTo(StringTuple other)
        {
            if (other == null)
                return 1;
            int num = Comparer<object>.Default.Compare(First, other.First);
            return num != 0 ? num : Comparer<object>.Default.Compare(Second, other.Second);
        }

        public bool Equals(StringTuple other)
        {
            if (other == null)
                return false;
            return EqualityComparer<object>.Default.Equals(First, other.First) && EqualityComparer<object>.Default.Equals(Second, other.Second);
        }
    }
}
