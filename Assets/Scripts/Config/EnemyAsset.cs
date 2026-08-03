using System;
using System.Collections.Generic;
using Game.Core.Data;
using UnityEngine;

namespace Game.Unity.Config
{
    [Serializable]
    public sealed class IntentEntry
    {
        public int Damage;
        public int Block;
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Enemy", fileName = "Enemy")]
    public sealed class EnemyAsset : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _title;
        [SerializeField] private int _maxHealth;
        [SerializeField] private List<IntentEntry> _cycle = new List<IntentEntry>();

        public string Id => _id;
        public string Title => _title;
        public int MaxHealth => _maxHealth;

        public EnemyDefinition ToDefinition()
        {
            var cycle = new List<EnemyIntent>();
            foreach (var entry in _cycle)
                cycle.Add(new EnemyIntent(entry.Damage, entry.Block));

            return new EnemyDefinition(_id, _maxHealth, cycle);
        }
    }
}