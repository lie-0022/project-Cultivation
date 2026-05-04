using Cultivation.Data;
using UnityEngine;

namespace Cultivation.Runtime
{
    /// <summary>
    /// 밭의 단일 플롯. 씨앗을 보유하고 elapsedTime을 누적해 Growing→Ready로 자동 전환된다.
    /// </summary>
    public class FarmPlot
    {
        public PlotState State { get; private set; } = PlotState.Empty;
        public SeedData CurrentSeed { get; private set; }
        public float ElapsedTime { get; private set; }

        /// <summary>UI 표시용 진행도 0~1.</summary>
        public float Progress
        {
            get
            {
                if (CurrentSeed == null || CurrentSeed.GrowthTime <= 0f) return 0f;
                return Mathf.Clamp01(ElapsedTime / CurrentSeed.GrowthTime);
            }
        }

        public bool IsEmpty => State == PlotState.Empty;
        public bool IsReady => State == PlotState.Ready;

        /// <summary>Empty 상태일 때만 호출. 씨앗 심기 처리.</summary>
        public bool Plant(SeedData seed)
        {
            if (!IsEmpty || seed == null) return false;
            CurrentSeed = seed;
            ElapsedTime = 0f;
            State = PlotState.Growing;
            return true;
        }

        /// <summary>Tick. Growing 상태에서만 시간 누적, growthTime 도달 시 Ready로 전환되며 true 리턴.</summary>
        public bool Tick(float deltaTime)
        {
            if (State != PlotState.Growing || CurrentSeed == null) return false;
            ElapsedTime += deltaTime;
            if (ElapsedTime >= CurrentSeed.GrowthTime)
            {
                State = PlotState.Ready;
                return true;
            }
            return false;
        }

        /// <summary>Ready 상태일 때만 호출. 결과 씨앗 정보 보존 후 비우기.</summary>
        public SeedData Harvest()
        {
            if (!IsReady) return null;
            var seed = CurrentSeed;
            Reset();
            return seed;
        }

        public void Reset()
        {
            CurrentSeed = null;
            ElapsedTime = 0f;
            State = PlotState.Empty;
        }
    }
}
