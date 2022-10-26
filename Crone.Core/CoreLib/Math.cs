namespace Crone;

public static partial class CoreLib
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<decimal> DivideEvenlyAsc(this decimal total, int count, int round = 2)
    {
        return DivideEvenly(total, count, round).OrderBy(d => d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<decimal> DivideEvenlyDesc(this decimal total, int count, int round = 2)
    {
        return DivideEvenly(total, count, round).OrderByDescending(d => d);
    }

    public static IEnumerable<decimal> DivideEvenly(this decimal total, int count, int round = 2)
    {
        while (count > 0)
        {
            var value = Math.Round(total / count, round);
            total -= value;
            count -= 1;
            yield return value;
        }
    }
}
