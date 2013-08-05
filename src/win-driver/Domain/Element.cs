using System;
using interop.UIAutomationCore;

namespace WinDriver.Domain
{
    public class Element
    {
        private readonly int _handle;

        public Element(int handle)
        {
            _handle = handle;
        }

        protected Element()
        {
        }

        public virtual IUIAutomationElement GetUIAutomationElement(IUIAutomation automation)
        {
            return automation.ElementFromHandle(new IntPtr(_handle));
        }
    }
}