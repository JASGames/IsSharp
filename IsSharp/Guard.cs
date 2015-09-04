using System;
using System.Linq.Expressions;

namespace IsSharp
{
    public static class Guard
    {
        public static void Is<TException>(Expression<Func<bool>> condition) where TException : Exception
        {
            if (condition.Compile().Invoke()) 
                return;
            var conditionString = new ConditionToEnglish().Translate(condition.Body);
            throw (Exception)Activator.CreateInstance(typeof(TException), conditionString);
        }

        public static void Is(Expression<Func<bool>> condition)
        {
            Is<ArgumentException>(condition);
        }
    }
}