using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Internal;

namespace adc.Dal {
    public static class IncludesBuilder {
        public static bool TryParsePath(Expression expression, out string path) {
            path = null;
            var withoutConvert = expression.RemoveConvert(); // Removes boxing
            var callExpression = withoutConvert as MethodCallExpression;

            if (withoutConvert is MemberExpression memberExpression) {
                var thisPart = memberExpression.Member.Name;
                if (!TryParsePath(memberExpression.Expression, out var parentPart)) {
                    return false;
                }
                path = parentPart == null ? thisPart : (parentPart + "." + thisPart);
            } else if (callExpression != null) {
                if (callExpression.Method.Name == "Select"
                    && callExpression.Arguments.Count == 2) {
                    if (!TryParsePath(callExpression.Arguments[0], out var parentPart)) {
                        return false;
                    }
                    if (parentPart != null) {
                        if (callExpression.Arguments[1] is LambdaExpression subExpression) {
                            if (!TryParsePath(subExpression.Body, out var thisPart)) {
                                return false;
                            }
                            if (thisPart != null) {
                                path = parentPart + "." + thisPart;
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            return true;
        }
    }
}

