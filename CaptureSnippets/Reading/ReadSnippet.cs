using NuGet.Versioning;

namespace CaptureSnippets
{
    /// <summary>
    /// A sub item of <see cref="ReadSnippets"/>.
    /// </summary>
    public class ReadSnippet
    {
        /// <summary>
        /// Initialise a new insatnce of <see cref="ReadSnippet"/>.
        /// </summary>
        public ReadSnippet(int startLine, int endLine, string value, string key, string language, string file, VersionRange version, string package)
        {
            Guard.AgainstNullAndEmpty(key, "key");
            Guard.AgainstUpperCase(key, "key");
            Guard.AgainstNull(language, "language");
            Guard.AgainstUpperCase(language, "language");
            Guard.AgainstNegativeAndZero(startLine, "startLine");
            Guard.AgainstNegativeAndZero(endLine, "endLine");
           
            StartLine = startLine;
            EndLine = endLine;
            Value = value;
            ValueHash = value.RemoveWhitespace().GetHashCode();
            Key = key;
            Language = language;
            File = file;
            Version = version;
            Package = package;
        }

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
        /// The file path the snippet was read from.
        /// </summary>
        public readonly string File;
        /// <summary>
        /// The <see cref="VersionRange"/> that was inferred for the snippet.
        /// </summary>
        public readonly VersionRange Version;
        /// <summary>
        /// The Package that was inferred for the snippet.
        /// </summary>
        public readonly string Package;


        public string FileLocation => $"{File}({StartLine}-{EndLine})";
    }
}