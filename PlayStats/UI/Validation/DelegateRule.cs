using System;

namespace PlayStats.UI.Validation
{
    public class DelegateRule<T> : Rule<T> where T : class
    {
        private readonly Func<T, string> _validationFunction;

        public DelegateRule(string propertyName, Func<T, string> validationFunction)
        {
            _validationFunction = validationFunction;
            PropertyName = propertyName;
        }

        public override string PropertyName { get; }
        public override void Validate(T obj)
        {
            Error = _validationFunction(obj);
        }
    }
}