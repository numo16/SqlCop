﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.Schema.ScriptDom;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlCop.Rules;

namespace SqlCop.UnitTests
{
  [TestClass]
  public class TsqlParserTest
  {
    [TestMethod]
    public void SmokeTest()
    {
      TSqlScript script = Parse("TsqlSample1.sql");
           

      foreach (TSqlBatch batch in script.Batches)
      {
        foreach (TSqlStatement statement in batch.Statements)
        {
          var selectStatement = statement as SelectStatement;
          if (selectStatement != null && selectStatement.QueryExpression != null)
          {
            var querySpecification = selectStatement.QueryExpression as QuerySpecification;            
            foreach (TableSource tableSource in querySpecification.FromClauses)
            {
              tableSource.Accept(null);
            }
          }
        }
      }
    }
    
    [TestMethod]
    public void SmokeTestWithVisitor()
    {
      var visitor = new TopRowFilterVisitor();
      TSqlScript script = Parse("TsqlSample1.sql");
      script.Accept(visitor);
      Assert.IsTrue(visitor.HasParenthesis);
    }

    private TSqlScript Parse(string fileName)
    {
      var parser = new TSql100Parser(true);
      var parseErrors = new List<ParseError>() as IList<ParseError>;
      TSqlScript script;
      using (var sr = new StreamReader(GetFilePath(fileName)))
      {
        script = parser.Parse(sr, out parseErrors) as TSqlScript;
      }
      return script;
    }

    private string GetFilePath(string fileName)
    {
      return Path.Combine("..\\..\\TestFiles", fileName).ToString();
    }
  }
}
