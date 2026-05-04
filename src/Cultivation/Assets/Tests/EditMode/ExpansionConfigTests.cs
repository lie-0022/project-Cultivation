using Cultivation.Data;
using NUnit.Framework;
using UnityEngine;

namespace Cultivation.Tests
{
    public class ExpansionConfigTests
    {
        private ExpansionConfig MakeConfig(int farmBase = 100, int barnBase = 200, float multiplier = 1.5f)
        {
            var cfg = ScriptableObject.CreateInstance<ExpansionConfig>();
            // SerializeField는 SerializedObject로 설정해야 하지만 Editor 외에서는 reflection 사용
            var t = typeof(ExpansionConfig);
            t.GetField("_farmBaseCost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cfg, farmBase);
            t.GetField("_barnBaseCost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cfg, barnBase);
            t.GetField("_costMultiplier", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cfg, multiplier);
            return cfg;
        }

        [Test]
        public void FarmCost_2Slots_ExpectedValue()
        {
            // 100 * 2^1.5 ≈ 282.84 → RoundToInt = 283
            var cfg = MakeConfig();
            Assert.AreEqual(283, cfg.CalculateCost(cfg.FarmBaseCost, 2));
        }

        [Test]
        public void FarmCost_3Slots_ExpectedValue()
        {
            // 100 * 3^1.5 ≈ 519.62 → 520
            var cfg = MakeConfig();
            Assert.AreEqual(520, cfg.CalculateCost(cfg.FarmBaseCost, 3));
        }

        [Test]
        public void FarmCost_4Slots_ExpectedValue()
        {
            // 100 * 4^1.5 = 800 → 800
            var cfg = MakeConfig();
            Assert.AreEqual(800, cfg.CalculateCost(cfg.FarmBaseCost, 4));
        }

        [Test]
        public void BarnCost_2Slots_ExpectedValue()
        {
            // 200 * 2^1.5 ≈ 565.69 → 566
            var cfg = MakeConfig();
            Assert.AreEqual(566, cfg.CalculateCost(cfg.BarnBaseCost, 2));
        }

        [Test]
        public void BarnCost_5Slots_ExpectedValue()
        {
            // 200 * 5^1.5 ≈ 2236.07 → 2236
            var cfg = MakeConfig();
            Assert.AreEqual(2236, cfg.CalculateCost(cfg.BarnBaseCost, 5));
        }

        [Test]
        public void BarnCost_10Slots_ExpectedValue()
        {
            // 200 * 10^1.5 ≈ 6324.55 → 6325
            var cfg = MakeConfig();
            Assert.AreEqual(6325, cfg.CalculateCost(cfg.BarnBaseCost, 10));
        }
    }
}
