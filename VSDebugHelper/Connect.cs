using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using VSMemoryDump.DebuggerCommands;
using VSMemoryDump;

/// <summary>The object for implementing an Add-in.</summary>
/// <seealso class='IDTExtensibility2' />
public class VSDebugHelper : IDTExtensibility2, IDTCommandTarget {
    #region private
    private DTE2 _application;
    private AddIn _addInInstance;

    private List<VSMemoryDump.ICommand> _commands = new List<ICommand>();
    private Dictionary<string, VSMemoryDump.ICommand> _vsCommandTextToCommandMap = new Dictionary<string, ICommand>();

    #endregion

    /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
    public VSDebugHelper() {

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
        switch (connectMode) {
            case ext_ConnectMode.ext_cm_AfterStartup: {
                _application = (DTE2)application;
                _addInInstance = (AddIn)addInInst;

                _InitializeCommands();
                _DeleteCommands();
                _ConnectCommands();
            } break;
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
        _DeleteCommands();
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
            } catch (Exception e) {
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
            try {
                command.QueryStatus(cmdName, neededText, ref statusOption, ref commandText);
            } catch (Exception e) {
                var commandWindow = _application.Windows.Item(EnvDTE.Constants.vsWindowKindCommandWindow).Object as CommandWindow;

                commandWindow.OutputString("QueryStatus on " + cmdName + " failed with exception: " + e.ToString());
            }
        }
    }


    #endregion

    #region private
    private void _InitializeCommands() {
        try {
            foreach (var c in _commands) {
                c.Initialize(_application, _addInInstance);
            }
        } catch (NotImplementedException e) {
            System.Diagnostics.Debugger.Log(0, "Diag", e.ToString());
        }
    }


    private void _DeleteCommands() {
        try {
            Commands2 cmds = _application.Commands as Commands2;

            foreach (var c in _commands) {
                _DeleteIndividualCommand(cmds, CreateFullCommandName(c));
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
                Command command = null;

                try {
                    command = cmds.Item(CreateFullCommandName(c), -1);
                } catch (ArgumentException) {
                    //treat argument exception as command not found
                }

                if (null == command) {
                    command = _AddIndividualCommand(cmds, statusValue, c);
                }
                _vsCommandTextToCommandMap.Add(command.Name, c);
            }
        } catch (NotImplementedException e) {
            System.Diagnostics.Debugger.Log(0, "Diag", e.ToString());
        }
    }

    private string CreateFullCommandName(ICommand c) {
        return this.GetType().Name + "." + c.CommandText;
    }

    private Command _AddIndividualCommand(Commands2 cmds, vsCommandStatus statusValue, ICommand c) {
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
        return command;
    }

    private void _DeleteIndividualCommand(Commands2 cmds, String fullCommandName) {
        try {
            Command cmdToDelete = cmds.Item(fullCommandName, -1);
            cmdToDelete.Delete();
        } catch (ArgumentException) {
            // The ArgumentException will be thrown if the command does not
            // exist. It is fine to eat this exception as that means it's 
            // the first run of this add in.
        }
    }
    #endregion
}