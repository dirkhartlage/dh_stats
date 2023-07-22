using System.Collections.Generic;
using System.Reflection;
using dh_stats.Modifiers;
using dh_stats.ScriptableObjects.StatData;
using NUnit.Framework;
using UnityEngine;

namespace dh_stats.Tests.Editor
{
    public class IntStatTest : StatsTest<int>
    {
        const string targetStatId = "exampleId";
        private IntStatData _baseData;

        [SetUp]
        public void SetUp()
        {
            _baseData = ScriptableObject.CreateInstance<IntStatData>();
            FieldInfo idFieldInfo =
                    typeof(StatData<int>).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
            if (idFieldInfo != null)
            {
                // Use the FieldInfo to set the value of the private field
                idFieldInfo.SetValue(_baseData, targetStatId);
            }

            FieldInfo baseValueFieldInfo =
                    typeof(StatData<int>).GetField("baseValue", BindingFlags.NonPublic | BindingFlags.Instance);
            if (baseValueFieldInfo != null)
            {
                // Use the FieldInfo to set the value of the private field
                baseValueFieldInfo.SetValue(_baseData, 100);
            }

            Stat = new Stat<int>(_baseData);
        }


        [TearDown]
        public void TearDown()
        {
            if (_baseData != null)
            {
                // ReSharper disable once AccessToStaticMemberViaDerivedType
                ScriptableObject.DestroyImmediate(_baseData);
            }
        }

        public List<Modifier<int>> PopulateModifierList()
        {
            List<Modifier<int>> modifiers = new List<Modifier<int>>
            {
                    new IntModifier<int>(5, Operation.Add, targetStatId, order: 1),
                    new IntModifier<int>(7, Operation.Add, targetStatId, order: 5),
                    new FloatModifier<int>(1.2f, Operation.Multiply, targetStatId, order: 10)
                    // (100 + 5 + 7) * 1.2 = 134.4, rounded with default policy => 134
            };

            Stat.AddModifiersBatch(modifiers, OrderMode.Custom);

            return modifiers;
        }

        [Test]
        public override void AddFirstModifier()
        {
            const int expectedResult = 122;
            IntModifier<int> modifier = new IntModifier<int>(22, Operation.Add, targetStatId);
            Stat.AddModifier(modifier, OrderMode.Custom);
            Assert.AreEqual(expectedResult, Stat.CurrentValue);
        }

        [Test]
        public override void PrependNthModifier()
        {
            // (100 + 5 + 7) * 1.2 = 134.4, rounded with default policy => 134
            const int expectedResult1 = 134;
            // (80 + 5 + 7) * 1.2 = 110.399, rounded with default policy => 110
            const int expectedResult2 = 110;

            PopulateModifierList();
            Assert.AreEqual(expectedResult1, Stat.CurrentValue);

            Modifier<int> modifier = new IntModifier<int>(80, Operation.Set, targetStatId);
            Stat.AddModifier(modifier, OrderMode.Prepend);

            Assert.AreEqual(expectedResult2, Stat.CurrentValue);
        }

        public override void AppendNthModifier()
        {
            throw new System.NotImplementedException();
        }

        public override void InsertNthModifier()
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveNthModifier()
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveTheOnlyModifier()
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveAllModifiers()
        {
            throw new System.NotImplementedException();
        }
    }
}