using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SysCfgTools.IdfCompiler;
using System.Collections.Generic;

namespace SysCfgTools.IdfCompiler.Tests
{
    [TestClass]
    public class TokenTests
    { 
            private List<IdfFunction> functions = new List<IdfFunction>();
            private List<IdfOperator> operators = new List<IdfOperator>();
            private List<IdfToken> tokens = new List<IdfToken>();
        
        public TokenTests()
        {
            AddFunctions();
            AddOperators();
        }
        
        [TestMethod]
        public void TestTokeniserOperators()
        {
            IdfToken tk = new IdfToken("+", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Operator);
            Assert.AreEqual(tk.Operator.Op, "+");

            tk = new IdfToken("-", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Operator);
            Assert.AreEqual(tk.Operator.Op, "-");

            tk = new IdfToken("*", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Operator);
            Assert.AreEqual(tk.Operator.Op, "*");

            tk = new IdfToken("/", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Operator);
            Assert.AreEqual(tk.Operator.Op, "/");

            tk = new IdfToken("and", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Operator);
            Assert.AreEqual(tk.Operator.Op, "and");

            tk = new IdfToken("xor", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Operator);
            Assert.AreEqual(tk.Operator.Op, "xor");


        }
        [TestMethod]
        public void TestTokeniserLiterals()
        {
            IdfToken tk = new IdfToken("point1", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Literal);

            tk = new IdfToken("poiandt1", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Literal);

            tk = new IdfToken("andsksks", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Literal);

            tk = new IdfToken("ftojsjdand", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Literal);
        }

        [TestMethod]
        public void TestTokeniserFunctions()
        {
            IdfToken tk = new IdfToken("formcfilter", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Function);

            tk = new IdfToken("acib", operators, functions);
            Assert.AreEqual(tk.Type, IdfTokenType.Function);

        }



        private void AddFunctions()
        {
            functions.Add(new IdfFunction("acib", 9, "40, \"0\""));
            functions.Add(new IdfFunction("acob", 5, "36, \"0\""));
            functions.Add(new IdfFunction("abs", 1, "8, \"0\""));
            functions.Add(new IdfFunction("analogmax", 2, "59, \"x\"", true));
            functions.Add(new IdfFunction("analogaverage", 2, "60, \"x\"", true));
            functions.Add(new IdfFunction("analogmin", 2, "58, \"x\"", true));
            functions.Add(new IdfFunction("analograteofchange", 2, "61, \"x\"", true));
            functions.Add(new IdfFunction("andsoe", 3, "49, \"0\""));

            functions.Add(new IdfFunction("attchflg", 2, "31, \"0\""));
            functions.Add(new IdfFunction("autocontrolselect", 2, "57, \"0\""));
            functions.Add(new IdfFunction("autoinputselect", 2, "56, \"0\""));
            functions.Add(new IdfFunction("cibalarm", 6, "37, \"0\""));
            functions.Add(new IdfFunction("ciboride", 4, "38, \"0\""));
            functions.Add(new IdfFunction("clrflg", 3, "33, \"0\""));
            functions.Add(new IdfFunction("cos", 1, "12, \"0\""));
            functions.Add(new IdfFunction("ctrlwithfeedback", 5, "53, \"0\""));
            functions.Add(new IdfFunction("dcib", 8, "39, \"0\""));
            functions.Add(new IdfFunction("dcibsoe", 7, "54, \"0\""));
            functions.Add(new IdfFunction("dcob", 5, "35, \"0\""));
            functions.Add(new IdfFunction("defa", 1, "44, \"0\""));
            functions.Add(new IdfFunction("defd", 1, "43, \"0\""));
            functions.Add(new IdfFunction("dicount", 1, "50, \"0\""));
            functions.Add(new IdfFunction("disoeinhibit", 4, "47, \"0\""));
            functions.Add(new IdfFunction("eq", 2, "21, \"0\""));
            functions.Add(new IdfFunction("exp", 1, "9, \"0\""));
            functions.Add(new IdfFunction("flt2int", 1, "15, \"0\""));
            functions.Add(new IdfFunction("formcfilter", 2, "55, \"0\""));
            functions.Add(new IdfFunction("ge", 2, "24, \"0\""));
            functions.Add(new IdfFunction("gt", 2, "23, \"0\""));
            functions.Add(new IdfFunction("hialarm", 4, "30, \"0\""));
            functions.Add(new IdfFunction("iif", 3, "27, \"0\""));
            functions.Add(new IdfFunction("inputselect", 3, "52, \"0\""));
            functions.Add(new IdfFunction("int2flt", 1, "14, \"0\""));
            functions.Add(new IdfFunction("le", 2, "26, \"0\""));
            functions.Add(new IdfFunction("linscale", 3, "28, \"0\""));

            functions.Add(new IdfFunction("ln", 1, "10, \"0\""));
            functions.Add(new IdfFunction("loalarm", 4, "29, \"0\""));
            functions.Add(new IdfFunction("lt", 2, "25, \"0\""));
            functions.Add(new IdfFunction("maskedcib", 9, "42, \"0\""));
            functions.Add(new IdfFunction("mod", 2, "6, \"0\""));
            functions.Add(new IdfFunction("ne", 2, "22, \"0\""));
            functions.Add(new IdfFunction("not", 1, "20, \"0\""));
            functions.Add(new IdfFunction("operate", 3, "34, \"0\""));
            functions.Add(new IdfFunction("orsoe", 3, "48, \"0\""));
            functions.Add(new IdfFunction("pscond", 2, "41, \"0\""));
            functions.Add(new IdfFunction("pow", 2, "7, \"0\""));
            functions.Add(new IdfFunction("readflg", 1, "46, \"0\""));
            functions.Add(new IdfFunction("setdicount", 2, "51, \"0\""));
            functions.Add(new IdfFunction("setflg", 3, "32, \"0\""));
            functions.Add(new IdfFunction("sin", 1, "13, \"0\""));
            functions.Add(new IdfFunction("sqrt", 1, "11, \"0\""));
            functions.Add(new IdfFunction("writeflg", 2, "45, \"0\""));
        }

        private void AddOperators()
        {
            operators.Add(new IdfOperator("+", Associativity.Left, "2, \"0\"", 4));
            operators.Add(new IdfOperator("-", Associativity.Left, "3, \"0\"", 4));
            operators.Add(new IdfOperator("*", Associativity.Left, "4, \"0\"", 5));
            operators.Add(new IdfOperator("/", Associativity.Left, "5, \"0\"", 5));
            operators.Add(new IdfOperator("and", Associativity.Left, "18, \"0\"", 3));
            operators.Add(new IdfOperator("or", Associativity.Left, "17, \"0\"", 1));
            operators.Add(new IdfOperator("xor", Associativity.Left, "19, \"0\"", 2));
        }
    }
}

    

