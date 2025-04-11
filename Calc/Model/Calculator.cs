using System;

namespace curc_c_.Model
{
    internal class Calculator
    {
        public double EvaluateExpression(string expression)
        {
            var dataTable = new System.Data.DataTable();
            var value = dataTable.Compute(expression, null);
            return Convert.ToDouble(value);
        }
    }
}