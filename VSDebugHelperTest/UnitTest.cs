using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Extensibility;

using VSMemoryDump;
using System.IO;
using VSMemoryDump.Test.Mock;

namespace VSMemoryDump.Test {
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest {
        public UnitTest() {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestInstanciation() {
            MockDTE dte = new MockDTE();
            MockAddin addin = new MockAddin();

            VSDebugHelper debugHelper = new VSDebugHelper();

            IDTExtensibility2 extenasibility = debugHelper as IDTExtensibility2;

            extenasibility.OnConnection(dte, ext_ConnectMode.ext_cm_Startup, addin, null);
            extenasibility.OnConnection(dte, ext_ConnectMode.ext_cm_UISetup, addin, null);
            extenasibility.OnDisconnection(ext_DisconnectMode.ext_dm_UISetupComplete, null);
            extenasibility.OnDisconnection(ext_DisconnectMode.ext_dm_HostShutdown, null);
        }

        [TestMethod]
        unsafe public void TestReadWriteMemoryFromToFile() {
            byte[] array = new byte[32];
            for (int i = 0, n = array.Length; i < n; i++) {
                array[i] = (byte)('a' + i);
            }

            var processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            string filename = Path.GetTempFileName();

            fixed (byte* p = &array[0]) {
                IntPtr ip = (IntPtr)p;

                VSMemoryDump.Util.WriteMemoryToFile(filename, processId, (int)ip, array.Length);

            }

            byte[] readData = new byte[array.Length];
            fixed (byte* p = &readData[0]) {
                IntPtr ip = (IntPtr)p;

                VSMemoryDump.Util.ReadFileToMemory(filename, processId, (int)ip, readData.Length);
            }

            for (int i = 0, n = array.Length; i < n; i++) {
                Assert.AreEqual(array[i], readData[i]);
            }

        }

        [TestMethod]
        public void TestTryParse() {
            string stringToConvert;
            System.Globalization.NumberStyles styles;
            int number;

            stringToConvert = "0x0001";
            styles = System.Globalization.NumberStyles.AllowHexSpecifier;
            Util.CallTryParse(stringToConvert, styles, out number);
            Assert.AreEqual(1, number);
            

        }

    }
}