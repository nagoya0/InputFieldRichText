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

    void OnToggleValueChanged(Toggle toggle)
    {
        ISpan span = null;
        int start, end;

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
        var anchorPos = _inputField.selectionAnchorPosition;
        var focusPos = _inputField.selectionFocusPosition;
        if (anchorPos <= focusPos)
        {
            start = anchorPos;
            end = focusPos;
        }
        else
        {
            start = focusPos;
            end = anchorPos;
        }

        var result = _styledTextParser.Parse(inputText);

        var spannable = new SpannableString(result.parsedText);
        foreach (var spanInfo in result.spanInfos)
        {
            spannable.SetSpan(spanInfo.what, spanInfo.start, spanInfo.end);
        }
        spannable.SetSpan(span, start, end);

        _inputField.text = spannable.ToString();
    }
}
