using System;
using System.Data;

namespace NebimEnfotekIrsaliye_SYS.Data
{
	public sealed class DbString : SqlMapper.ICustomQueryParameter
	{
		public DbString()
		{
			this.Length = -1;
		}
		public bool IsAnsi { get; set; }
		public bool IsFixedLength { get; set; }
		public int Length { get; set; }
		public string Value { get; set; }
        public void AddParameter(IDbCommand command, string name)
        {
            if (this.IsFixedLength && this.Length == -1)
                throw new InvalidOperationException("If specifying IsFixedLength,  a Length must also be specified");
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = (object)this.Value ?? (object)DBNull.Value;
            parameter.Size = this.Length != -1 || this.Value == null || this.Value.Length > 4000 ? this.Length : 4000;
            parameter.DbType = this.IsAnsi ? (this.IsFixedLength ? DbType.AnsiStringFixedLength : DbType.AnsiString) : (this.IsFixedLength ? DbType.StringFixedLength : DbType.String);
            command.Parameters.Add((object)parameter);
        }
        public const int DefaultLength = 4000;
	}
}
