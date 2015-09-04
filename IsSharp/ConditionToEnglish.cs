using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace IsSharp
{
    /// <summary>
    /// ConvertExpressionToReadableEnglish
    /// </summary>
    internal class ConditionToEnglish : ExpressionVisitor
    {
        StringBuilder _sb;
        private string _name;
        private object _value;

        private ConditionToEnglish()
        {
            _name = string.Empty;
            _value = null;
        }

        internal static string Translate(Expression expression, string parameterName = "", object value = null)
        {
            return new ConditionToEnglish().TranslateInternal(expression, parameterName, value);
        }

        private  string TranslateInternal(Expression expression, string parameterName = "", object value = null)
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
                        _sb.Append("\"" + c.Value + "\"");
                        break;
                }
            }
            return c;
        }

        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            var container = ((ConstantExpression)memberExpression.Expression).Value;
            var value = container.GetType().GetFields().First(i => i.Name == memberExpression.Member.Name).GetValue(container);

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
            Visit(b.Left);

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
                    _sb.Append(string.Format(" {0} ", b.NodeType));
                    break;
            }

            Visit(b.Right);
            return b;
        }
    }
}
