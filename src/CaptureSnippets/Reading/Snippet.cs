using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using NuGet.Versioning;

namespace CaptureSnippets
{
    [DebuggerDisplay("Key={Key}, FileLocation={FileLocation}, Error={Error}")]
    public class Snippet
    {
        /// <summary>
        /// Initialise a new instance of <see cref="Snippet"/>.
        /// </summary>
        public static Snippet BuildError(string key, int lineNumberInError, string path, string error)
        {
            Guard.AgainstNegativeAndZero(lineNumberInError, nameof(lineNumberInError));
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNullAndEmpty(error, nameof(error));
            return new Snippet
            {
                Key = key,
                StartLine = lineNumberInError,
                EndLine = lineNumberInError,
                IsInError = true,
                Path = path,
                Error = error
            };
        }

        /// <summary>
        /// Initialise a new instance of <see cref="Snippet"/>.
        /// </summary>
        public static Snippet Build(int startLine, int endLine, string value, string key, string language, string path, VersionRange version, string package, bool isCurrent, ISet<string> includes)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstUpperCase(key, nameof(key));
            Guard.AgainstNull(language, nameof(language));
            Guard.AgainstUpperCase(language, nameof(language));
            Guard.AgainstEmpty(package, nameof(package));
            Guard.AgainstNegativeAndZero(startLine, nameof(startLine));
            Guard.AgainstNegativeAndZero(endLine, nameof(endLine));
            return new Snippet
            {
                version = version,
                package = package,
                StartLine = startLine,
                EndLine = endLine,
                value = value,
                Key = key,
                Language = language,
                Path = path,
                IsCurrent = isCurrent,
                Includes = new ReadOnlyCollection<string>(includes?.OrderBy(x => x.ToLowerInvariant()).ToList() ?? new List<string>())
            };
        }

        /// <summary>
        /// Initialise a new instance of <see cref="Snippet"/>.
        /// </summary>
        public static Snippet BuildShared(int startLine, int endLine, string value, string key, string language, string path, ISet<string> includes)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstUpperCase(key, nameof(key));
            Guard.AgainstNull(language, nameof(language));
            Guard.AgainstUpperCase(language, nameof(language));
            Guard.AgainstNegativeAndZero(startLine, nameof(startLine));
            Guard.AgainstNegativeAndZero(endLine, nameof(endLine));
            return new Snippet
            {
                IsShared = true,
                StartLine = startLine,
                EndLine = endLine,
                value = value,
                Key = key,
                Language = language,
                Path = path,
                Includes = new ReadOnlyCollection<string>(includes?.OrderBy(x => x.ToLowerInvariant()).ToList() ?? new List<string>())
            };
        }

        public bool IsShared { get; private set; }

        public string Error { get; private set; }

        public bool IsInError { get; private set; }

        /// <summary>
        /// The key used to identify the snippet
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// The language of the snippet, extracted from the file extension of the input file.
        /// </summary>
        public string Language { get; private set; }

        /// <summary>
        /// The path the snippet was read from.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The line the snippets started on
        /// </summary>
        public int StartLine { get; private set; }

        /// <summary>
        /// The line the snippet ended on.
        /// </summary>
        public int EndLine { get; private set; }

        public bool IsCurrent { get; private set; }

        /// <summary>
        /// The <see cref="Path"/>, <see cref="StartLine"/> and <see cref="EndLine"/> concatenated.
        /// </summary>
        public string FileLocation => $"{Path}({StartLine}-{EndLine})";

        /// <summary>
        /// A string with all the includes in the snippets
        /// </summary>
        public IReadOnlyList<string> Includes { get; private set; }

        /// <summary>
        /// The <see cref="VersionRange"/> that was inferred for the snippet.
        /// </summary>
        public VersionRange Version
        {
            get
            {
                ThrowIfIsInError();
                return version;
            }
        }
        VersionRange version;

        public string Value
        {
            get
            {
                ThrowIfIsInError();
                return value;
            }
        }
        string value;

        public string Package
        {
            get
            {
                ThrowIfIsInError();
                return package;
            }
        }
        string package;

        void ThrowIfIsInError()
        {
            if (IsInError)
            {
                throw new SnippetReadingException($"Cannot access when {nameof(IsInError)}. Key: {Key}. FileLocation: {FileLocation}. Error: {Error}");
            }
        }

        public override string ToString()
        {
            return $@"ReadSnippet.
  Key: {Key}
  FileLocation: {FileLocation}
  Error: {Error}
";
        }
    }
}