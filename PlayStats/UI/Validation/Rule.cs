namespace PlayStats.UI.Validation
{
    public abstract class Rule<T> : Rule where T : class
    {
        public override void Validate(object obj)
        {
            Validate(obj as T);
        }

        public abstract void Validate(T obj);
    }

    public abstract class Rule
    {
        public abstract string PropertyName { get; }
        public string Error { get; protected set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
        public abstract void Validate(object obj);
    }
}