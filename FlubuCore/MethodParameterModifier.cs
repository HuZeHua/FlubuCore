﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FlubuCore
{
    public class MethodParameterModifier : ExpressionVisitor
    {
        private List<string> _values;

        public Expression Modify(Expression expression, List<string> values)
        {
            _values = values;
            return Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            List<ConstantExpression> newMethodParameters = new List<ConstantExpression>();

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                newMethodParameters.Add(Expression.Constant(ParseValue(_values[i], node.Arguments[i].Type), node.Arguments[i].Type));
            }

            MethodCallExpression methodCallExpression = node.Update(node.Object, newMethodParameters);
            return base.VisitMethodCall(methodCallExpression);
        }

        private object ParseValue(string value, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                {
                    return value;
                }

                case TypeCode.Int16:
                case TypeCode.Int32:
                {

                    return int.Parse(value);
                }

                case TypeCode.Boolean:
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }

                    return bool.Parse(value);
                }

                case TypeCode.Int64:
                {
                    return long.Parse(value);
                }

                default:
                {
                    throw new NotSupportedException("Type not supported for mapping");
                }
            }
        }
    }
}
