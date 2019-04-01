using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;

namespace PlayStats.UI.Validation
{
    public class ValidationViewModel : ReactiveObject, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<Rule>> _rules = new Dictionary<string, List<Rule>>();

        protected ValidationViewModel()
        {
            WhenAnyPropertyChanged = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    p => PropertyChanged += p,
                    p => PropertyChanged -= p)
                .Select(p => p.EventArgs.PropertyName);
            
            WhenAnyPropertyChanged.Subscribe(Validate);

            WhenDataErrorsChanged = Observable
                .FromEventPattern<DataErrorsChangedEventArgs>(
                    p => ErrorsChanged += p, 
                    p => ErrorsChanged -= p)
                .Select(p => HasErrors);
        }

        protected IObservable<string> WhenAnyPropertyChanged { get; }

        protected IObservable<bool> WhenDataErrorsChanged { get; }

        protected void AddRule(Rule rule)
        {
            if (!_rules.ContainsKey(rule.PropertyName))
            {
                _rules.Add(rule.PropertyName, new List<Rule>());
            }

            var rulesForProperty = _rules[rule.PropertyName];
            rulesForProperty.Add(rule);

            this.RaisePropertyChanged(rule.PropertyName);
        }

        private void Validate(string propertyName)
        {
            if (!_rules.ContainsKey(propertyName)) return;
            _rules[propertyName].ForEach(p => p.Validate(this));
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return _rules.ContainsKey(propertyName)
                ? _rules[propertyName].Where(p => p.HasError).Select(p => p.Error)
                : Enumerable.Empty<string>();
        }

        public bool HasErrors => _rules.Values.SelectMany(p => p).Any(p => p.HasError);

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}