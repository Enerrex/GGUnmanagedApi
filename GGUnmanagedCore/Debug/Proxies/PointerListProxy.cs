using UnmanagedAPI;
using UnmanagedCore.Containers;

namespace UnmanagedCore.Debug.Proxies
{
    internal sealed unsafe class PointerListProxy<TUnmanaged> where TUnmanaged : unmanaged
    {
        private PointerList<TUnmanaged> _proxiedList;
        
        public int Capacity => _proxiedList.Capacity;
        public int Count => _proxiedList.Count;
        public TUnmanaged* Reference => _proxiedList.Reference.Pointer;

        public PointerListProxy(PointerList<TUnmanaged> proxiedList)
        {
            _proxiedList = proxiedList;
        }

        public TUnmanaged[] Items
        {
            get
            {
                var result = new TUnmanaged[_proxiedList.Count];
                for (int item_ix = 0; item_ix < _proxiedList.Count; item_ix++)
                {
                    result[item_ix] = _proxiedList[item_ix];
                }
                return result;
            }
        }
    }
}