using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;

namespace VSMemoryDump.Test.Mock {

    class MockCommands : Commands2 {
        public void Add(string Guid, int ID, ref object Control) {
            throw new NotImplementedException();
        }

        public dynamic AddCommandBar(string Name, EnvDTE.vsCommandBarType Type, [System.Runtime.InteropServices.OptionalAttribute][System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.IDispatch)][System.Runtime.CompilerServices.IDispatchConstantAttribute]object CommandBarParent, int Position = 1) {
            throw new NotImplementedException();
        }

        public EnvDTE.Command AddNamedCommand(EnvDTE.AddIn AddInInstance, string Name, string ButtonText, string Tooltip, bool MSOButton, [System.Runtime.InteropServices.OptionalAttribute][System.Runtime.InteropServices.DefaultParameterValueAttribute(0)]int Bitmap, [System.Runtime.InteropServices.OptionalAttribute]ref object[] ContextUIGUIDs, int vsCommandDisabledFlagsValue = 16) {
            throw new NotImplementedException();
        }

        public EnvDTE.Command AddNamedCommand2(EnvDTE.AddIn AddInInstance, string Name, string ButtonText, string Tooltip, bool MSOButton, [System.Runtime.InteropServices.OptionalAttribute]object Bitmap, [System.Runtime.InteropServices.OptionalAttribute]ref object[] ContextUIGUIDs, int vsCommandStatusValue = 3, int CommandStyleFlags = 3, vsCommandControlType ControlType = vsCommandControlType.vsCommandControlTypeButton) {
            MockCommand command = new MockCommand(AddInInstance, Name);

            return command;
        }

        public void CommandInfo(object CommandBarControl, out string Guid, out int ID) {
            throw new NotImplementedException();
        }

        public int Count {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.DTE DTE {
            get { throw new NotImplementedException(); }
        }

        public System.Collections.IEnumerator GetEnumerator() {
            throw new NotImplementedException();
        }

        public EnvDTE.Command Item(object index, int ID = -1) {
            throw new ArgumentException();
        }

        public EnvDTE.DTE Parent {
            get { throw new NotImplementedException(); }
        }

        public void Raise(string Guid, int ID, ref object CustomIn, ref object CustomOut) {
            throw new NotImplementedException();
        }

        public void RemoveCommandBar(object CommandBar) {
            throw new NotImplementedException();
        }

        public void UpdateCommandUI(bool PerformImmediately) {
            throw new NotImplementedException();
        }
    }
}
