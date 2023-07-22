using System.Linq;
using dh_stats;
using UnityEngine;

namespace Stats.StatUI
{
    public sealed class StatUI : MonoBehaviour
    {
        [SerializeField]
        private StatsSystemComponent statsSystemComponent;

        [SerializeField]
        private RectTransform contentTransform;

        [SerializeField]
        private GameObject statUIElementPrefab;

        private void Start()
        {
            SpawnStatElementsInOrder();
        }

        private void SpawnStatElementsInOrder()
        {
            var enumerator1 = statsSystemComponent.AvailableBoolStats.Values.OrderBy(x => x.Order).GetEnumerator();
            var enumerator2 = statsSystemComponent.AvailableFloatStats.Values.OrderBy(x => x.Order).GetEnumerator();
            var enumerator3 = statsSystemComponent.AvailableIntStats.Values.OrderBy(x => x.Order).GetEnumerator();
            bool hasNext1 = enumerator1.MoveNext();
            bool hasNext2 = enumerator2.MoveNext();
            bool hasNext3 = enumerator3.MoveNext();

            while (hasNext1 || hasNext2 || hasNext3)
            {
                int smallestEnumeratorId = 0;
                int smallestOrder = int.MaxValue;

                if (hasNext1)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    smallestOrder = enumerator1.Current.Order;
                    smallestEnumeratorId = 1;
                }

                // ReSharper disable once PossibleNullReferenceException
                if (hasNext2 && enumerator2.Current.Order < smallestOrder)
                {
                    smallestOrder = enumerator2.Current.Order;
                    smallestEnumeratorId = 2;
                }

                // ReSharper disable once PossibleNullReferenceException
                if (hasNext3 && enumerator3.Current.Order < smallestOrder)
                {
                    smallestEnumeratorId = 3;
                }

                switch (smallestEnumeratorId)
                {
                    case 1:
                        SpawnStatUIElement(enumerator1.Current);
                        hasNext1 = enumerator1.MoveNext();
                        break;
                    case 2:
                        SpawnStatUIElement(enumerator2.Current);
                        hasNext2 = enumerator2.MoveNext();
                        break;
                    case 3:
                        SpawnStatUIElement(enumerator3.Current);
                        hasNext3 = enumerator3.MoveNext();
                        break;
                }
            }

            enumerator1.Dispose();
            enumerator2.Dispose();
            enumerator3.Dispose();
        }

        // If u ever need dynamic stat addition, just make events on your stat component and listen here
        // private void AddBoolStat(Stat<bool> obj) => SpawnStatUIElement(obj);
        //
        // private void AddFloatStat(Stat<float> obj) => SpawnStatUIElement(obj);
        //
        // private void AddIntStat(Stat<int> obj) => SpawnStatUIElement(obj);

        private void SpawnStatUIElement<T>(Stat<T> stat) where T : struct
        {
            GameObject newUIElement = Instantiate(statUIElementPrefab, contentTransform);
            StatUIElement statUIElement = newUIElement.GetComponent<StatUIElement>();
            statUIElement.Init(stat);
        }
    }
}