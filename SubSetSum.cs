using System;
using System.Collections.Generic;

using System.IO;

namespace Druzil.Poe.Libs
{
    /// <summary>
    /// Base 
    /// </summary>
    public interface setData
    {
        int getValue();
    }

    public class SubSet : IComparable<SubSet>
    {
        public readonly List<setData> Values;
        public readonly int TotalValue;

        public SubSet(List<setData> qualities, int totalQuality)
        {
            Values = qualities;
            TotalValue = totalQuality;
        }

        //For sort !
        public int CompareTo(SubSet other)
        {
            int diff = TotalValue - other.TotalValue;
            if (diff == 0) return Values.Count - other.Values.Count;
            return diff;
        }
    }



    public class SetFinder
    {
        private readonly List<setData> _numbers;
        private readonly List<SubSet> _sets;
        private SubSet _perfectSet;

        public SubSet BestSet
        {
            get
            {
                if (_perfectSet != null)
                    return _perfectSet;

                if (_sets.Count > 0)
                {
                    _sets.Sort();
                    return _sets[0];
                }
                return null;
            }
        }

        public SetFinder(List<setData> numbers, int Value)
        {
            _numbers = numbers;
            _sets = new List<SubSet>();
            FindSets(new bool[numbers.Count], 0, 0, Value);
        }

        // Recursion for Subset
        private void FindSets(bool[] solution, int currentSum, int index, int sum)
        {
            if (currentSum == sum) // Found the wanted Sum
            {
                _perfectSet = CreateSubSet(solution, currentSum); // Save the 
                return;
            }

            if (currentSum > sum) // Suolution already extends the wanted Sum, so leave 
                return;

            if (_perfectSet != null || index == _numbers.Count) // Searched every element, and didnt found the wanted Value
                return;
            solution[index] = true;
            currentSum += _numbers[index].getValue();
            FindSets(solution, currentSum, index + 1, sum);

            solution[index] = false;
            currentSum -= _numbers[index].getValue();
            FindSets(solution, currentSum, index + 1, sum);
        }

        private SubSet CreateSubSet(bool[] solution, int sum)
        {
            var list = new List<setData>();
            for (int i = 0; i < solution.Length; ++i)
            {
                if (solution[i])
                {
                    list.Add(_numbers[i]);
                }
            }
            return new SubSet(list, sum);
        }
    }
}
