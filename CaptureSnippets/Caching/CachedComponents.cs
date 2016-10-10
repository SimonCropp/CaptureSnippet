namespace CaptureSnippets
{
    /// <summary>
    /// The information cached by <see cref="CachedSnippetExtractor"/>.
    /// </summary>
    public class CachedComponents
    {
        /// <summary>
        /// Initialise a new instance of <see cref="CachedComponents"/>.
        /// </summary>
        public CachedComponents(ReadComponents components, long ticks)
        {
            Guard.AgainstNull(components, nameof(components));
            Guard.AgainstNegativeAndZero(ticks, nameof(ticks));
            Components = components;
            Ticks = ticks;
        }

        /// <summary>
        /// The <see cref="ReadComponents"/> from the passed in directory.
        /// </summary>
        public readonly ReadComponents Components;

        /// <summary>
        /// The ticks of the last file change in the passed in directory.
        /// </summary>
        public readonly long Ticks;
    }
}