using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Components={Components.Count}")]
    public class ReadComponents
    {
        public IReadOnlyList<Component> Components { get; }
        public string Directory { get; }
        public IReadOnlyList<Snippet> AllSnippets { get; }
        public IReadOnlyList<Snippet> SnippetsInError { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup { get; }

        public ReadComponents(IReadOnlyList<Component> components, string directory)
        {
            Guard.AgainstNull(components, nameof(components));
            Guard.AgainstNullAndEmpty(directory, nameof(directory));
            Components = components;
            Directory = directory;
            AllSnippets = Components.SelectMany(_ => _.Snippets).Distinct().ToList();
            SnippetsInError = AllSnippets.Where(_ => _.IsInError).Distinct().ToList();
            Lookup = AllSnippets.ToDictionary();
        }

        public Component GetComponent(string key)
        {
            var item = Components.SingleOrDefault(_ => string.Equals(_.Identifier, key, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                throw new Exception($"Could not find Component for '{key}'");
            }
            return item;
        }
    }
}