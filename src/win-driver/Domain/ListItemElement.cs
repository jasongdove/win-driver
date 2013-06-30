using System.Windows.Automation;
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
    }
}