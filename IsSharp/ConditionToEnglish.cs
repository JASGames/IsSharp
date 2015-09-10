using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
            if (memberExpression.Expression == null)
            {
                var lamdba = Expression.Lambda(memberExpression);
                var result = lamdba.Compile().DynamicInvoke();

                if (memberExpression.Member is FieldInfo)
                {
                    var field = memberExpression.Member as FieldInfo;
                    var fieldName = field.Name;
                    var fieldType = field.FieldType.Name;

                    _sb.Append(string.Format("{0} {1} : ",fieldType, fieldName));
                }

                if (result is string)
                {
                    _sb.Append("\"" + result + "\"");
                }
                else
                {
                    _sb.Append(result);
                }
            }

            var expression = Visit(memberExpression.Expression);

            if (expression is ConstantExpression)
            {
                object container = ((ConstantExpression)expression).Value;
                var member = memberExpression.Member;
                if (member is FieldInfo)
                {
                    object value = ((FieldInfo)member).GetValue(container);
                    _sb.Append(member.Name + " : ");
                    return VisitConstant(Expression.Constant(value));
                }
                if (member is PropertyInfo)
                {
                    object value = ((PropertyInfo)member).GetValue(container, null);
                    _sb.Append(member.Name + " : ");
                    return VisitConstant(Expression.Constant(value));
                }
            }

            return memberExpression;
        }

        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            if (_name != string.Empty)
            {
                var formatedValue = _value == null ? "null" : _value.ToString();

                if (_value is string)
                {
                    formatedValue = "\"" + formatedValue + "\"";
                }

                _sb.Append(string.Format("{0} : {1}", _name, formatedValue));

            }

            return parameterExpression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            var methodDeclare = m.Method.DeclaringType;
            var methodName = m.Method.Name;

            if (methodDeclare != null)
            {
                if (methodDeclare.Name == "String" && methodName == "IsNullOrWhiteSpace")
                {
                    Visit(m.Arguments.First());
                    _sb.Append(" should not be null or whitespace ");
                    return m;
                }

                _sb.Append(string.Format("{1} {0} (", methodName, methodDeclare.Name));
            }
            else
            {
                _sb.Append(string.Format("{0} (", methodName));
            }

            foreach (var expresion in m.Arguments)
            {
                Visit(expresion);
            }

            var lamdba = Expression.Lambda(m);
            var result = lamdba.Compile().DynamicInvoke();

            _sb.Append(string.Format(") returned {0}",result));

            return m;
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
                    _sb.Append(" should not be equal to ");
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
