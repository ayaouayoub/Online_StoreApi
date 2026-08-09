using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Domain.ValueObjs
{
    public record Email
    {
        public string Value { get; } = null!;

        public Email(string value)
        {
            if (_IsNotValidEmail(value)) throw new DomainException("invalid email");
            Value = value;
        }

        private bool _IsNotValidEmail(string? email)
        {
            return string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", RegexOptions.IgnoreCase);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
