using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SysCfgTools.IdfCompiler;

namespace SysCfgTools.IdfCompiler.Tests
{
    [TestClass]
    public class TokeniserTests
    {
        [TestMethod]
        public void TestBasicTokeniser()
        {
            IdfTokeniser tks = new IdfTokeniser();

            tks.ParseTokens("one + two * three");

            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Operator);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Operator);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
        }

        [TestMethod]
        public void TestAdvancedTokeniser()
        {
            IdfTokeniser tks = new IdfTokeniser();

            tks.ParseTokens("one + two * three - andsoe(p1, p2, p3) + (1 + 5-6/7)");

            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Operator);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Operator);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Operator);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Function);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.LeftParen);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.ParamSeperator);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.ParamSeperator);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.RightParen);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Operator);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.LeftParen);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Operator);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Operator);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Operator);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.Literal);
            Assert.AreEqual(tks.NextToken().Type, IdfTokenType.RightParen);
            Assert.AreEqual(tks.PeekToken(), null);
            ;
        }
    }
}
