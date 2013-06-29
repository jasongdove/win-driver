using System;
using System.Collections.Concurrent;
using WinDriver.Exceptions;

namespace WinDriver.Repository
{
    public class ElementRepository : IElementRepository
    {
        private readonly ConcurrentDictionary<Guid, int> _cache;

        public ElementRepository()
        {
            _cache = new ConcurrentDictionary<Guid, int>();
        }

        public Guid Add(int handle)
        {
            var key = Guid.NewGuid();
            _cache.AddOrUpdate(key, handle, (x, oldValue) => handle);
            return key;
        }

        public int GetById(Guid id)
        {
            int handle;
            if (!_cache.TryGetValue(id, out handle))
            {
                throw new VariableResourceNotFoundException();
            }

            return handle;
        }
    }
}