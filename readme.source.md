CaptureSnippet
==============


![Icon](https://raw.github.com/SimonCropp/CaptureSnippet/master/Icons/package_icon.png)

Extract code snippets from any language to be used when building documentation

**This project is supported by the community via [Patreon sponsorship](https://www.patreon.com/join/simoncropp). If you are using this project to deliver business value or build commercial software it is expected that you will provide support [via Patreon](https://www.patreon.com/join/simoncropp).**

Loosely based on some code from  https://github.com/shiftkey/scribble


## Variants

This project produces two NuGet packages with different levels of features, and associated complexity. [CaptureSnippets.Simple](#CaptureSnippets.Simple) which exposes the core features via a simplified API, and [CaptureSnippets](#CaptureSnippets) which has more features but also has a more complex API.


## Shared behavior

The below behaviors are shared between both [CaptureSnippets.Simple](#CaptureSnippets.Simple) and[CaptureSnippets](#CaptureSnippets).


### Using Snippets

The keyed snippets can then be used in any documentation `.md` file by adding the text

&#96;&#96;&#96;
snippet&colon; KEY
&#96;&#96;&#96;

Then snippets with the key (all versions) will be rendered in a tabbed manner. If there is only a single version then it will be rendered as a simple code block with no tabs.

For example

<pre>
<code>Some blurb about the below snippet
&lt;!-- snippet MySnippetName --></code>
</pre>

The resulting markdown will be will be:

    Some blurb about the below snippet
    ```
    My Snippet Code
    ``` 


### Code indentation

The code snippets will do smart trimming of snippet indentation.

For example given this snippet:

<pre>
&#8226;&#8226;#region MySnippetName
&#8226;&#8226;Line one of the snippet
&#8226;&#8226;&#8226;&#8226;Line two of the snippet
&#8226;&#8226;#endregion
</pre>

The leading two spaces (&#8226;&#8226;) will be trimmed and the result will be:

```
Line one of the snippet
••Line two of the snippet
```

The same behavior will apply to leading tabs.


#### Do not mix tabs and spaces

If tabs and spaces are mixed there is no way for the snippets to work out what to trim.

So given this snippet:

<pre>
&#8226;&#8226;#region MySnippetNamea
&#8226;&#8226;Line one of the snippet
&#10137;&#10137;Line one of the snippet
&#8226;&#8226;#endregion
</pre>

Where &#10137; is a tab.

The resulting markdown will be will be

<pre>
Line one of the snippet
&#10137;&#10137;Line one of the snippet
</pre>

Note none of the tabs have been trimmed.


### Defining Snippets


#### Using comments

Any code wrapped in a convention based comment will be picked up. The comment needs to start with `startcode` which is followed by the key. The snippet is then terminated by `endcode`.

```
// startcode MySnippetName
My Snippet Code
// endcode
```


#### Using regions

Any code wrapped in a named C# region will be picked up. The name of the region is used as the key.

```
#region MySnippetName
My Snippet Code
#endregion
```


## CaptureSnippets.Simple


### The NuGet package [![NuGet Status](http://img.shields.io/nuget/v/CaptureSnippets.Simple.svg?style=flat)](https://www.nuget.org/packages/CaptureSnippets.Simple/)

https://nuget.org/packages/CaptureSnippets.Simple/

    PM> Install-Package CaptureSnippets.Simple


### Api Usage


#### Reading snippets from files

snippet: ReadingFiles


#### Reading snippets from a directory structure

snippet: ReadingDirectorySimple


#### Full Usage

snippet: markdownProcessingSimple


## CaptureSnippets


### The NuGet package [![NuGet Status](http://img.shields.io/nuget/v/CaptureSnippets.svg?style=flat)](https://www.nuget.org/packages/CaptureSnippets/)

https://nuget.org/packages/CaptureSnippets/

    PM> Install-Package CaptureSnippets


### Api Usage


#### Reading snippets from files

snippet: ReadingFiles


#### Reading snippets from a directory structure

snippet: ReadingDirectory


#### Full Usage

snippet: markdownProcessing


### Versioning

Snippets are versioned

Version follows the [NuGet version range syntax](https://docs.nuget.org/create/versioning#specifying-version-ranges-in-.nuspec-files).

For more details on NuGet versioning see https://github.com/NuGet/NuGet.Versioning/.


#### Version suffix on snippets

Appending a version to the end of a snippet definition as follows.

```
#region MySnippetName 4.5
My Snippet Code
#endregion
```

Or a version range

```
#region MySnippetName [1.0,2.0]
My Snippet Code
#endregion
```


## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com) and  is licensed under Creative Commons Attribution as: 

> "Net" by Stanislav Cherenkov from The Noun Project