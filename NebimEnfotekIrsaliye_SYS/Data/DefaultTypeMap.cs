using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NebimEnfotekIrsaliye_SYS.Data
{
	public sealed class DefaultTypeMap : SqlMapper.ITypeMap
	{
		public DefaultTypeMap(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this._fields = DefaultTypeMap.GetSettableFields(type);
			this._properties = DefaultTypeMap.GetSettableProps(type);
			this._type = type;
		}
		internal static MethodInfo GetPropertySetter(PropertyInfo propertyInfo, Type type)
		{
			if (!(propertyInfo.DeclaringType == type))
			{
				return propertyInfo.DeclaringType.GetProperty(propertyInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Type.DefaultBinder, propertyInfo.PropertyType, (from p in propertyInfo.GetIndexParameters()
				select p.ParameterType).ToArray<Type>(), null).GetSetMethod(true);
			}
			return propertyInfo.GetSetMethod(true);
		}
		internal static List<PropertyInfo> GetSettableProps(Type t)
		{
			return (from p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where DefaultTypeMap.GetPropertySetter(p, t) != null
			select p).ToList<PropertyInfo>();
		}
		internal static List<FieldInfo> GetSettableFields(Type t)
		{
			return t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList<FieldInfo>();
		}
        
		public ConstructorInfo FindConstructor(string[] names, Type[] types)
		{
			foreach (ConstructorInfo constructorInfo in this._type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).OrderBy(delegate(ConstructorInfo c)
			{
				if (c.IsPublic)
				{
					return 0;
				}
				if (!c.IsPrivate)
				{
					return 1;
				}
				return 2;
			}).ThenBy((ConstructorInfo c) => c.GetParameters().Length))
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				if (parameters.Length == 0)
				{
					return constructorInfo;
				}
				if (parameters.Length == types.Length)
				{
					int num = 0;
					while (num < parameters.Length && string.Equals(parameters[num].Name, names[num], StringComparison.OrdinalIgnoreCase))
					{
						if (!(types[num] == typeof(byte[])) || !(parameters[num].ParameterType.FullName == "System.Data.Linq.Binary"))
						{
							Type type = Nullable.GetUnderlyingType(parameters[num].ParameterType) ?? parameters[num].ParameterType;
							if (type != types[num] && (!type.IsEnum || !(Enum.GetUnderlyingType(type) == types[num])) && (!(type == typeof(char)) || !(types[num] == typeof(string))))
							{
								break;
							}
						}
						num++;
					}
					if (num == parameters.Length)
					{
						return constructorInfo;
					}
				}
			}
			return null;
		}
		public ConstructorInfo FindExplicitConstructor()
		{
			List<ConstructorInfo> list = (from c in this._type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where c.GetCustomAttributes(typeof(ExplicitConstructorAttribute), true).Length != 0
			select c).ToList<ConstructorInfo>();
			if (list.Count == 1)
			{
				return list[0];
			}
			return null;
		}
        
		public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
		{
			ParameterInfo[] parameters = constructor.GetParameters();
			return new SimpleMemberMap(columnName, parameters.FirstOrDefault((ParameterInfo p) => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase)));
		}
		public SqlMapper.IMemberMap GetMember(string columnName)
		{
			PropertyInfo propertyInfo = this._properties.FirstOrDefault((PropertyInfo p) => string.Equals(p.Name, columnName, StringComparison.Ordinal)) ?? this._properties.FirstOrDefault((PropertyInfo p) => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));
			if (propertyInfo == null && DefaultTypeMap.MatchNamesWithUnderscores)
			{
				propertyInfo = (this._properties.FirstOrDefault((PropertyInfo p) => string.Equals(p.Name, columnName.Replace("_", ""), StringComparison.Ordinal)) ?? this._properties.FirstOrDefault((PropertyInfo p) => string.Equals(p.Name, columnName.Replace("_", ""), StringComparison.OrdinalIgnoreCase)));
			}
			if (propertyInfo != null)
			{
				return new SimpleMemberMap(columnName, propertyInfo);
			}
			FieldInfo fieldInfo = this._fields.FirstOrDefault((FieldInfo p) => string.Equals(p.Name, columnName, StringComparison.Ordinal)) ?? this._fields.FirstOrDefault((FieldInfo p) => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));
			if (fieldInfo == null && DefaultTypeMap.MatchNamesWithUnderscores)
			{
				fieldInfo = (this._fields.FirstOrDefault((FieldInfo p) => string.Equals(p.Name, columnName.Replace("_", ""), StringComparison.Ordinal)) ?? this._fields.FirstOrDefault((FieldInfo p) => string.Equals(p.Name, columnName.Replace("_", ""), StringComparison.OrdinalIgnoreCase)));
			}
			if (fieldInfo != null)
			{
				return new SimpleMemberMap(columnName, fieldInfo);
			}
			return null;
		}
		public static bool MatchNamesWithUnderscores { get; set; }
		private readonly List<FieldInfo> _fields;
		private readonly List<PropertyInfo> _properties;
		private readonly Type _type;
	}
}
