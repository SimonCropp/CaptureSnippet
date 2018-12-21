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
        public IReadOnlyList<Snippet> Shared { get; }
        public IReadOnlyList<Snippet> SnippetsInError { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup { get; }

        public ReadComponents(IReadOnlyList<Component> components, string directory, IReadOnlyList<Snippet> shared)
        {
            Guard.AgainstNull(components, nameof(components));
            Guard.AgainstNullAndEmpty(directory, nameof(directory));
            Guard.AgainstNull(shared, nameof(shared));
            Shared = shared;
            Components = components;
            Directory = directory;
            AllSnippets = Components.SelectMany(_ => _.Snippets).Concat(shared).Distinct().ToList();
            SnippetsInError = AllSnippets.Where(_ => _.IsInError).Distinct().ToList();
            Lookup = AllSnippets.ToDictionary();
        }

        public Component GetComponent(string key)
        {
            var item = Components.SingleOrDefault(_ => _.Identifier == key);
            if (item == null)
            {
                throw new Exception($"Could not find Component for '{key}'");
            }
            return item;
        }
    }
}