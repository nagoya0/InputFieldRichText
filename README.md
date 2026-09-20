# InputFieldRichText

A rich text editing control for Unity, built on TextMeshPro's `TMP_InputField`.

Select a range of text, hit a toggle, and the text becomes bold, underlined, or colored — the
way a word processor behaves. Under the hood the field's raw rich text markup is parsed into a
structured representation, edited there, and serialized back, so overlapping and nested tags
stay correct no matter how the user selects.

> **Status:** archived. Written in March 2022 against Unity 2019.4 and no longer maintained.

## Demo

<!-- Add a screen capture here -->

## Why not just insert tags?

Inserting `<b>` and `</b>` around a selection is easy until the user selects across an existing
tag. Then you need to split it, merge adjacent runs, and keep the markup well-formed — string
surgery that gets worse with every tag you support.

This project borrows Android's [`SpannableString`](https://developer.android.com/reference/android/text/SpannableString)
model instead. Text and formatting are kept apart: a plain string plus a list of *spans*, each
knowing only its type and its range. Every edit is a round trip.

```
"あい<b>うえ</b>お"          raw markup from TMP_InputField
        │
        │  StyledTextParser.Parse()
        ▼
  text:  "あいうえお"          plain text, tags stripped
  spans: [ BoldSpan 2..4 ]     structured formatting
        │
        │  SpannableString.SetSpan()  ← the actual edit happens here
        ▼
  spans: [ BoldSpan 2..4, UnderlineSpan 3..5 ]
        │
        │  SpannableString.ToString()
        ▼
"あい<b>う<u>え</b>お</u>"    back into the input field
```

Because the edit operates on ranges rather than on text, overlapping spans are a non-issue —
the serializer emits whatever tag order the ranges imply, including the improperly nested form
above, which Unity's rich text renderer accepts.

## Features

- **Bold, underline, and text color**, applied to arbitrary selections.
- **Toggle state reflects the caret.** Move the caret or change the selection and the toolbar
  updates to show which styles are active across every character in range.
- **Caret-aware typing.** The caret always binds to the character on its left, so typing after a
  bold word continues in bold, and typing after a plain word does not.
- **Tag-agnostic parser.** New tags are added declaratively (see below), not by editing the parser.
- **Unit tested.** `Assets/Tests/NewTestScript.cs` covers parsing and serialization, including
  nested and overlapping spans.

## Adding a tag

The parser discovers tag types by reflection at construction time. To support Unity's `<size>`
tag, the whole change is a new class:

```csharp
[TagObject("size")]
public class SizeSpan : ISpan
{
    public string size { get; private set; }

    public SizeSpan([TagParameter] string s) { size = s; }

    public string WriteStartTag() => $"<size={size}>";
    public string WriteEndTag()   => "</size>";
    public bool ValueEquals(object obj) => obj is SizeSpan other && other.size == size;
}
```

`[TagObject]` names the markup tag; `[TagParameter]` marks the constructor argument that receives
the tag's value (`50` in `<size=50>`). `StyledTextParser` scans the assembly for `ISpan`
implementations, reads these attributes, and instantiates the right type when it meets the tag.
Tags with no matching type are ignored rather than mangled.

## Layout

| File | Role |
| --- | --- |
| `Assets/Scripts/InputFieldRichText.cs` | The control. Wires toggles to the input field, tracks the selection, drives the round trip. |
| `Assets/Scripts/StyledTextParser.cs` | Parses Unity rich text markup into plain text plus spans. Builds its tag table by reflection. |
| `Assets/Scripts/SpannableString.cs` | Immutable text with attached spans; serializes back to markup. |
| `Assets/Scripts/ISpan.cs` | The span interface, the two attributes, and the bold/underline/color span types. |
| `Assets/Scripts/BitArrayExtension.cs` | `SetRange` / `All` / `Any` / `None` helpers, used to decide toggle state over a selection. |

## Requirements

Unity 2019.4 (developed on 2019.4.28f1) with TextMeshPro 2.1.6. Open the project and run
`Assets/Scenes/Scene.unity`.

## Known limitations

- The parser handles unnamed tag parameters only. `<size=50>` works; `<quad material=1 size=20 ...>`
  does not.
- The color toggle is fixed to red rather than offering a picker.
- `<` and `>` are rejected as input, since the field's text doubles as the markup.
- Selection changes are detected by polling in `Update()`.
- Spans are shared rather than copied when a range is split, which is fine for the immutable span
  types here but would need revisiting for mutable ones.

## Third-party assets

`Assets/TextMesh Pro/` holds the TextMeshPro essential resources distributed by Unity
Technologies, including the Liberation Sans font (SIL Open Font License) and EmojiOne sample
sprites. They are subject to their own licenses.
