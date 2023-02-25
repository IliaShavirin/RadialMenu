namespace BaseProj
{
    public class BooleanEvaluator
    {
        private static bool Operate(Operation operation, bool value1, bool value2)
        {
            switch (operation)
            {
                case Operation.and:
                    return value1 && value2;
                case Operation.or:
                    return value1 || value2;
            }

            return value2;
        }

        private static bool CalcEquation(string expression, ref int pos)
        {
            var value = false;
            var prevValue = false;
            var neg = false;
            var operation = Operation.none;

            while (pos < expression.Length)
            {
                var current = expression[pos];
                switch (current)
                {
                    case 'F':
                        value = Operate(operation, prevValue, neg);
                        neg = false;
                        operation = Operation.none;
                        pos += 4;
                        break;
                    case 'T':
                        value = Operate(operation, prevValue, !neg);
                        operation = Operation.none;
                        neg = false;
                        pos += 3;
                        break;
                    case '!':
                        neg = !neg;
                        break;
                    case '&':
                        operation = Operation.and;
                        prevValue = value;
                        pos++;
                        break;
                    case '|':
                        operation = Operation.or;
                        prevValue = value;
                        pos++;
                        break;
                    case ' ':
                        break;
                    case ')':
                        return value;
                    case '(':
                        pos++;
                        value = Operate(operation, prevValue, neg ^ CalcEquation(expression, ref pos));
                        operation = Operation.none;
                        neg = false;
                        break;
                }

                pos++;
            }

            return value;
        }


        public static bool Evaluate(string expression)
        {
            var pos = 0;
            return CalcEquation(expression, ref pos);
        }


        private enum Operation
        {
            none,
            and,
            or
        }
    }
}