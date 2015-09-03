using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace IsSharp
{
    public class IsCondition<T>
    {
        private readonly string _name;
        private readonly T _value;

        public IsCondition(string name, T value)
        {
            _name = name;
            _value = value;
        }

        public IsCondition<T> Check<E>(Expression<Func<T, bool>> expression)
        {
            if (!(expression.Compile().Invoke(_value)))
            {
                string conditionString = new ConditionToEnglish().Translate(expression.Body, _name, _value);
                throw (Exception)Activator.CreateInstance(typeof(E), conditionString);
            }

            return this;
        }

        public IsCondition<T> Check(Expression<Func<T, bool>> expression)
        {
            return this.Check<ArgumentException>(expression);
        }
    }

    public static class IsExtensions
    {
        public static IsCondition<T> Is<T>(this T member, string name)
        {
            return new IsCondition<T>(name, member);
        }

        public static IsCondition<T> InRange<T>(this IsCondition<T> condition, T min, T max)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var left = Expression.GreaterThanOrEqual(parameter, Expression.Convert(Expression.Constant(min), typeof(T)));
            var right = Expression.LessThanOrEqual(parameter, Expression.Convert(Expression.Constant(max), typeof(T)));

            var body = Expression.AndAlso(left, right);
            var lamdba = Expression.Lambda<Func<T, bool>>(body, parameter);

            return condition.Check<ArgumentOutOfRangeException>(lamdba);
        }

        public static IsCondition<T> NotNull<T>(this IsCondition<T> condition)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var body = Expression.NotEqual(parameter, Expression.Constant(null));

            var lamdba = Expression.Lambda<Func<T, bool>>(body, parameter);

            return condition.Check<NullReferenceException>(lamdba);
        }

        public static IsCondition<T> NotEqualTo<T>(this IsCondition<T> condition, T compare)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var body = Expression.NotEqual(parameter, Expression.Convert(Expression.Constant(compare), typeof(T)));

            var lamdba = Expression.Lambda<Func<T, bool>>(body, parameter);

            return condition.Check<ArgumentException>(lamdba);
        }

        public static IsCondition<T> EqualTo<T>(this IsCondition<T> condition, T compare)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var body = Expression.Equal(parameter, Expression.Convert(Expression.Constant(compare), typeof(T)));

            var lamdba = Expression.Lambda<Func<T, bool>>(body, parameter);

            return condition.Check<ArgumentException>(lamdba);
        }
    }

    public static class Guard
    {
        public static void Is<E>(Expression<Func<bool>> condition)
        {
            if (!(condition.Compile().Invoke()))
            {
                string conditionString = new ConditionToEnglish().Translate(condition.Body);
                throw (Exception)Activator.CreateInstance(typeof(E), conditionString);
            }
        }

        public static void Is(Expression<Func<bool>> condition)
        {
            Is<ArgumentException>(condition);
        }
    }

    //ConvertExpressionToReadableEnglish
    internal class ConditionToEnglish : ExpressionVisitor
    {
        StringBuilder _sb;
        private string _name;
        private object _value;

        internal ConditionToEnglish()
        {
            _name = string.Empty;
            _value = null;
        }

        internal string Translate(Expression expression, string parameterName = "", object value = null)
        {
            _sb = new StringBuilder();
            _name = parameterName;
            _value = value;
            Visit(expression);
            return _sb.ToString();
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value == null)
            {
                _sb.Append("null");
            }
            else
            {
                switch (Type.GetTypeCode(c.Type))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Boolean:
                    case TypeCode.Byte:
                    case TypeCode.Char:
                    case TypeCode.DateTime:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.Single:
                        _sb.Append(c.Value);
                        break;
                    case TypeCode.String:
                        _sb.Append("\""+c.Value+"\"");
                        break;
                }
            }
            return c;
        }

        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            object container = ((ConstantExpression)memberExpression.Expression).Value;
            object value = container.GetType().GetFields().Where(i => i.Name == memberExpression.Member.Name).First().GetValue(container);

            var memberName = memberExpression.Member.Name;
            var memberValue = value == null ? "null" : value.ToString();

            _sb.Append(string.Format("{0}({1})", memberName, memberValue));
            return memberExpression;
        }

        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            if (_name != string.Empty)
            {
                var formatedValue = _value == null ? "null" : _value.ToString();

                _sb.Append(string.Format("{0}({1})", _name, formatedValue));
            }

            return parameterExpression;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            this.Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                    _sb.Append(" and ");
                    break;
                case ExpressionType.Or:
                    _sb.Append(" or ");
                    break;
                case ExpressionType.Equal:
                    _sb.Append(" should be equal to ");
                    break;
                case ExpressionType.NotEqual:
                    _sb.Append(" should be not equal to ");
                    break;
                case ExpressionType.LessThan:
                    _sb.Append(" should be less than ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _sb.Append(" should be less than or equal to ");
                    break;
                case ExpressionType.GreaterThan:
                    _sb.Append(" should be greater than ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _sb.Append(" should be greater than or equal to ");
                    break;
                case ExpressionType.AndAlso:
                    _sb.Append(" and also ");
                    break;
                case ExpressionType.Add:
                    _sb.Append(" plus ");
                    break;
                case ExpressionType.Subtract:
                    _sb.Append(" subtract ");
                    break;
                case ExpressionType.Multiply:
                    _sb.Append(" multiply by ");
                    break;
                case ExpressionType.Divide:
                    _sb.Append(" divide by ");
                    break;
                default:
                    _sb.Append(string.Format(" {0} ", b.NodeType.ToString()));
                    break;
            }

            this.Visit(b.Right);
            return b;
        }
    }
}
