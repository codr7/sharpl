using System.Runtime.CompilerServices;
using System.Text;

namespace Sharpl;

using T = long;
using UT = ulong;

public static class Fix
{
    public static readonly byte ExpBits = 4;
    public static readonly byte HeaderBits = (byte)(ExpBits + 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UT Add(UT left, UT right)
    {
        var le = Exp(left);
        var re = Exp(right);
        
        return (le == re) 
            ? Make(le, Val(left) + Val(right)) 
            : Make(le, Val(left) + Val(right) * Scale(le) / Scale(re));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(UT left, UT right) =>
        Val(left) * Scale(Exp(right)) == Val(right) * Scale(Exp(left));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UT Divide(UT left, UT right) =>
        Make(Exp(left), Val(left) / (Val(right) / Scale(Exp(right))));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Exp(UT it) => (byte)(it & (UT)((1 << ExpBits) - 1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UT Make(byte exp, T val) =>
        (UT)(exp & ((1 << ExpBits) - 1)) +
        (UT)(((val < 0) ? 1 : 0) << ExpBits) +
        (UT)(((val < 0) ? -val : val) << HeaderBits);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UT Multiply(UT left, UT right) =>
        Make(Exp(left), Val(left) * Val(right) / Scale(Exp(right)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UT Negate(UT it) => Make(Exp(it), -Val(it));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Negative(UT it) => ((it >> ExpBits) & 1) == 1;

    private static readonly T[] scaleTable = [
        1, 
        10, 
        100, 
        1000, 
        10000, 
        100000, 
        1000000, 
        10000000, 
        100000000, 
        1000000000, 
        10000000000, 
        100000000000, 
        1000000000000, 
        10000000000000, 
        100000000000000];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Scale(byte exp) => scaleTable[exp];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UT Subtract(UT left, UT right)
    {
        var le = Exp(left);
        var re = Exp(right);
        
        return (le == re) 
            ? Make(le, Val(left) - Val(right)) 
            : Make(re, Val(left) * Scale(re) / Scale(le) - Val(right));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Val(UT it)
    {
        UT v = it >> HeaderBits;
        return Negative(it) ? -(long)v : (long)v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Trunc(UT it) => Val(it) / Scale(Exp(it));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Frac(UT it) => Val(it) % Scale(Exp(it));

    public static string ToString(UT it, bool forceZero = false)
    {
        var result = new StringBuilder();
        if (Negative(it)) { result.Append('-'); }
        var t = Math.Abs(Trunc(it));
        if (t > 0 || forceZero) { result.Append(t); }
        result.Append('.');
        result.Append(Math.Abs(Frac(it)));
        return result.ToString();
    }
}