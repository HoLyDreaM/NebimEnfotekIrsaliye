using System;
using System.Data;

namespace NebimEnfotekIrsaliye.Data
{
	internal class WrappedReader : IDataReader, IDisposable, IDataRecord, IWrappedDataReader
	{
		public IDataReader Reader
		{
			get
			{
				IDataReader dataReader = this.reader;
				if (dataReader == null)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return dataReader;
			}
		}
        
		IDbCommand IWrappedDataReader.Command
		{
			get
			{
				IDbCommand dbCommand = this.cmd;
				if (dbCommand == null)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return dbCommand;
			}
		}
        
		public WrappedReader(IDbCommand cmd, IDataReader reader)
		{
			this.cmd = cmd;
			this.reader = reader;
		}
		void IDataReader.Close()
		{
			if (this.reader != null)
			{
				this.reader.Close();
			}
		}
        
		int IDataReader.Depth
		{
			get
			{
				return this.Reader.Depth;
			}
		}
		DataTable IDataReader.GetSchemaTable()
		{
			return this.Reader.GetSchemaTable();
		}
        
		bool IDataReader.IsClosed
		{
			get
			{
				return this.reader == null || this.reader.IsClosed;
			}
		}
        
		bool IDataReader.NextResult()
		{
			return this.Reader.NextResult();
		}
        
		bool IDataReader.Read()
		{
			return this.Reader.Read();
		}
        
		int IDataReader.RecordsAffected
		{
			get
			{
				return this.Reader.RecordsAffected;
			}
		}
		void IDisposable.Dispose()
		{
			if (this.reader != null)
			{
				this.reader.Close();
			}
			if (this.reader != null)
			{
				this.reader.Dispose();
			}
			this.reader = null;
			if (this.cmd != null)
			{
				this.cmd.Dispose();
			}
			this.cmd = null;
		}
        
		int IDataRecord.FieldCount
		{
			get
			{
				return this.Reader.FieldCount;
			}
		}
		bool IDataRecord.GetBoolean(int i)
		{
			return this.Reader.GetBoolean(i);
		}
        
		byte IDataRecord.GetByte(int i)
		{
			return this.Reader.GetByte(i);
		}
        
		long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return this.Reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}
        
		char IDataRecord.GetChar(int i)
		{
			return this.Reader.GetChar(i);
		}
        
		long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return this.Reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}
        
		IDataReader IDataRecord.GetData(int i)
		{
			return this.Reader.GetData(i);
		}
        
		string IDataRecord.GetDataTypeName(int i)
		{
			return this.Reader.GetDataTypeName(i);
		}
		DateTime IDataRecord.GetDateTime(int i)
		{
			return this.Reader.GetDateTime(i);
		}
        
		decimal IDataRecord.GetDecimal(int i)
		{
			return this.Reader.GetDecimal(i);
		}
		double IDataRecord.GetDouble(int i)
		{
			return this.Reader.GetDouble(i);
		}
        
		Type IDataRecord.GetFieldType(int i)
		{
			return this.Reader.GetFieldType(i);
		}
        
		float IDataRecord.GetFloat(int i)
		{
			return this.Reader.GetFloat(i);
		}
		Guid IDataRecord.GetGuid(int i)
		{
			return this.Reader.GetGuid(i);
		}
        
		short IDataRecord.GetInt16(int i)
		{
			return this.Reader.GetInt16(i);
		}
        
		int IDataRecord.GetInt32(int i)
		{
			return this.Reader.GetInt32(i);
		}
		long IDataRecord.GetInt64(int i)
		{
			return this.Reader.GetInt64(i);
		}
        
		string IDataRecord.GetName(int i)
		{
			return this.Reader.GetName(i);
		}
        
		int IDataRecord.GetOrdinal(string name)
		{
			return this.Reader.GetOrdinal(name);
		}
        
		string IDataRecord.GetString(int i)
		{
			return this.Reader.GetString(i);
		}
        
		object IDataRecord.GetValue(int i)
		{
			return this.Reader.GetValue(i);
		}
        
		int IDataRecord.GetValues(object[] values)
		{
			return this.Reader.GetValues(values);
		}
        
		bool IDataRecord.IsDBNull(int i)
		{
			return this.Reader.IsDBNull(i);
		}
        
		object IDataRecord.this[string name]
		{
			get
			{
				return this.Reader[name];
			}
		}
        
		object IDataRecord.this[int i]
		{
			get
			{
				return this.Reader[i];
			}
		}
        
		private IDataReader reader;
		private IDbCommand cmd;
	}
}
