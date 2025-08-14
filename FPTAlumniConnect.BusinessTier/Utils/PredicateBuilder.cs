using System;
using System.Linq.Expressions;

namespace FPTAlumniConnect.BusinessTier.Utils
{
    public static class PredicateBuilder
    {
        /// <summary>
        /// Creates an expression that always returns true.
        /// </summary>
        public static Expression<Func<T, bool>> True<T>() => x => true;

        /// <summary>
        /// Creates an expression that always returns false.
        /// </summary>
        public static Expression<Func<T, bool>> False<T>() => x => false;

        /// <summary>
        /// Combines two expressions with a logical AND.
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var combined = new ReplaceParameterVisitor(expr2.Parameters[0], parameter)
                .Visit(expr2.Body);

            var body = Expression.AndAlso(expr1.Body, combined!);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
        /// Combines two expressions with a logical OR.
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var combined = new ReplaceParameterVisitor(expr2.Parameters[0], parameter)
                .Visit(expr2.Body);

            var body = Expression.OrElse(expr1.Body, combined!);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParam;
            private readonly ParameterExpression _newParam;

            public ReplaceParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam)
            {
                _oldParam = oldParam;
                _newParam = newParam;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParam ? _newParam : base.VisitParameter(node);
            }
        }
    }
}
