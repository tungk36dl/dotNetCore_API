using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Domain.ValueObjects.User
{
    public sealed record UserName
    {
        public string Value { get; }

        private UserName() { Value = default!; }

        public UserName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Username is required.");

            if (value.Length < 3 || value.Length > 50)
                throw new ArgumentException("Username must be 3–50 characters.");

            Value = value.Trim();
        }

        public override string ToString() => Value;
    }

}
