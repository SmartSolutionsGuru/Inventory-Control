using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.SQLiteCipher
{
   internal  class SQLiteParameterCollection : DbParameterCollection
    {
        private List<DbParameter> parameters = new List<DbParameter>();

        public override int Count => parameters.Count;

        public override object SyncRoot => parameters;

        public override int Add(object value)
        {
            parameters.Add(value as DbParameter);
            return parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var item in values)
            {
                parameters.Add(item as DbParameter);
            }
        }

        public override void Clear()
        {
            parameters.Clear();
        }

        public override bool Contains(object value)
        {
            return parameters.Contains(value);
        }

        public override bool Contains(string value)
        {
            return parameters.Any(x => x.ParameterName == value);
        }

        public override void CopyTo(Array array, int index)
        {
            var i = index;
            foreach (DbParameter param in array)
            {
                parameters.Insert(i, param);
                //if (i < parameters.Count)
                //    parameters[i] = param;
                //else
                //    parameters.Add(param);
                i++;
            }
        }

        public override IEnumerator GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        public override int IndexOf(object value)
        {
            return parameters.IndexOf(value as DbParameter);
        }

        public override int IndexOf(string parameterName)
        {
            return parameters.FindIndex(x => x.ParameterName == parameterName);
        }

        public override void Insert(int index, object value)
        {
            parameters.Insert(index, value as DbParameter);
        }

        public override void Remove(object value)
        {
            parameters.Remove(value as DbParameter);
        }

        public override void RemoveAt(int index)
        {
            parameters.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            parameters.RemoveAll(x => x.ParameterName == parameterName);
        }

        protected override DbParameter GetParameter(int index)
        {
            return parameters.ElementAtOrDefault(index);
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return parameters.FirstOrDefault(x => x.ParameterName == parameterName);
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            parameters[index] = value;
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = parameters.FindIndex(x => x.ParameterName == parameterName);
            if (index >= 0)
                parameters[index] = value;
        }
    }

    internal class SQLiteDbParameterCollection2 : DbParameterCollection
    {
        private Dictionary<string, DbParameter> parameters = new Dictionary<string, DbParameter>();

        public override int Count => parameters.Count;

        public override object SyncRoot => parameters;

        public override int Add(object value)
        {
            var index = parameters.Count;
            if (value is DbParameter)
            {
                var p = value as DbParameter;
                if (parameters.ContainsKey(p.ParameterName))
                    index = parameters.Keys.ToList().FindIndex(x => x == p.ParameterName);
                parameters[p.ParameterName] = p;
            }
            return parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (DbParameter item in values)
            {
                parameters[item.ParameterName] = item;
            }
        }

        public override void Clear()
        {
            parameters.Clear();
        }

        public override bool Contains(object value)
        {
            return parameters.ContainsValue(value as DbParameter);
        }

        public override bool Contains(string value)
        {
            return parameters.ContainsKey(value);
        }

        public override void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        public override IEnumerator GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        public override int IndexOf(object value)
        {
            return parameters.Values.ToList().IndexOf(value as DbParameter);
        }

        public override int IndexOf(string parameterName)
        {
            return parameters.Keys.ToList().FindIndex(x => x == parameterName);
        }

        public override void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        public override void Remove(object value)
        {
            parameters.Remove((value as DbParameter)?.ParameterName);
        }

        public override void RemoveAt(int index)
        {
            parameters.Remove(parameters.ElementAtOrDefault(index).Key);
        }

        public override void RemoveAt(string parameterName)
        {
            parameters.Remove(parameterName);
        }

        protected override DbParameter GetParameter(int index)
        {
            return parameters.ElementAtOrDefault(index).Value;
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return parameters[parameterName];
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            throw new NotSupportedException();
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            parameters[parameterName] = value;
        }
    }
}
