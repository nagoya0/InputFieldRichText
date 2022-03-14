using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class TagObjectAttribute : Attribute
{
    public string tagName;
    public TagObjectAttribute(string tagName) { this.tagName = tagName; }
}

[AttributeUsage(AttributeTargets.Parameter)]
public class TagParameterAttribute : Attribute
{
    public string name;
    public TagParameterAttribute(string name = null) { this.name = name; }
}

public interface ISpan
{
    bool ValueEquals(object obj);
}

[TagObject("b")]
public class BoldSpan : ISpan
{
    public bool ValueEquals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

[TagObject("u")]
public class UnderlineSpan : ISpan
{
    public bool ValueEquals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

[TagObject("color")]
public class TextColorSpan : ISpan
{
    Color _color;

    public TextColorSpan([TagParameter]string c)
    {
        if (ColorUtility.TryParseHtmlString(c, out var color))
        {
            _color = color;
        }
        else
        {
            Debug.LogWarning("colorタグのパラメータが不正です");
            _color = Color.white;
        }
    }

    public bool ValueEquals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            TextColorSpan p = (TextColorSpan)obj;
            return (_color == p._color);
        }
    }
}