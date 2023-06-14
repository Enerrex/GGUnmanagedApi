#if ALLOC_DEBUG
namespace UnmanagedAPI.DebugItems
{
    public struct Demarker
    {
        public string Name;
        public int Index;

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
#endif