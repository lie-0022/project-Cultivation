using Cultivation.Data;
using Cultivation.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace Cultivation.Tests
{
    public class FarmPlotTests
    {
        private SeedData MakeSeed(float growthTime)
        {
            var s = ScriptableObject.CreateInstance<SeedData>();
            var t = typeof(SeedData);
            t.GetField("_seedId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(s, "seed_test");
            t.GetField("_growthTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(s, growthTime);
            t.GetField("_resultCropId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(s, "crop_test");
            return s;
        }

        [Test]
        public void NewPlot_IsEmpty()
        {
            var plot = new FarmPlot();
            Assert.AreEqual(PlotState.Empty, plot.State);
            Assert.IsTrue(plot.IsEmpty);
            Assert.IsFalse(plot.IsReady);
        }

        [Test]
        public void Plant_OnEmpty_TransitionsToGrowing()
        {
            var plot = new FarmPlot();
            var seed = MakeSeed(10f);
            Assert.IsTrue(plot.Plant(seed));
            Assert.AreEqual(PlotState.Growing, plot.State);
            Assert.AreSame(seed, plot.CurrentSeed);
        }

        [Test]
        public void Plant_OnNonEmpty_ReturnsFalse()
        {
            var plot = new FarmPlot();
            plot.Plant(MakeSeed(10f));
            Assert.IsFalse(plot.Plant(MakeSeed(5f)));
        }

        [Test]
        public void Tick_BeforeGrowthTime_StaysGrowing()
        {
            var plot = new FarmPlot();
            plot.Plant(MakeSeed(10f));
            plot.Tick(5f);
            Assert.AreEqual(PlotState.Growing, plot.State);
        }

        [Test]
        public void Tick_PassGrowthTime_TransitionsToReady_ReturnsTrue()
        {
            var plot = new FarmPlot();
            plot.Plant(MakeSeed(10f));
            plot.Tick(5f);
            bool transitioned = plot.Tick(5.1f);
            Assert.IsTrue(transitioned);
            Assert.AreEqual(PlotState.Ready, plot.State);
        }

        [Test]
        public void Progress_ReflectsElapsedRatio()
        {
            var plot = new FarmPlot();
            plot.Plant(MakeSeed(10f));
            plot.Tick(2.5f);
            Assert.AreEqual(0.25f, plot.Progress, 0.001f);
        }

        [Test]
        public void Harvest_OnReady_ReturnsSeed_AndResets()
        {
            var plot = new FarmPlot();
            var seed = MakeSeed(10f);
            plot.Plant(seed);
            plot.Tick(11f);
            var harvested = plot.Harvest();
            Assert.AreSame(seed, harvested);
            Assert.IsTrue(plot.IsEmpty);
            Assert.IsNull(plot.CurrentSeed);
        }

        [Test]
        public void Harvest_OnGrowing_ReturnsNull_NoChange()
        {
            var plot = new FarmPlot();
            plot.Plant(MakeSeed(10f));
            var result = plot.Harvest();
            Assert.IsNull(result);
            Assert.AreEqual(PlotState.Growing, plot.State);
        }
    }
}
