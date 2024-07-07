namespace Sharpl;

using System.Text;
using T = long;
using UT = ulong;

public static class Fix
{
    public static readonly int HeaderBits = 8;
    public static readonly int ExpBits = 7;

    public static T Scale(byte exp)
    {
        var result = 1;

        while (exp > 0)
        {
            result *= 10;
            exp--;
        }

        return result;
    }

    public static UT Make(byte exp, T val)
    {
        return exp +
         (UT)(((val < 0) ? 1 : 0) << ExpBits) +
         (UT)(((val < 0) ? -val : val) << HeaderBits);
    }

    public static byte Exp(UT it)
    {
        return (byte)(it & (UT)((1 << ExpBits) - 1));
    }

    public static T Val(UT it)
    {
        UT v = it >> HeaderBits;
        return (((it >> ExpBits) & 1) == 1) ? (long)v * -1 : (long)v;
    }

    public static T Trunc(UT it)
    {
        return Val(it) / Scale(Exp(it));
    }

    public static T Frac(UT it)
    {
        var v = Val(it);
        var s = Scale(Exp(it));
        return v % s;
    }

    public static string ToString(UT it)
    {
        var result = new StringBuilder();
        var t = Trunc(it);

        if (t >= 0)
        {
            if (t > 0) { result.Append(t); }
            result.Append('.');
            result.Append(Frac(it));
        }
        else
        {
            result.Append(t);
            result.Append('.');
            result.Append(Math.Abs(Frac(it)));
        }

        return result.ToString();
    }
}