using System;
using UnityEngine;

namespace Cultivation.Runtime
{
    /// <summary>
    /// 진행 중인 교배 작업. 두 부모 InstanceId와 누적 시간을 보유하며, IsComplete 도달 시 BreedingManager가 결과를 처리한다.
    /// </summary>
    public class BreedingTask
    {
        public string TaskId { get; }
        public string ParentAInstanceId { get; }
        public string ParentBInstanceId { get; }
        public float RequiredTime { get; }
        public float ElapsedTime { get; private set; }

        public bool IsComplete => ElapsedTime >= RequiredTime;
        public float Progress => RequiredTime > 0f ? Mathf.Clamp01(ElapsedTime / RequiredTime) : 0f;

        public BreedingTask(string parentAInstanceId, string parentBInstanceId, float requiredTime)
        {
            TaskId = Guid.NewGuid().ToString();
            ParentAInstanceId = parentAInstanceId;
            ParentBInstanceId = parentBInstanceId;
            RequiredTime = requiredTime;
            ElapsedTime = 0f;
        }

        public void Tick(float deltaTime)
        {
            if (IsComplete) return;
            ElapsedTime += deltaTime;
        }
    }
}
