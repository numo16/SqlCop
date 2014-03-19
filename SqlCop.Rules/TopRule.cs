﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Schema.SchemaModel;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Microsoft.Data.Schema.Sql.SchemaModel;
using Microsoft.Data.Schema.StaticCodeAnalysis;
using SqlCop.Common;

namespace SqlCop.Rules
{  
  public class TopRule : SqlRule
  {
    public override IList<SqlRuleProblem> Analyze(SqlRuleContext context)
    {
      List<SqlRuleProblem> problems = new List<SqlRuleProblem>();
      TSqlScript script = context.ScriptFragment as TSqlScript;
      Debug.Assert(script != null, "TSqlScript is expected");

      var visitor = new TopRowFilterVisitor();
      script.Accept(visitor);
      if (visitor.WasVisited && !visitor.HasParenthesis)
      {              
        problems.Add(new SqlRuleProblem(this, Resources.TOP_parenthesis_rule, visitor.SqlFragment));
      }

      return problems;
    }
  }
}
