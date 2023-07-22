using System;
using System.Collections.Generic;
using System.Text;
using dh_stats.Modifiers;
using dh_stats.ScriptableObjects.StatData;

namespace dh_stats
{
    public enum OrderMode
    {
        Custom,
        Append,
        Prepend,
    }

    public class Stat<TValue> where TValue : struct
    {
        public event Action<TValue> OnCurrentValueChanged;
        public readonly string Id;
        public string DisplayName => BaseData.DisplayName;
        public int Order => BaseData.Order;

        public TValue BaseValue
        {
            get => _baseValue;
            set
            {
                _baseValue = value;
                UpdateAllFrom(Modifiers.First);
            }
        }

        public TValue CurrentValue
        {
            get => _currentValue;
            private set
            {
                _currentValue = value;
                OnCurrentValueChanged?.Invoke(value);
            }
        }

        public readonly LinkedList<Modifier<TValue>> Modifiers = new LinkedList<Modifier<TValue>>();

        public readonly Dictionary<string, LinkedListNode<Modifier<TValue>>> ModifiersById =
                new Dictionary<string, LinkedListNode<Modifier<TValue>>>(); // Modifier.id -> Node

        private TValue _baseValue;
        private TValue _currentValue;
        protected readonly StatData<TValue> BaseData;

        public Stat(StatData<TValue> baseData, in IEnumerable<Modifier<TValue>> initialModifiers = null)
        {
            BaseData = baseData;
            Id = baseData.Id;

            _baseValue = baseData.BaseValue;

            if (initialModifiers != null)
            {
                AddModifiersBatch(initialModifiers, OrderMode.Append);

                for (var node = Modifiers.First; node.Next != null; node = node.Next)
                    ModifiersById.Add(node.Value.Id, node);
            }
            else
                CurrentValue = _baseValue;
        }
        
        /// <returns>Weather the Added modifier is new. (Otherwise it's just an additional stack or an extension of the duration)</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool AddModifier(Modifier<TValue> modifier, OrderMode orderMode = default)
        {
            if (ModifiersById.TryGetValue(modifier.Id, out LinkedListNode<Modifier<TValue>> existingModifier))
            {
                if (modifier.IsStackable && existingModifier.Value.StackAmount < modifier.MaxStackAmount)
                {
                    existingModifier.Value.StackAmount++;
                    var previousNode = existingModifier.Previous;
                    if (previousNode == null) // is first node? 
                        existingModifier.Value.Apply(_baseValue);
                    else
                        existingModifier.Value.Apply(previousNode.Value.CachedValue);
                    UpdateAllFrom(existingModifier);
                }

                if (existingModifier.Value.Expiry < modifier.Expiry)
                    existingModifier.Value.Expiry = modifier.Expiry;
                
                return false;
            }

            LinkedListNode<Modifier<TValue>> newNode = null;
            if (Modifiers.Count == 0)
            {
                newNode = Modifiers.AddFirst(modifier);
                CurrentValue = newNode.Value.Apply(_baseValue);
            }
            else
            {
                LinkedListNode<Modifier<TValue>> previousNode;
                switch (orderMode)
                {
                    case OrderMode.Prepend:
                        modifier.Order = Modifiers.First.Value.Order;
                        newNode = Modifiers.AddFirst(modifier);
                        modifier.Apply(_baseValue);
                        UpdateAllFrom(newNode);
                        CurrentValue = Modifiers.Last.Value.CachedValue;
                        break;
                    case OrderMode.Append:
                        previousNode = Modifiers.Last;
                        modifier.Order = previousNode.Value.Order;
                        newNode = Modifiers.AddAfter(previousNode, modifier);
                        CurrentValue = modifier.Apply(previousNode.Value.CachedValue);
                        break;
                    case OrderMode.Custom:
                        // find
                        var node = Modifiers.First;
                        bool found = false;
                        while (node != null)
                        {
                            if (modifier.Order < node.Value.Order)
                            {
                                // insert when spot found
                                newNode = Modifiers.AddBefore(node, modifier);
                                previousNode = newNode.Previous;
                                if (previousNode == null) // is first node? 
                                    newNode.Value.Apply(_baseValue);
                                else
                                    newNode.Value.Apply(previousNode.Value.CachedValue);
                                UpdateAllFrom(newNode);
                                found = true;
                                break;
                            }

                            node = node.Next;
                        }

                        if (!found)
                        {
                            previousNode = Modifiers.Last;
                            newNode = Modifiers.AddLast(modifier);
                            CurrentValue = modifier.Apply(previousNode.Value.CachedValue);
                            // UpdateFrom not needed since it's at the end
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(orderMode), orderMode, null);
                }
            }

            ModifiersById.Add(modifier.Id, newNode);
            return true;
        }

