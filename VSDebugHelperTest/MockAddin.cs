using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;

namespace VSMemoryDump.Test.Mock {

    class MockAddin : AddIn {
        public AddIns Collection {
            get { throw new NotImplementedException(); }
        }

        public bool Connected {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public DTE DTE {
            get { throw new NotImplementedException(); }
        }

        public string Description {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string Guid {
            get { throw new NotImplementedException(); }
        }

        public string Name {
            get { throw new NotImplementedException(); }
        }

        public new dynamic Object {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string ProgID {
            get { throw new NotImplementedException(); }
        }

        public void Remove() {
            throw new NotImplementedException();
        }

        public string SatelliteDllPath {
            get { throw new NotImplementedException(); }
        }
    }
}
