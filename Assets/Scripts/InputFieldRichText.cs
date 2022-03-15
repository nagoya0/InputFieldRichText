using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldRichText : MonoBehaviour
{
    [SerializeField] Toggle _toggleBold = null;
    [SerializeField] Toggle _toggleUnderline = null;
    [SerializeField] Toggle _toggleRed = null;
    [SerializeField] TMP_InputField _inputField = null;

    StyledTextParser _styledTextParser;
    int _preSelectionAnchorPosition;
    int _preSelectionFocusPosition;

    RangeInt GetInputFieldSelectionRange()
    {
        var anchorPos = _inputField.selectionAnchorPosition;
        var focusPos = _inputField.selectionFocusPosition;
        return anchorPos <= focusPos ?
            new RangeInt(anchorPos, focusPos - anchorPos) :
            new RangeInt(focusPos, anchorPos - focusPos);
    }

    void Awake()
    {
        _styledTextParser = new StyledTextParser();

        _toggleBold.onValueChanged.AddListener(delegate {
            _inputField.ActivateInputField();
            OnToggleValueChanged(_toggleBold);
        });
        _toggleUnderline.onValueChanged.AddListener(delegate {
            _inputField.ActivateInputField();
            OnToggleValueChanged(_toggleUnderline);
        });
        _toggleRed.onValueChanged.AddListener(delegate {
            _inputField.ActivateInputField();
            OnToggleValueChanged(_toggleRed);
        });
        _inputField.onFocusSelectAll = false;
    }

    void Update()
    {
        var anchorPos = _inputField.selectionAnchorPosition;
        var focusPos = _inputField.selectionFocusPosition;

        if (anchorPos != _preSelectionAnchorPosition
            || focusPos != _preSelectionFocusPosition)
        {
            OnSelectionRangeChanged();
        }

        _preSelectionAnchorPosition = anchorPos;
        _preSelectionFocusPosition = focusPos;
    }

    void OnSelectionRangeChanged()
    {
        var inputText = _inputField.text;
        var currentRange = GetInputFieldSelectionRange();

        var result = _styledTextParser.Parse(inputText);

        // 選択範囲の各文字に各装飾が適用されているかどうかをチェックしてトグルに反映する関数
        Action<RangeInt> checkRange = (range) => {
            var boldBits = new BitArray(range.length);
            var underlineBits = new BitArray(range.length);
            var redBits = new BitArray(range.length);
            foreach (var spanInfo in result.spanInfos)
            {
                if (spanInfo.start >= range.end
                    || spanInfo.end <= range.start)
                {
                    continue;
                }

                var s = Mathf.Max(spanInfo.start - range.start, 0);
                var e = Mathf.Min(spanInfo.end - range.start, range.length);
                if (spanInfo.what is BoldSpan)
                {
                    boldBits.SetRange(s, e, true);
                }
                else if (spanInfo.what is UnderlineSpan)
                {
                    underlineBits.SetRange(s, e, true);
                }
                else if (spanInfo.what is TextColorSpan)
                {
                    redBits.SetRange(s, e, true);
                }
            }

            _toggleBold.SetIsOnWithoutNotify(boldBits.All());
            _toggleUnderline.SetIsOnWithoutNotify(underlineBits.All());
            _toggleRed.SetIsOnWithoutNotify(redBits.All());
        };

        if (currentRange.length == 0)
        {
            checkRange(new RangeInt(currentRange.start - 1, 1)); // currentRange.start - 1が負になっても問題ない
        }
        else
        {
            checkRange(currentRange);
        }
    }

    void OnToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            OnToggleValueChanged_ON(toggle);
        }
        else
        {
            OnToggleValueChanged_OFF(toggle);
        }
    }

    void OnToggleValueChanged_ON(Toggle toggle)
    {
        ISpan span = null;
        if (toggle == _toggleBold)
        {
            span = new BoldSpan();
        }
        else if (toggle == _toggleUnderline)
        {
            span = new UnderlineSpan();
        }
        else if (toggle == _toggleRed)
        {
            span = new TextColorSpan("red");
        }

        var inputText = _inputField.text;
        var range = GetInputFieldSelectionRange();

        var result = _styledTextParser.Parse(inputText);

        // SpannableStringで同じ構成になるように再構成
        var spannable = new SpannableString(result.parsedText);
        foreach (var spanInfo in result.spanInfos)
        {
            spannable.SetSpan(spanInfo.what, spanInfo.start, spanInfo.end);
        }
        // 新しいSpanを追加
        spannable.SetSpan(span, range.start, range.end);

        _inputField.text = spannable.ToString();
        _inputField.selectionAnchorPosition = range.start;
        _inputField.selectionFocusPosition = range.end;
    }

    void OnToggleValueChanged_OFF(Toggle toggle)
    {
        Type spanType = null;
        if (toggle == _toggleBold)
        {
            spanType = typeof(BoldSpan);
        }
        else if (toggle == _toggleUnderline)
        {
            spanType = typeof(UnderlineSpan);
        }
        else if (toggle == _toggleRed)
        {
            spanType = typeof(TextColorSpan);
        }

        var inputText = _inputField.text;
        var range = GetInputFieldSelectionRange();

        var result = _styledTextParser.Parse(inputText);

        var spannable = new SpannableString(result.parsedText);
        foreach (var spanInfo in result.spanInfos)
        {
            if (spanInfo.start >= range.end
                || spanInfo.end <= range.start
                || !spanType.IsInstanceOfType(spanInfo.what))
            {
                spannable.SetSpan(spanInfo.what, spanInfo.start, spanInfo.end);
                continue;
            }

            if (spanInfo.start < range.start)
            {
                // TODO; シャローコピーすべき？
                spannable.SetSpan(spanInfo.what, spanInfo.start, range.start);
            }
            if (spanInfo.end > range.end)
            {
                // TODO; シャローコピーすべき？
                spannable.SetSpan(spanInfo.what, range.end, spanInfo.end);
            }
        }

        _inputField.text = spannable.ToString();
        _inputField.selectionAnchorPosition = range.start;
        _inputField.selectionFocusPosition = range.end;
    }
}
