namespace DotNetCoreSortedDictionaryCrossPlatform
{
    using NUnit.Framework;

    using System;
    using System.Collections.Generic;
    using System.Text;

    public class SortedDictionaryTest
    {
        [Test]
        public void Test1()
        {
            SortedDictionary<string, int> sortedDictionary = new SortedDictionary<string, int>();

            sortedDictionary.Add("Microsoft.CodeAnalysis.VisualBasic.CodeStyle", 0);
            sortedDictionary.Add("Microsoft.CodeAnalysis.VisualBasic", 0);

            StringBuilder sb = new StringBuilder();

            foreach (var kvp in sortedDictionary)
            {
                sb.AppendLine(kvp.Key);
            }

            string actual = sb.ToString();

            // This is how it will appear on Windows, but in Ubuntu this fails? (maybe?)
            string expected = "Microsoft.CodeAnalysis.VisualBasic" + Environment.NewLine + "Microsoft.CodeAnalysis.VisualBasic.CodeStyle" + Environment.NewLine;

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}