        public void RemoveAllModifiers()
        {
            foreach (var modifier in Modifiers)
                modifier.Dispose();
            Modifiers.Clear();
            ModifiersById.Clear();
            CurrentValue = _baseValue;
        }

        public void RemoveModifier(string identifier, bool wholeStack = false)
        {
            LinkedListNode<Modifier<TValue>> linkedListNode = ModifiersById[identifier];
            if (!wholeStack && linkedListNode.Value.StackAmount > 1)
            {
                linkedListNode.Value.StackAmount--;
                UpdateAllFrom(linkedListNode);
                return;
            }

            linkedListNode.Value.Dispose();
            if (linkedListNode.Previous != null)
                UpdateAllFrom(linkedListNode.Previous);
            else if (Modifiers.Count != 0)
            {
                Modifiers.First.Value.Apply(_baseValue);
                UpdateAllFrom(Modifiers.First);
            }
            else
                CurrentValue = _baseValue;
            Modifiers.Remove(linkedListNode);
            ModifiersById.Remove(identifier);
        }

        public void RemoveModifier(Modifier<TValue> modifier, bool wholeStack = false)
        {
            if (!wholeStack && modifier.StackAmount > 1)
            {
                modifier.StackAmount--;
                UpdateAllFrom(ModifiersById[modifier.Id]);
                return;
            }

            modifier.Dispose();
            LinkedListNode<Modifier<TValue>> nodeToDelete = ModifiersById[modifier.Id];
            LinkedListNode<Modifier<TValue>> previous = nodeToDelete.Previous;
            Modifiers.Remove(nodeToDelete);
            if (previous != null)
                UpdateAllFrom(previous);
            else if (Modifiers.Count != 0)
            {
                Modifiers.First.Value.Apply(_baseValue);
                UpdateAllFrom(Modifiers.First);
            }
            else
                CurrentValue = _baseValue;

            ModifiersById.Remove(modifier.Id);
            Modifiers.Remove(modifier);
        }

        public void RemoveModifiersBatch(IEnumerable<string> identifiers)
        {
            foreach (string identifier in identifiers)
                RemoveModifier(identifier);
        }

        public void RemoveModifiersBatch(IEnumerable<Modifier<TValue>> modifiers)
        {
            foreach (Modifier<TValue> modifier in modifiers)
                RemoveModifier(modifier);
        }

        /// <remarks>Starts from node.CachedValue onwards (which will not be recalculated anymore).</remarks>
        private void UpdateAllFrom(LinkedListNode<Modifier<TValue>> node)
        {
            TValue value = node.Value.CachedValue;
            node = node.Next;
            while (node != null)
            {
                value = node.Value.Apply(value);
                node = node.Next;
            }

            CurrentValue = value;
        }

        public void AddModifiersBatch(IEnumerable<Modifier<TValue>> sortedModifiers, OrderMode orderMode)
        {
            // TODO: optimization - this can all be done in O(n+m) instead of O(n*m) where n is sortedModifiers and m is the existing modifiers
            foreach (Modifier<TValue> sortedModifier in sortedModifiers)
                AddModifier(sortedModifier, orderMode);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Stat<TValue>));
            sb.Append(", type: " + typeof(TValue));
            sb.Append(", baseValue: " + _baseValue);
            sb.Append(", modifiers: \n");

            foreach (Modifier<TValue> modifier in Modifiers)
                sb.Append(modifier + "\n");

            return sb.ToString();
        }
    }
}