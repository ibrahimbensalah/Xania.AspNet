using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xania.AspNet.Simulator.Tests
{
    public class PropertiesFile
    {
        private readonly IDictionary<string, string> _properties;

        public PropertiesFile(string path)
        {
            _properties = (from line in File.ReadAllLines(path)
                let i = line.IndexOf('=')
                select new
                {
                    Key = line.Substring(0, i).Trim(), 
                    Value = line.Substring(i + 1).Trim()
                }).ToDictionary(e => e.Key, e => e.Value, StringComparer.OrdinalIgnoreCase);
        }

        public string Get(string key)
        {
            string value;
            return _properties.TryGetValue(key, out value) ? value : null;
        }
    }
}
