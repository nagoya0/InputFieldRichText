using System;
using UnityEngine;

/// <summary>
/// タグオブジェクトを表すクラスに付ける属性
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TagObjectAttribute : Attribute
{
    /// <summary>
    /// 修飾タグ名
    /// </summary>
    public string tagName;
    public TagObjectAttribute(string tagName) { this.tagName = tagName; }
}

/// <summary>
/// Unityのリッチテキストのタグパラメータに対応する、タグオブジェクトクラスのコンストラクタ引数を表すパラメータ
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class TagParameterAttribute : Attribute
{
    /// <summary>
    /// パラメータ名（nullの場合は名前なしのパラメータ ex."<color=#FF0000>"の#FF0000の部分）
    /// </summary>
    public string name;
    public TagParameterAttribute(string name = null) { this.name = name; }
}

/// <summary>
/// テキスト修飾内容を表す内部表現クラスが実装すべきインタフェース
/// </summary>
public interface ISpan
{
    /// <summary>
    /// 開始タグ文字列を返す
    /// </summary>
    string WriteStartTag();
    /// <summary>
    /// 終了タグ文字列を返す
    /// </summary>
    string WriteEndTag();
    /// <summary>
    /// 表現が同値である場合はtrueを返す
    /// </summary>
    bool ValueEquals(object obj);
}


/////////////////////////////////
// テキスト修飾表現クラス
// 
// 現時点はb, u, colorタグしか定義していないため、パーサを通してもこれら以外のタグは無視される
// 将来他のタグの解析が必要になった場合はここに定義を追加すれば自動的に対応される

/// <summary>
/// bタグに対応する太字表現オブジェクト
/// </summary>
[TagObject("b")]
public class BoldSpan : ISpan
{
    public string WriteStartTag() { return $"<b>"; }
    public string WriteEndTag() { return $"</b>"; }
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

/// <summary>
/// uタグに対応する下線表現オブジェクト
/// </summary>
[TagObject("u")]
public class UnderlineSpan : ISpan
{
    public string WriteStartTag() { return $"<u>"; }
    public string WriteEndTag() { return $"</u>"; }
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

/// <summary>
/// colorタグに対応する文字色表現オブジェクト
/// </summary>
[TagObject("color")]
public class TextColorSpan : ISpan
{
    public Color color { get; private set; }

    public TextColorSpan([TagParameter]string c)
    {
        if (ColorUtility.TryParseHtmlString(c, out var color))
        {
            this.color = color;
        }
        else
        {
            Debug.LogWarning("colorタグのパラメータが不正です");
            this.color = Color.white;
        }
    }

    public string WriteStartTag() { return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>"; }
    public string WriteEndTag() { return $"</color>"; }
    public bool ValueEquals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            TextColorSpan p = (TextColorSpan)obj;
            return color == p.color;
        }
    }
}