using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace NebimEnfotekIrsaliye.Data
{
	public class DynamicParameters : SqlMapper.IDynamicParameters, SqlMapper.IParameterLookup, SqlMapper.IParameterCallbacks
	{
		object SqlMapper.IParameterLookup.this[string member]
		{
			get
			{
				DynamicParameters.ParamInfo paramInfo;
				if (!this.parameters.TryGetValue(member, out paramInfo))
				{
					return null;
				}
				return paramInfo.Value;
			}
		}
		public DynamicParameters()
		{
			this.RemoveUnused = true;
		}
		public DynamicParameters(object template)
		{
			this.RemoveUnused = true;
			this.AddDynamicParams(template);
		}
		public void AddDynamicParams(object param)
		{
			if (param != null)
			{
				DynamicParameters dynamicParameters = param as DynamicParameters;
				if (dynamicParameters == null)
				{
					IEnumerable<KeyValuePair<string, object>> enumerable = param as IEnumerable<KeyValuePair<string, object>>;
					if (enumerable == null)
					{
						this.templates = (this.templates ?? new List<object>());
						this.templates.Add(param);
						return;
					}
					using (IEnumerator<KeyValuePair<string, object>> enumerator = enumerable.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, object> keyValuePair = enumerator.Current;
							this.Add(keyValuePair.Key, keyValuePair.Value, null, null, null);
						}
						return;
					}
				}
				if (dynamicParameters.parameters != null)
				{
					foreach (KeyValuePair<string, DynamicParameters.ParamInfo> keyValuePair2 in dynamicParameters.parameters)
					{
						this.parameters.Add(keyValuePair2.Key, keyValuePair2.Value);
					}
				}
				if (dynamicParameters.templates != null)
				{
					this.templates = (this.templates ?? new List<object>());
					foreach (object item in dynamicParameters.templates)
					{
						this.templates.Add(item);
					}
				}
			}
		}
		public void Add(string name, object value = null, DbType? dbType = null, ParameterDirection? direction = null, int? size = null)
		{
			this.parameters[DynamicParameters.Clean(name)] = new DynamicParameters.ParamInfo
			{
				Name = name,
				Value = value,
				ParameterDirection = (direction ?? ParameterDirection.Input),
				DbType = dbType,
				Size = size
			};
		}
		private static string Clean(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				char c = name[0];
				if (c == ':' || c == '?' || c == '@')
				{
					return name.Substring(1);
				}
			}
			return name;
		}
		void SqlMapper.IDynamicParameters.AddParameters(IDbCommand command, SqlMapper.Identity identity)
		{
			this.AddParameters(command, identity);
		}
        
		public bool RemoveUnused { get; set; }
        
		protected void AddParameters(IDbCommand command, SqlMapper.Identity identity)
		{
			IList<SqlMapper.LiteralToken> literalTokens = SqlMapper.GetLiteralTokens(identity.sql);
			if (this.templates != null)
			{
				foreach (object obj in this.templates)
				{
					SqlMapper.Identity identity2 = identity.ForDynamicParameters(obj.GetType());
					Dictionary<SqlMapper.Identity, Action<IDbCommand, object>> obj2 = DynamicParameters.paramReaderCache;
					Action<IDbCommand, object> action;
					lock (obj2)
					{
						if (!DynamicParameters.paramReaderCache.TryGetValue(identity2, out action))
						{
							action = SqlMapper.CreateParamInfoGenerator(identity2, true, this.RemoveUnused, literalTokens);
							DynamicParameters.paramReaderCache[identity2] = action;
						}
					}
					action(command, obj);
				}
				foreach (object obj3 in command.Parameters)
				{
					IDbDataParameter dbDataParameter = (IDbDataParameter)obj3;
					if (!this.parameters.ContainsKey(dbDataParameter.ParameterName))
					{
						this.parameters.Add(dbDataParameter.ParameterName, new DynamicParameters.ParamInfo
						{
							AttachedParam = dbDataParameter,
							CameFromTemplate = true,
							DbType = new DbType?(dbDataParameter.DbType),
							Name = dbDataParameter.ParameterName,
							ParameterDirection = dbDataParameter.Direction,
							Size = new int?(dbDataParameter.Size),
							Value = dbDataParameter.Value
						});
					}
				}
				List<Action> list = this.outputCallbacks;
				if (list != null)
				{
					foreach (Action action2 in list)
					{
						action2();
					}
				}
			}
			foreach (DynamicParameters.ParamInfo paramInfo in this.parameters.Values)
			{
				if (!paramInfo.CameFromTemplate)
				{
					DbType? dbType = paramInfo.DbType;
					object value = paramInfo.Value;
					string text = DynamicParameters.Clean(paramInfo.Name);
					bool flag2 = value is SqlMapper.ICustomQueryParameter;
					SqlMapper.ITypeHandler typeHandler = null;
					if (dbType == null && value != null && !flag2)
					{
						dbType = new DbType?(SqlMapper.LookupDbType(value.GetType(), text, true, out typeHandler));
					}
					if (dbType == (DbType)(-1))
					{
						SqlMapper.PackListParameters(command, text, value);
					}
					else if (flag2)
					{
						((SqlMapper.ICustomQueryParameter)value).AddParameter(command, text);
					}
					else
					{
						bool flag3 = !command.Parameters.Contains(text);
						IDbDataParameter dbDataParameter2;
						if (flag3)
						{
							dbDataParameter2 = command.CreateParameter();
							dbDataParameter2.ParameterName = text;
						}
						else
						{
							dbDataParameter2 = (IDbDataParameter)command.Parameters[text];
						}
						dbDataParameter2.Direction = paramInfo.ParameterDirection;
						if (typeHandler == null)
						{
							dbDataParameter2.Value = (value ?? DBNull.Value);
							if (dbType != null && dbDataParameter2.DbType != dbType)
							{
								dbDataParameter2.DbType = dbType.Value;
							}
							string text2 = value as string;
							if (text2 != null && text2.Length <= 4000)
							{
								dbDataParameter2.Size = 4000;
							}
							if (paramInfo.Size != null)
							{
								dbDataParameter2.Size = paramInfo.Size.Value;
							}
						}
						else
						{
							if (dbType != null)
							{
								dbDataParameter2.DbType = dbType.Value;
							}
							if (paramInfo.Size != null)
							{
								dbDataParameter2.Size = paramInfo.Size.Value;
							}
							typeHandler.SetValue(dbDataParameter2, value ?? DBNull.Value);
						}
						if (flag3)
						{
							command.Parameters.Add(dbDataParameter2);
						}
						paramInfo.AttachedParam = dbDataParameter2;
					}
				}
			}
			if (literalTokens.Count != 0)
			{
				SqlMapper.ReplaceLiterals(this, command, literalTokens);
			}
		}
		public IEnumerable<string> ParameterNames
		{
			get
			{
				return from p in this.parameters
				select p.Key;
			}
		}
		public T Get<T>(string name)
		{
			object value = this.parameters[DynamicParameters.Clean(name)].AttachedParam.Value;
			if (value != DBNull.Value)
			{
				return (T)((object)value);
			}
			if (default(T) != null)
			{
				throw new ApplicationException("Attempting to cast a DBNull to a non nullable type!");
			}
			return default(T);
		}
        
		public DynamicParameters Output<T>(T target, Expression<Func<T, object>> expression, DbType? dbType = null, int? size = null)
		{
			string failMessage = "Expression must be a property/field chain off of a(n) {0} instance";
			failMessage = string.Format(failMessage, typeof(T).Name);
			Action action = delegate()
			{
				throw new InvalidOperationException(failMessage);
			};
			MemberExpression lastMemberAccess = expression.Body as MemberExpression;
			if (lastMemberAccess == null || (lastMemberAccess.Member.MemberType != MemberTypes.Property && lastMemberAccess.Member.MemberType != MemberTypes.Field))
			{
				if (expression.Body.NodeType == ExpressionType.Convert && expression.Body.Type == typeof(object) && ((UnaryExpression)expression.Body).Operand is MemberExpression)
				{
					lastMemberAccess = (MemberExpression)((UnaryExpression)expression.Body).Operand;
				}
				else
				{
					action();
				}
			}
			MemberExpression memberExpression = lastMemberAccess;
			List<string> list = new List<string>();
			List<MemberExpression> list2 = new List<MemberExpression>();
			do
			{
				list.Insert(0, memberExpression.Member.Name);
				list2.Insert(0, memberExpression);
				ParameterExpression parameterExpression = memberExpression.Expression as ParameterExpression;
				memberExpression = (memberExpression.Expression as MemberExpression);
				if (parameterExpression != null && parameterExpression.Type == typeof(T))
				{
					break;
				}
				if (memberExpression == null || (memberExpression.Member.MemberType != MemberTypes.Property && memberExpression.Member.MemberType != MemberTypes.Field))
				{
					action();
				}
			}
			while (memberExpression != null);
			string dynamicParamName = string.Join(string.Empty, list.ToArray());
			string key = string.Join("|", list.ToArray());
			Hashtable cache = DynamicParameters.CachedOutputSetters<T>.Cache;
			Action<object, DynamicParameters> setter = (Action<object, DynamicParameters>)cache[key];
			if (setter == null)
			{
				DynamicMethod dynamicMethod = new DynamicMethod(string.Format("ExpressionParam{0}", Guid.NewGuid()), null, new Type[]
				{
					typeof(object),
					base.GetType()
				}, true);
				ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
				ilgenerator.Emit(OpCodes.Ldarg_0);
				ilgenerator.Emit(OpCodes.Castclass, typeof(T));
				for (int i = 0; i < list2.Count - 1; i++)
				{
					MemberInfo member = list2[0].Member;
					if (member.MemberType == MemberTypes.Property)
					{
						MethodInfo getMethod = ((PropertyInfo)member).GetGetMethod(true);
						ilgenerator.Emit(OpCodes.Callvirt, getMethod);
					}
					else
					{
						ilgenerator.Emit(OpCodes.Ldfld, (FieldInfo)member);
					}
				}
				MethodInfo meth = base.GetType().GetMethod("Get", new Type[]
				{
					typeof(string)
				}).MakeGenericMethod(new Type[]
				{
					lastMemberAccess.Type
				});
				ilgenerator.Emit(OpCodes.Ldarg_1);
				ilgenerator.Emit(OpCodes.Ldstr, dynamicParamName);
				ilgenerator.Emit(OpCodes.Callvirt, meth);
				MemberInfo member2 = lastMemberAccess.Member;
				if (member2.MemberType == MemberTypes.Property)
				{
					MethodInfo setMethod = ((PropertyInfo)member2).GetSetMethod(true);
					ilgenerator.Emit(OpCodes.Callvirt, setMethod);
				}
				else
				{
					ilgenerator.Emit(OpCodes.Stfld, (FieldInfo)member2);
				}
				ilgenerator.Emit(OpCodes.Ret);
				setter = (Action<object, DynamicParameters>)dynamicMethod.CreateDelegate(typeof(Action<object, DynamicParameters>));
				Hashtable obj = cache;
				lock (obj)
				{
					cache[key] = setter;
				}
			}
			List<Action> list3;
			if ((list3 = this.outputCallbacks) == null)
			{
				list3 = (this.outputCallbacks = new List<Action>());
			}
			list3.Add(delegate
			{
				Type type = lastMemberAccess.Type;
				int num = (size == null && type == typeof(string)) ? 4000 : (size ?? 0);
				DynamicParameters.ParamInfo paramInfo;
				if (this.parameters.TryGetValue(dynamicParamName, out paramInfo))
				{
					paramInfo.ParameterDirection = (paramInfo.AttachedParam.Direction = ParameterDirection.InputOutput);
					if (paramInfo.AttachedParam.Size == 0)
					{
						paramInfo.Size = new int?(paramInfo.AttachedParam.Size = num);
					}
				}
				else
				{
					SqlMapper.ITypeHandler typeHandler;
					dbType = ((dbType == null) ? new DbType?(SqlMapper.LookupDbType(type, type.Name, true, out typeHandler)) : dbType);
					this.Add(dynamicParamName, expression.Compile()(target), null, new ParameterDirection?(ParameterDirection.InputOutput), new int?(num));
				}
				paramInfo = this.parameters[dynamicParamName];
				paramInfo.OutputCallback = setter;
				paramInfo.OutputTarget = target;
			});
			return this;
		}
		void SqlMapper.IParameterCallbacks.OnCompleted()
		{
			foreach (DynamicParameters.ParamInfo paramInfo in this.parameters.Select(delegate(KeyValuePair<string, DynamicParameters.ParamInfo> p)
			{
				KeyValuePair<string, DynamicParameters.ParamInfo> keyValuePair = p;
				return keyValuePair.Value;
			}))
			{
				if (paramInfo.OutputCallback != null)
				{
					paramInfo.OutputCallback(paramInfo.OutputTarget, this);
				}
			}
		}
		internal const DbType EnumerableMultiParameter = (DbType)(-1);
		private static Dictionary<SqlMapper.Identity, Action<IDbCommand, object>> paramReaderCache = new Dictionary<SqlMapper.Identity, Action<IDbCommand, object>>();
        
		private Dictionary<string, DynamicParameters.ParamInfo> parameters = new Dictionary<string, DynamicParameters.ParamInfo>();
        
		private List<object> templates;
		private List<Action> outputCallbacks;
		private readonly Dictionary<string, Action<object, DynamicParameters>> cachedOutputSetters = new Dictionary<string, Action<object, DynamicParameters>>();
		private class ParamInfo
		{
			public string Name { get; set; }
			public object Value { get; set; }
			public ParameterDirection ParameterDirection { get; set; }
			public DbType? DbType { get; set; }
			public int? Size { get; set; }
			public IDbDataParameter AttachedParam { get; set; }
			internal Action<object, DynamicParameters> OutputCallback { get; set; }
			internal object OutputTarget { get; set; }
			internal bool CameFromTemplate { get; set; }
		}
		internal static class CachedOutputSetters<T>
		{
			public static readonly Hashtable Cache = new Hashtable();
		}
	}
}
