namespace CaptureSnippets
{
    /// <summary>
    /// The information cached by <see cref="CachedSnippetExtractor"/>.
    /// </summary>
    public class CachedSnippets
    {
        /// <summary>
        /// Initialise a new instance of <see cref="CachedPackages"/>.
        /// </summary>
        public CachedSnippets(ReadSnippets readSnippets, long ticks)
        {
            Guard.AgainstNull(readSnippets, nameof(readSnippets));
            Guard.AgainstNegativeAndZero(ticks, nameof(ticks));
            ReadSnippets = readSnippets;
            Ticks = ticks;
        }

        /// <summary>
        /// The <see cref="ReadSnippets"/> from the passed in directory.
        /// </summary>
        public readonly ReadSnippets ReadSnippets;

        /// <summary>
        /// The ticks of the last file change in the passed in directory.
        /// </summary>
        public readonly long Ticks;
    }
}