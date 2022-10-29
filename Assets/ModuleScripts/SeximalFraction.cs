using KeepCoding;
using System.Collections.Generic;
using System.Linq;

public class SeximalFraction
{
    private int _numerator, _denominator;
    private List<int> _nonRecurring, _recurring;

    public SeximalFraction(int numerator, int denominator)
    {
        _numerator = numerator;
        _denominator = denominator;

        _nonRecurring = new List<int>();
        _recurring = new List<int>();

        Calculate();
    }

    private void Calculate()
    {
        LinkedList<int> remainders = new LinkedList<int>();
        int current = _numerator % _denominator;
        int index = -1;
        
        while (index == -1)
        {
            remainders.AddLast(current);
            current *= 6;
            _nonRecurring.Add(current / _denominator);
            current %= _denominator;
            index = remainders.IndexOf(current);
        }

        _recurring = _nonRecurring.Skip(index).ToList();
        _nonRecurring = _nonRecurring.Take(index).ToList();
        if (_recurring.Sum() == 0)
            _recurring = new List<int>();
    }

    public int Get(int index)
    {
        if(index < _nonRecurring.Count)
            return _nonRecurring[index];
        if(_recurring.Count == 0)
            return -1;

        index -= _nonRecurring.Count;
        return _recurring[index % _recurring.Count];
    }

    public int GetLoopLength(int loops)
    {
        return _nonRecurring.Count + _recurring.Count * (_recurring.Count > 16 ? 1 : loops);
    }

    public override string ToString()
    {
        return "." + _nonRecurring.Join("") + (_recurring.Count != 0 ? "[" + _recurring.Join("") + "]" : "");
    }
}