using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Api.Collection
{
    public class ParameterCollection : IEnumerable
    {
        private readonly List<Parameter> _parameters;

        public IEnumerable<string> Keys
        {
            get
            {
                return _parameters.Select(x => x.Key);
            }
        }
        
        public IEnumerable<object> Values
        {
            get
            {
                return _parameters.Select(x => x.Value);
            }
        }

        public int Count => _parameters.Count;

        public ParameterCollection()
        {
            _parameters = new List<Parameter>();
        }
        
        public ParameterCollection(List<Parameter> parameters)
        {
            _parameters = parameters;
        }

        public void Add(string key, object value, UrlEncode urlEncode = UrlEncode.NoEncode)
        {
            _parameters.Add(new Parameter(key, value, urlEncode));
        }

        public void AddRange(ParameterCollection parameterPairs)
        {
            foreach (Parameter keyValuePair in parameterPairs)
            {
                _parameters.Add(keyValuePair);
            }
        }
        
        public IEnumerator GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }
    }
}