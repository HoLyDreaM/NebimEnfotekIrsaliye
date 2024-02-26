using System;
using System.Reflection;

namespace NebimEnfotekIrsaliye.Data
{
	public sealed class SimpleMemberMap : SqlMapper.IMemberMap
	{
		public SimpleMemberMap(string columnName, PropertyInfo property)
		{
			if (columnName == null)
			{
				throw new ArgumentNullException("columnName");
			}
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			this._columnName = columnName;
			this._property = property;
		}
		public SimpleMemberMap(string columnName, FieldInfo field)
		{
			if (columnName == null)
			{
				throw new ArgumentNullException("columnName");
			}
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}
			this._columnName = columnName;
			this._field = field;
		}
		public SimpleMemberMap(string columnName, ParameterInfo parameter)
		{
			if (columnName == null)
			{
				throw new ArgumentNullException("columnName");
			}
			if (parameter == null)
			{
				throw new ArgumentNullException("parameter");
			}
			this._columnName = columnName;
			this._parameter = parameter;
		}
		public string ColumnName
		{
			get
			{
				return this._columnName;
			}
		}
		public Type MemberType
		{
			get
			{
				if (this._field != null)
				{
					return this._field.FieldType;
				}
				if (this._property != null)
				{
					return this._property.PropertyType;
				}
				if (this._parameter != null)
				{
					return this._parameter.ParameterType;
				}
				return null;
			}
		}
		public PropertyInfo Property
		{
			get
			{
				return this._property;
			}
		}
		public FieldInfo Field
		{
			get
			{
				return this._field;
			}
		}
		public ParameterInfo Parameter
		{
			get
			{
				return this._parameter;
			}
		}

		private readonly string _columnName;
		private readonly PropertyInfo _property;
		private readonly FieldInfo _field;
		private readonly ParameterInfo _parameter;
	}
}
