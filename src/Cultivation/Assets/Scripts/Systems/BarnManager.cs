using System;
using System.Collections.Generic;
using Cultivation.Data;
using Cultivation.Runtime;
using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 사육장. 슬롯 기반 크리처 보관 및 확장 처리.
    /// </summary>
    public class BarnManager
    {
        private readonly EconomyManager _economy;
        private readonly ExpansionConfig _expansionConfig;
        private readonly List<BarnSlot> _slots = new List<BarnSlot>();

        public event Action<CreatureInstance> OnCreatureAdded;
        public event Action<string> OnCreatureRemoved;
        public event Action<int> OnBarnExpanded;

        public IReadOnlyList<BarnSlot> Slots => _slots;
        public int SlotCount => _slots.Count;

        public BarnManager(EconomyManager economy, ExpansionConfig expansionConfig, int startingSlots)
        {
            _economy = economy;
            _expansionConfig = expansionConfig;
            for (int i = 0; i < startingSlots; i++)
                _slots.Add(new BarnSlot());
        }

        public bool HasFreeSlot()
        {
            for (int i = 0; i < _slots.Count; i++)
                if (_slots[i].IsEmpty) return true;
            return false;
        }

        /// <summary>크리처 추가. 빈 슬롯 없으면 false.</summary>
        public bool AddCreature(CreatureInstance creature)
        {
            if (creature == null) return false;
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i].TryPlace(creature))
                {
                    OnCreatureAdded?.Invoke(creature);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveCreature(string instanceId)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                var c = _slots[i].Creature;
                if (c != null && c.InstanceId == instanceId)
                {
                    _slots[i].Clear();
                    OnCreatureRemoved?.Invoke(instanceId);
                    return true;
                }
            }
            return false;
        }

        public CreatureInstance FindCreature(string instanceId)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                var c = _slots[i].Creature;
                if (c != null && c.InstanceId == instanceId) return c;
            }
            return null;
        }

        /// <summary>Busy=false인 모든 크리처.</summary>
        public List<CreatureInstance> GetAvailableCreatures()
        {
            var list = new List<CreatureInstance>();
            for (int i = 0; i < _slots.Count; i++)
            {
                var c = _slots[i].Creature;
                if (c != null && !c.IsBusy) list.Add(c);
            }
            return list;
        }

        public int GetExpansionCost()
        {
            if (_expansionConfig == null) return 0;
            return _expansionConfig.CalculateCost(_expansionConfig.BarnBaseCost, _slots.Count);
        }

        public bool TryExpand()
        {
            int cost = GetExpansionCost();
            if (!_economy.TrySpendGold(cost)) return false;
            _slots.Add(new BarnSlot());
            OnBarnExpanded?.Invoke(_slots.Count);
            return true;
        }
    }
}
