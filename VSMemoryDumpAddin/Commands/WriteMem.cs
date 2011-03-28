using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace VSMemoryDumpAddin.Commands {
    class WriteMem : ICommand {
        #region members
        private DTE2 _application;
        private AddIn _addin;
        #endregion

        #region ICommand Members

        void ICommand.Initialize(EnvDTE80.DTE2 application, EnvDTE.AddIn addin) {
            _application = application;
            _addin = addin;
        }

        string ICommand.CommandText {
            get { return "writemem"; }
        }

        void ICommand.Exec(string cmdName
            , vsCommandExecOption executeOption
            , ref object variantIn
            , ref object variantOut
            , ref bool handled
        ) {

            switch (executeOption) {
            case vsCommandExecOption.vsCommandExecOptionDoDefault: {
                string fileName = @"c:\temp\test.txt";
                using (FileStream fs = new FileStream(fileName, FileMode.Create)) {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, "data");
                }

                var commandWindow = _application.Windows.Item(EnvDTE.Constants.vsWindowKindCommandWindow).Object as CommandWindow;

                commandWindow.OutputString("SavedBPFormat\r\n");
            } break;
            }
        }

        void ICommand.QueryStatus(string cmdName
            , vsCommandStatusTextWanted neededText
            , ref vsCommandStatus statusOption
            , ref object commandText
        ) {
            switch (neededText) {
            case vsCommandStatusTextWanted.vsCommandStatusTextWantedNone: {
                if (_application.Debugger.DebuggedProcesses.Count > 0) {
                    statusOption = vsCommandStatus.vsCommandStatusSupported
                        | vsCommandStatus.vsCommandStatusEnabled;
                }
            } break;
            }

        }

        #endregion
    }
}
