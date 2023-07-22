using System;
using System.Collections.Generic;
using System.Reflection;
using dh_stats.Modifiers;
using dh_stats.ScriptableObjects.StatData;
using NUnit.Framework;
using UnityEngine;

namespace dh_stats.Tests.Editor
{
    public class BoolStatTest : StatsTest<bool>
    {
        private BoolStatData _baseData;

        [SetUp]
        public void SetUp()
        {
            _baseData = ScriptableObject.CreateInstance<BoolStatData>();
            FieldInfo idFieldInfo =
                    typeof(StatData<bool>).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
            if (idFieldInfo != null)
            {
                // Use the FieldInfo to set the value of the private field
                idFieldInfo.SetValue(_baseData, "exampleId");
            }

            FieldInfo baseValueFieldInfo =
                    typeof(StatData<bool>).GetField("baseValue", BindingFlags.NonPublic | BindingFlags.Instance);
            if (baseValueFieldInfo != null)
            {
                // Use the FieldInfo to set the value of the private field
                baseValueFieldInfo.SetValue(_baseData, false);
            }

            Stat = new Stat<bool>(_baseData);
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

        public List<Modifier<bool>> PopulateModifierList()
        {
            List<Modifier<bool>> modifiers = new List<Modifier<bool>>
            {
                    new BoolModifier<bool>(true, Operation.Set, "exampleId", order: 1),
                    new BoolModifier<bool>(false, Operation.Set, "exampleId", order: 5),
                    new BoolModifier<bool>(false, Operation.Set, "exampleId", order: 10),
            };

            Stat.AddModifiersBatch(modifiers, OrderMode.Custom);

            return modifiers;
        }

        [Test]
        public void AddFirstModifierAdd()
        {
            BoolModifier<bool> boolModifier = new BoolModifier<bool>(true, Operation.Add, "exampleId");
            Assert.Throws<NotSupportedException>(() => Stat.AddModifier(boolModifier, OrderMode.Custom));
        }

        [Test]
        public void AddFirstModifierMultiply()
        {
            BoolModifier<bool> boolModifier = new BoolModifier<bool>(true, Operation.Multiply, "exampleId");
            Assert.Throws<NotSupportedException>(() => Stat.AddModifier(boolModifier, OrderMode.Custom));
        }

        [Test]
        public override void AddFirstModifier()
        {
            BoolModifier<bool> boolModifier = new BoolModifier<bool>(true, Operation.Set, "exampleId");
            Stat.AddModifier(boolModifier, OrderMode.Custom);
            Assert.IsTrue(Stat.CurrentValue);
        }

        [Test]
        public override void PrependNthModifier()
        {
            const bool expectedResult = false;

            PopulateModifierList();
            Modifier<bool> modifier = new BoolModifier<bool>(true, Operation.Set, "exampleId");
            Stat.AddModifier(modifier, OrderMode.Prepend);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.AreEqual(expectedResult, Stat.CurrentValue);
        }

        [Test]
        public override void AppendNthModifier()
        {
            const bool expectedResult = true;

            PopulateModifierList();
            Modifier<bool> modifier = new BoolModifier<bool>(true, Operation.Set, "exampleId");
            Stat.AddModifier(modifier, OrderMode.Append);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.AreEqual(expectedResult, Stat.CurrentValue);
        }

        [Test]
        public override void InsertNthModifier()
        {
            const bool expectedResult = false;

            PopulateModifierList();
            Modifier<bool> modifier = new BoolModifier<bool>(true, Operation.Set, "exampleId", order: 3);
            Stat.AddModifier(modifier, OrderMode.Custom);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.AreEqual(expectedResult, Stat.CurrentValue);
        }

        [Test]
        public override void RemoveNthModifier()
        {
            const bool expectedResult = false;

            var modifiers = PopulateModifierList();
            Modifier<bool> modifierToDelete = modifiers[2];
            Stat.RemoveModifier(modifierToDelete);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.AreEqual(expectedResult, Stat.CurrentValue);
        }

        [Test]
        public override void RemoveTheOnlyModifier()
        {
            BoolModifier<bool> boolModifier = new BoolModifier<bool>(true, Operation.Set, "exampleId");
            Stat.AddModifier(boolModifier, OrderMode.Custom);
            Stat.RemoveModifier(boolModifier);
            Assert.AreEqual(Stat.BaseValue, Stat.CurrentValue);
        }

        [Test]
        public override void RemoveAllModifiers()
        {
            PopulateModifierList();
            Stat.RemoveAllModifiers();
            Assert.AreEqual(Stat.BaseValue, Stat.CurrentValue);
        }
    }
}