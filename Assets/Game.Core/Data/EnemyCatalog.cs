using System;
using System.Collections.Generic;

namespace Game.Core.Data
{
    public sealed class EnemyCatalog
    {
        private readonly Dictionary<string, EnemyDefinition> _definitions = new Dictionary<string, EnemyDefinition>();

        public EnemyCatalog(IEnumerable<EnemyDefinition> definitions)
        {
            if (definitions == null)
                throw new ArgumentNullException(nameof(definitions));

            foreach (var definition in definitions)
            {
                if (_definitions.ContainsKey(definition.Id))
                    throw new ArgumentException($"enemy '{definition.Id}' is declared twice");

                _definitions.Add(definition.Id, definition);
            }
        }

        public bool Contains(string id) => _definitions.ContainsKey(id);

        public EnemyDefinition Get(string id)
        {
            if (!_definitions.TryGetValue(id, out var definition))
                throw new KeyNotFoundException($"enemy '{id}' is not in the catalog");

            return definition;
        }
    }
}