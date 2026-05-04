using System;
using System.Collections.Generic;
using Cultivation.Data;
using Cultivation.Runtime;
using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 교배 시스템. 동시 무제한, 레시피 우선, 매칭 없을 시 부모 50/50 자기복제.
    /// 부모는 교배 동안 IsBusy=true, 완료/취소 시 false. 사육장 가득 시 결과 폐기 + 경고 로그(MVP).
    /// </summary>
    public class BreedingManager
    {
        private readonly BarnManager _barn;
        private readonly GameDataRegistry _registry;
        private readonly float _selfCloneTime;

        private readonly List<BreedingTask> _activeTasks = new List<BreedingTask>();

        public IReadOnlyList<BreedingTask> ActiveTasks => _activeTasks;

        public event Action<BreedingTask> OnBreedingStarted;
        public event Action<BreedingTask, CreatureInstance> OnBreedingCompleted;
        public event Action<BreedingTask> OnBreedingCancelled;

        public BreedingManager(BarnManager barn, GameDataRegistry registry, float selfCloneTime)
        {
            _barn = barn;
            _registry = registry;
            _selfCloneTime = selfCloneTime;
        }

        /// <summary>두 인스턴스로 교배 시작. 실패 시 null. 부모 IsBusy=true 자동 처리.</summary>
        public BreedingTask StartBreeding(string parentAInstanceId, string parentBInstanceId)
        {
            if (string.IsNullOrEmpty(parentAInstanceId) || string.IsNullOrEmpty(parentBInstanceId))
                return null;
            if (parentAInstanceId == parentBInstanceId)
            {
                Debug.LogWarning("[BreedingManager] 같은 인스턴스를 부모로 지정할 수 없습니다.");
                return null;
            }

            var a = _barn.FindCreature(parentAInstanceId);
            var b = _barn.FindCreature(parentBInstanceId);
            if (a == null || b == null)
            {
                Debug.LogWarning("[BreedingManager] 부모 크리처를 사육장에서 찾을 수 없습니다.");
                return null;
            }
            if (a.IsBusy || b.IsBusy)
            {
                Debug.LogWarning("[BreedingManager] 점유 중인 크리처는 교배할 수 없습니다.");
                return null;
            }

            float requiredTime = DetermineRequiredTime(a.CreatureId, b.CreatureId);
            var task = new BreedingTask(parentAInstanceId, parentBInstanceId, requiredTime);

            a.SetBusy(true);
            b.SetBusy(true);

            _activeTasks.Add(task);
            OnBreedingStarted?.Invoke(task);
            return task;
        }

        /// <summary>교배 취소. 부모 IsBusy 즉시 해제, 이벤트 발행.</summary>
        public bool CancelBreeding(string taskId)
        {
            int idx = FindTaskIndex(taskId);
            if (idx < 0) return false;

            var task = _activeTasks[idx];
            ReleaseParents(task);
            _activeTasks.RemoveAt(idx);
            OnBreedingCancelled?.Invoke(task);
            return true;
        }

        public void Tick(float deltaTime)
        {
            // 완료된 태스크를 안전하게 처리하기 위해 역순 순회
            for (int i = _activeTasks.Count - 1; i >= 0; i--)
            {
                var task = _activeTasks[i];
                task.Tick(deltaTime);
                if (task.IsComplete)
                {
                    var result = ResolveAndProduceResult(task);
                    ReleaseParents(task);
                    _activeTasks.RemoveAt(i);
                    OnBreedingCompleted?.Invoke(task, result);
                }
            }
        }

        public bool HasActiveBreeding() => _activeTasks.Count > 0;

        private int FindTaskIndex(string taskId)
        {
            for (int i = 0; i < _activeTasks.Count; i++)
                if (_activeTasks[i].TaskId == taskId) return i;
            return -1;
        }

        private void ReleaseParents(BreedingTask task)
        {
            var a = _barn.FindCreature(task.ParentAInstanceId);
            var b = _barn.FindCreature(task.ParentBInstanceId);
            if (a != null) a.SetBusy(false);
            if (b != null) b.SetBusy(false);
        }

        private float DetermineRequiredTime(string creatureIdA, string creatureIdB)
        {
            if (_registry != null)
            {
                var recipe = _registry.FindRecipe(creatureIdA, creatureIdB);
                if (recipe != null) return recipe.BreedingTime;
            }
            return _selfCloneTime;
        }

        /// <summary>레시피 우선, 매칭 없으면 부모 50/50 자기복제. 사육장 가득 시 결과 폐기.</summary>
        private CreatureInstance ResolveAndProduceResult(BreedingTask task)
        {
            var a = _barn.FindCreature(task.ParentAInstanceId);
            var b = _barn.FindCreature(task.ParentBInstanceId);
            if (a == null || b == null)
            {
                Debug.LogWarning("[BreedingManager] 완료 시 부모를 찾을 수 없습니다. 결과 폐기.");
                return null;
            }

            string resultCreatureId = null;

            if (_registry != null)
            {
                var recipe = _registry.FindRecipe(a.CreatureId, b.CreatureId);
                if (recipe != null) resultCreatureId = recipe.ResultCreatureId;
            }

            if (string.IsNullOrEmpty(resultCreatureId))
            {
                // 자기복제: 50/50
                resultCreatureId = UnityEngine.Random.value < 0.5f ? a.CreatureId : b.CreatureId;
            }

            // 결과 종 검증
            var creatureData = _registry != null ? _registry.FindCreature(resultCreatureId) : null;
            if (creatureData == null)
            {
                Debug.LogError($"[BreedingManager] 결과 CreatureData가 레지스트리에 없습니다: {resultCreatureId}");
                return null;
            }

            if (!_barn.HasFreeSlot())
            {
                Debug.LogWarning($"[BreedingManager] 사육장이 가득 차서 결과({resultCreatureId})를 폐기했습니다.");
                return null;
            }

            var instance = new CreatureInstance(creatureData.CreatureId);
            if (!_barn.AddCreature(instance))
            {
                Debug.LogError("[BreedingManager] 사육장 추가 실패. 결과 폐기.");
                return null;
            }
            return instance;
        }
    }
}
