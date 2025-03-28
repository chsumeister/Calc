namespace curc_c_.Model
{
    // модель для калькулятора
    internal class Calculator
    {
        public double Add(double a, double b) => a + b;
        public double Subtract(double a, double b) => a - b;
        public double Multiply(double a, double b) => a * b;
        public double Divide(double a, double b) => a / b;
        public double Percentage(double a) => a / 100;
        public double Square(double a) => a * a;
        public double SquareRoot(double a) => Math.Sqrt(a);
        public double Reciprocal(double a) => 1 / a;

        // считает выражение и возвращает число double
        public double EvaluateExpression(string expression)
        {
            var dataTable = new System.Data.DataTable();
            var value = dataTable.Compute(expression, null);
            return Convert.ToDouble(value);
        }
    }
}