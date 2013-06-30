using System;
using System.Collections.Concurrent;
using WinDriver.Domain;
using WinDriver.Exceptions;

namespace WinDriver.Repository
{
    public class ElementRepository : IElementRepository
    {
        private readonly ConcurrentDictionary<Guid, Element> _cache;

        public ElementRepository()
        {
            _cache = new ConcurrentDictionary<Guid, Element>();
        }

        public Guid AddByHandle(int handle)
        {
            var element = new Element(handle);
            return Add(element);
        }

        public Guid Add(Element element)
        {
            var key = Guid.NewGuid();
            _cache.AddOrUpdate(key, element, (x, oldValue) => element);
            return key;
        }

        public Element GetById(Guid id)
        {
            Element element;
            if (!_cache.TryGetValue(id, out element))
            {
                throw new VariableResourceNotFoundException();
            }

            return element;
        }
    }
}