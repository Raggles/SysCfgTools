using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SysCfgTools.IdfCompiler;

namespace SysCfgTools.IdfCompiler.Tests
{
    [TestClass]
    public class CompilerTests
    {
        [TestMethod]
        public void TestCompiler1()
        {
            Compiler com = new Compiler();
            string output = com.Compile("a = b + c");
            string expected =
@"0, ""b""
0, ""c""
2, ""0""
1, ""a""";
            Assert.AreEqual(output, expected);
 
 
        }

        [TestMethod]
        public void TestCompiler2()
        {
            Compiler com = new Compiler();
            string output = com.Compile("a = b+ c * d+e");
            string expected =
@"0, ""b""
0, ""c""
0, ""d""
4, ""0""
2, ""0""
0, ""e""
2, ""0""
1, ""a""";
            Assert.AreEqual(output, expected);
        }

        [TestMethod]
        public void TestCompiler3()
        {
            Compiler com = new Compiler();
            string output = com.Compile("a = b and c OR d XoR e");
            string expected =
@"0, ""b""
0, ""c""
18, ""0""
0, ""d""
0, ""e""
19, ""0""
17, ""0""
1, ""a""";

            Assert.AreEqual(output, expected);
        }
    }
}
