using System;
using WinDriver.Domain;

namespace WinDriver.Repository
{
    public interface IElementRepository
    {
        Guid AddByHandle(int handle);

        Guid Add(Element element);

        Element GetById(Guid id);
    }
}