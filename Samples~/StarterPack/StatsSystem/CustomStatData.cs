using dh_stats.ScriptableObjects.StatData;

namespace StatsSystem
{
    public abstract class CustomStatData<T> : StatData<T> where T : struct
    {
        // game specific changes and additions here
    }
}