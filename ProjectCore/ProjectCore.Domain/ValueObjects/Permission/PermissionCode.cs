namespace ProjectCore.Domain.ValueObjects.Permission
{
    public sealed record PermissionCode
    {
        public string Value { get; }

        private PermissionCode() { }

        public PermissionCode(string module, string action)
        {
            if (string.IsNullOrWhiteSpace(module))
                throw new ArgumentException("Module is required");

            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Action is required");

            Value = $"{module.Trim()}.{action.Trim()}".ToUpper();
        }

        public PermissionCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("PermissionCode is required");

            Value = value.Trim().ToUpper();
        }

        public override string ToString() => Value;
    }
}
