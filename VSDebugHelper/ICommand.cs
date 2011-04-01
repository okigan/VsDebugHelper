using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;

namespace VSMemoryDump {
    interface ICommand {

        void Initialize(DTE2 application, AddIn addin);

        string CommandText {
            get;
        }


        void Exec(string cmdName
            , vsCommandExecOption executeOption
            , ref object variantIn
            , ref object variantOut
            , ref bool handled
        );

        void QueryStatus(string cmdName
            , vsCommandStatusTextWanted neededText
            , ref vsCommandStatus statusOption
            , ref object commandText
        );
    }
}
