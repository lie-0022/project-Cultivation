using System;

namespace Cultivation.Runtime
{
    /// <summary>
    /// 사육장에 보관되는 크리처 개체. 고유 instanceId(GUID)로 식별되며 교배 시 isBusy=true가 된다.
    /// </summary>
    public class CreatureInstance
    {
        public string InstanceId { get; }
        public string CreatureId { get; }
        public DateTime CreatedAt { get; }
        public bool IsBusy { get; private set; }

        public CreatureInstance(string creatureId)
        {
            InstanceId = Guid.NewGuid().ToString();
            CreatureId = creatureId;
            CreatedAt = DateTime.Now;
            IsBusy = false;
        }

        /// <summary>교배 등에서 점유 상태를 변경. BarnManager/BreedingManager만 호출 권장.</summary>
        public void SetBusy(bool busy) => IsBusy = busy;
    }
}
