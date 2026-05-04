using UnityEngine;

namespace Cultivation.Data
{
    /// <summary>
    /// 크리처 종 정의. 사육장에 추가될 수 있고 교배의 부모/결과로 사용된다.
    /// </summary>
    [CreateAssetMenu(menuName = "Cultivation/Data/Creature", fileName = "Creature_New")]
    public class CreatureData : ScriptableObject
    {
        [SerializeField] private string _creatureId;
        [SerializeField] private string _creatureName;
        [SerializeField] private int _baseSellPrice;
        [SerializeField] private CreatureCategory _category;

        public string CreatureId => _creatureId;
        public string CreatureName => _creatureName;
        public int BaseSellPrice => _baseSellPrice;
        public CreatureCategory Category => _category;
    }
}
