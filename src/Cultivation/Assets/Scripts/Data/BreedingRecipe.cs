using UnityEngine;

namespace Cultivation.Data
{
    /// <summary>
    /// 교배 레시피. 부모 두 마리 조합과 매칭되면 결과 크리처가 생성된다. 매칭은 순서 무관(A+B == B+A).
    /// </summary>
    [CreateAssetMenu(menuName = "Cultivation/Data/Breeding Recipe", fileName = "Recipe_New")]
    public class BreedingRecipe : ScriptableObject
    {
        [SerializeField] private string _parentAId;
        [SerializeField] private string _parentBId;
        [SerializeField] private string _resultCreatureId;
        [SerializeField] private float _breedingTime;

        public string ParentAId => _parentAId;
        public string ParentBId => _parentBId;
        public string ResultCreatureId => _resultCreatureId;
        public float BreedingTime => _breedingTime;

        /// <summary>두 부모 ID가 이 레시피와 매칭되는지 검사. 순서 무관.</summary>
        public bool Matches(string idA, string idB)
        {
            return (_parentAId == idA && _parentBId == idB)
                || (_parentAId == idB && _parentBId == idA);
        }
    }
}
