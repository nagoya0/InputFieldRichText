using System.Collections.Generic;
using System.Text;

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

    public int Length { get { return _str.Length; } }

    public SpannableString(string source)
    {
        _str = source;
    }

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
