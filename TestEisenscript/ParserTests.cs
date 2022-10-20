﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eisenscript;

namespace TestEisenscript
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void TestSetParse()
        {
            var scriptSets = @"
set maxdepth 100
set maxobjects 1000
set minsize 0.1
set maxsize 20.1"[2..];
            var tr = new StringReader(scriptSets);
            var parser = new Parser(tr);
            var rules = parser.Rules();
            Assert.AreEqual(100, rules.MaxDepth);
            Assert.AreEqual(1000, rules.MaxObjects);
            Assert.IsTrue(Math.Abs(rules.MinSize - 0.1) < 0.001);
            Assert.IsTrue(Math.Abs(rules.MaxSize - 20.1) < 0.001);
        }
    }
}
