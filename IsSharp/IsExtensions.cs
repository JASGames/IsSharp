using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IsSharp
{
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

        public static IsCondition<string> NotNullOrWhiteSpace(this IsCondition<string> condition)
        {
            return condition.Check(x => !string.IsNullOrWhiteSpace(x));
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
}