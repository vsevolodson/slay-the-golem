using System;
using System.Collections.Generic;

namespace Game.Core.Data
{
    public sealed class RelicCatalog
    {
        private readonly Dictionary<string, RelicDefinition> _definitions = new Dictionary<string, RelicDefinition>();

        public RelicCatalog(IEnumerable<RelicDefinition> definitions)
        {
            if (definitions == null)
                throw new ArgumentNullException(nameof(definitions));

            foreach (var definition in definitions)
            {
                if (_definitions.ContainsKey(definition.Id))
                    throw new ArgumentException($"relic '{definition.Id}' is declared twice");

                _definitions.Add(definition.Id, definition);
            }
        }

        public bool Contains(string id) => _definitions.ContainsKey(id);

        public RelicDefinition Get(string id)
        {
            if (!_definitions.TryGetValue(id, out var definition))
                throw new KeyNotFoundException($"relic '{id}' is not in the catalog");

            return definition;
        }
    }
}