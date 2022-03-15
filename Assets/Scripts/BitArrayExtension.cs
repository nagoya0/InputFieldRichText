using System.Collections;

/// <summary>
/// BitArray拡張メソッド
/// </summary>
public static class BitArrayExtension
{
    /// <summary>
    /// startからendまでの範囲のビットを指定した値に設定する(endはexclusive)
    /// </summary>
    static public void SetRange(this BitArray self, int start, int end, bool value)
    {
        for (var i = start; i < end; i++)
        {
            self.Set(i, value);
        }
    }

    /// <summary>
    /// いずれかのビットがONになっているかを返す
    /// </summary>
    static public bool Any(this BitArray self)
    {
        foreach (bool bit in self)
        {
            if (bit)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 全てのビットがONになっているかを返す
    /// </summary>
    public static bool All(this BitArray self)
    {
        foreach (bool bit in self)
        {
            if (!bit)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// すべてのビットがOFFになっているかを返す
    /// </summary>
    public static bool None(this BitArray self)
    {
        return !Any(self);
    }

    /// <summary>
    /// 指定ビットを反転する
    /// </summary>
    public static void Flip(this BitArray self, int index)
    {
        self.Set(index, !self.Get(index));
    }
}