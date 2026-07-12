using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Domain.ValueObjects.User
{
    public sealed record FullName
    {
        public string Value { get; }

        private FullName() { Value = default!; }

        public FullName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Full name is required.");

            Value = value.Trim();
        }

        public override string ToString() => Value;
    }

}
