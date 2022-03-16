using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Unityのリッチテキスト書式を解析するパーサ
/// 
/// https://docs.unity3d.com/ja/2019.4/Manual/StyledText.html
/// </summary>
public class StyledTextParser
{
    /// <summary>
    /// 解析結果
    /// </summary>
    public class ParserResult
    {
        /// <summary>
        /// タグを取り除いた文字列
        /// </summary>
        public string parsedText;
        /// <summary>
        /// 適用されている修飾表現と適用範囲
        /// </summary>
        public SpanInfo[] spanInfos;
    }

    /// <summary>
    /// 修飾表現情報
    /// </summary>
    public class SpanInfo
    {
        public ISpan what;
        public int start = -1;
        public int end = -1;
    }

    class TagObjectInfo
    {
        public Type type;
        public string[] parameters;
    }
    Dictionary<string, TagObjectInfo> _tagDictionary;

    // 開始タグと終了タグにマッチする正規表現
    // FIXME: 現状名前付きパラメータに非対応 (ex."<size=50>"はOK、"<quad material=1 size=20 x=0.1 y=0.1 width=0.5 height=0.5>"はNG)
    const string TagPattern = @"(<([a-zA-Z-]+)(=([^>]+))?>)|(</([a-zA-Z-]+)>)";

    public StyledTextParser()
    {
        InitializeTagDictionary();
    }

    private void InitializeTagDictionary()
    {
        // ISpanを実装しているクラスを解析してタグオブジェクト辞書を作成する
        var iSpanSubclassTypes = typeof(ISpan).Assembly.GetTypes().Where(type => type != typeof(ISpan) && typeof(ISpan).IsAssignableFrom(type));

        _tagDictionary = new Dictionary<string, TagObjectInfo>();
        foreach (var type in iSpanSubclassTypes)
        {
            var tagObjAttribute = type.GetCustomAttribute<TagObjectAttribute>();
            if (tagObjAttribute == null)
            {
                Debug.LogWarning("ISpanを実装したクラスにTagObject属性が付いていません: " + type.FullName);
                continue;
            }
            var publicConstructors = type.GetConstructors();
            if (publicConstructors.Length > 1)
            {
                Debug.LogWarning("タグオブジェクトは1つのコンストラクタしか持てません: " + type.FullName);
                continue;
            }
            if (publicConstructors.Length == 0)
            {
                Debug.LogWarning("タグオブジェクトにpublicなコンストラクタがありません: " + type.FullName);
                continue;
            }

            var parameterList = new List<string>();
            foreach (var parameter in publicConstructors[0].GetParameters())
            {
                if (parameter.ParameterType != typeof(string))
                {
                    Debug.LogWarning("タグオブジェクトのコンストラクタ引数の型はstringのみです: " + type.FullName + " " + parameter.Name);
                    continue;
                }
                var tagParaAttribute = parameter.GetCustomAttribute<TagParameterAttribute>();
                if (tagParaAttribute == null)
                {
                    Debug.LogWarning("タグオブジェクトのコンストラクタ引数にTagParameter属性が付いていません: " + type.FullName + " " + parameter.Name);
                    continue;
                }
                parameterList.Add(tagParaAttribute.name);
            }

            var info = new TagObjectInfo()
            {
                type = type,
                parameters = parameterList.ToArray(),
            };
            _tagDictionary[tagObjAttribute.tagName.ToLower()] = info;
        }
    }

    /// <summary>
    /// Unityのリッチテキスト書式の文字列を解析して解析結果を返す
    /// </summary>
    public ParserResult Parse(string styledText)
    {
        var stringBuilder = new StringBuilder();
        var tagRegex = new Regex(TagPattern, RegexOptions.Compiled);
        var spanInfoList = new List<SpanInfo>();

        // 文字列内のタグ位置を検索
        var index = 0;
        while (index < styledText.Length)
        {
            var match = tagRegex.Match(styledText, index);
            if (match.Success)
            {
                stringBuilder.Append(styledText.Substring(index, match.Index - index));

                if (match.Groups[1].Success)
                {
                    // 開始タグを検出した
                    var tagName = match.Groups[2].Value;
                    var parameterValue = match.Groups[4].Value;
                    ParseStartTag(stringBuilder.Length, tagName, parameterValue, ref spanInfoList);
                }
                else if (match.Groups[5].Success)
                {
                    // 終了タグを検出した
                    var tagName = match.Groups[6].Value;
                    ParseEndTag(stringBuilder.Length, tagName, ref spanInfoList);
                }

                index = match.Index + match.Length;
            }
            else
            {
                // 文字列にはもうタグがない
                stringBuilder.Append(styledText.Substring(index));

                index = styledText.Length;
            }
        }

        return new ParserResult()
        {
            parsedText = stringBuilder.ToString(),
            spanInfos = spanInfoList.ToArray(),
        };
    }

    private void ParseStartTag(int position, string tagName, string parameterValue, ref List<SpanInfo> spanInfoList)
    {
        if (_tagDictionary.TryGetValue(tagName.ToLower(), out var tag))
        {
            // コンストラクタの引数リスト作成
            List<object> argList = new List<object>();
            foreach (var p in tag.parameters)
            {
                if (p == null)
                {
                    argList.Add(parameterValue);
                }
            }

            // spanをインスタンス化してspanInfoListに追加
            var instance = (ISpan)Activator.CreateInstance(tag.type, argList.ToArray());
            spanInfoList.Add(new SpanInfo()
            {
                what = instance,
                start = position,
            });
        }
        else
        {
            Debug.LogWarning("タグオブジェクトが定義されていないタグは無視されます: " + tagName);
        }
    }

    private void ParseEndTag(int position, string tagName, ref List<SpanInfo> spanInfoList)
    {
        if (_tagDictionary.TryGetValue(tagName.ToLower(), out var tag))
        {
            // spanInfoListに追加済みのタグオブジェクトを後ろから検索して終了位置をセットする
            for (var i = spanInfoList.Count - 1; i >= 0; i--)
            {
                var spanInfo = spanInfoList[i];
                if (spanInfo.what.GetType() == tag.type && spanInfo.end == -1)
                {
                    spanInfo.end = position;
                    break;
                }
        }
        }
    }
}
