namespace Cultivation.Runtime
{
    /// <summary>사육장의 단일 슬롯. CreatureInstance 0~1마리를 보유.</summary>
    public class BarnSlot
    {
        public CreatureInstance Creature { get; private set; }
        public bool IsEmpty => Creature == null;

        public bool TryPlace(CreatureInstance creature)
        {
            if (!IsEmpty || creature == null) return false;
            Creature = creature;
            return true;
        }

        public CreatureInstance Clear()
        {
            var c = Creature;
            Creature = null;
            return c;
        }
    }
}
