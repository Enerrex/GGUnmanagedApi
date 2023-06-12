using UnmanagedAPI;
using UnmanagedCore.Containers;

namespace UnmanagedCore.Debug.Proxies
{
    internal sealed unsafe class PointerArrayProxy<TUnmanaged> where TUnmanaged : unmanaged
    {
        private PointerArray<TUnmanaged> _proxiedArray;
        
        public int Length => _proxiedArray.Length;
        public TUnmanaged* Reference => _proxiedArray.Reference.Pointer;

        public PointerArrayProxy(PointerArray<TUnmanaged> proxiedArray)
        {
            _proxiedArray = proxiedArray;
        }

        public TUnmanaged[] Items
        {
            get
            {
                var result = new TUnmanaged[_proxiedArray.Length];
                for (int item_ix = 0; item_ix < _proxiedArray.Length; item_ix++)
                {
                    result[item_ix] = _proxiedArray[item_ix];
                }
                return result;
            }
        }
    }
}