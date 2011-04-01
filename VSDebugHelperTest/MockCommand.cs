using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;

namespace VSMemoryDump.Test.Mock {


    class MockCommand : Command {
        private AddIn AddInInstance;
        private string shortName;

        private MockCommand() {
        }

        public MockCommand(AddIn AddInInstance, string shortName) {
            // TODO: Complete member initialization
            this.AddInInstance = AddInInstance;
            this.shortName = shortName;
        }
        public dynamic AddControl(object Owner, int Position = 1) {
            throw new NotImplementedException();
        }

        public dynamic Bindings {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public Commands Collection {
            get { throw new NotImplementedException(); }
        }

        public DTE DTE {
            get { throw new NotImplementedException(); }
        }

        public void Delete() {
            throw new NotImplementedException();
        }

        public string Guid {
            get { throw new NotImplementedException(); }
        }

        public int ID {
            get { throw new NotImplementedException(); }
        }

        public bool IsAvailable {
            get { throw new NotImplementedException(); }
        }

        public string LocalizedName {
            get { throw new NotImplementedException(); }
        }

        public string Name {
            get { return AddInInstance.GetType().Name + "." + shortName; }
        }
    }
}
