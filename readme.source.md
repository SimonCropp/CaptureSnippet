# <img src="https://raw.github.com/SimonCropp/CaptureSnippet/master/src/icon.png" height="40px"> CaptureSnippets

Extract code snippets from any language to be used when building documentation

Loosely based on some code from  https://github.com/shiftkey/scribble


## CaptureSnippetsSimple

This repository previously contained a two variants. Both a complex (CaptureSnippets), and simplified (CaptureSnippetsSimple) variants. CaptureSnippetsSimple has since been moved and renamed to [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).


## Using Snippets

The keyed snippets can then be used in any documentation `.md` file by adding the text `snippet: KEY`.

Then snippets with the key (all versions) will be rendered in a tabbed manner. If there is only a single version then it will be rendered as a simple code block with no tabs.

For example

<pre>
Some blurb about the below snippet
snippet&#58; MySnippetName
</pre>

The resulting markdown will be will be:

    Some blurb about the below snippet
    ```
    My Snippet Code
    ```


## Defining Snippets


### Using comments

Any code wrapped in a convention based comment will be picked up. The comment needs to start with `startcode` which is followed by the key. The snippet is then terminated by `endcode`.

```
// startcode MySnippetName
My Snippet Code
// endcode
```


### Using regions

Any code wrapped in a named C# region will be picked up. The name of the region is used as the key.

```
#region MySnippetName
My Snippet Code
#endregion
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


### Do not mix tabs and spaces

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


### Ignore paths

When scanning for snippets the following are ignored:

 * All directories and files starting with a period `.`
 * All binary files as defined by https://github.com/sindresorhus/binary-extensions/
 * Any of the following directory names: `bin`, `obj`

To change these conventions manipulate lists `CaptureSnippets.Exclusions.ExcludedDirectorySuffixes` and `CaptureSnippets.Exclusions.ExcludedFileExtensions`.


## The NuGet package [![NuGet Status](http://img.shields.io/nuget/v/CaptureSnippets.svg?style=flat)](https://www.nuget.org/packages/CaptureSnippets/)

https://nuget.org/packages/CaptureSnippets/

    PM> Install-Package CaptureSnippets


## Data model

Component -> Package -> VersionGroup -> Snippets

The requirement for Component and Package is required due to Package being strongly tied to a deployment concept, where Component allows a logical concept. This enabled several scenarios:

 * Packages to be renamed, but still tied to a single parent Component from a functionality perspective.
 * Multiple Packages, that represent different parts of that same logical feature, to be grouped under a Component.


## Directory Convention

The directory convention follows the below structure:

Root Dir -> Component Dirs -> Package Version Dirs -> Snippet Files

A `Shared` directory can exist at the Root and/or Component levels that contains snippet files that are shared either globally or for a Component respectively.

So an example directory structure could be as follows:

 * ComponentX
   * PackageA_1
   * PackageA_2
   * PackageB_1
   * Shared
 * ComponentY
   * PackageC_1
   * PackageC_2
 * Shared


## Versioning

Snippets are versioned.

Version follows the [NuGet version range syntax](https://docs.nuget.org/create/versioning#specifying-version-ranges-in-.nuspec-files).

For more details on NuGet versioning see https://www.nuget.org/packages/NuGet.Versioning/ and https://github.com/NuGet/NuGet.Client.


### Version suffix on package directory

Package directories can be suffixed with a version delimited by an underscore `_`. For example `PackageA_2` contains all snippets for `PackageA` version 2. Version ranges are not supported as package suffixes.


### Version suffix on snippets

Appending a version to the end of a snippet definition as follows.

```cs
#region MySnippetName 4.5
My Snippet Code
#endregion
```

Or a version range

```cs
#region MySnippetName [1.0,2.0]
My Snippet Code
#endregion
```


## Api Usage


### Reading snippets from files

snippet: ReadingFiles


### Reading snippets from a directory structure

snippet: ReadingDirectory


### Full Usage

snippet: markdownProcessing


## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com) and is licensed under Creative Commons Attribution as: 

> "Net" by Stanislav Cherenkov from The Noun Project