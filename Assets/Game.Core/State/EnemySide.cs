using System;
using Newtonsoft.Json;

namespace Game.Core.State
{
    public sealed class EnemySide
    {
        [JsonProperty] public Combatant Combatant { get; private set; }
        [JsonProperty] public string EnemyId { get; private set; }
        [JsonProperty] public int IntentIndex { get; private set; }

        internal EnemySide(Combatant combatant, string enemyId)
        {
            if (string.IsNullOrWhiteSpace(enemyId))
                throw new ArgumentException("enemy id must not be empty");

            Combatant = combatant ?? throw new ArgumentNullException(nameof(combatant));
            EnemyId = enemyId;
            IntentIndex = 0;
        }

        [JsonConstructor]
        private EnemySide() {}

        internal void SetIntentIndex(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "intent index must not be negative");

            IntentIndex = index;
        }
    }
}
