using Cultivation.Systems;
using NUnit.Framework;

namespace Cultivation.Tests
{
    public class InventoryManagerTests
    {
        [Test]
        public void AddSeed_IncreasesCount()
        {
            var inv = new InventoryManager();
            inv.AddSeed("seed_carrot", 3);
            Assert.AreEqual(3, inv.GetSeedCount("seed_carrot"));
        }

        [Test]
        public void AddSeed_ZeroOrNegative_DoesNothing()
        {
            var inv = new InventoryManager();
            inv.AddSeed("seed_carrot", 0);
            inv.AddSeed("seed_carrot", -1);
            Assert.AreEqual(0, inv.GetSeedCount("seed_carrot"));
        }

        [Test]
        public void RemoveSeed_PartialAmount_DecreasesCount()
        {
            var inv = new InventoryManager();
            inv.AddSeed("seed_carrot", 5);
            bool ok = inv.RemoveSeed("seed_carrot", 2);
            Assert.IsTrue(ok);
            Assert.AreEqual(3, inv.GetSeedCount("seed_carrot"));
        }

        [Test]
        public void RemoveSeed_Insufficient_ReturnsFalse_NoChange()
        {
            var inv = new InventoryManager();
            inv.AddSeed("seed_carrot", 2);
            bool ok = inv.RemoveSeed("seed_carrot", 99);
            Assert.IsFalse(ok);
            Assert.AreEqual(2, inv.GetSeedCount("seed_carrot"));
        }

        [Test]
        public void RemoveSeed_AllAmount_RemovesKey()
        {
            var inv = new InventoryManager();
            inv.AddSeed("seed_carrot", 1);
            inv.RemoveSeed("seed_carrot", 1);
            Assert.AreEqual(0, inv.GetSeedCount("seed_carrot"));
            Assert.IsFalse(inv.Seeds.ContainsKey("seed_carrot"));
        }

        [Test]
        public void OnSeedChanged_Fires_OnAddAndRemove()
        {
            var inv = new InventoryManager();
            int callCount = 0;
            int lastCount = -1;
            inv.OnSeedChanged += (id, c) => { callCount++; lastCount = c; };

            inv.AddSeed("seed_carrot", 2);
            Assert.AreEqual(1, callCount);
            Assert.AreEqual(2, lastCount);

            inv.RemoveSeed("seed_carrot", 1);
            Assert.AreEqual(2, callCount);
            Assert.AreEqual(1, lastCount);

            inv.RemoveSeed("seed_carrot", 99); // 실패 → 이벤트 발생하지 않음
            Assert.AreEqual(2, callCount);

            inv.RemoveSeed("seed_carrot", 1); // 0 도달 후 제거
            Assert.AreEqual(3, callCount);
            Assert.AreEqual(0, lastCount);
        }

        [Test]
        public void Crops_HaveSameSemantics()
        {
            var inv = new InventoryManager();
            inv.AddCrop("crop_carrot", 4);
            Assert.AreEqual(4, inv.GetCropCount("crop_carrot"));
            Assert.IsTrue(inv.RemoveCrop("crop_carrot", 4));
            Assert.IsFalse(inv.Crops.ContainsKey("crop_carrot"));
        }
    }
}
