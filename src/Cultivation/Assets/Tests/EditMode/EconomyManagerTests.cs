using Cultivation.Systems;
using NUnit.Framework;

namespace Cultivation.Tests
{
    public class EconomyManagerTests
    {
        [Test]
        public void Constructor_SetsStartingGold()
        {
            var eco = new EconomyManager(200);
            Assert.AreEqual(200, eco.Gold);
        }

        [Test]
        public void TrySpend_Sufficient_DecreasesAndReturnsTrue()
        {
            var eco = new EconomyManager(200);
            Assert.IsTrue(eco.TrySpendGold(50));
            Assert.AreEqual(150, eco.Gold);
        }

        [Test]
        public void TrySpend_Insufficient_ReturnsFalse_NoChange()
        {
            var eco = new EconomyManager(50);
            Assert.IsFalse(eco.TrySpendGold(100));
            Assert.AreEqual(50, eco.Gold);
        }

        [Test]
        public void TrySpend_ZeroOrNegative_ReturnsFalse()
        {
            var eco = new EconomyManager(100);
            Assert.IsFalse(eco.TrySpendGold(0));
            Assert.IsFalse(eco.TrySpendGold(-5));
            Assert.AreEqual(100, eco.Gold);
        }

        [Test]
        public void AddGold_Positive_Increases()
        {
            var eco = new EconomyManager(0);
            eco.AddGold(100);
            Assert.AreEqual(100, eco.Gold);
        }

        [Test]
        public void AddGold_ZeroOrNegative_DoesNothing()
        {
            var eco = new EconomyManager(50);
            eco.AddGold(0);
            eco.AddGold(-10);
            Assert.AreEqual(50, eco.Gold);
        }

        [Test]
        public void OnGoldChanged_Fires_OnAddAndSpend_Only()
        {
            var eco = new EconomyManager(100);
            int callCount = 0;
            int lastGold = -1;
            eco.OnGoldChanged += g => { callCount++; lastGold = g; };

            eco.AddGold(50);
            Assert.AreEqual(1, callCount);
            Assert.AreEqual(150, lastGold);

            eco.TrySpendGold(30);
            Assert.AreEqual(2, callCount);
            Assert.AreEqual(120, lastGold);

            eco.TrySpendGold(9999); // 실패 → 이벤트 X
            Assert.AreEqual(2, callCount);

            eco.AddGold(0); // no-op → 이벤트 X
            Assert.AreEqual(2, callCount);
        }
    }
}
