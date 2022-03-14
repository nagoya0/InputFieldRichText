using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        [Test]
        public void StyledTextParserTest()
		{
            var parser = new StyledTextParser();
            var result1 = parser.Parse("あいうえお");
            Assert.That(result1.parsedText, Is.EqualTo("あいうえお"));
            var result2 = parser.Parse("<b>あいう</b>えお");
            Assert.That(result2.parsedText, Is.EqualTo("あいうえお"));
            var result3 = parser.Parse("<b>あいうえお</b>");
            Assert.That(result3.parsedText, Is.EqualTo("あいうえお"));
            var result4 = parser.Parse("あいう<b>えお</b>");
            Assert.That(result4.parsedText, Is.EqualTo("あいうえお"));
            var result5 = parser.Parse("あいう<b></b>えお");
            Assert.That(result5.parsedText, Is.EqualTo("あいうえお"));
            var result6 = parser.Parse(@"<b>あいう<b>えおか</b>きくけ</b>こ");
            Assert.That(result6.parsedText, Is.EqualTo("あいうえおかきくけこ"));
            var result7 = parser.Parse(@"<b>あいう<u>えお<color=yellow>かきく</u>けこ</color>さし</b>すせそ");
            Assert.That(result7.parsedText, Is.EqualTo("あいうえおかきくけこさしすせそ"));
        }

        // A Test behaves as an ordinary method
        /*
        [Test]
        public void SpannableStringBuilderTest()
        {
            // Use the Assert class to test conditions
            var builder = new SpannableStringBuilder();
            builder.ParseStyledText("あいうえおかきくけこさしすせそ");
            builder.Append("あいうえおかきくけこさしすせそ")
                .SetSpan(new BoldSpan(), 5, 10)
                .Insert(5, "ABC");
            Debug.Log(builder.GetStyledText());
        }
        */

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        /*
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        */
    }
}
