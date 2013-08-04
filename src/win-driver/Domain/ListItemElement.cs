﻿using System.Windows.Automation;
using interop.UIAutomationCore;
using White.Core.UIItems.Actions;
using White.Core.UIItems.ListBoxItems;

namespace WinDriver.Domain
{
    public class ListItemElement : Element
    {
        private readonly Element _listElement;
        private readonly int _index;

        public ListItemElement(Element listElement, int index)
        {
            _listElement = listElement;
            _index = index;
        }

        public override AutomationElement GetAutomationElement(ActionListener actionListener)
        {
            var list = new ListControl(_listElement.GetAutomationElement(actionListener), actionListener);
            return list.Items[_index].AutomationElement;
        }

        public override IUIAutomationElement GetUIAutomationElement(IUIAutomation automation)
        {
            var listElement = _listElement.GetUIAutomationElement(automation);
         
            var listItems = listElement.FindAll(
                interop.UIAutomationCore.TreeScope.TreeScope_Descendants,
                automation.CreatePropertyCondition(
                    UIA_PropertyIds.UIA_ControlTypePropertyId,
                    UIA_ControlTypeIds.UIA_ListItemControlTypeId));
            
            return listItems.GetElement(_index);
        }
    }
}