using Game.Core.State;
using TMPro;
using UnityEngine;

namespace Game.Unity.View
{
    public sealed class CombatantView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _health;
        [SerializeField] private TextMeshProUGUI _block;
        [SerializeField] private TextMeshProUGUI _statuses;

        public void Show(string title, Combatant combatant)
        {
            _title.text = title;
            _health.text = $"{combatant.Health}/{combatant.MaxHealth}";
            _block.text = combatant.Block > 0 ? $"Block {combatant.Block}" : string.Empty;
            _statuses.text = Statuses(combatant);
        }

        private static string Statuses(Combatant combatant)
        {
            if (combatant.Poison > 0 && combatant.Vulnerable > 0)
                return $"Poison {combatant.Poison}   Vulnerable {combatant.Vulnerable}";

            if (combatant.Poison > 0)
                return $"Poison {combatant.Poison}";

            if (combatant.Vulnerable > 0)
                return $"Vulnerable {combatant.Vulnerable}";

            return string.Empty;
        }
    }
}