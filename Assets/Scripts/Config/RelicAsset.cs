using System.Collections.Generic;
using Game.Core.Data;
using Game.Core.Effects;
using UnityEngine;

namespace Game.Unity.Config
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Relic", fileName = "Relic")]
    public sealed class RelicAsset : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _title;
        [SerializeField] private List<EffectEntry> _combatStartEffects = new List<EffectEntry>();

        public string Id => _id;
        public string Title => _title;

        public RelicDefinition ToDefinition()
        {
            var effects = new List<IEffect>();
            foreach (var entry in _combatStartEffects)
                effects.Add(entry.ToCoreEffect());

            return new RelicDefinition(_id, effects);
        }
    }
}