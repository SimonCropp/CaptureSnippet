using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Components={Components.Count}")]
    public class ReadComponents
    {
        public readonly IReadOnlyList<Component> Components;
        public readonly string Directory;
        public readonly IReadOnlyList<Snippet> AllSnippets;
        public readonly IReadOnlyList<Snippet> Shared;
        public readonly IReadOnlyList<Snippet> SnippetsInError;
        public readonly IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup;

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