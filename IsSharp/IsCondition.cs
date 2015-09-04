using System;
using System.Linq.Expressions;

namespace IsSharp
{
    public class IsCondition<T>
    {
        private readonly string _name;
        private readonly T _value;

        internal IsCondition(string name, T value)
        {
            _name = name;
            _value = value;
        }

        public IsCondition<T> Check<TException>(Expression<Func<T, bool>> expression) where TException : Exception
        {
            if (!(expression.Compile().Invoke(_value)))
            {
                var conditionString = new ConditionToEnglish().Translate(expression.Body, _name, _value);
                throw (Exception)Activator.CreateInstance(typeof(TException), conditionString);
            }

            return this;
        }

        public IsCondition<T> Check(Expression<Func<T, bool>> expression)
        {
            return Check<ArgumentException>(expression);
        }
    }
}