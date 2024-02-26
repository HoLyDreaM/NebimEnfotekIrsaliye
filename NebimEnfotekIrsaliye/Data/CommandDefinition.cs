using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace NebimEnfotekIrsaliye.Data
{
	public struct CommandDefinition
	{
		internal static CommandDefinition ForCallback(object parameters)
		{
			if (parameters is DynamicParameters)
			{
				return new CommandDefinition(parameters);
			}
			return default(CommandDefinition);
		}
		internal void OnCompleted()
		{
			if (this.parameters is SqlMapper.IParameterCallbacks)
			{
				((SqlMapper.IParameterCallbacks)this.parameters).OnCompleted();
			}
		}
		public string CommandText
		{
			get
			{
				return this.commandText;
			}
		}
		public object Parameters
		{
			get
			{
				return this.parameters;
			}
		}
		public IDbTransaction Transaction
		{
			get
			{
				return this.transaction;
			}
		}
		public int? CommandTimeout
		{
			get
			{
				return this.commandTimeout;
			}
		}
		public CommandType? CommandType
		{
			get
			{
				return this.commandType;
			}
		}
		public bool Buffered
		{
			get
			{
				return (this.flags & CommandFlags.Buffered) > CommandFlags.None;
			}
		}
		internal bool AddToCache
		{
			get
			{
				return (this.flags & CommandFlags.NoCache) == CommandFlags.None;
			}
		}
		public CommandFlags Flags
		{
			get
			{
				return this.flags;
			}
		}
		public bool Pipelined
		{
			get
			{
				return (this.flags & CommandFlags.Pipelined) > CommandFlags.None;
			}
		}
		public CommandDefinition(string commandText, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandFlags flags = CommandFlags.Buffered)
		{
			this.commandText = commandText;
			this.parameters = parameters;
			this.transaction = transaction;
			this.commandTimeout = commandTimeout;
			this.commandType = commandType;
			this.flags = flags;
		}
		private CommandDefinition(object parameters)
		{
			this = default(CommandDefinition);
			this.parameters = parameters;
		}
		internal IDbCommand SetupCommand(IDbConnection cnn, Action<IDbCommand, object> paramReader)
		{
			IDbCommand dbCommand = cnn.CreateCommand();
			Action<IDbCommand> init = CommandDefinition.GetInit(dbCommand.GetType());
			if (init != null)
			{
				init(dbCommand);
			}
			if (this.transaction != null)
			{
				dbCommand.Transaction = this.transaction;
			}
			dbCommand.CommandText = this.commandText;
			if (this.commandTimeout != null)
			{
				dbCommand.CommandTimeout = this.commandTimeout.Value;
			}
			if (this.commandType != null)
			{
				dbCommand.CommandType = this.commandType.Value;
			}
			if (paramReader != null)
			{
				paramReader(dbCommand, this.parameters);
			}
			return dbCommand;
		}
		private static Action<IDbCommand> GetInit(Type commandType)
		{
			if (commandType == null)
			{
				return null;
			}
			Action<IDbCommand> result;
			if (SqlMapper.Link<Type, Action<IDbCommand>>.TryGet(CommandDefinition.commandInitCache, commandType, out result))
			{
				return result;
			}
			MethodInfo basicPropertySetter = CommandDefinition.GetBasicPropertySetter(commandType, "BindByName", typeof(bool));
			MethodInfo basicPropertySetter2 = CommandDefinition.GetBasicPropertySetter(commandType, "InitialLONGFetchSize", typeof(int));
			result = null;
			if (basicPropertySetter != null || basicPropertySetter2 != null)
			{
				DynamicMethod dynamicMethod = new DynamicMethod(commandType.Name + "_init", null, new Type[]
				{
					typeof(IDbCommand)
				});
				ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
				if (basicPropertySetter != null)
				{
					ilgenerator.Emit(OpCodes.Ldarg_0);
					ilgenerator.Emit(OpCodes.Castclass, commandType);
					ilgenerator.Emit(OpCodes.Ldc_I4_1);
					ilgenerator.EmitCall(OpCodes.Callvirt, basicPropertySetter, null);
				}
				if (basicPropertySetter2 != null)
				{
					ilgenerator.Emit(OpCodes.Ldarg_0);
					ilgenerator.Emit(OpCodes.Castclass, commandType);
					ilgenerator.Emit(OpCodes.Ldc_I4_M1);
					ilgenerator.EmitCall(OpCodes.Callvirt, basicPropertySetter2, null);
				}
				ilgenerator.Emit(OpCodes.Ret);
				result = (Action<IDbCommand>)dynamicMethod.CreateDelegate(typeof(Action<IDbCommand>));
			}
			SqlMapper.Link<Type, Action<IDbCommand>>.TryAdd(ref CommandDefinition.commandInitCache, commandType, ref result);
			return result;
		}
		private static MethodInfo GetBasicPropertySetter(Type declaringType, string name, Type expectedType)
		{
			PropertyInfo property = declaringType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
			ParameterInfo[] indexParameters;
			if (property != null && property.CanWrite && property.PropertyType == expectedType && ((indexParameters = property.GetIndexParameters()) == null || indexParameters.Length == 0))
			{
				return property.GetSetMethod();
			}
			return null;
		}
		private readonly string commandText;
		private readonly object parameters;
		private readonly IDbTransaction transaction;
		private readonly int? commandTimeout;
		private readonly CommandType? commandType;
		private readonly CommandFlags flags;
		private static SqlMapper.Link<Type, Action<IDbCommand>> commandInitCache;
	}
}
