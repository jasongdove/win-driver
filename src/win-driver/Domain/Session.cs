using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using White.Core;
using White.Core.Factory;
using White.Core.InputDevices;
using White.Core.UIItems;
using White.Core.UIItems.Finders;
using White.Core.UIItems.ListBoxItems;
using White.Core.WindowsAPI;
using WinDriver.Exceptions;
using WinDriver.Repository;

namespace WinDriver.Domain
{
    public sealed class Session : IDisposable
    {
        private readonly Capabilities _capabilities;
        private readonly Guid _sessionId;
        private readonly IElementRepository _elementRepository;

        private readonly Application _application;

        public Session(Capabilities capabilities)
        {
            _capabilities = capabilities;
            _sessionId = Guid.NewGuid();

            // TODO: inject this somehow
            _elementRepository = new ElementRepository();

            if (_capabilities.App != null)
            {
                _application = Application.Launch(Capabilities.App);

                while (_application.GetWindows().Count == 0)
                {
                    Thread.Sleep(500);
                }

                _application.WaitWhileBusy();
            }
        }

        public Capabilities Capabilities
        {
            get { return _capabilities; }
        }

        public Guid SessionId
        {
            get { return _sessionId; }
        }

        public string Title
        {
            get
            {
                var window = _application.GetWindows().FirstOrDefault(x => x.IsCurrentlyActive);
                return window != null ? window.Title : _application.Process.MainWindowTitle;
            }
        }

        public IEnumerable<int> GetWindowHandles()
        {
            return _application.GetWindows().Select(x => x.AutomationElement.Current.NativeWindowHandle);
        }

        public int GetWindowHandle()
        {
            var window = _application.GetWindows().FirstOrDefault(x => x.IsCurrentlyActive);
            return window != null ? window.AutomationElement.Current.NativeWindowHandle : (int)_application.Process.MainWindowHandle;
        }

        public bool SwitchToWindow(string title)
        {
            try
            {
                var window = _application.GetWindow(title, InitializeOption.NoCache);
                return NativeMethods.SetForegroundWindow(new IntPtr(window.AutomationElement.Current.NativeWindowHandle));
            }
            catch (UIActionException)
            {
                return false;
            }
        }

        public Guid FindElement(string locator, string value)
        {
            var window = _application.GetWindow(Title);

            var criteria = new List<SearchCriteria>();

            switch (locator.ToLowerInvariant())
            {
                case "name":
                    criteria.Add(SearchCriteria.ByText(value));
                    criteria.Add(SearchCriteria.ByAutomationId(value));
                    break;
                case "id":
                    criteria.Add(SearchCriteria.ByAutomationId(value));
                    break;
                default:
                    throw new VariableResourceNotFoundException(); // TODO: should this be method not supported?
            }

            foreach (var searchCriteria in criteria)
            {
                var element = window.GetElement(searchCriteria);
                if (element != null)
                {
                    return _elementRepository.Add(element.Current.NativeWindowHandle);
                }
            }

            throw new VariableResourceNotFoundException();
        }

        public IEnumerable<Guid> FindElements(string locator, string value, Guid? elementId)
        {
            var window = _application.GetWindow(Title);
            var containers = new List<IUIItemContainer> { window };

            var criteria = new List<SearchCriteria>();
            switch (locator.ToLowerInvariant())
            {
                case "name":
                    criteria.Add(SearchCriteria.ByText(value));
                    criteria.Add(SearchCriteria.ByAutomationId(value));
                    break;
                case "tag name":
                    var controlType = MapControlType(value);
                    if (controlType.Id == ControlType.ListItem.Id && elementId.HasValue)
                    {
                        var elementHandle = _elementRepository.GetById(elementId.Value);
                        var element = AutomationElement.FromHandle(new IntPtr(elementHandle));
                        if (element.Current.ControlType.Id == ControlType.ComboBox.Id)
                        {
                            // this is the one combo box case that's "easy" to support
                            // if we're looking for list items, and given a combo box
                            // pop open the combo box, return all items, and leave the
                            // combo box open so the handles don't become invalid
                            var comboBox = new ComboBox(element, window);
                            comboBox.Click(); // open()?

                            var handles = comboBox.Items.Select(x => x.AutomationElement.Current.NativeWindowHandle);
                            return handles.Select(x => _elementRepository.Add(x));
                        }
                    }
                    criteria.Add(SearchCriteria.ByControlType(controlType));
                    break;
                case "id":
                    criteria.Add(SearchCriteria.ByAutomationId(value));
                    break;
                default:
                    throw new VariableResourceNotFoundException(); // TODO: should this be method not supported?
            }

            var allElementIds = new List<Guid>();
            foreach (var container in containers)
            {
                foreach (var searchCriteria in criteria)
                {
                    allElementIds.AddRange(FindElements(container, searchCriteria));
                }
            }

            if (allElementIds.Any())
            {
                return allElementIds;
            }

            throw new VariableResourceNotFoundException();
        }

