# DotNetCoreSortedDictionaryCrossPlatform
This toy repository shows how the default `StringComparer` for `SortedDictionary<T1,T2>` behaves differently across the various .NET Core Platforms.

See https://github.com/dotnet/runtime/issues/43802

The contents are reproduced below:

## dotnet/runtime/issues/43802
### Description
`SortedDictionary` appears to behave differently across .NET Core Platforms.

I have created the following GitHub Repository that demonstrates this issue: https://github.com/aolszowka/DotNetCoreSortedDictionaryCrossPlatform

The crux of the issue can be highlighted in this NUnit Test:

```csharp
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
```

This unit test will pass on Windows based platforms but fail in Ubuntu and MacOS (at least the build agents as provided by GitHub Actions).

Furthermore this issue appears to be length dependent, as the following will pass on all platforms:

```csharp
[Test]
public void DifferenceBetweenPlatforms_LengthDependent_Theory()
{
    SortedDictionary<string, int> sortedDictionary = new SortedDictionary<string, int>();

    sortedDictionary.Add("Microsoft.CodeAnalysis.VisualBasic.CodeStyle.Fixes", 0);
    sortedDictionary.Add("Microsoft.CodeAnalysis.VisualBasic", 0);

    StringBuilder sb = new StringBuilder();

    foreach (var kvp in sortedDictionary)
    {
        sb.AppendLine(kvp.Key);
    }

    string actual = sb.ToString();

    // This apparently works in both Windows and Ubuntu?
    string expected =
        "Microsoft.CodeAnalysis.VisualBasic" + Environment.NewLine +
        "Microsoft.CodeAnalysis.VisualBasic.CodeStyle.Fixes" + Environment.NewLine;

    Assert.That(actual, Is.EqualTo(expected));
}
```

I have not bothered to narrow this down due to the limits on GitHub Actions (not looking to burn all my CI Minutes right now).

### Configuration
This is using windows-latest, ubuntu-latest, macos-latest as defined by GitHub Actions here: https://github.com/actions/virtual-environments.

You should be able to clone this repository and instantly have GitHub spin up the appropriate CI environments for you allowing you to test across all platforms serviced by GitHub Actions.

### Other information
In searching existing bug reports I may be affected by one of the following, but I am not familiar enough with the runtime to know if this is the case:

* https://github.com/dotnet/runtime/issues/20599
* https://github.com/dotnet/runtime/issues/29112

Neither of these cases mentions the length of the string being in play. In the issues however they all appears to be using various forms of localization/localized strings.

Here are the results from my area in GitHub Actions:

**Windows**
```
Run dotnet test --configuration Release
Test run for D:\a\DotNetCoreSortedDictionaryCrossPlatform\DotNetCoreSortedDictionaryCrossPlatform\bin\Release\netcoreapp3.1\DotNetCoreSortedDictionaryCrossPlatform.dll(.NETCoreApp,Version=v3.1)
Microsoft (R) Test Execution Command Line Tool Version 16.7.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...

A total of 1 test files matched the specified pattern.

Test Run Successful.
Total tests: 2
     Passed: 2
 Total time: 1.1261 Seconds
```

**Ubuntu**
```
Run dotnet test --configuration Release
Test run for /home/runner/work/DotNetCoreSortedDictionaryCrossPlatform/DotNetCoreSortedDictionaryCrossPlatform/bin/Release/netcoreapp3.1/DotNetCoreSortedDictionaryCrossPlatform.dll(.NETCoreApp,Version=v3.1)
Microsoft (R) Test Execution Command Line Tool Version 16.7.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...

A total of 1 test files matched the specified pattern.
  X DifferenceBetweenPlatforms [192ms]
  Error Message:
     String lengths are both 455. Strings differ at index 87.
  Expected: "...osoft.CodeAnalysis.VisualBasic", "src\\Compilers\\VisualBasi..."
  But was:  "...osoft.CodeAnalysis.VisualBasic.CodeStyle.Fixes", "src\\Code..."
  --------------------------------------------^

  Stack Trace:
     at DotNetCoreSortedDictionaryCrossPlatform.SortedDictionaryTest.DifferenceBetweenPlatforms() in /home/runner/work/DotNetCoreSortedDictionaryCrossPlatform/DotNetCoreSortedDictionaryCrossPlatform/SortedDictionaryTest.cs:line 34


Test Run Failed.
Total tests: 2
     Passed: 1
     Failed: 1
 Total time: 1.2799 Seconds
/usr/share/dotnet/sdk/3.1.403/Microsoft.TestPlatform.targets(32,5): error MSB4181: The "Microsoft.TestPlatform.Build.Tasks.VSTestTask" task returned false but did not log an error. [/home/runner/work/DotNetCoreSortedDictionaryCrossPlatform/DotNetCoreSortedDictionaryCrossPlatform/DotNetCoreSortedDictionaryCrossPlatform.csproj]
Error: Process completed with exit code 1.
```

**MacOS**
```
Run dotnet test --configuration Release
Test run for /Users/runner/work/DotNetCoreSortedDictionaryCrossPlatform/DotNetCoreSortedDictionaryCrossPlatform/bin/Release/netcoreapp3.1/DotNetCoreSortedDictionaryCrossPlatform.dll(.NETCoreApp,Version=v3.1)
Microsoft (R) Test Execution Command Line Tool Version 16.7.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...

A total of 1 test files matched the specified pattern.
  X DifferenceBetweenPlatforms [405ms]
  Error Message:
     String lengths are both 455. Strings differ at index 87.
  Expected: "...osoft.CodeAnalysis.VisualBasic", "src\\Compilers\\VisualBasi..."
  But was:  "...osoft.CodeAnalysis.VisualBasic.CodeStyle.Fixes", "src\\Code..."
  --------------------------------------------^

  Stack Trace:
     at DotNetCoreSortedDictionaryCrossPlatform.SortedDictionaryTest.DifferenceBetweenPlatforms() in /Users/runner/work/DotNetCoreSortedDictionaryCrossPlatform/DotNetCoreSortedDictionaryCrossPlatform/SortedDictionaryTest.cs:line 34


Test Run Failed.
Total tests: 2
     Passed: 1
     Failed: 1
 Total time: 1.9944 Seconds
/Users/runner/.dotnet/sdk/3.1.403/Microsoft.TestPlatform.targets(32,5): error MSB4181: The "Microsoft.TestPlatform.Build.Tasks.VSTestTask" task returned false but did not log an error. [/Users/runner/work/DotNetCoreSortedDictionaryCrossPlatform/DotNetCoreSortedDictionaryCrossPlatform/DotNetCoreSortedDictionaryCrossPlatform.csproj]
Error: Process completed with exit code 1.
```

Thank you

## Reply from dagood
When you create your `SortedDictionary<string, int>` here, you aren't passing it a comparer, [so it uses the default string comparer](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2.-ctor?view=netcore-3.1#System_Collections_Generic_SortedDictionary_2__ctor):

```cs
SortedDictionary<string, intsortedDictionary = new SortedDictionary<string, int>();
```

The default string comparer can have different results based on the running environment (such as platform or culture). Here's an earlier instance of this expected behavior, specifically about Windows vs. macOS/Linux and the ICU library: #20109.

It's generally best practice to avoid using string comparison defaults so you don't run into this kind of issue: https://docs.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings#specifying-string-comparisons-explicitly

It looks like you're sorting `.sln` file lines--I think [`StringComparer.Ordinal`](https://docs.microsoft.com/en-us/dotnet/api/system.stringcomparer.ordinal?view=netcore-3.1) would work fine for this. Can you try that?