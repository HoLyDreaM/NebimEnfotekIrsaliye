using System;
using System.Reflection;

namespace NebimEnfotekIrsaliye_SYS.Data
{
	public sealed class CustomPropertyTypeMap : SqlMapper.ITypeMap
	{
		public CustomPropertyTypeMap(Type type, Func<Type, string, PropertyInfo> propertySelector)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (propertySelector == null)
			{
				throw new ArgumentNullException("propertySelector");
			}
			this._type = type;
			this._propertySelector = propertySelector;
		}
		public ConstructorInfo FindConstructor(string[] names, Type[] types)
		{
			return this._type.GetConstructor(new Type[0]);
		}
		public ConstructorInfo FindExplicitConstructor()
		{
			return null;
		}
		public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
		{
			throw new NotSupportedException();
		}
		public SqlMapper.IMemberMap GetMember(string columnName)
		{
			PropertyInfo propertyInfo = this._propertySelector(this._type, columnName);
			if (!(propertyInfo != null))
			{
				return null;
			}
			return new SimpleMemberMap(columnName, propertyInfo);
		}
        
		private readonly Type _type;
        
		private readonly Func<Type, string, PropertyInfo> _propertySelector;
	}
}
