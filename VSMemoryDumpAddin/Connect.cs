using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using VSMemoryDumpAddin.Commands;

namespace VSMemoryDumpAddin {
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget {
        #region private
        private DTE2 _application;
        private AddIn _addInInstance;

        private List<ICommand> _commands = new List<ICommand>();
        private Dictionary<string, ICommand> _vsCommandTextToCommandMap = new Dictionary<string, ICommand>();

        #endregion

        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect() {

            try {
                _commands.Add(new ReadMem());
                _commands.Add(new WriteMem());
            } catch (NotImplementedException e) {
                System.Diagnostics.Debugger.Log(0, "Diag", e.ToString());
            }
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom) {
            _application = (DTE2)application;
            _addInInstance = (AddIn)addInInst;

            _InitializeCommands();
            _ConnectCommands();

        }

        private void _InitializeCommands() {
            try {
                foreach (var c in _commands) {
                    c.Initialize(_application, _addInInstance);
                }
            } catch (NotImplementedException e) {
                System.Diagnostics.Debugger.Log(0, "Diag", e.ToString());
            }
        }

        private void _ConnectCommands() {
            try {

                Commands2 cmds = _application.Commands as Commands2;
                vsCommandStatus statusValue =
                    vsCommandStatus.vsCommandStatusSupported
                    | vsCommandStatus.vsCommandStatusEnabled;


                foreach (var c in _commands) {
                    object[] contextGUIDS = new object[] { };
                    vsCommandStyle style = vsCommandStyle.vsCommandStylePict;

                    String buttonText = c.CommandText;
                    String toolTip = c.CommandText;
                    Command command = cmds.AddNamedCommand2(_addInInstance
                        , c.CommandText
                        , buttonText
                        , toolTip
                        , false
                        , null
                        , ref contextGUIDS
                        , (int)statusValue
                        , (int)style
                        , vsCommandControlType.vsCommandControlTypeButton
                    );

                    _vsCommandTextToCommandMap.Add(command.Name, c);
                }
            } catch (NotImplementedException e) {
                System.Diagnostics.Debugger.Log(0, "Diag", e.ToString());
            }
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
            handled = false;

            ICommand command = null;
            _vsCommandTextToCommandMap.TryGetValue(cmdName, out command);

            if (null != command) {
                try {
                    command.Exec(cmdName, executeOption, ref variantIn, ref variantOut, ref handled);
                }catch(Exception e){
                    var commandWindow = _application.Windows.Item(EnvDTE.Constants.vsWindowKindCommandWindow).Object as CommandWindow;

                    commandWindow.OutputString(cmdName + " failed with exception: " + e.ToString());
                }
            }
        }

        void IDTCommandTarget.QueryStatus(string cmdName
            , vsCommandStatusTextWanted neededText
            , ref vsCommandStatus statusOption
            , ref object commandText
        ) {
            statusOption = vsCommandStatus.vsCommandStatusUnsupported;

            ICommand command = null;
            _vsCommandTextToCommandMap.TryGetValue(cmdName, out command);

            if (null != command) {
                command.QueryStatus(cmdName, neededText, ref statusOption, ref commandText);
            }
        }


        #endregion

        #region private
 
        #endregion

    }
}