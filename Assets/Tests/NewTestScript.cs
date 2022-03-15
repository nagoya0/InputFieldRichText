using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void StyledTextParserTest()
        {
            var parser = new StyledTextParser();
            {
                var result = parser.Parse("あいうえお");
                Assert.That(result.parsedText, Is.EqualTo("あいうえお"));
            }
            {
                var result = parser.Parse("<b>あいう</b>えお");
                Assert.That(result.parsedText, Is.EqualTo("あいうえお"));
            }
            {
                var result = parser.Parse("<b>あいうえお</b>");
                Assert.That(result.parsedText, Is.EqualTo("あいうえお"));
            }
            {
                var result = parser.Parse("あいう<b>えお</b>");
                Assert.That(result.parsedText, Is.EqualTo("あいうえお"));
            }
            {
                var result = parser.Parse("あいう<b></b>えお");
                Assert.That(result.parsedText, Is.EqualTo("あいうえお"));
            }
            {
                var result = parser.Parse(@"<b>あいう<b>えおか</b>きくけ</b>こ");
                Assert.That(result.parsedText, Is.EqualTo("あいうえおかきくけこ"));
            }
            {
                var result = parser.Parse(@"<b>あいう<u>えお<color=yellow>かきく</u>けこ</color>さし</b>すせそ");
                Assert.That(result.parsedText, Is.EqualTo("あいうえおかきくけこさしすせそ"));
            }
        }

        [Test]
        public void SpannableStringBuilderTest()
        {
            {
                var spannable = new SpannableString("あいうえおかきくけこさしすせそ");
                spannable.SetSpan(new BoldSpan(), 2, 4);
                var str = spannable.ToString();
                Assert.That(str, Is.EqualTo("あい<b>うえ</b>おかきくけこさしすせそ"));
            }
        }

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
