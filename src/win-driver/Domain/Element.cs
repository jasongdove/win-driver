using System;
using System.Windows.Automation;
using interop.UIAutomationCore;
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

        public virtual IUIAutomationElement GetUIAutomationElement(IUIAutomation automation)
        {
            return automation.ElementFromHandle(new IntPtr(_handle));
        }
    }
}