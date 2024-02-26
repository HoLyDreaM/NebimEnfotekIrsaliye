using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace NebimEnfotekIrsaliye.Data
{
	internal sealed class TableValuedParameter : SqlMapper.ICustomQueryParameter
	{
		public TableValuedParameter(DataTable table) : this(table, null)
		{
		}
		public TableValuedParameter(DataTable table, string typeName)
		{
			this.table = table;
			this.typeName = typeName;
		}
        
		static TableValuedParameter()
		{
			PropertyInfo property = typeof(SqlParameter).GetProperty("TypeName", BindingFlags.Instance | BindingFlags.Public);
			if (property != null && property.PropertyType == typeof(string) && property.CanWrite)
			{
				TableValuedParameter.setTypeName = (Action<SqlParameter, string>)Delegate.CreateDelegate(typeof(Action<SqlParameter, string>), property.GetSetMethod());
			}
		}
        
		void SqlMapper.ICustomQueryParameter.AddParameter(IDbCommand command, string name)
		{
			IDbDataParameter dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = name;
			TableValuedParameter.Set(dbDataParameter, this.table, this.typeName);
			command.Parameters.Add(dbDataParameter);
		}

        internal static void Set(IDbDataParameter parameter, DataTable table, string typeName)
        {
            parameter.Value = (object)table ?? (object)DBNull.Value;
            if (string.IsNullOrEmpty(typeName) && table != null)
                typeName = table.GetTypeName();
            if (string.IsNullOrEmpty(typeName))
                return;
            SqlParameter sqlParameter = parameter as SqlParameter;
            if (sqlParameter == null)
                return;
            if (TableValuedParameter.setTypeName != null)
                TableValuedParameter.setTypeName(sqlParameter, typeName);
            sqlParameter.SqlDbType = SqlDbType.Structured;
        }
        private readonly DataTable table;
		private readonly string typeName;
		private static readonly Action<SqlParameter, string> setTypeName;
	}
}