        public void SendKeys(Guid elementId, char[] keys)
        {
            // TODO: support modifier keys

            var elementHandle = _elementRepository.GetById(elementId);
            var window = _application.GetWindow(Title);
            var element = AutomationElement.FromHandle(new IntPtr(elementHandle));
            if (element == null)
            {
                throw new VariableResourceNotFoundException();
            }

            var item = new UIItem(element, window.ActionListener);
            item.Enter(new String(keys));
        }

        public void Click(Guid elementId)
        {
            var elementHandle = _elementRepository.GetById(elementId);
            var window = _application.GetWindow(Title);
            var element = AutomationElement.FromHandle(new IntPtr(elementHandle));
            if (element == null)
            {
                throw new VariableResourceNotFoundException();
            }

            var item = new UIItem(element, window.ActionListener);
            item.Click();
        }

        public string GetElementName(Guid elementId)
        {
            var elementHandle = _elementRepository.GetById(elementId);
            var element = AutomationElement.FromHandle(new IntPtr(elementHandle));
            if (element == null)
            {
                throw new VariableResourceNotFoundException(); // TODO: stale element reference
            }

            // TODO: support more control types
            var controlType = (ControlType)element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty);
            if (controlType.Id == ControlType.ComboBox.Id)
            {
                return "select";
            }

            return controlType.LocalizedControlType;
        }

        public string GetElementAttribute(Guid elementId, string attribute)
        {
            var elementHandle = _elementRepository.GetById(elementId);
            var element = AutomationElement.FromHandle(new IntPtr(elementHandle));
            if (element == null)
            {
                throw new VariableResourceNotFoundException(); // TODO: stale element reference
            }

            // TODO: support more control types/attributes
            var controlType = (ControlType)element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty);
            if (controlType.Id == ControlType.ComboBox.Id)
            {
                var selectionPattern = (SelectionPattern)element.GetCurrentPattern(SelectionPattern.Pattern);
                //var comboBox = new ComboBox(element, window);
                switch (attribute.ToLowerInvariant())
                {
                    case "multiple": // simulating attribute <select multiple="multiple">
                        return selectionPattern.Current.CanSelectMultiple ? "multiple" : null;
                }
            }

            return null;
        }

        public string GetElementText(Guid elementId)
        {
            var elementHandle = _elementRepository.GetById(elementId);
            var element = AutomationElement.FromHandle(new IntPtr(elementHandle));
            if (element == null)
            {
                throw new VariableResourceNotFoundException(); // TODO: stale element reference
            }

            // TODO: is this correct for controls other than ListItem?
            return element.Current.Name;
        }

        public void Dispose()
        {
            if (_application != null)
            {
                _application.Dispose();
            }
        }

        private IList<Guid> FindElements(IUIItemContainer container, SearchCriteria criteria)
        {
            var results = container.GetMultiple(criteria);
            if (results != null && results.Any())
            {
                var handles = results.Select(x => x.AutomationElement.Current.NativeWindowHandle);
                return handles.Select(handle => _elementRepository.Add(handle)).ToList();
            }

            return new List<Guid>();
        }

        private ControlType MapControlType(string tagName)
        {
            // TODO: support more control types/tag names
            switch (tagName.ToLowerInvariant())
            {
                case "option":
                    return ControlType.ListItem;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}