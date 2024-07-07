namespace Sharpl;

using System.Reflection.Emit;
using System.Text;
using T = long;
using UT = ulong;

public static class Fix
{
    public static readonly byte HeaderBits = 5;
    public static readonly byte ExpBits = 4;

    public static UT Add(UT left, UT right)
    {
        var le = Exp(left);
        return Make(le, Val(left) + Val(right) * Scale(le) / Scale(Exp(right)));
    }

    public static bool Equals(UT left, UT right)
    {
        return Val(left) * Scale(Exp(right)) == Val(right) * Scale(Exp(left));
    }

    public static UT Divide(UT left, UT right)
    {
        return Make(Exp(left), Val(left) / (Val(right) / Scale(Exp(right))));
    }

    public static byte Exp(UT it)
    {
        return (byte)(it & (UT)((1 << ExpBits) - 1));
    }

    public static UT Make(byte exp, T val)
    {
        return (UT)(exp & ((1 << ExpBits) - 1)) +
         (UT)(((val < 0) ? 1 : 0) << ExpBits) +
         (UT)(((val < 0) ? -val : val) << HeaderBits);
    }

    public static UT Multiply(UT left, UT right)
    {
        return Make(Exp(left), Val(left) * Val(right) / Scale(Exp(right)));
    }

    public static UT Negate(UT it)
    {
        return Make(Exp(it), -Val(it));
    }

    public static bool Negative(UT it)
    {
        return ((it >> ExpBits) & 1) == 1;
    }

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

    public static UT Subtract(UT left, UT right)
    {
        var le = Exp(left);
        var re = Exp(right);

        if (le == re)
        {
            return Make(le, Val(left) - Val(right));
        }

        if (le > re)
        {
            return Make(le, Val(left) - Val(right) * Scale(le) / Scale(Exp(right)));
        }

        return Make(re, Val(left) * Scale(re) / Scale(le) - Val(right));
    }

    public static T Val(UT it)
    {
        UT v = it >> HeaderBits;
        return Negative(it) ? -(long)v : (long)v;
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
        if (Negative(it)) { result.Append('-'); }
        var t = Math.Abs(Trunc(it));
        if (t > 0) { result.Append(t); }
        result.Append('.');
        result.Append(Math.Abs(Frac(it)));
        return result.ToString();
    }
}