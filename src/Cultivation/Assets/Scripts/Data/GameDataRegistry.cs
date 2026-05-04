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
    }
}
