using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using White.Core;
using White.Core.Factory;
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

        public Guid FindElementByName(string name)
        {
            var window = _application.GetWindow(Title);

            var byText = window.GetElement(SearchCriteria.ByText(name));
            if (byText != null)
            {
                return _elementRepository.Add(byText.Current.NativeWindowHandle);
            }

            var byAutomationId = window.GetElement(SearchCriteria.ByAutomationId(name));
            if (byAutomationId != null)
            {
                return _elementRepository.Add(byAutomationId.Current.NativeWindowHandle);
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
            foreach (var key in keys)
            {
                switch (key)
                {
                    case '\ue006': // return
                    case '\ue007': // enter
                        item.KeyIn(KeyboardInput.SpecialKeys.RETURN);
                        break;
                    default:
                        item.Enter(key.ToString(CultureInfo.InvariantCulture));
                        break;
                }
            }
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

        public void Dispose()
        {
            if (_application != null)
            {
                _application.Dispose();
            }
        }
    }
}