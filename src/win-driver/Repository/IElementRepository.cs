using System;

namespace WinDriver.Repository
{
    public interface IElementRepository
    {
        Guid Add(int handle);

        int GetById(Guid id);
    }
}