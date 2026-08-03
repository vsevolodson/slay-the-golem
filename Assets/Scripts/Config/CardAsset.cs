using System.Collections.Generic;
using Game.Core.Data;
using Game.Core.Effects;
using Game.Core.State;
using UnityEngine;

namespace Game.Unity.Config
{
    [CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
    public sealed class CardAsset : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _title;
        [SerializeField] private int _cost;
        [SerializeField] private List<EffectEntry> _effects = new List<EffectEntry>();

        public CardId Id => new CardId(_id);
        public string Title => _title;
        public int Cost => _cost;

        public CardDefinition ToDefinition()
        {
            var effects = new List<IEffect>();
            foreach (var entry in _effects)
                effects.Add(entry.ToCoreEffect());

            return new CardDefinition(Id, _cost, effects);
        }
    }
}