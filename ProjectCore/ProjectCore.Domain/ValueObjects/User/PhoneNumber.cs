using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectCore.Domain.ValueObjects.User
{
    public sealed record PhoneNumber
    {
        public string Value { get; }

        private PhoneNumber() { Value = default!; }

        private static readonly Regex PhoneRegex =
            new(@"^(0|\+84)[0-9]{9}$");

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number is required.");

            if (!PhoneRegex.IsMatch(value))
                throw new ArgumentException("Invalid phone number.");

            Value = value;
        }

        public override string ToString() => Value;
    }

}
