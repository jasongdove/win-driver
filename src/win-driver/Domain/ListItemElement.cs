using System.Windows.Automation;
using White.Core.UIItems.Actions;
using White.Core.UIItems.ListBoxItems;

namespace WinDriver.Domain
{
    public class ListItemElement : Element
    {
        private readonly Element _comboBoxElement;
        private readonly int _index;

        public ListItemElement(Element comboBoxElement, int index)
        {
            _comboBoxElement = comboBoxElement;
            _index = index;
        }

        public override AutomationElement GetAutomationElement(ActionListener actionListener)
        {
            var comboBox = new ComboBox(_comboBoxElement.GetAutomationElement(actionListener), actionListener);
            return comboBox.Items[_index].AutomationElement;
        }
    }
}