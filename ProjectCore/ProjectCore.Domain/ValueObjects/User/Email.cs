using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Domain.ValueObjects.User
{

    public sealed record Email
    {
        public string Value { get; }

        private Email() { Value = default!; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email is required.");

            var normalized = value.Trim().ToLowerInvariant();

            if (!IsValid(normalized))
                throw new ArgumentException("Invalid email format.");

            Value = normalized;
        }

        public static bool IsValid(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString() => Value;
    }

}
