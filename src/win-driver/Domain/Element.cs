using System;
using System.Windows.Automation;
using White.Core.UIItems.Actions;

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

        public virtual AutomationElement GetAutomationElement(ActionListener actionListener)
        {
            return AutomationElement.FromHandle(new IntPtr(_handle));
        }
    }
}