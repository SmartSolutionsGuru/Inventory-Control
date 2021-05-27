using SmartSolutions.Util.DictionaryUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.SQLiteCipher
{
    internal class SoloDbDataReader : DbDataReader
    {
        private List<Dictionary<string, object>> Values { get; set; }
        private int CurentIndex { get; set; } = -1;

        public override object this[int ordinal] => Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value;

        public override object this[string name] => Values?.ElementAtOrDefault(CurentIndex)?.GetValueFromDictonary(name);

        public override int Depth => Values?.Count ?? 0;

        public override int FieldCount => Values?.ElementAtOrDefault(0)?.Count ?? 0;

        public override bool HasRows => Values?.Count > 0;

        public override bool IsClosed => false;

        public override int RecordsAffected => 0;

        internal SoloDbDataReader(List<Dictionary<string, object>> values)
        {
            this.Values = values;
        }

        public override bool GetBoolean(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as bool? ?? default(bool);
        }

        public override byte GetByte(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as byte? ?? default(byte);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            var bytes = Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as byte[];
            Array.Copy(bytes, dataOffset, buffer, bufferOffset, length);
            return length;
            //bytes = bytes?.Skip(bufferOffset).ToArray();
            //bytes = bytes?.Take(Math.Min(length, bytes.Length))?.ToArray();
            //bytes?.CopyTo(buffer, bufferOffset);
            //return bytes?.Length ?? 0;
        }

        public override char GetChar(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as char? ?? default(char);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            var bytes = Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as char[];
            Array.Copy(bytes, dataOffset, buffer, bufferOffset, length);
            return length;

        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as DateTime? ?? default(DateTime);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as decimal? ?? default(decimal);
        }

        public override double GetDouble(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as double? ?? default(double);
        }

        public override IEnumerator GetEnumerator()
        {
            return Values?.GetEnumerator();
        }

        public override Type GetFieldType(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override float GetFloat(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as float? ?? default(float);
        }

        public override Guid GetGuid(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as Guid? ?? default(Guid);
        }

        public override short GetInt16(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as Int16? ?? default(Int16);
        }

        public override int GetInt32(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as int? ?? default(int);
        }

        public override long GetInt64(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as long? ?? default(long);
        }

        public override string GetName(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Key;
        }

        public override int GetOrdinal(string name)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.Keys.ToList().IndexOf(name) ?? -1;
        }

        public override string GetString(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value as string;
        }

        public override object GetValue(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value;
        }

        public override int GetValues(object[] values)
        {
            Array.Copy(Values?.ElementAtOrDefault(CurentIndex)?.Values?.ToArray(), values, Values?.ElementAtOrDefault(CurentIndex)?.Keys?.Count ?? 0);
            return Values?.ElementAtOrDefault(CurentIndex)?.Keys?.Count ?? 0;
        }

        public override bool IsDBNull(int ordinal)
        {
            return Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value == null
                    || Values?.ElementAtOrDefault(CurentIndex)?.ElementAtOrDefault(ordinal).Value == DBNull.Value;
        }

        public override bool NextResult()
        {
            CurentIndex++;
            return CurentIndex >= Values?.Count;
        }

        public override bool Read()
        {
            CurentIndex++;
            return CurentIndex < Values?.Count;
        }
    }
}
