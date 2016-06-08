using System;
using System.Diagnostics;
using NuGet.Versioning;

namespace CaptureSnippets
{
    /// <summary>
    /// A sub item of <see cref="ReadSnippets"/>.
    /// </summary>
    [DebuggerDisplay("Key={Key}, FileLocation={FileLocation}, Error={Error}, Package={Package.ValueOrUndefined}, Component={Component.ValueOrUndefined}")]
    public class ReadSnippet
    {

        /// <summary>
        /// Initialise a new instance of <see cref="ReadSnippet"/>.
        /// </summary>
        public ReadSnippet(string key, int lineNumberInError, string path, string error)
        {
            Guard.AgainstNegativeAndZero(lineNumberInError, nameof(lineNumberInError));
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNullAndEmpty(error, nameof(error));
            Key = key;
            StartLine = lineNumberInError;
            EndLine = lineNumberInError;
            IsInError = true;
            Path = path;
            Error = error;
        }

        /// <summary>
        /// Initialise a new instance of <see cref="ReadSnippet"/>.
        /// </summary>
        public ReadSnippet(int startLine, int endLine, string value, string key, string language, string path, VersionRange version, Package package, Component component)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstUpperCase(key, nameof(key));
            Guard.AgainstNull(language, nameof(language));
            Guard.AgainstNull(package, nameof(package));
            Guard.AgainstNull(component, nameof(component));
            Guard.AgainstUpperCase(language, nameof(language));
            Guard.AgainstNegativeAndZero(startLine, nameof(startLine));
            Guard.AgainstNegativeAndZero(endLine, nameof(endLine));

            StartLine = startLine;
            EndLine = endLine;
            Value = value;
            ValueHash = value.RemoveWhitespace().GetHashCode();
            Key = key;
            Language = language;
            Path = path;
            this.version = version;
            this.package = package;
            this.component = component;
        }

        public readonly string Error;

        public readonly bool IsInError;


        /// <summary>
        /// A hash of the <see cref="Value"/>.
        /// </summary>
        public readonly int ValueHash;

        /// <summary>
        /// The line the snippets started on
        /// </summary>
        public readonly int StartLine;

        /// <summary>
        /// The line the snippet ended on.
        /// </summary>
        public readonly int EndLine;

        /// <summary>
        /// The contents of the snippet
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// The key used to identify the snippet
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// The language of the snippet, extracted from the file extension of the input file.
        /// </summary>
        public readonly string Language;

        /// <summary>
        /// The path the snippet was read from.
        /// </summary>
        public readonly string Path;

        VersionRange version;
        Package package;
        Component component;


        /// <summary>
        /// The <see cref="Path"/>, <see cref="StartLine"/> and <see cref="EndLine"/> concatenated.
        /// </summary>
        public string FileLocation => $"{Path}({StartLine}-{EndLine})";

        /// <summary>
        /// The <see cref="VersionRange"/> that was inferred for the snippet.
        /// </summary>
        public VersionRange Version
        {
            get
            {
                if (IsInError)
                {
                    throw new Exception("Cannot access Version when IsInError.");
                }
                return version;
            }
        }

        /// <summary>
        /// The Package that was inferred for the snippet.
        /// </summary>
        public Package Package
        {
            get
            {
                if (IsInError)
                {
                    throw new Exception("Cannot access Package when IsInError.");
                }
                return package;
            }
        }

        /// <summary>
        /// The Component that was inferred for the snippet.
        /// </summary>
        public Component Component
        {
            get
            {
                if (IsInError)
                {
                    throw new Exception("Cannot access Component when IsInError.");
                }
                return component;
            }
        }
    }
}