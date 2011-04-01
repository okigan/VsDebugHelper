using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;

namespace VSMemoryDump.Test.Mock {

    class MockDTE : DTE2 {
        private MockCommands commands = new MockCommands();

        public EnvDTE.Document ActiveDocument {
            get { throw new NotImplementedException(); }
        }

        public dynamic ActiveSolutionProjects {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.Window ActiveWindow {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.AddIns AddIns {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.DTE Application {
            get { throw new NotImplementedException(); }
        }

        public dynamic CommandBars {
            get { throw new NotImplementedException(); }
        }

        public string CommandLineArguments {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.Commands Commands {
            get { return commands; }
        }

        public EnvDTE.ContextAttributes ContextAttributes {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.DTE DTE {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.Debugger Debugger {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.vsDisplay DisplayMode {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public EnvDTE.Documents Documents {
            get { throw new NotImplementedException(); }
        }

        public string Edition {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.Events Events {
            get { throw new NotImplementedException(); }
        }

        public void ExecuteCommand(string CommandName, string CommandArgs = "") {
            throw new NotImplementedException();
        }

        public string FileName {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.Find Find {
            get { throw new NotImplementedException(); }
        }

        public string FullName {
            get { throw new NotImplementedException(); }
        }

        public dynamic GetObject(string Name) {
            throw new NotImplementedException();
        }

        public uint GetThemeColor(vsThemeColors Element) {
            throw new NotImplementedException();
        }

        public EnvDTE.Globals Globals {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.ItemOperations ItemOperations {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.wizardResult LaunchWizard(string VSZFile, ref object[] ContextParams) {
            throw new NotImplementedException();
        }

        public int LocaleID {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.Macros Macros {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.DTE MacrosIDE {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.Window MainWindow {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.vsIDEMode Mode {
            get { throw new NotImplementedException(); }
        }

        public string Name {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.ObjectExtenders ObjectExtenders {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.Window OpenFile(string ViewKind, string FileName) {
            throw new NotImplementedException();
        }

        public void Quit() {
            throw new NotImplementedException();
        }

        public string RegistryRoot {
            get { throw new NotImplementedException(); }
        }

        public string SatelliteDllPath(string Path, string Name) {
            throw new NotImplementedException();
        }

        public EnvDTE.SelectedItems SelectedItems {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.Solution Solution {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.SourceControl SourceControl {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.StatusBar StatusBar {
            get { throw new NotImplementedException(); }
        }

        public bool SuppressUI {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public ToolWindows ToolWindows {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.UndoContext UndoContext {
            get { throw new NotImplementedException(); }
        }

        public bool UserControl {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string Version {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.WindowConfigurations WindowConfigurations {
            get { throw new NotImplementedException(); }
        }

        public EnvDTE.Windows Windows {
            get { throw new NotImplementedException(); }
        }

        public bool get_IsOpenFile(string ViewKind, string FileName) {
            throw new NotImplementedException();
        }

        public EnvDTE.Properties get_Properties(string Category, string Page) {
            throw new NotImplementedException();
        }
    }
}
