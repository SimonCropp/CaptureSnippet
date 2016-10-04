using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    [DebuggerDisplay("Count={Components.Count}")]
    public class ReadComponents
    {
        public readonly IReadOnlyList<Component> Components;
        public readonly IReadOnlyList<Snippet> Shared;
        public readonly string Directory;
        public readonly IReadOnlyList<Snippet> AllSnippets;
        public readonly IReadOnlyList<Snippet> SnippetsInError;
        public readonly IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup;

        public ReadComponents(IReadOnlyList<Component> components, IReadOnlyList<Snippet> shared, string directory)
        {
            Guard.AgainstNull(components, nameof(components));
            Guard.AgainstNull(shared, nameof(shared));
            Guard.AgainstNullAndEmpty(directory, nameof(directory));
            Components = components;
            Shared = shared;
            Directory = directory;
            AllSnippets = GetAll().ToList();
            SnippetsInError = AllSnippets.Where(_ => _.IsInError).ToList();
            Lookup = AllSnippets.ToDictionary();
        }

        IEnumerable<Snippet> GetAll()
        {
            foreach (var component in Components)
            {
                foreach (var snippet in component.AllSnippets)
                {
                    yield return snippet;
                }
            }
            foreach (var snippet in Shared)
            {
                yield return snippet;
            }
        }

        public Component GetComponent(string key)
        {
            var item = Components.SingleOrDefault(package => package.Identifier == key);
            if (item == null)
            {
                throw new Exception($"Could not find Component for '{key}'");
            }
            return item;
        }

    }
}