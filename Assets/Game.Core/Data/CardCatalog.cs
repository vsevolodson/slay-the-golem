using System;
using System.Collections.Generic;
using Game.Core.State;

namespace Game.Core.Data
{
    public sealed class CardCatalog
    {
        private readonly Dictionary<CardId, CardDefinition> _definitions = new Dictionary<CardId, CardDefinition>();

        public CardCatalog(IEnumerable<CardDefinition> definitions)
        {
            if (definitions == null)
                throw new ArgumentNullException(nameof(definitions));

            foreach (var definition in definitions)
            {
                if (_definitions.ContainsKey(definition.Id))
                    throw new ArgumentException($"card '{definition.Id}' is declared twice");

                _definitions.Add(definition.Id, definition);
            }
        }

        public bool Contains(CardId id) => _definitions.ContainsKey(id);

        public CardDefinition Get(CardId id)
        {
            if (!_definitions.TryGetValue(id, out var definition))
                throw new KeyNotFoundException($"card '{id}' is not in the catalog");

            return definition;
        }
    }
}