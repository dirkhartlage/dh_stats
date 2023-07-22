using System.Collections.Generic;
using System.Reflection;
using dh_stats.Modifiers;
using dh_stats.ScriptableObjects.StatData;
using NUnit.Framework;
using UnityEngine;

namespace dh_stats.Tests.Editor
{
    public class FloatStatTest : StatsTest<float>
    {
        private FloatStatData _baseData;

        [SetUp]
        public void SetUp()
        {
            _baseData = ScriptableObject.CreateInstance<FloatStatData>();
            FieldInfo idFieldInfo =
                    typeof(StatData<float>).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
            if (idFieldInfo != null)
            {
                // Use the FieldInfo to set the value of the private field
                idFieldInfo.SetValue(_baseData, "exampleId");
            }

            FieldInfo baseValueFieldInfo =
                    typeof(StatData<float>).GetField("baseValue", BindingFlags.NonPublic | BindingFlags.Instance);
            if (baseValueFieldInfo != null)
            {
                // Use the FieldInfo to set the value of the private field
                baseValueFieldInfo.SetValue(_baseData, 95f);
            }

            Stat = new Stat<float>(_baseData);
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

        public Modifier<float>[] PopulateModifierList()
        {
            Modifier<float>[] modifiers =
            {
                    new IntModifier<float>(5, Operation.Add, "exampleId", order: 1),
                    new FloatModifier<float>(20.0f, Operation.Add, "exampleId", order: 5),
                    new FloatModifier<float>(1.2f, Operation.Multiply, "exampleId", order: 10),
                    // (95 + 5 + 20) * 1.2 = 144.0
            };

            Stat.AddModifiersBatch(modifiers, OrderMode.Custom);

            const float expectedResult2 = 144f;
            Assert.AreEqual(expectedResult2, Stat.CurrentValue);

            return modifiers;
        }

        [Test]
        public override void AddFirstModifier()
        {
            const float expectedResult = 117f;
            IntModifier<float> modifier = new IntModifier<float>(22, Operation.Add, "exampleId");
            Stat.AddModifier(modifier, OrderMode.Custom);
            Assert.AreEqual(expectedResult, Stat.CurrentValue, Mathf.Epsilon);
        }

        [Test]
        public override void PrependNthModifier()
        {
            const float expectedResult = 84f;

            PopulateModifierList();
            Modifier<float> modifier = new FloatModifier<float>(45f, Operation.Set, "exampleId");
            Stat.AddModifier(modifier, OrderMode.Prepend);
            // (45 + 5 + 20) * 1.2 = 84.0

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.AreEqual(expectedResult, Stat.CurrentValue, Mathf.Epsilon);
        }

        [Test]
        public override void AppendNthModifier()
        {
            const float expectedResult = 45f;

            PopulateModifierList();
            Modifier<float> modifier = new FloatModifier<float>(45f, Operation.Set, "exampleId");
            Stat.AddModifier(modifier, OrderMode.Append);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.AreEqual(expectedResult, Stat.CurrentValue, Mathf.Epsilon);
        }

        [Test]
        public override void InsertNthModifier()
        {
            const float expectedResult = 60f;

            PopulateModifierList();

            Modifier<float> modifier = new FloatModifier<float>(50f, Operation.Set, "exampleId", order: 6);
            Stat.AddModifier(modifier, OrderMode.Custom);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.AreEqual(expectedResult, Stat.CurrentValue, 0.00001f);
        }

        [Test]
        public override void RemoveNthModifier()
        {
            const float expectedResult1 = 120f;

            var modifierList = PopulateModifierList();
            Stat.RemoveModifier(modifierList[1]);
            
            Assert.AreEqual(expectedResult1, Stat.CurrentValue, 0.00001f);
        }

        public override void RemoveTheOnlyModifier()
        {
            throw new System.NotImplementedException();
        }

        [Test]
        public override void RemoveAllModifiers()
        {
            PopulateModifierList();
            Stat.RemoveAllModifiers();
            Assert.AreEqual(_baseData.BaseValue, Stat.CurrentValue);
        }
    }
}