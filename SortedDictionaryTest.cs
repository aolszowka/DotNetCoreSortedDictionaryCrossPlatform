namespace DotNetCoreSortedDictionaryCrossPlatform
{
    using NUnit.Framework;

    using System;
    using System.Collections.Generic;
    using System.Text;

    public class SortedDictionaryTest
    {
        [Test]
        public void DifferenceBetweenPlatforms()
        {
            SortedDictionary<string, int> sortedDictionary = new SortedDictionary<string, int>();

            sortedDictionary.Add("Project(\"{778DAE3C-4631-46EA-AA77-85C1314464D9}\") = \"Microsoft.CodeAnalysis.VisualBasic.CodeStyle.Fixes\", \"src\\CodeStyle\\VisualBasic\\CodeFixes\\Microsoft.CodeAnalysis.VisualBasic.CodeStyle.Fixes.vbproj\", \"{0141285D-8F6C-42C7-BAF3-3C0CCD61C716}\"", 0);
            sortedDictionary.Add("Project(\"{778DAE3C-4631-46EA-AA77-85C1314464D9}\") = \"Microsoft.CodeAnalysis.VisualBasic\", \"src\\Compilers\\VisualBasic\\Portable\\Microsoft.CodeAnalysis.VisualBasic.vbproj\", \"{2523D0E6-DF32-4A3E-8AE0-A19BFFAE2EF6}\"", 0);

            StringBuilder sb = new StringBuilder();

            foreach (var kvp in sortedDictionary)
            {
                sb.AppendLine(kvp.Key);
            }

            string actual = sb.ToString();

            // This is how it will appear on Windows, but in Ubuntu this fails? (maybe?)
            string expected =
                "Project(\"{778DAE3C-4631-46EA-AA77-85C1314464D9}\") = \"Microsoft.CodeAnalysis.VisualBasic\", \"src\\Compilers\\VisualBasic\\Portable\\Microsoft.CodeAnalysis.VisualBasic.vbproj\", \"{2523D0E6-DF32-4A3E-8AE0-A19BFFAE2EF6}\"" + Environment.NewLine +
                "Project(\"{778DAE3C-4631-46EA-AA77-85C1314464D9}\") = \"Microsoft.CodeAnalysis.VisualBasic.CodeStyle.Fixes\", \"src\\CodeStyle\\VisualBasic\\CodeFixes\\Microsoft.CodeAnalysis.VisualBasic.CodeStyle.Fixes.vbproj\", \"{0141285D-8F6C-42C7-BAF3-3C0CCD61C716}\"" + Environment.NewLine;

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}