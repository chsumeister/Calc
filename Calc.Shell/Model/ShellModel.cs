using NCalc;
using System;
using System.Diagnostics;
using System.Numerics;

namespace Calc.Model
{
    public class ShellModel
    {
        public double EvaluateExpression(string expression)
        {
            var e = new Expression(expression, EvaluateOptions.IgnoreCase);

            e.EvaluateFunction += (name, args) =>
            {
                if (name.Equals("factorial", StringComparison.OrdinalIgnoreCase))
                {
                    int n = Convert.ToInt32(args.Parameters[0].Evaluate());

                    if (n > 20)
                    {
                        args.Result = double.PositiveInfinity;
                        return;
                    }
                    long fact = 1;
                    for (int i = 2; i <= n; i++)
                        fact *= i;

                    args.Result = (double)fact;
                }
            };

            try
            {
                return Convert.ToDouble(e.Evaluate());
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка вычисления", ex);
            }
        }
    }
    }