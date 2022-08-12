using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace ImageMap4;

public class EqualityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 0)
            return false;
        for (int i = 1; i < values.Length; i++)
        {
            if (values[0] != values[i])
                return false;
        }
        return true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}

public class MultiplyConverter : ParameterConverter<double, double, double>
{
    public override double Convert(double value, double parameter)
    {
        return value * parameter;
    }

    public static readonly MultiplyConverter Instance = new();
}

public class GreaterThanConverter : ParameterConverter<int, bool, int>
{
    public override bool Convert(int value, int parameter)
    {
        return value > parameter;
    }
}
