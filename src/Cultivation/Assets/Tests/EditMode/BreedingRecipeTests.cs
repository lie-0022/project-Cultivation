using Cultivation.Data;
using NUnit.Framework;
using UnityEngine;

namespace Cultivation.Tests
{
    public class BreedingRecipeTests
    {
        private BreedingRecipe MakeRecipe(string a, string b, string result, float time = 60f)
        {
            var r = ScriptableObject.CreateInstance<BreedingRecipe>();
            var t = typeof(BreedingRecipe);
            t.GetField("_parentAId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(r, a);
            t.GetField("_parentBId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(r, b);
            t.GetField("_resultCreatureId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(r, result);
            t.GetField("_breedingTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(r, time);
            return r;
        }

        [Test]
        public void Matches_ExactOrder_Returns_True()
        {
            var r = MakeRecipe("A", "B", "AB");
            Assert.IsTrue(r.Matches("A", "B"));
        }

        [Test]
        public void Matches_ReversedOrder_Returns_True()
        {
            var r = MakeRecipe("A", "B", "AB");
            Assert.IsTrue(r.Matches("B", "A"));
        }

        [Test]
        public void Matches_DifferentParents_Returns_False()
        {
            var r = MakeRecipe("A", "B", "AB");
            Assert.IsFalse(r.Matches("A", "C"));
            Assert.IsFalse(r.Matches("X", "Y"));
        }

        [Test]
        public void Matches_SameSpeciesRecipe_OnlyMatchesSamePair()
        {
            var r = MakeRecipe("Tomato", "Tomato", "Ketchup");
            Assert.IsTrue(r.Matches("Tomato", "Tomato"));
            Assert.IsFalse(r.Matches("Tomato", "Carrot"));
        }
    }
}
