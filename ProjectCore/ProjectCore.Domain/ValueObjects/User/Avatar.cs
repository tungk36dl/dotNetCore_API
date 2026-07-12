using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Domain.ValueObjects.User
{
    public sealed record Avatar
    {
        public string Value { get; }

        private Avatar() { Value = default!; }

        private Avatar(string value)
        {
            Value = value;
        }

        public static Avatar FromUrl(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ArgumentException("Invalid avatar URL");

            return new Avatar(url.Trim());
        }

        public static Avatar FromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Invalid avatar file path");

            return new Avatar(path.Trim());
        }

        public override string ToString() => Value;
    }

}
