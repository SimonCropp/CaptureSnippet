namespace CaptureSnippets
{
    /// <summary>
    /// The information cached by <see cref="CachedSnippetExtractor"/>.
    /// </summary>
    public class CachedPackages
    {
        /// <summary>
        /// Initialise a new instance of <see cref="CachedPackages"/>.
        /// </summary>
        public CachedPackages(ReadPackages readPackages, long ticks)
        {
            Guard.AgainstNull(readPackages, nameof(readPackages));
            Guard.AgainstNegativeAndZero(ticks, nameof(ticks));
            ReadPackages = readPackages;
            Ticks = ticks;
        }

        /// <summary>
        /// The <see cref="ReadPackages"/> from the passed in directory.
        /// </summary>
        public ReadPackages ReadPackages { get; }

        /// <summary>
        /// The ticks of the last file change in the passed in directory.
        /// </summary>
        public long Ticks { get; }
    }
}