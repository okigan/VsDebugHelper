using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
namespace VSMemoryDumpAddin {
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget {
        #region private
        private DTE2 _applicationObject;
        private AddIn _addInInstance;

        private string _memoryDumpCommandText;
        private string _memoryDumpCommandTextShort = "MemoryDump";
        #endregion

        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect() {
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom) {
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;

            _CreateCommands();

        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom) {
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        void IDTExtensibility2.OnAddInsUpdate(ref Array custom) {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnStartupComplete(ref Array custom) {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnBeginShutdown(ref Array custom) {
        }



        #region IDTCommandTarget Members

        void IDTCommandTarget.Exec(string cmdName
            , vsCommandExecOption executeOption
            , ref object variantIn
            , ref object variantOut
            , ref bool handled
        ) {
            if (0 == String.Compare(cmdName, _memoryDumpCommandText)) {
                switch (executeOption) {
                    case vsCommandExecOption.vsCommandExecOptionDoDefault: {
                            string arg = variantIn as string;
                            var x = _applicationObject.Debugger.GetExpression(arg, true, 100);
                            handled = true;
                        } break;
                }
            }
        }

        void IDTCommandTarget.QueryStatus(string cmdName
            , vsCommandStatusTextWanted neededText
            , ref vsCommandStatus statusOption
            , ref object commandText
        ) {
            statusOption = vsCommandStatus.vsCommandStatusUnsupported;

            if (this._memoryDumpCommandText == cmdName) {
                switch (neededText) {
                    case vsCommandStatusTextWanted.vsCommandStatusTextWantedNone: {
                            if (_applicationObject.Debugger.DebuggedProcesses.Count > 0) {
                                statusOption = vsCommandStatus.vsCommandStatusSupported
                                    | vsCommandStatus.vsCommandStatusEnabled;
                            }
                        } break;
                }
            }
        }


        #endregion

        #region private
        private void _CreateCommands() {
            // Because of very annoying bugs in Visual Studio, it does not 
            // properly call your AddIns with the ext_cm_UISetup on the 
            // OnConnect. Consequently, I'm stuck with nothing better than 
            // forcibly adding the commands every time this add in starts.

            // Create the commands.
            Commands2 cmds = _applicationObject.Commands as Commands2;
            object[] contextGUIDS = new object[] { };
            vsCommandStatus statusValue = vsCommandStatus.vsCommandStatusSupported
                | vsCommandStatus.vsCommandStatusEnabled;
            vsCommandStyle style = vsCommandStyle.vsCommandStylePict;

            String buttonText = "SaveSolutionButtonText";
            String toolTip = "SaveSolutionTooltipText";
            Command command = cmds.AddNamedCommand2(_addInInstance,
                this._memoryDumpCommandTextShort, 
                buttonText, 
                toolTip, 
                false, 
                null,
                ref contextGUIDS, 
                (int)statusValue, 
                (int)style, 
                vsCommandControlType.vsCommandControlTypeButton
            );
            _memoryDumpCommandText = command.Name;
        }
        #endregion

    }
}