using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SpannableStringBuilder
{
    class SpanInfo
	{
        public ISpan span;
        public int start;
        public int end;
    }

    StringBuilder _stringBuilder;

    List<SpanInfo> _spans = new List<SpanInfo>();

    public int Length { get { return _stringBuilder.Length; } }

    public SpannableStringBuilder()
    {
        _stringBuilder = new StringBuilder();
    }

    public SpannableStringBuilder(string value)
    {
        _stringBuilder = new StringBuilder(value);
    }

    public SpannableStringBuilder Append(string value)
    {
        _stringBuilder.Append(value);
        return this;
    }

    public SpannableStringBuilder Clear()
    {
        _stringBuilder.Clear();
        return this;
    }

    public SpannableStringBuilder Insert(int index, string value)
    {
        _stringBuilder.Insert(index, value);
        return this;
    }

    public SpannableStringBuilder Remove(int startIndex, int length)
    {
        _stringBuilder.Remove(startIndex, length);
        return this;
    }

    public SpannableStringBuilder Replace(string oldValue, string newValue)
    {
        _stringBuilder.Replace(oldValue, newValue);
        return this;
    }

    public ISpan[] GetSpans(int start, int end)
    {
        return null;
    }

    public SpannableStringBuilder SetSpan(ISpan what, int start, int end)
    {
        var spanInfo = new SpanInfo()
        {
            span = what,
            start = start,
            end = end,
        };
        // 他のspanとマージできればする

        _spans.Add(spanInfo);
        return this;
    }

    public string GetStyledText()
    {
        return _stringBuilder.ToString();
    }

    public void ParseStyledText(string text)
	{

	}
}
