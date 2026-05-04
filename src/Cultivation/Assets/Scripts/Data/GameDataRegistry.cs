using System.Collections.Generic;
using UnityEngine;

namespace Cultivation.Data
{
    /// <summary>
    /// 모든 게임 데이터 SO를 중앙에서 보관 + id 기반 조회. GameManager가 단일 인스턴스를 참조하여 매니저들에 주입한다.
    /// </summary>
    [CreateAssetMenu(menuName = "Cultivation/Data/Game Data Registry", fileName = "GameDataRegistry")]
    public class GameDataRegistry : ScriptableObject
    {
        [SerializeField] private List<SeedData> _seeds = new List<SeedData>();
        [SerializeField] private List<CropData> _crops = new List<CropData>();
        [SerializeField] private List<CreatureData> _creatures = new List<CreatureData>();
        [SerializeField] private List<BreedingRecipe> _breedingRecipes = new List<BreedingRecipe>();

        public IReadOnlyList<SeedData> Seeds => _seeds;
        public IReadOnlyList<CropData> Crops => _crops;
        public IReadOnlyList<CreatureData> Creatures => _creatures;
        public IReadOnlyList<BreedingRecipe> BreedingRecipes => _breedingRecipes;

        public SeedData FindSeed(string id)
        {
            for (int i = 0; i < _seeds.Count; i++)
                if (_seeds[i] != null && _seeds[i].SeedId == id) return _seeds[i];
            return null;
        }

        public CropData FindCrop(string id)
        {
            for (int i = 0; i < _crops.Count; i++)
                if (_crops[i] != null && _crops[i].CropId == id) return _crops[i];
            return null;
        }

        public CreatureData FindCreature(string id)
        {
            for (int i = 0; i < _creatures.Count; i++)
                if (_creatures[i] != null && _creatures[i].CreatureId == id) return _creatures[i];
            return null;
        }

        public BreedingRecipe FindRecipe(string parentAId, string parentBId)
        {
            for (int i = 0; i < _breedingRecipes.Count; i++)
                if (_breedingRecipes[i] != null && _breedingRecipes[i].Matches(parentAId, parentBId))
                    return _breedingRecipes[i];
            return null;
        }

#if UNITY_EDITOR
        /// <summary>Editor에서 SO 수정 시 자동 호출. id 중복/누락, 참조 무결성을 검사하여 콘솔에 경고.</summary>
        private void OnValidate()
        {
            ValidateUniqueIds(_seeds, s => s == null ? null : s.SeedId, "SeedData");
            ValidateUniqueIds(_crops, c => c == null ? null : c.CropId, "CropData");
            ValidateUniqueIds(_creatures, c => c == null ? null : c.CreatureId, "CreatureData");

            // CropData.CreatureId가 등록된 크리처와 매칭되는지
            for (int i = 0; i < _crops.Count; i++)
            {
                var crop = _crops[i];
                if (crop == null) continue;
                if (string.IsNullOrEmpty(crop.CreatureId))
                {
                    Debug.LogWarning($"[GameDataRegistry] CropData '{crop.CropId}'의 CreatureId가 비어있습니다.", crop);
                    continue;
                }
                if (FindCreature(crop.CreatureId) == null)
                {
                    Debug.LogError($"[GameDataRegistry] CropData '{crop.CropId}'가 미등록 크리처를 참조: {crop.CreatureId}", crop);
                }
            }

            // BreedingRecipe의 부모/결과 참조 무결성
            for (int i = 0; i < _breedingRecipes.Count; i++)
            {
                var r = _breedingRecipes[i];
                if (r == null) continue;
                if (FindCreature(r.ParentAId) == null)
                    Debug.LogError($"[GameDataRegistry] Recipe '{r.name}'의 ParentAId 미등록: {r.ParentAId}", r);
                if (FindCreature(r.ParentBId) == null)
                    Debug.LogError($"[GameDataRegistry] Recipe '{r.name}'의 ParentBId 미등록: {r.ParentBId}", r);
                if (FindCreature(r.ResultCreatureId) == null)
                    Debug.LogError($"[GameDataRegistry] Recipe '{r.name}'의 ResultCreatureId 미등록: {r.ResultCreatureId}", r);
            }

            // SeedData.ResultCropId가 등록된 작물과 매칭되는지
            for (int i = 0; i < _seeds.Count; i++)
            {
                var seed = _seeds[i];
                if (seed == null) continue;
                if (string.IsNullOrEmpty(seed.ResultCropId))
                {
                    Debug.LogWarning($"[GameDataRegistry] SeedData '{seed.SeedId}'의 ResultCropId가 비어있습니다.", seed);
                    continue;
                }
                if (FindCrop(seed.ResultCropId) == null)
                {
                    Debug.LogError($"[GameDataRegistry] SeedData '{seed.SeedId}'가 미등록 작물 참조: {seed.ResultCropId}", seed);
                }
            }
        }

        private static void ValidateUniqueIds<T>(List<T> list, System.Func<T, string> idGetter, string typeName)
            where T : Object
        {
            var seen = new HashSet<string>();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item == null)
                {
                    Debug.LogWarning($"[GameDataRegistry] {typeName} 목록 인덱스 {i}이 null입니다.");
                    continue;
                }
                string id = idGetter(item);
                if (string.IsNullOrEmpty(id))
                {
                    Debug.LogWarning($"[GameDataRegistry] {typeName} '{item.name}'의 id가 비어있습니다.", item);
                    continue;
                }
                if (!seen.Add(id))
                {
                    Debug.LogError($"[GameDataRegistry] {typeName} id 중복: '{id}' ({item.name})", item);
                }
            }
        }
#endif
    }
}
