using System.Collections.Generic;
using System.Text;

/// <summary>
/// 固定長文字列にテキスト修飾表現を付加することができるクラス
/// </summary>
public class SpannableString
{
    class SpanInfo
    {
        public ISpan span;
        public int start;
        public int end;
    }

    string _str;
    List<SpanInfo> _spanInfos = new List<SpanInfo>();

    public SpannableString(string source)
    {
        _str = source;
    }

    /// <summary>
    /// テキスト修飾表現を付加する
    /// </summary>
    public void SetSpan(ISpan what, int start, int end)
    {
        if (start > end)
        {
            throw new System.ArgumentException("start must be less than end!");
        }

        var spanInfo = new SpanInfo()
        {
            span = what,
            start = start,
            end = end,
        };
        _spanInfos.Add(spanInfo);

        _spanInfos.Sort(delegate (SpanInfo x, SpanInfo y)
        {
            return x.start.CompareTo(y.start);
        });
    }

    /// <summary>
    /// 追加済みのテキスト修飾表現を検索する
    /// </summary>
    public T[] SearchSpans<T>(int queryStart, int queryEnd) where T : ISpan
    {
        var ret = new List<T>();

        foreach (var spanInfo in _spanInfos)
        {
            if (spanInfo.start >= queryEnd
                || spanInfo.end <= queryStart)
            {
                continue;
            }

            if (spanInfo.span is T)
            {
                ret.Add((T)spanInfo.span);
            }
        }

        return ret.ToArray();
    }

    /// <summary>
    /// 追加済みのテキスト修飾表現の開始位置を返す
    /// </summary>
    public int GetSpanStart(ISpan what)
    {
        foreach (var spanInfo in _spanInfos)
        {
            if (what == spanInfo.span)
            {
                return spanInfo.start;
            }
        }

        return -1;
    }

    /// <summary>
    /// 追加済みのテキスト修飾表現の終了位置を返す
    /// </summary>
    public int GetSpanEnd(ISpan what)
    {
        foreach (var spanInfo in _spanInfos)
        {
            if (what == spanInfo.span)
            {
                return spanInfo.end;
            }
        }

        return -1;
    }

    public override string ToString()
    {
        var chars = _str.ToCharArray();
        var chunk = new StringBuilder();

        var index = 0;
        while (index < chars.Length)
        {
            foreach (var spanInfo in _spanInfos)
            {
                if (index == spanInfo.start)
                {
                    chunk.Append(spanInfo.span.WriteStartTag());
                }
            }

            chunk.Append(chars[index]);

            foreach (var spanInfo in _spanInfos)
            {
                if (index == spanInfo.end - 1)
                {
                    chunk.Append(spanInfo.span.WriteEndTag());
                }
            }

            index++;
        }

        return chunk.ToString();
    }
}
