using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Infrastructure
{
    public class SqlConnectionConfig
    {
        public SqlConnectionConfig(string value) => Value = value;

        public string Value { get; }
    }
}
