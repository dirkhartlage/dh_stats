using dh_stats.ScriptableObjects.ModifierData;

namespace dh_stats
{
    public static class ModifierUtil
    {
        public static ModifierData<T> Load<T>(string name) where T : struct
        {
            return null; //TODO:
        }
        

        public static ModifierData<T> GetModifierData<T>(string id) where T : struct
        {
            throw new System.NotImplementedException();
        }
    }
}