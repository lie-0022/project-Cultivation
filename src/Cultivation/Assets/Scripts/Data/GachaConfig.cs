using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cultivation.Data
{
    /// <summary>
    /// 가챠 풀 정의. 1회 뽑기 비용과 가중치 기반 추첨 엔트리 목록.
    /// </summary>
    [CreateAssetMenu(menuName = "Cultivation/Data/Gacha Config", fileName = "GachaConfig")]
    public class GachaConfig : ScriptableObject
    {
        [SerializeField] private int _pullCost = 50;
        [SerializeField] private List<GachaEntry> _entries = new List<GachaEntry>();

        public int PullCost => _pullCost;
        public IReadOnlyList<GachaEntry> Entries => _entries;
    }

    /// <summary>가챠 풀의 단일 엔트리. 씨앗과 가중치(상대적 등장 확률).</summary>
    [Serializable]
    public class GachaEntry
    {
        [SerializeField] private SeedData _seed;
        [SerializeField] private float _weight;

        public SeedData Seed => _seed;
        public float Weight => _weight;
    }
}
