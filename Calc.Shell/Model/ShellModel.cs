using NCalc;
using System;

namespace Calc.Model
{
    public class ShellModel
    {
        public double EvaluateExpression(string expression)
        {
            var e = new Expression(expression, EvaluateOptions.IgnoreCase);

            var result = e.Evaluate();
            return Convert.ToDouble(result);
        }
    }
}