// ReSharper disable once CheckNamespace

namespace dh_stats.Tests.Editor
{
    public abstract class StatsTest<T> where T : struct
    {
        protected Stat<T> Stat;
        public abstract void AddFirstModifier();
        public abstract void PrependNthModifier();
        public abstract void AppendNthModifier();
        public abstract void InsertNthModifier();
        public abstract void RemoveNthModifier();
        public abstract void RemoveTheOnlyModifier();
        public abstract void RemoveAllModifiers();

        // TODO: BatchAdd and BatchRemove
    }
}