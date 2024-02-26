using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NebimEnfotekIrsaliye.Data
{
	public static class SqlMapper
	{
		private static int GetColumnHash(IDataReader reader)
		{
			int fieldCount = reader.FieldCount;
			int num = fieldCount;
			for (int i = 0; i < fieldCount; i++)
			{
				object name = reader.GetName(i);
				num = num * 31 + ((name == null) ? 0 : name.GetHashCode());
			}
			return num;
		}
        
		public static event EventHandler QueryCachePurged;
        
		private static void OnQueryCachePurged()
		{
			EventHandler queryCachePurged = SqlMapper.QueryCachePurged;
			if (queryCachePurged != null)
			{
				queryCachePurged(null, EventArgs.Empty);
			}
		}
        
		private static void SetQueryCache(SqlMapper.Identity key, SqlMapper.CacheInfo value)
		{
			if (Interlocked.Increment(ref SqlMapper.collect) == 1000)
			{
				SqlMapper.CollectCacheGarbage();
			}
			SqlMapper._queryCache[key] = value;
		}
		private static void CollectCacheGarbage()
		{
			try
			{
				foreach (KeyValuePair<SqlMapper.Identity, SqlMapper.CacheInfo> keyValuePair in SqlMapper._queryCache)
				{
					if (keyValuePair.Value.GetHitCount() <= 0)
					{
						SqlMapper.CacheInfo cacheInfo;
						SqlMapper._queryCache.TryRemove(keyValuePair.Key, out cacheInfo);
					}
				}
			}
			finally
			{
				Interlocked.Exchange(ref SqlMapper.collect, 0);
			}
		}
		private static bool TryGetQueryCache(SqlMapper.Identity key, out SqlMapper.CacheInfo value)
		{
			if (SqlMapper._queryCache.TryGetValue(key, out value))
			{
				value.RecordHit();
				return true;
			}
			value = null;
			return false;
		}
		public static void PurgeQueryCache()
		{
			SqlMapper._queryCache.Clear();
			SqlMapper.OnQueryCachePurged();
		}
		private static void PurgeQueryCacheByType(Type type)
		{
			foreach (KeyValuePair<SqlMapper.Identity, SqlMapper.CacheInfo> keyValuePair in SqlMapper._queryCache)
			{
				if (keyValuePair.Key.type == type)
				{
					SqlMapper.CacheInfo cacheInfo;
					SqlMapper._queryCache.TryRemove(keyValuePair.Key, out cacheInfo);
				}
			}
		}
		public static int GetCachedSQLCount()
		{
			return SqlMapper._queryCache.Count;
		}
        
		public static IEnumerable<Tuple<string, string, int>> GetCachedSQL(int ignoreHitCountAbove = 2147483647)
		{
			IEnumerable<Tuple<string, string, int>> enumerable = from pair in SqlMapper._queryCache
			select Tuple.Create<string, string, int>(pair.Key.connectionString, pair.Key.sql, pair.Value.GetHitCount());
			if (ignoreHitCountAbove < 2147483647)
			{
				enumerable = from tuple in enumerable
				where tuple.Item3 <= ignoreHitCountAbove
				select tuple;
			}
			return enumerable;
		}
		public static IEnumerable<Tuple<int, int>> GetHashCollissions()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (SqlMapper.Identity identity in SqlMapper._queryCache.Keys)
			{
				int num;
				if (!dictionary.TryGetValue(identity.hashCode, out num))
				{
					dictionary.Add(identity.hashCode, 1);
				}
				else
				{
					dictionary[identity.hashCode] = num + 1;
				}
			}
			return dictionary.Where(delegate(KeyValuePair<int, int> pair)
			{
				KeyValuePair<int, int> keyValuePair = pair;
				return keyValuePair.Value > 1;
			}).Select(delegate(KeyValuePair<int, int> pair)
			{
				KeyValuePair<int, int> keyValuePair = pair;
				int key = keyValuePair.Key;
				keyValuePair = pair;
				return Tuple.Create<int, int>(key, keyValuePair.Value);
			});
		}
		static SqlMapper()
		{
			SqlMapper.typeMap = new Dictionary<Type, DbType>();
			SqlMapper.typeMap[typeof(byte)] = DbType.Byte;
			SqlMapper.typeMap[typeof(sbyte)] = DbType.SByte;
			SqlMapper.typeMap[typeof(short)] = DbType.Int16;
			SqlMapper.typeMap[typeof(ushort)] = DbType.UInt16;
			SqlMapper.typeMap[typeof(int)] = DbType.Int32;
			SqlMapper.typeMap[typeof(uint)] = DbType.UInt32;
			SqlMapper.typeMap[typeof(long)] = DbType.Int64;
			SqlMapper.typeMap[typeof(ulong)] = DbType.UInt64;
			SqlMapper.typeMap[typeof(float)] = DbType.Single;
			SqlMapper.typeMap[typeof(double)] = DbType.Double;
			SqlMapper.typeMap[typeof(decimal)] = DbType.Decimal;
			SqlMapper.typeMap[typeof(bool)] = DbType.Boolean;
			SqlMapper.typeMap[typeof(string)] = DbType.String;
			SqlMapper.typeMap[typeof(char)] = DbType.StringFixedLength;
			SqlMapper.typeMap[typeof(Guid)] = DbType.Guid;
			SqlMapper.typeMap[typeof(DateTime)] = DbType.DateTime;
			SqlMapper.typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
			SqlMapper.typeMap[typeof(TimeSpan)] = DbType.Time;
			SqlMapper.typeMap[typeof(byte[])] = DbType.Binary;
			SqlMapper.typeMap[typeof(byte?)] = DbType.Byte;
			SqlMapper.typeMap[typeof(sbyte?)] = DbType.SByte;
			SqlMapper.typeMap[typeof(short?)] = DbType.Int16;
			SqlMapper.typeMap[typeof(ushort?)] = DbType.UInt16;
			SqlMapper.typeMap[typeof(int?)] = DbType.Int32;
			SqlMapper.typeMap[typeof(uint?)] = DbType.UInt32;
			SqlMapper.typeMap[typeof(long?)] = DbType.Int64;
			SqlMapper.typeMap[typeof(ulong?)] = DbType.UInt64;
			SqlMapper.typeMap[typeof(float?)] = DbType.Single;
			SqlMapper.typeMap[typeof(double?)] = DbType.Double;
			SqlMapper.typeMap[typeof(decimal?)] = DbType.Decimal;
			SqlMapper.typeMap[typeof(bool?)] = DbType.Boolean;
			SqlMapper.typeMap[typeof(char?)] = DbType.StringFixedLength;
			SqlMapper.typeMap[typeof(Guid?)] = DbType.Guid;
			SqlMapper.typeMap[typeof(DateTime?)] = DbType.DateTime;
			SqlMapper.typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
			SqlMapper.typeMap[typeof(TimeSpan?)] = DbType.Time;
			SqlMapper.typeMap[typeof(object)] = DbType.Object;
			SqlMapper.AddTypeHandlerImpl(typeof(DataTable), new DataTableHandler(), false);
		}
		public static void ResetTypeHandlers()
		{
			SqlMapper.typeHandlers = new Dictionary<Type, SqlMapper.ITypeHandler>();
			SqlMapper.AddTypeHandlerImpl(typeof(DataTable), new DataTableHandler(), true);
		}
        
		public static void AddTypeMap(Type type, DbType dbType)
		{
			Dictionary<Type, DbType> dictionary = SqlMapper.typeMap;
			DbType dbType2;
			if (dictionary.TryGetValue(type, out dbType2) && dbType2 == dbType)
			{
				return;
			}
			Dictionary<Type, DbType> dictionary2 = new Dictionary<Type, DbType>(dictionary);
			dictionary2[type] = dbType;
			SqlMapper.typeMap = dictionary2;
		}
		public static void AddTypeHandler(Type type, SqlMapper.ITypeHandler handler)
		{
			SqlMapper.AddTypeHandlerImpl(type, handler, true);
		}
        
		public static void AddTypeHandlerImpl(Type type, SqlMapper.ITypeHandler handler, bool clone)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Type type2 = null;
			if (type.IsValueType)
			{
				Type underlyingType = Nullable.GetUnderlyingType(type);
				if (underlyingType == null)
				{
					type2 = typeof(Nullable<>).MakeGenericType(new Type[]
					{
						type
					});
				}
				else
				{
					type2 = type;
					type = underlyingType;
				}
			}
			Dictionary<Type, SqlMapper.ITypeHandler> dictionary = SqlMapper.typeHandlers;
			SqlMapper.ITypeHandler typeHandler;
			if (dictionary.TryGetValue(type, out typeHandler) && handler == typeHandler)
			{
				return;
			}
			Dictionary<Type, SqlMapper.ITypeHandler> dictionary2 = clone ? new Dictionary<Type, SqlMapper.ITypeHandler>(dictionary) : dictionary;
			typeof(SqlMapper.TypeHandlerCache<>).MakeGenericType(new Type[]
			{
				type
			}).GetMethod("SetHandler", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[]
			{
				handler
			});
			if (type2 != null)
			{
				typeof(SqlMapper.TypeHandlerCache<>).MakeGenericType(new Type[]
				{
					type2
				}).GetMethod("SetHandler", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[]
				{
					handler
				});
			}
			if (handler == null)
			{
				dictionary2.Remove(type);
				if (type2 != null)
				{
					dictionary2.Remove(type2);
				}
			}
			else
			{
				dictionary2[type] = handler;
				if (type2 != null)
				{
					dictionary2[type2] = handler;
				}
			}
			SqlMapper.typeHandlers = dictionary2;
		}
		public static void AddTypeHandler<T>(SqlMapper.TypeHandler<T> handler)
		{
			SqlMapper.AddTypeHandlerImpl(typeof(T), handler, true);
		}
        
		[Obsolete("This method is for internal use only")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static DbType GetDbType(object value)
		{
			if (value == null || value is DBNull)
			{
				return DbType.Object;
			}
			SqlMapper.ITypeHandler typeHandler;
			return SqlMapper.LookupDbType(value.GetType(), "n/a", false, out typeHandler);
		}
        
		internal static DbType LookupDbType(Type type, string name, bool demand, out SqlMapper.ITypeHandler handler)
		{
			handler = null;
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
			}
			if (type.IsEnum && !SqlMapper.typeMap.ContainsKey(type))
			{
				type = Enum.GetUnderlyingType(type);
			}
			DbType result;
			if (SqlMapper.typeMap.TryGetValue(type, out result))
			{
				return result;
			}
			if (type.FullName == "System.Data.Linq.Binary")
			{
				return DbType.Binary;
			}
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				return (DbType)(-1);
			}
			if (SqlMapper.typeHandlers.TryGetValue(type, out handler))
			{
				return DbType.Object;
			}
			string fullName = type.FullName;
			if (fullName == "Microsoft.SqlServer.Types.SqlGeography")
			{
				Type type2 = type;
				SqlMapper.ITypeHandler handler2;
				handler = (handler2 = new SqlMapper.UdtTypeHandler("GEOGRAPHY"));
				SqlMapper.AddTypeHandler(type2, handler2);
				return DbType.Object;
			}
			if (fullName == "Microsoft.SqlServer.Types.SqlGeometry")
			{
				Type type3 = type;
				SqlMapper.ITypeHandler handler2;
				handler = (handler2 = new SqlMapper.UdtTypeHandler("GEOMETRY"));
				SqlMapper.AddTypeHandler(type3, handler2);
				return DbType.Object;
			}
			if (fullName == "Microsoft.SqlServer.Types.SqlHierarchyId")
			{
				Type type4 = type;
				SqlMapper.ITypeHandler handler2;
				handler = (handler2 = new SqlMapper.UdtTypeHandler("HIERARCHYID"));
				SqlMapper.AddTypeHandler(type4, handler2);
				return DbType.Object;
			}
			if (demand)
			{
				throw new NotSupportedException(string.Format("The member {0} of type {1} cannot be used as a parameter value", name, type.FullName));
			}
			return DbType.Object;
		}
		public static int Execute(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			CommandDefinition commandDefinition = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
			return cnn.ExecuteImpl(ref commandDefinition);
		}
        
		public static int Execute(this IDbConnection cnn, CommandDefinition command)
		{
			return cnn.ExecuteImpl(ref command);
		}
		public static object ExecuteScalar(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			CommandDefinition commandDefinition = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
			return SqlMapper.ExecuteScalarImpl<object>(cnn, ref commandDefinition);
		}
        
		public static T ExecuteScalar<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			CommandDefinition commandDefinition = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
			return SqlMapper.ExecuteScalarImpl<T>(cnn, ref commandDefinition);
		}
        
		public static object ExecuteScalar(this IDbConnection cnn, CommandDefinition command)
		{
			return SqlMapper.ExecuteScalarImpl<object>(cnn, ref command);
		}
        
		public static T ExecuteScalar<T>(this IDbConnection cnn, CommandDefinition command)
		{
			return SqlMapper.ExecuteScalarImpl<T>(cnn, ref command);
		}
		private static IEnumerable GetMultiExec(object param)
		{
			if (!(param is IEnumerable) || param is string || param is IEnumerable<KeyValuePair<string, object>>)
			{
				return null;
			}
			return (IEnumerable)param;
		}
		private static int ExecuteImpl(this IDbConnection cnn, ref CommandDefinition command)
		{
			object parameters = command.Parameters;
			IEnumerable multiExec = SqlMapper.GetMultiExec(parameters);
			SqlMapper.CacheInfo cacheInfo = null;
			if (multiExec != null)
			{
				bool flag = true;
				int num = 0;
				bool flag2 = cnn.State == ConnectionState.Closed;
				try
				{
					if (flag2)
					{
						cnn.Open();
					}
					using (IDbCommand dbCommand = command.SetupCommand(cnn, null))
					{
						string commandText = null;
						foreach (object obj in multiExec)
						{
							if (flag)
							{
								commandText = dbCommand.CommandText;
								flag = false;
								cacheInfo = SqlMapper.GetCacheInfo(new SqlMapper.Identity(command.CommandText, new CommandType?(dbCommand.CommandType), cnn, null, obj.GetType(), null), obj, command.AddToCache);
							}
							else
							{
								dbCommand.CommandText = commandText;
								dbCommand.Parameters.Clear();
							}
							cacheInfo.ParamReader(dbCommand, obj);
							num += dbCommand.ExecuteNonQuery();
						}
					}
					command.OnCompleted();
				}
				finally
				{
					if (flag2)
					{
						cnn.Close();
					}
				}
				return num;
			}
			if (parameters != null)
			{
				cacheInfo = SqlMapper.GetCacheInfo(new SqlMapper.Identity(command.CommandText, command.CommandType, cnn, null, parameters.GetType(), null), parameters, command.AddToCache);
			}
			return SqlMapper.ExecuteCommand(cnn, ref command, (parameters == null) ? null : cacheInfo.ParamReader);
		}
        
		public static IDataReader ExecuteReader(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			CommandDefinition commandDefinition = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
			IDbCommand cmd;
			IDataReader reader = SqlMapper.ExecuteReaderImpl(cnn, ref commandDefinition, CommandBehavior.Default, out cmd);
			return new WrappedReader(cmd, reader);
		}
		public static IDataReader ExecuteReader(this IDbConnection cnn, CommandDefinition command)
		{
			IDbCommand cmd;
			IDataReader reader = SqlMapper.ExecuteReaderImpl(cnn, ref command, CommandBehavior.Default, out cmd);
			return new WrappedReader(cmd, reader);
		}
        public static IDataReader ExecuteReader(this IDbConnection cnn, CommandDefinition command, CommandBehavior commandBehavior)
        {
            IDbCommand cmd;
            IDataReader reader = SqlMapper.ExecuteReaderImpl(cnn, ref command, commandBehavior, out cmd);
            return (IDataReader)new WrappedReader(cmd, reader);
        }
        public static IEnumerable<object> Query(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return (IEnumerable<object>)cnn.Query<SqlMapper.DapperRow>(sql, param, transaction, buffered, commandTimeout, commandType);
        }

        public static IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            CommandDefinition command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            IEnumerable<T> source = cnn.QueryImpl<T>(command, typeof(T));
            if (!command.Buffered)
                return source;
            return (IEnumerable<T>)source.ToList<T>();
        }

        public static IEnumerable<object> Query(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == (Type)null)
                throw new ArgumentNullException(nameof(type));
            CommandDefinition command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            IEnumerable<object> source = cnn.QueryImpl<object>(command, type);
            if (!command.Buffered)
                return source;
            return (IEnumerable<object>)source.ToList<object>();
        }

        public static IEnumerable<T> Query<T>(this IDbConnection cnn, CommandDefinition command)
        {
            IEnumerable<T> source = cnn.QueryImpl<T>(command, typeof(T));
            if (!command.Buffered)
                return source;
            return (IEnumerable<T>)source.ToList<T>();
        }

        public static SqlMapper.GridReader QueryMultiple(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            CommandDefinition command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
            return cnn.QueryMultipleImpl(ref command);
        }

        public static SqlMapper.GridReader QueryMultiple(this IDbConnection cnn, CommandDefinition command)
        {
            return cnn.QueryMultipleImpl(ref command);
        }

        private static SqlMapper.GridReader QueryMultipleImpl(this IDbConnection cnn, ref CommandDefinition command)
		{
			object parameters = command.Parameters;
			SqlMapper.Identity identity = new SqlMapper.Identity(command.CommandText, command.CommandType, cnn, typeof(SqlMapper.GridReader), (parameters == null) ? null : parameters.GetType(), null);
			SqlMapper.CacheInfo cacheInfo = SqlMapper.GetCacheInfo(identity, parameters, command.AddToCache);
			IDbCommand dbCommand = null;
			IDataReader dataReader = null;
			bool flag = cnn.State == ConnectionState.Closed;
			SqlMapper.GridReader result;
			try
			{
				if (flag)
				{
					cnn.Open();
				}
				dbCommand = command.SetupCommand(cnn, cacheInfo.ParamReader);
				dataReader = dbCommand.ExecuteReader(flag ? (CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection) : CommandBehavior.SequentialAccess);
				SqlMapper.GridReader gridReader = new SqlMapper.GridReader(dbCommand, dataReader, identity, command.Parameters as DynamicParameters);
				dbCommand = null;
				flag = false;
				result = gridReader;
			}
			catch
			{
				if (dataReader != null)
				{
					if (!dataReader.IsClosed)
					{
						try
						{
							dbCommand.Cancel();
						}
						catch
						{
						}
					}
					dataReader.Dispose();
				}
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (flag)
				{
					cnn.Close();
				}
				throw;
			}
			return result;
		}

        private static IEnumerable<T> QueryImpl<T>(this IDbConnection cnn, CommandDefinition command, Type effectiveType)
        {
            object parameters = command.Parameters;
            SqlMapper.Identity identity = new SqlMapper.Identity(command.CommandText, command.CommandType, cnn, effectiveType, parameters == null ? (Type)null : parameters.GetType(), (Type[])null);
            SqlMapper.CacheInfo cacheInfo = SqlMapper.GetCacheInfo(identity, parameters, command.AddToCache);
            IDbCommand cmd = (IDbCommand)null;
            IDataReader reader = (IDataReader)null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            cmd = command.SetupCommand(cnn, cacheInfo.ParamReader);
            if (wasClosed)
                cnn.Open();
            reader = cmd.ExecuteReader(wasClosed ? CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection : CommandBehavior.SequentialAccess);
            wasClosed = false;
            SqlMapper.DeserializerState deserializerState = cacheInfo.Deserializer;
            int columnHash = SqlMapper.GetColumnHash(reader);
            if (deserializerState.Func == null || deserializerState.Hash != columnHash)
            {
                if (reader.FieldCount == 0)
                {
                    yield break;
                }
                else
                {
                    deserializerState = cacheInfo.Deserializer = new SqlMapper.DeserializerState(columnHash, SqlMapper.GetDeserializer(effectiveType, reader, 0, -1, false));
                    if (command.AddToCache)
                        SqlMapper.SetQueryCache(identity, cacheInfo);
                }
            }
            Func<IDataReader, object> func = deserializerState.Func;
            Type type = Nullable.GetUnderlyingType(effectiveType);
            if ((object)type == null)
                type = effectiveType;
            Type convertToType = type;
            while (reader.Read())
            {
                object obj = func(reader);
                if (obj == null || obj is T)
                    yield return (T)obj;
                else
                    yield return (T)Convert.ChangeType(obj, convertToType, (IFormatProvider)CultureInfo.InvariantCulture);
            }
            do
                ;
            while (reader.NextResult());
            reader.Dispose();
            reader = (IDataReader)null;
            command.OnCompleted();
            func = (Func<IDataReader, object>)null;
            convertToType = (Type)null;
        }
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return cnn.MultiMap<TFirst, TSecond, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, TReturn>(sql, (Delegate)map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return cnn.MultiMap<TFirst, TSecond, TThird, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, TReturn>(sql, (Delegate)map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return cnn.MultiMap<TFirst, TSecond, TThird, TFourth, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, TReturn>(sql, (Delegate)map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return cnn.MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, SqlMapper.DontMap, SqlMapper.DontMap, TReturn>(sql, (Delegate)map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return cnn.MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, SqlMapper.DontMap, TReturn>(sql, (Delegate)map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return cnn.MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(sql, (Delegate)map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }

        public static IEnumerable<TReturn> Query<TReturn>(this IDbConnection cnn, string sql, Type[] types, Func<object[], TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            CommandDefinition command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            IEnumerable<TReturn> source = cnn.MultiMapImpl<TReturn>(command, types, map, splitOn, (IDataReader)null, (SqlMapper.Identity)null, true);
            if (!buffered)
                return source;
            return (IEnumerable<TReturn>)source.ToList<TReturn>();
        }

        private static IEnumerable<TReturn> MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, string sql, Delegate map, object param, IDbTransaction transaction, bool buffered, string splitOn, int? commandTimeout, CommandType? commandType)
        {
            CommandDefinition command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            IEnumerable<TReturn> source = cnn.MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(command, map, splitOn, (IDataReader)null, (SqlMapper.Identity)null, true);
            if (!buffered)
                return source;
            return (IEnumerable<TReturn>)source.ToList<TReturn>();
        }

        private static IEnumerable<TReturn> MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, CommandDefinition command, Delegate map, string splitOn, IDataReader reader, SqlMapper.Identity identity, bool finalize)
        {
            object parameters = command.Parameters;
            SqlMapper.Identity identity1 = identity;
            if (identity1 == null)
                identity1 = new SqlMapper.Identity(command.CommandText, command.CommandType, cnn, typeof(TFirst), parameters == null ? (Type)null : parameters.GetType(), new Type[7]
                {
          typeof (TFirst),
          typeof (TSecond),
          typeof (TThird),
          typeof (TFourth),
          typeof (TFifth),
          typeof (TSixth),
          typeof (TSeventh)
                });
            identity = identity1;
            SqlMapper.CacheInfo cacheInfo = SqlMapper.GetCacheInfo(identity, parameters, command.AddToCache);
            IDbCommand ownedCommand = (IDbCommand)null;
            IDataReader ownedReader = (IDataReader)null;
            bool wasClosed = cnn != null && cnn.State == ConnectionState.Closed;
            try
            {
                if (reader == null)
                {
                    ownedCommand = command.SetupCommand(cnn, cacheInfo.ParamReader);
                    if (wasClosed)
                        cnn.Open();
                    ownedReader = ownedCommand.ExecuteReader(wasClosed ? CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection : CommandBehavior.SequentialAccess);
                    reader = ownedReader;
                }
                SqlMapper.DeserializerState deserializerState1 = new SqlMapper.DeserializerState();
                int columnHash = SqlMapper.GetColumnHash(reader);
                SqlMapper.DeserializerState deserializerState2;
                Func<IDataReader, object>[] otherDeserializers;
                if ((deserializerState2 = cacheInfo.Deserializer).Func == null || (otherDeserializers = cacheInfo.OtherDeserializers) == null || columnHash != deserializerState2.Hash)
                {
                    Func<IDataReader, object>[] deserializers = SqlMapper.GenerateDeserializers(new Type[7]
                    {
            typeof (TFirst),
            typeof (TSecond),
            typeof (TThird),
            typeof (TFourth),
            typeof (TFifth),
            typeof (TSixth),
            typeof (TSeventh)
                    }, splitOn, reader);
                    deserializerState2 = cacheInfo.Deserializer = new SqlMapper.DeserializerState(columnHash, deserializers[0]);
                    otherDeserializers = cacheInfo.OtherDeserializers = ((IEnumerable<Func<IDataReader, object>>)deserializers).Skip<Func<IDataReader, object>>(1).ToArray<Func<IDataReader, object>>();
                    if (command.AddToCache)
                        SqlMapper.SetQueryCache(identity, cacheInfo);
                }
                Func<IDataReader, TReturn> mapIt = SqlMapper.GenerateMapper<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(deserializerState2.Func, otherDeserializers, (object)map);
                if (mapIt != null)
                {
                    while (reader.Read())
                        yield return mapIt(reader);
                    if (finalize)
                    {
                        do
                            ;
                        while (reader.NextResult());
                        command.OnCompleted();
                    }
                }
                mapIt = (Func<IDataReader, TReturn>)null;
            }
            finally
            {
                try
                {
                    ownedReader?.Dispose();
                }
                finally
                {
                    ownedCommand?.Dispose();
                    if (wasClosed)
                        cnn.Close();
                }
            }
        }

        private static IEnumerable<TReturn> MultiMapImpl<TReturn>(this IDbConnection cnn, CommandDefinition command, Type[] types, Func<object[], TReturn> map, string splitOn, IDataReader reader, SqlMapper.Identity identity, bool finalize)
        {
            if (types.Length < 1)
                throw new ArgumentException("you must provide at least one type to deserialize");
            object parameters = command.Parameters;
            identity = identity ?? new SqlMapper.Identity(command.CommandText, command.CommandType, cnn, types[0], parameters == null ? (Type)null : parameters.GetType(), types);
            SqlMapper.CacheInfo cacheInfo = SqlMapper.GetCacheInfo(identity, parameters, command.AddToCache);
            IDbCommand ownedCommand = (IDbCommand)null;
            IDataReader ownedReader = (IDataReader)null;
            bool wasClosed = cnn != null && cnn.State == ConnectionState.Closed;
            try
            {
                if (reader == null)
                {
                    ownedCommand = command.SetupCommand(cnn, cacheInfo.ParamReader);
                    if (wasClosed)
                        cnn.Open();
                    ownedReader = ownedCommand.ExecuteReader();
                    reader = ownedReader;
                }
                SqlMapper.DeserializerState deserializerState1 = new SqlMapper.DeserializerState();
                int columnHash = SqlMapper.GetColumnHash(reader);
                SqlMapper.DeserializerState deserializerState2;
                Func<IDataReader, object>[] otherDeserializers;
                if ((deserializerState2 = cacheInfo.Deserializer).Func == null || (otherDeserializers = cacheInfo.OtherDeserializers) == null || columnHash != deserializerState2.Hash)
                {
                    Func<IDataReader, object>[] deserializers = SqlMapper.GenerateDeserializers(types, splitOn, reader);
                    deserializerState2 = cacheInfo.Deserializer = new SqlMapper.DeserializerState(columnHash, deserializers[0]);
                    otherDeserializers = cacheInfo.OtherDeserializers = ((IEnumerable<Func<IDataReader, object>>)deserializers).Skip<Func<IDataReader, object>>(1).ToArray<Func<IDataReader, object>>();
                    SqlMapper.SetQueryCache(identity, cacheInfo);
                }
                Func<IDataReader, TReturn> mapIt = SqlMapper.GenerateMapper<TReturn>(types.Length, deserializerState2.Func, otherDeserializers, map);
                if (mapIt != null)
                {
                    while (reader.Read())
                        yield return mapIt(reader);
                    if (finalize)
                    {
                        do
                            ;
                        while (reader.NextResult());
                        command.OnCompleted();
                    }
                }
                mapIt = (Func<IDataReader, TReturn>)null;
            }
            finally
            {
                try
                {
                    ownedReader?.Dispose();
                }
                finally
                {
                    ownedCommand?.Dispose();
                    if (wasClosed)
                        cnn.Close();
                }
            }
        }

        private static Func<IDataReader, TReturn> GenerateMapper<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<IDataReader, object> deserializer, Func<IDataReader, object>[] otherDeserializers, object map)
		{
			switch (otherDeserializers.Length)
			{
			case 1:
				return (IDataReader r) => ((Func<TFirst, TSecond, TReturn>)map)((TFirst)((object)deserializer(r)), (TSecond)((object)otherDeserializers[0](r)));
			case 2:
				return (IDataReader r) => ((Func<TFirst, TSecond, TThird, TReturn>)map)((TFirst)((object)deserializer(r)), (TSecond)((object)otherDeserializers[0](r)), (TThird)((object)otherDeserializers[1](r)));
			case 3:
				return (IDataReader r) => ((Func<TFirst, TSecond, TThird, TFourth, TReturn>)map)((TFirst)((object)deserializer(r)), (TSecond)((object)otherDeserializers[0](r)), (TThird)((object)otherDeserializers[1](r)), (TFourth)((object)otherDeserializers[2](r)));
			case 4:
				return (IDataReader r) => ((Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>)map)((TFirst)((object)deserializer(r)), (TSecond)((object)otherDeserializers[0](r)), (TThird)((object)otherDeserializers[1](r)), (TFourth)((object)otherDeserializers[2](r)), (TFifth)((object)otherDeserializers[3](r)));
			case 5:
				return (IDataReader r) => ((Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>)map)((TFirst)((object)deserializer(r)), (TSecond)((object)otherDeserializers[0](r)), (TThird)((object)otherDeserializers[1](r)), (TFourth)((object)otherDeserializers[2](r)), (TFifth)((object)otherDeserializers[3](r)), (TSixth)((object)otherDeserializers[4](r)));
			case 6:
				return (IDataReader r) => ((Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>)map)((TFirst)((object)deserializer(r)), (TSecond)((object)otherDeserializers[0](r)), (TThird)((object)otherDeserializers[1](r)), (TFourth)((object)otherDeserializers[2](r)), (TFifth)((object)otherDeserializers[3](r)), (TSixth)((object)otherDeserializers[4](r)), (TSeventh)((object)otherDeserializers[5](r)));
			default:
				throw new NotSupportedException();
			}
		}
        
		private static Func<IDataReader, TReturn> GenerateMapper<TReturn>(int length, Func<IDataReader, object> deserializer, Func<IDataReader, object>[] otherDeserializers, Func<object[], TReturn> map)
		{
			return delegate(IDataReader r)
			{
				object[] array = new object[length];
				array[0] = deserializer(r);
				for (int i = 1; i < length; i++)
				{
					array[i] = otherDeserializers[i - 1](r);
				}
				return map(array);
			};
		}
		private static Func<IDataReader, object>[] GenerateDeserializers(Type[] types, string splitOn, IDataReader reader)
		{
			List<Func<IDataReader, object>> list = new List<Func<IDataReader, object>>();
			string[] array = (from s in splitOn.Split(new char[]
			{
				','
			})
			select s.Trim()).ToArray<string>();
			bool flag = array.Length > 1;
			if (types.First<Type>() == typeof(object))
			{
				bool flag2 = true;
				int num = 0;
				int num2 = 0;
				string splitOn2 = array[num2];
				foreach (Type type in types)
				{
					if (type == typeof(SqlMapper.DontMap))
					{
						break;
					}
					int nextSplitDynamic = SqlMapper.GetNextSplitDynamic(num, splitOn2, reader);
					if (flag && num2 < array.Length - 1)
					{
						splitOn2 = array[++num2];
					}
					list.Add(SqlMapper.GetDeserializer(type, reader, num, nextSplitDynamic - num, !flag2));
					num = nextSplitDynamic;
					flag2 = false;
				}
			}
			else
			{
				int num3 = reader.FieldCount;
				int num4 = array.Length - 1;
				string splitOn3 = array[num4];
				for (int j = types.Length - 1; j >= 0; j--)
				{
					Type type2 = types[j];
					if (!(type2 == typeof(SqlMapper.DontMap)))
					{
						int num5 = 0;
						if (j > 0)
						{
							num5 = SqlMapper.GetNextSplit(num3, splitOn3, reader);
							if (flag && num4 > 0)
							{
								splitOn3 = array[--num4];
							}
						}
						list.Add(SqlMapper.GetDeserializer(type2, reader, num5, num3 - num5, j > 0));
						num3 = num5;
					}
				}
				list.Reverse();
			}
			return list.ToArray();
		}
        
		private static int GetNextSplitDynamic(int startIdx, string splitOn, IDataReader reader)
		{
			if (startIdx == reader.FieldCount)
			{
				throw SqlMapper.MultiMapException(reader);
			}
			if (splitOn == "*")
			{
				return ++startIdx;
			}
			for (int i = startIdx + 1; i < reader.FieldCount; i++)
			{
				if (string.Equals(splitOn, reader.GetName(i), StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return reader.FieldCount;
		}
		private static int GetNextSplit(int startIdx, string splitOn, IDataReader reader)
		{
			if (splitOn == "*")
			{
				return --startIdx;
			}
			for (int i = startIdx - 1; i > 0; i--)
			{
				if (string.Equals(splitOn, reader.GetName(i), StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			throw SqlMapper.MultiMapException(reader);
		}
		private static SqlMapper.CacheInfo GetCacheInfo(SqlMapper.Identity identity, object exampleParameters, bool addToCache)
		{
			SqlMapper.CacheInfo cacheInfo;
			if (!SqlMapper.TryGetQueryCache(identity, out cacheInfo))
			{
				cacheInfo = new SqlMapper.CacheInfo();
				if (identity.parametersType != null)
				{
					Action<IDbCommand, object> action;
					if (exampleParameters is SqlMapper.IDynamicParameters)
					{
						action = delegate(IDbCommand cmd, object obj)
						{
							((SqlMapper.IDynamicParameters)obj).AddParameters(cmd, identity);
						};
					}
					else if (exampleParameters is IEnumerable<KeyValuePair<string, object>>)
					{
						action = delegate(IDbCommand cmd, object obj)
						{
							((SqlMapper.IDynamicParameters)new DynamicParameters(obj)).AddParameters(cmd, identity);
						};
					}
					else
					{
						IList<SqlMapper.LiteralToken> literals = SqlMapper.GetLiteralTokens(identity.sql);
						action = SqlMapper.CreateParamInfoGenerator(identity, false, true, literals);
					}
					if ((identity.commandType == null || identity.commandType == CommandType.Text) && SqlMapper.ShouldPassByPosition(identity.sql))
					{
						Action<IDbCommand, object> tail = action;
						string sql = identity.sql;
						action = delegate(IDbCommand cmd, object obj)
						{
							tail(cmd, obj);
							SqlMapper.PassByPosition(cmd);
						};
					}
					cacheInfo.ParamReader = action;
				}
				if (addToCache)
				{
					SqlMapper.SetQueryCache(identity, cacheInfo);
				}
			}
			return cacheInfo;
		}
		private static bool ShouldPassByPosition(string sql)
		{
			return sql != null && sql.IndexOf('?') >= 0 && SqlMapper.pseudoPositional.IsMatch(sql);
		}
		private static void PassByPosition(IDbCommand cmd)
		{
			if (cmd.Parameters.Count == 0)
			{
				return;
			}
			Dictionary<string, IDbDataParameter> parameters = new Dictionary<string, IDbDataParameter>(StringComparer.InvariantCulture);
			foreach (object obj in cmd.Parameters)
			{
				IDbDataParameter dbDataParameter = (IDbDataParameter)obj;
				if (!string.IsNullOrEmpty(dbDataParameter.ParameterName))
				{
					parameters[dbDataParameter.ParameterName] = dbDataParameter;
				}
			}
			HashSet<string> consumed = new HashSet<string>(StringComparer.InvariantCulture);
			bool firstMatch = true;
			cmd.CommandText = SqlMapper.pseudoPositional.Replace(cmd.CommandText, delegate(Match match)
			{
				string value = match.Groups[1].Value;
				if (!consumed.Add(value))
				{
					throw new InvalidOperationException("When passing parameters by position, each parameter can only be referenced once");
				}
				IDbDataParameter value2;
				if (parameters.TryGetValue(value, out value2))
				{
					if (firstMatch)
					{
						firstMatch = false;
						cmd.Parameters.Clear();
					}
					cmd.Parameters.Add(value2);
					parameters.Remove(value);
					consumed.Add(value);
					return "?";
				}
				return match.Value;
			});
		}
		private static Func<IDataReader, object> GetDeserializer(Type type, IDataReader reader, int startBound, int length, bool returnNullIfFirstMissing)
		{
			if (type == typeof(object) || type == typeof(SqlMapper.DapperRow))
			{
				return SqlMapper.GetDapperRowDeserializer(reader, startBound, length, returnNullIfFirstMissing);
			}
			Type type2 = null;
			if (SqlMapper.typeMap.ContainsKey(type) || type.IsEnum || type.FullName == "System.Data.Linq.Binary" || (type.IsValueType && (type2 = Nullable.GetUnderlyingType(type)) != null && type2.IsEnum))
			{
				return SqlMapper.GetStructDeserializer(type, type2 ?? type, startBound);
			}
			SqlMapper.ITypeHandler handler;
			if (SqlMapper.typeHandlers.TryGetValue(type, out handler))
			{
				return SqlMapper.GetHandlerDeserializer(handler, type, startBound);
			}
			return SqlMapper.GetTypeDeserializer(type, reader, startBound, length, returnNullIfFirstMissing);
		}
		private static Func<IDataReader, object> GetHandlerDeserializer(SqlMapper.ITypeHandler handler, Type type, int startBound)
		{
			return (IDataReader reader) => handler.Parse(type, reader.GetValue(startBound));
		}
		private static Exception MultiMapException(IDataRecord reader)
		{
			bool flag = false;
			try
			{
				flag = (reader != null && reader.FieldCount != 0);
			}
			catch
			{
			}
			if (flag)
			{
				return new ArgumentException("When using the multi-mapping APIs ensure you set the splitOn param if you have keys other than Id", "splitOn");
			}
			return new InvalidOperationException("No columns were selected");
		}
		internal static Func<IDataReader, object> GetDapperRowDeserializer(IDataRecord reader, int startBound, int length, bool returnNullIfFirstMissing)
		{
			int fieldCount = reader.FieldCount;
			if (length == -1)
			{
				length = fieldCount - startBound;
			}
			if (fieldCount <= startBound)
			{
				throw SqlMapper.MultiMapException(reader);
			}
			int effectiveFieldCount = Math.Min(fieldCount - startBound, length);
			SqlMapper.DapperTable table = null;
			return delegate(IDataReader r)
			{
				if (table == null)
				{
					string[] array = new string[effectiveFieldCount];
					for (int i = 0; i < effectiveFieldCount; i++)
					{
						array[i] = r.GetName(i + startBound);
					}
					table = new SqlMapper.DapperTable(array);
				}
				object[] array2 = new object[effectiveFieldCount];
				if (returnNullIfFirstMissing)
				{
					array2[0] = r.GetValue(startBound);
					if (array2[0] is DBNull)
					{
						return null;
					}
				}
				if (startBound == 0)
				{
					r.GetValues(array2);
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[j] is DBNull)
						{
							array2[j] = null;
						}
					}
				}
				else
				{
					for (int k = returnNullIfFirstMissing ? 1 : 0; k < effectiveFieldCount; k++)
					{
						object value = r.GetValue(k + startBound);
						array2[k] = ((value is DBNull) ? null : value);
					}
				}
				return new SqlMapper.DapperRow(table, array2);
			};
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is for internal usage only", false)]
		public static char ReadChar(object value)
		{
			if (value == null || value is DBNull)
			{
				throw new ArgumentNullException("value");
			}
			string text = value as string;
			if (text == null || text.Length != 1)
			{
				throw new ArgumentException("A single-character was expected", "value");
			}
			return text[0];
		}
        
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is for internal usage only", false)]
		public static char? ReadNullableChar(object value)
		{
			if (value == null || value is DBNull)
			{
				return null;
			}
			string text = value as string;
			if (text == null || text.Length != 1)
			{
				throw new ArgumentException("A single-character was expected", "value");
			}
			return new char?(text[0]);
		}
        
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is for internal usage only", true)]
		public static IDbDataParameter FindOrAddParameter(IDataParameterCollection parameters, IDbCommand command, string name)
		{
			IDbDataParameter dbDataParameter;
			if (parameters.Contains(name))
			{
				dbDataParameter = (IDbDataParameter)parameters[name];
			}
			else
			{
				dbDataParameter = command.CreateParameter();
				dbDataParameter.ParameterName = name;
				parameters.Add(dbDataParameter);
			}
			return dbDataParameter;
		}
        
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is for internal usage only", false)]
		public static void PackListParameters(IDbCommand command, string namePrefix, object value)
		{
			if (FeatureSupport.Get(command.Connection).Arrays)
			{
				IDbDataParameter dbDataParameter = command.CreateParameter();
				dbDataParameter.Value = (value ?? DBNull.Value);
				dbDataParameter.ParameterName = namePrefix;
				command.Parameters.Add(dbDataParameter);
				return;
			}
			IEnumerable enumerable = value as IEnumerable;
			int count = 0;
			bool flag = value is IEnumerable<string>;
			bool flag2 = value is IEnumerable<DbString>;
			foreach (object obj in enumerable)
			{
				int count2 = count;
				count = count2 + 1;
				IDbDataParameter dbDataParameter2 = command.CreateParameter();
				dbDataParameter2.ParameterName = namePrefix + count;
				if (flag)
				{
					dbDataParameter2.Size = 4000;
					if (obj != null && ((string)obj).Length > 4000)
					{
						dbDataParameter2.Size = -1;
					}
				}
				if (flag2 && obj is DbString)
				{
					(obj as DbString).AddParameter(command, dbDataParameter2.ParameterName);
				}
				else
				{
					dbDataParameter2.Value = (obj ?? DBNull.Value);
					command.Parameters.Add(dbDataParameter2);
				}
			}
			string pattern = "([?@:]" + Regex.Escape(namePrefix) + ")(?!\\w)(\\s+(?i)unknown(?-i))?";
			if (count == 0)
			{
				command.CommandText = Regex.Replace(command.CommandText, pattern, delegate(Match match)
				{
					string value2 = match.Groups[1].Value;
					if (match.Groups[2].Success)
					{
						return match.Value;
					}
					return "(SELECT " + value2 + " WHERE 1 = 0)";
				}, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
				IDbDataParameter dbDataParameter3 = command.CreateParameter();
				dbDataParameter3.ParameterName = namePrefix;
				dbDataParameter3.Value = DBNull.Value;
				command.Parameters.Add(dbDataParameter3);
				return;
			}
			command.CommandText = Regex.Replace(command.CommandText, pattern, delegate(Match match)
			{
				string value2 = match.Groups[1].Value;
				if (match.Groups[2].Success)
				{
					string value3 = match.Groups[2].Value;
					StringBuilder stringBuilder = SqlMapper.GetStringBuilder().Append(value2).Append(1).Append(value3);
					for (int i = 2; i <= count; i++)
					{
						stringBuilder.Append(',').Append(value2).Append(i).Append(value3);
					}
					return stringBuilder.__ToStringRecycle();
				}
				StringBuilder stringBuilder2 = SqlMapper.GetStringBuilder().Append('(').Append(value2).Append(1);
				for (int j = 2; j <= count; j++)
				{
					stringBuilder2.Append(',').Append(value2).Append(j);
				}
				return stringBuilder2.Append(')').__ToStringRecycle();
			}, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
		}
        
		private static IEnumerable<PropertyInfo> FilterParameters(IEnumerable<PropertyInfo> parameters, string sql)
		{
			return from p in parameters
			where Regex.IsMatch(sql, "[?@:]" + p.Name + "([^a-z0-9_]+|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant)
			select p;
		}
        
		public static void ReplaceLiterals(this SqlMapper.IParameterLookup parameters, IDbCommand command)
		{
			IList<SqlMapper.LiteralToken> list = SqlMapper.GetLiteralTokens(command.CommandText);
			if (list.Count != 0)
			{
				SqlMapper.ReplaceLiterals(parameters, command, list);
			}
		}

		[Obsolete("This is intended for internal usage only")]
		public static string Format(object value)
		{
			if (value == null)
			{
				return "null";
			}
			switch (Type.GetTypeCode(value.GetType()))
			{
			case TypeCode.DBNull:
				return "null";
			case TypeCode.Boolean:
				if (!(bool)value)
				{
					return "0";
				}
				return "1";
			case TypeCode.SByte:
				return ((sbyte)value).ToString(CultureInfo.InvariantCulture);
			case TypeCode.Byte:
				return ((byte)value).ToString(CultureInfo.InvariantCulture);
			case TypeCode.Int16:
				return ((short)value).ToString(CultureInfo.InvariantCulture);
			case TypeCode.UInt16:
				return ((ushort)value).ToString(CultureInfo.InvariantCulture);
			case TypeCode.Int32:
				return ((int)value).ToString(CultureInfo.InvariantCulture);
			case TypeCode.UInt32:
				return ((uint)value).ToString(CultureInfo.InvariantCulture);
			case TypeCode.Int64:
				return ((long)value).ToString(CultureInfo.InvariantCulture);
			case TypeCode.UInt64:
				return ((ulong)value).ToString(CultureInfo.InvariantCulture);
			case TypeCode.Single:
				return ((float)value).ToString(CultureInfo.InvariantCulture);
			case TypeCode.Double:
				return ((double)value).ToString(CultureInfo.InvariantCulture);
			case TypeCode.Decimal:
				return ((decimal)value).ToString(CultureInfo.InvariantCulture);
			}
			IEnumerable multiExec = SqlMapper.GetMultiExec(value);
			if (multiExec == null)
			{
				throw new NotSupportedException(value.GetType().Name);
			}
			StringBuilder stringBuilder = null;
			bool flag = true;
			foreach (object value2 in multiExec)
			{
				if (flag)
				{
					stringBuilder = SqlMapper.GetStringBuilder().Append('(');
					flag = false;
				}
				else
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(SqlMapper.Format(value2));
			}
			if (flag)
			{
				return "(select null where 1=0)";
			}
			return stringBuilder.Append(')').__ToStringRecycle();
		}
        
		internal static void ReplaceLiterals(SqlMapper.IParameterLookup parameters, IDbCommand command, IList<SqlMapper.LiteralToken> tokens)
		{
			string text = command.CommandText;
			foreach (SqlMapper.LiteralToken literalToken in tokens)
			{
				string newValue = SqlMapper.Format(parameters[literalToken.Member]);
				text = text.Replace(literalToken.Token, newValue);
			}
			command.CommandText = text;
		}
        
		internal static IList<SqlMapper.LiteralToken> GetLiteralTokens(string sql)
		{
			if (string.IsNullOrEmpty(sql))
			{
				return SqlMapper.LiteralToken.None;
			}
			if (!SqlMapper.literalTokens.IsMatch(sql))
			{
				return SqlMapper.LiteralToken.None;
			}
			MatchCollection matchCollection = SqlMapper.literalTokens.Matches(sql);
			HashSet<string> hashSet = new HashSet<string>(StringComparer.InvariantCulture);
			List<SqlMapper.LiteralToken> list = new List<SqlMapper.LiteralToken>(matchCollection.Count);
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				string value = match.Value;
				if (hashSet.Add(match.Value))
				{
					list.Add(new SqlMapper.LiteralToken(value, match.Groups[1].Value));
				}
			}
			if (list.Count != 0)
			{
				return list;
			}
			return SqlMapper.LiteralToken.None;
		}
        
		public static Action<IDbCommand, object> CreateParamInfoGenerator(SqlMapper.Identity identity, bool checkForDuplicates, bool removeUnused)
		{
			return SqlMapper.CreateParamInfoGenerator(identity, checkForDuplicates, removeUnused, SqlMapper.GetLiteralTokens(identity.sql));
		}
        
		internal static Action<IDbCommand, object> CreateParamInfoGenerator(SqlMapper.Identity identity, bool checkForDuplicates, bool removeUnused, IList<SqlMapper.LiteralToken> literals)
		{
			Type parametersType = identity.parametersType;
			bool flag = false;
			if (removeUnused && identity.commandType.GetValueOrDefault(CommandType.Text) == CommandType.Text)
			{
				flag = !SqlMapper.smellsLikeOleDb.IsMatch(identity.sql);
			}
			DynamicMethod dynamicMethod = new DynamicMethod(string.Format("ParamInfo{0}", Guid.NewGuid()), null, new Type[]
			{
				typeof(IDbCommand),
				typeof(object)
			}, parametersType, true);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			bool isValueType = parametersType.IsValueType;
			bool flag2 = false;
			ilgenerator.Emit(OpCodes.Ldarg_1);
			if (isValueType)
			{
				ilgenerator.DeclareLocal(parametersType.MakePointerType());
				ilgenerator.Emit(OpCodes.Unbox, parametersType);
			}
			else
			{
				ilgenerator.DeclareLocal(parametersType);
				ilgenerator.Emit(OpCodes.Castclass, parametersType);
			}
			ilgenerator.Emit(OpCodes.Stloc_0);
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.EmitCall(OpCodes.Callvirt, typeof(IDbCommand).GetProperty("Parameters").GetGetMethod(), null);
			PropertyInfo[] array = (from p in parametersType.GetProperties()
			where p.GetIndexParameters().Length == 0
			select p).ToArray<PropertyInfo>();
			ConstructorInfo[] constructors = parametersType.GetConstructors();
			IEnumerable<PropertyInfo> enumerable = null;
			ParameterInfo[] parameters;
			if (constructors.Length == 1 && array.Length == (parameters = constructors[0].GetParameters()).Length)
			{
				bool flag3 = true;
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.Equals(array[i].Name, parameters[i].Name, StringComparison.InvariantCultureIgnoreCase))
					{
						flag3 = false;
						break;
					}
				}
				if (flag3)
				{
					enumerable = array;
				}
				else
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
					foreach (ParameterInfo parameterInfo in parameters)
					{
						dictionary[parameterInfo.Name] = parameterInfo.Position;
					}
					if (dictionary.Count == array.Length)
					{
						int[] array3 = new int[array.Length];
						flag3 = true;
						for (int k = 0; k < array.Length; k++)
						{
							int num;
							if (!dictionary.TryGetValue(array[k].Name, out num))
							{
								flag3 = false;
								break;
							}
							array3[k] = num;
						}
						if (flag3)
						{
							Array.Sort<int, PropertyInfo>(array3, array);
							enumerable = array;
						}
					}
				}
			}
			if (enumerable == null)
			{
				enumerable = from x in array
				orderby x.Name
				select x;
			}
			if (flag)
			{
				enumerable = SqlMapper.FilterParameters(enumerable, identity.sql);
			}
			OpCode opcode = isValueType ? OpCodes.Call : OpCodes.Callvirt;
			foreach (PropertyInfo propertyInfo in enumerable)
			{
				if (typeof(SqlMapper.ICustomQueryParameter).IsAssignableFrom(propertyInfo.PropertyType))
				{
					ilgenerator.Emit(OpCodes.Ldloc_0);
					ilgenerator.Emit(opcode, propertyInfo.GetGetMethod());
					ilgenerator.Emit(OpCodes.Ldarg_0);
					ilgenerator.Emit(OpCodes.Ldstr, propertyInfo.Name);
					ilgenerator.EmitCall(OpCodes.Callvirt, propertyInfo.PropertyType.GetMethod("AddParameter"), null);
				}
				else
				{
					SqlMapper.ITypeHandler typeHandler;
					DbType dbType = SqlMapper.LookupDbType(propertyInfo.PropertyType, propertyInfo.Name, true, out typeHandler);
					if (dbType == (DbType)(-1))
					{
						ilgenerator.Emit(OpCodes.Ldarg_0);
						ilgenerator.Emit(OpCodes.Ldstr, propertyInfo.Name);
						ilgenerator.Emit(OpCodes.Ldloc_0);
						ilgenerator.Emit(opcode, propertyInfo.GetGetMethod());
						if (propertyInfo.PropertyType.IsValueType)
						{
							ilgenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
						}
						ilgenerator.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod("PackListParameters"), null);
					}
					else
					{
						ilgenerator.Emit(OpCodes.Dup);
						ilgenerator.Emit(OpCodes.Ldarg_0);
						if (checkForDuplicates)
						{
							ilgenerator.Emit(OpCodes.Ldstr, propertyInfo.Name);
							ilgenerator.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod("FindOrAddParameter"), null);
						}
						else
						{
							ilgenerator.EmitCall(OpCodes.Callvirt, typeof(IDbCommand).GetMethod("CreateParameter"), null);
							ilgenerator.Emit(OpCodes.Dup);
							ilgenerator.Emit(OpCodes.Ldstr, propertyInfo.Name);
							ilgenerator.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty("ParameterName").GetSetMethod(), null);
						}
						if (dbType != DbType.Time && typeHandler == null)
						{
							ilgenerator.Emit(OpCodes.Dup);
							if (dbType == DbType.Object && propertyInfo.PropertyType == typeof(object))
							{
								ilgenerator.Emit(OpCodes.Ldloc_0);
								ilgenerator.Emit(opcode, propertyInfo.GetGetMethod());
								ilgenerator.Emit(OpCodes.Call, typeof(SqlMapper).GetMethod("GetDbType", BindingFlags.Static | BindingFlags.Public));
							}
							else
							{
								SqlMapper.EmitInt32(ilgenerator, (int)dbType);
							}
							ilgenerator.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty("DbType").GetSetMethod(), null);
						}
						ilgenerator.Emit(OpCodes.Dup);
						SqlMapper.EmitInt32(ilgenerator, 1);
						ilgenerator.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty("Direction").GetSetMethod(), null);
						ilgenerator.Emit(OpCodes.Dup);
						ilgenerator.Emit(OpCodes.Ldloc_0);
						ilgenerator.Emit(opcode, propertyInfo.GetGetMethod());
						bool flag4 = true;
						if (propertyInfo.PropertyType.IsValueType)
						{
							ilgenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
							if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) == null)
							{
								flag4 = false;
							}
						}
						if (flag4)
						{
							if ((dbType == DbType.String || dbType == DbType.AnsiString) && !flag2)
							{
								ilgenerator.DeclareLocal(typeof(int));
								flag2 = true;
							}
							ilgenerator.Emit(OpCodes.Dup);
							Label label = ilgenerator.DefineLabel();
							Label? label2 = (dbType == DbType.String || dbType == DbType.AnsiString) ? new Label?(ilgenerator.DefineLabel()) : null;
							ilgenerator.Emit(OpCodes.Brtrue_S, label);
							ilgenerator.Emit(OpCodes.Pop);
							ilgenerator.Emit(OpCodes.Ldsfld, typeof(DBNull).GetField("Value"));
							if (dbType == DbType.String || dbType == DbType.AnsiString)
							{
								SqlMapper.EmitInt32(ilgenerator, 0);
								ilgenerator.Emit(OpCodes.Stloc_1);
							}
							if (label2 != null)
							{
								ilgenerator.Emit(OpCodes.Br_S, label2.Value);
							}
							ilgenerator.MarkLabel(label);
							if (propertyInfo.PropertyType == typeof(string))
							{
								ilgenerator.Emit(OpCodes.Dup);
								ilgenerator.EmitCall(OpCodes.Callvirt, typeof(string).GetProperty("Length").GetGetMethod(), null);
								SqlMapper.EmitInt32(ilgenerator, 4000);
								ilgenerator.Emit(OpCodes.Cgt);
								Label label3 = ilgenerator.DefineLabel();
								Label label4 = ilgenerator.DefineLabel();
								ilgenerator.Emit(OpCodes.Brtrue_S, label3);
								SqlMapper.EmitInt32(ilgenerator, 4000);
								ilgenerator.Emit(OpCodes.Br_S, label4);
								ilgenerator.MarkLabel(label3);
								SqlMapper.EmitInt32(ilgenerator, -1);
								ilgenerator.MarkLabel(label4);
								ilgenerator.Emit(OpCodes.Stloc_1);
							}
							if (propertyInfo.PropertyType.FullName == "System.Data.Linq.Binary")
							{
								ilgenerator.EmitCall(OpCodes.Callvirt, propertyInfo.PropertyType.GetMethod("ToArray", BindingFlags.Instance | BindingFlags.Public), null);
							}
							if (label2 != null)
							{
								ilgenerator.MarkLabel(label2.Value);
							}
						}
						if (typeHandler != null)
						{
							ilgenerator.Emit(OpCodes.Call, typeof(SqlMapper.TypeHandlerCache<>).MakeGenericType(new Type[]
							{
								propertyInfo.PropertyType
							}).GetMethod("SetValue"));
						}
						else
						{
							ilgenerator.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty("Value").GetSetMethod(), null);
						}
						if (propertyInfo.PropertyType == typeof(string))
						{
							Label label5 = ilgenerator.DefineLabel();
							ilgenerator.Emit(OpCodes.Ldloc_1);
							ilgenerator.Emit(OpCodes.Brfalse_S, label5);
							ilgenerator.Emit(OpCodes.Dup);
							ilgenerator.Emit(OpCodes.Ldloc_1);
							ilgenerator.EmitCall(OpCodes.Callvirt, typeof(IDbDataParameter).GetProperty("Size").GetSetMethod(), null);
							ilgenerator.MarkLabel(label5);
						}
						if (checkForDuplicates)
						{
							ilgenerator.Emit(OpCodes.Pop);
						}
						else
						{
							ilgenerator.EmitCall(OpCodes.Callvirt, typeof(IList).GetMethod("Add"), null);
							ilgenerator.Emit(OpCodes.Pop);
						}
					}
				}
			}
			ilgenerator.Emit(OpCodes.Pop);
			if (literals.Count != 0 && array != null)
			{
				ilgenerator.Emit(OpCodes.Ldarg_0);
				ilgenerator.Emit(OpCodes.Ldarg_0);
				PropertyInfo property = typeof(IDbCommand).GetProperty("CommandText");
				ilgenerator.EmitCall(OpCodes.Callvirt, property.GetGetMethod(), null);
				Dictionary<Type, LocalBuilder> dictionary2 = null;
				LocalBuilder localBuilder = null;
				foreach (SqlMapper.LiteralToken literalToken in literals)
				{
					PropertyInfo propertyInfo2 = null;
					PropertyInfo propertyInfo3 = null;
					string member = literalToken.Member;
					for (int l = 0; l < array.Length; l++)
					{
						string name = array[l].Name;
						if (string.Equals(name, member, StringComparison.InvariantCultureIgnoreCase))
						{
							propertyInfo3 = array[l];
							if (string.Equals(name, member, StringComparison.InvariantCulture))
							{
								propertyInfo2 = propertyInfo3;
								break;
							}
						}
					}
					PropertyInfo propertyInfo4 = propertyInfo2 ?? propertyInfo3;
					if (propertyInfo4 != null)
					{
						ilgenerator.Emit(OpCodes.Ldstr, literalToken.Token);
						ilgenerator.Emit(OpCodes.Ldloc_0);
						ilgenerator.EmitCall(opcode, propertyInfo4.GetGetMethod(), null);
						Type propertyType = propertyInfo4.PropertyType;
						TypeCode typeCode = Type.GetTypeCode(propertyType);
						if (typeCode == TypeCode.Boolean || typeCode - TypeCode.SByte <= 10)
						{
							MethodInfo toString = SqlMapper.GetToString(typeCode);
							if (localBuilder == null || localBuilder.LocalType != propertyType)
							{
								if (dictionary2 == null)
								{
									dictionary2 = new Dictionary<Type, LocalBuilder>();
									localBuilder = null;
								}
								else if (!dictionary2.TryGetValue(propertyType, out localBuilder))
								{
									localBuilder = null;
								}
								if (localBuilder == null)
								{
									localBuilder = ilgenerator.DeclareLocal(propertyType);
									dictionary2.Add(propertyType, localBuilder);
								}
							}
							ilgenerator.Emit(OpCodes.Stloc, localBuilder);
							ilgenerator.Emit(OpCodes.Ldloca, localBuilder);
							ilgenerator.EmitCall(OpCodes.Call, SqlMapper.InvariantCulture, null);
							ilgenerator.EmitCall(OpCodes.Call, toString, null);
						}
						else
						{
							if (propertyType.IsValueType)
							{
								ilgenerator.Emit(OpCodes.Box, propertyType);
							}
							ilgenerator.EmitCall(OpCodes.Call, SqlMapper.format, null);
						}
						ilgenerator.EmitCall(OpCodes.Callvirt, SqlMapper.StringReplace, null);
					}
				}
				ilgenerator.EmitCall(OpCodes.Callvirt, property.GetSetMethod(), null);
			}
			ilgenerator.Emit(OpCodes.Ret);
			return (Action<IDbCommand, object>)dynamicMethod.CreateDelegate(typeof(Action<IDbCommand, object>));
		}
		private static MethodInfo GetToString(TypeCode typeCode)
		{
			MethodInfo result;
			if (!SqlMapper.toStrings.TryGetValue(typeCode, out result))
			{
				return null;
			}
			return result;
		}
		private static int ExecuteCommand(IDbConnection cnn, ref CommandDefinition command, Action<IDbCommand, object> paramReader)
		{
			IDbCommand dbCommand = null;
			bool flag = cnn.State == ConnectionState.Closed;
			int result;
			try
			{
				dbCommand = command.SetupCommand(cnn, paramReader);
				if (flag)
				{
					cnn.Open();
				}
				int num = dbCommand.ExecuteNonQuery();
				command.OnCompleted();
				result = num;
			}
			finally
			{
				if (flag)
				{
					cnn.Close();
				}
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
			}
			return result;
		}
        
		private static T ExecuteScalarImpl<T>(IDbConnection cnn, ref CommandDefinition command)
		{
			Action<IDbCommand, object> paramReader = null;
			object parameters = command.Parameters;
			if (parameters != null)
			{
				paramReader = SqlMapper.GetCacheInfo(new SqlMapper.Identity(command.CommandText, command.CommandType, cnn, null, parameters.GetType(), null), command.Parameters, command.AddToCache).ParamReader;
			}
			IDbCommand dbCommand = null;
			bool flag = cnn.State == ConnectionState.Closed;
			object value;
			try
			{
				dbCommand = command.SetupCommand(cnn, paramReader);
				if (flag)
				{
					cnn.Open();
				}
				value = dbCommand.ExecuteScalar();
				command.OnCompleted();
			}
			finally
			{
				if (flag)
				{
					cnn.Close();
				}
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
			}
			return SqlMapper.Parse<T>(value);
		}
		private static IDataReader ExecuteReaderImpl(IDbConnection cnn, ref CommandDefinition command, CommandBehavior commandBehavior, out IDbCommand cmd)
		{
			Action<IDbCommand, object> parameterReader = SqlMapper.GetParameterReader(cnn, ref command);
			cmd = null;
			bool flag = cnn.State == ConnectionState.Closed;
			bool flag2 = true;
			IDataReader result;
			try
			{
				cmd = command.SetupCommand(cnn, parameterReader);
				if (flag)
				{
					cnn.Open();
				}
				if (flag)
				{
					commandBehavior |= CommandBehavior.CloseConnection;
				}
				IDataReader dataReader = cmd.ExecuteReader(commandBehavior);
				flag = false;
				flag2 = false;
				result = dataReader;
			}
			finally
			{
				if (flag)
				{
					cnn.Close();
				}
				if (cmd != null && flag2)
				{
					cmd.Dispose();
				}
			}
			return result;
		}
		private static Action<IDbCommand, object> GetParameterReader(IDbConnection cnn, ref CommandDefinition command)
		{
			object parameters = command.Parameters;
			bool multiExec = SqlMapper.GetMultiExec(parameters) != null;
			SqlMapper.CacheInfo cacheInfo = null;
			if (multiExec)
			{
				throw new NotSupportedException("MultiExec is not supported by ExecuteReader");
			}
			if (parameters != null)
			{
				cacheInfo = SqlMapper.GetCacheInfo(new SqlMapper.Identity(command.CommandText, command.CommandType, cnn, null, parameters.GetType(), null), parameters, command.AddToCache);
			}
			if (cacheInfo != null)
			{
				return cacheInfo.ParamReader;
			}
			return null;
		}
        
		private static Func<IDataReader, object> GetStructDeserializer(Type type, Type effectiveType, int index)
		{
			if (type == typeof(char))
			{
				return (IDataReader r) => SqlMapper.ReadChar(r.GetValue(index));
			}
			if (type == typeof(char?))
			{
				return (IDataReader r) => SqlMapper.ReadNullableChar(r.GetValue(index));
			}
			if (type.FullName == "System.Data.Linq.Binary")
			{
				return (IDataReader r) => Activator.CreateInstance(type, new object[]
				{
					r.GetValue(index)
				});
			}
			if (effectiveType.IsEnum)
			{
				return delegate(IDataReader r)
				{
					object obj = r.GetValue(index);
					if (obj is float || obj is double || obj is decimal)
					{
						obj = Convert.ChangeType(obj, Enum.GetUnderlyingType(effectiveType), CultureInfo.InvariantCulture);
					}
					if (!(obj is DBNull))
					{
						return Enum.ToObject(effectiveType, obj);
					}
					return null;
				};
			}
			SqlMapper.ITypeHandler handler;
			if (SqlMapper.typeHandlers.TryGetValue(type, out handler))
			{
				return delegate(IDataReader r)
				{
					object value = r.GetValue(index);
					if (!(value is DBNull))
					{
						return handler.Parse(type, value);
					}
					return null;
				};
			}
			return delegate(IDataReader r)
			{
				object value = r.GetValue(index);
				if (!(value is DBNull))
				{
					return value;
				}
				return null;
			};
		}
		private static T Parse<T>(object value)
		{
			if (value == null || value is DBNull)
			{
				return default(T);
			}
			if (value is T)
			{
				return (T)((object)value);
			}
			Type type = typeof(T);
			type = (Nullable.GetUnderlyingType(type) ?? type);
			if (type.IsEnum)
			{
				if (value is float || value is double || value is decimal)
				{
					value = Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture);
				}
				return (T)((object)Enum.ToObject(type, value));
			}
			SqlMapper.ITypeHandler typeHandler;
			if (SqlMapper.typeHandlers.TryGetValue(type, out typeHandler))
			{
				return (T)((object)typeHandler.Parse(type, value));
			}
			return (T)((object)Convert.ChangeType(value, type, CultureInfo.InvariantCulture));
		}
        
		public static SqlMapper.ITypeMap GetTypeMap(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			SqlMapper.ITypeMap typeMap = (SqlMapper.ITypeMap)SqlMapper._typeMaps[type];
			if (typeMap == null)
			{
				Hashtable typeMaps = SqlMapper._typeMaps;
				lock (typeMaps)
				{
					typeMap = (SqlMapper.ITypeMap)SqlMapper._typeMaps[type];
					if (typeMap == null)
					{
						typeMap = new DefaultTypeMap(type);
						SqlMapper._typeMaps[type] = typeMap;
					}
				}
			}
			return typeMap;
		}
        
		public static void SetTypeMap(Type type, SqlMapper.ITypeMap map)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Hashtable typeMaps;
			if (map == null || map is DefaultTypeMap)
			{
				typeMaps = SqlMapper._typeMaps;
				lock (typeMaps)
				{
					SqlMapper._typeMaps.Remove(type);
					goto IL_6E;
				}
			}
			typeMaps = SqlMapper._typeMaps;
			lock (typeMaps)
			{
				SqlMapper._typeMaps[type] = map;
			}
			IL_6E:
			SqlMapper.PurgeQueryCacheByType(type);
		}
        
		public static Func<IDataReader, object> GetTypeDeserializer(Type type, IDataReader reader, int startBound = 0, int length = -1, bool returnNullIfFirstMissing = false)
		{
			DynamicMethod dynamicMethod = new DynamicMethod(string.Format("Deserialize{0}", Guid.NewGuid()), typeof(object), new Type[]
			{
				typeof(IDataReader)
			}, true);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.DeclareLocal(typeof(int));
			ilgenerator.DeclareLocal(type);
			ilgenerator.Emit(OpCodes.Ldc_I4_0);
			ilgenerator.Emit(OpCodes.Stloc_0);
			if (length == -1)
			{
				length = reader.FieldCount - startBound;
			}
			if (reader.FieldCount <= startBound)
			{
				throw SqlMapper.MultiMapException(reader);
			}
			string[] names = (from i in Enumerable.Range(startBound, length)
			select reader.GetName(i)).ToArray<string>();
			SqlMapper.ITypeMap typeMap = SqlMapper.GetTypeMap(type);
			int num = startBound;
			ConstructorInfo specializedConstructor = null;
			bool flag = false;
			if (type.IsValueType)
			{
				ilgenerator.Emit(OpCodes.Ldloca_S, 1);
				ilgenerator.Emit(OpCodes.Initobj, type);
			}
			else
			{
				Type[] array = new Type[length];
				for (int k = startBound; k < startBound + length; k++)
				{
					array[k - startBound] = reader.GetFieldType(k);
				}
				ConstructorInfo constructorInfo = typeMap.FindExplicitConstructor();
				if (constructorInfo != null)
				{
					Dictionary<Type, LocalBuilder> dictionary = new Dictionary<Type, LocalBuilder>();
					foreach (ParameterInfo parameterInfo in constructorInfo.GetParameters())
					{
						if (!parameterInfo.ParameterType.IsValueType)
						{
							ilgenerator.Emit(OpCodes.Ldnull);
						}
						else
						{
							LocalBuilder localBuilder;
							if (!dictionary.TryGetValue(parameterInfo.ParameterType, out localBuilder))
							{
								localBuilder = (dictionary[parameterInfo.ParameterType] = ilgenerator.DeclareLocal(parameterInfo.ParameterType));
							}
							ilgenerator.Emit(OpCodes.Ldloca, (short)localBuilder.LocalIndex);
							ilgenerator.Emit(OpCodes.Initobj, parameterInfo.ParameterType);
							ilgenerator.Emit(OpCodes.Ldloca, (short)localBuilder.LocalIndex);
							ilgenerator.Emit(OpCodes.Ldobj, parameterInfo.ParameterType);
						}
					}
					ilgenerator.Emit(OpCodes.Newobj, constructorInfo);
					ilgenerator.Emit(OpCodes.Stloc_1);
					flag = typeof(ISupportInitialize).IsAssignableFrom(type);
					if (flag)
					{
						ilgenerator.Emit(OpCodes.Ldloc_1);
						ilgenerator.EmitCall(OpCodes.Callvirt, typeof(ISupportInitialize).GetMethod("BeginInit"), null);
					}
				}
				else
				{
					ConstructorInfo constructorInfo2 = typeMap.FindConstructor(names, array);
					if (constructorInfo2 == null)
					{
						string arg = "(" + string.Join(", ", array.Select((Type t, int i) => t.FullName + " " + names[i]).ToArray<string>()) + ")";
						throw new InvalidOperationException(string.Format("A parameterless default constructor or one matching signature {0} is required for {1} materialization", arg, type.FullName));
					}
					if (constructorInfo2.GetParameters().Length == 0)
					{
						ilgenerator.Emit(OpCodes.Newobj, constructorInfo2);
						ilgenerator.Emit(OpCodes.Stloc_1);
						flag = typeof(ISupportInitialize).IsAssignableFrom(type);
						if (flag)
						{
							ilgenerator.Emit(OpCodes.Ldloc_1);
							ilgenerator.EmitCall(OpCodes.Callvirt, typeof(ISupportInitialize).GetMethod("BeginInit"), null);
						}
					}
					else
					{
						specializedConstructor = constructorInfo2;
					}
				}
			}
			ilgenerator.BeginExceptionBlock();
			if (type.IsValueType)
			{
				ilgenerator.Emit(OpCodes.Ldloca_S, 1);
			}
			else if (specializedConstructor == null)
			{
				ilgenerator.Emit(OpCodes.Ldloc_1);
			}
			List<SqlMapper.IMemberMap> list = ((specializedConstructor != null) ? (from n in names
			select typeMap.GetConstructorParameter(specializedConstructor, n)) : names.Select((string n) => typeMap.GetMember(n))).ToList<SqlMapper.IMemberMap>();
			bool flag2 = true;
			Label label = ilgenerator.DefineLabel();
			int num2 = -1;
			int localIndex = ilgenerator.DeclareLocal(typeof(object)).LocalIndex;
			foreach (SqlMapper.IMemberMap memberMap in list)
			{
				if (memberMap != null)
				{
					if (specializedConstructor == null)
					{
						ilgenerator.Emit(OpCodes.Dup);
					}
					Label label2 = ilgenerator.DefineLabel();
					Label label3 = ilgenerator.DefineLabel();
					ilgenerator.Emit(OpCodes.Ldarg_0);
					SqlMapper.EmitInt32(ilgenerator, num);
					ilgenerator.Emit(OpCodes.Dup);
					ilgenerator.Emit(OpCodes.Stloc_0);
					ilgenerator.Emit(OpCodes.Callvirt, SqlMapper.getItem);
					ilgenerator.Emit(OpCodes.Dup);
					SqlMapper.StoreLocal(ilgenerator, localIndex);
					Type fieldType = reader.GetFieldType(num);
					Type memberType = memberMap.MemberType;
					if (memberType == typeof(char) || memberType == typeof(char?))
					{
						ilgenerator.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod((memberType == typeof(char)) ? "ReadChar" : "ReadNullableChar", BindingFlags.Static | BindingFlags.Public), null);
					}
					else
					{
						ilgenerator.Emit(OpCodes.Dup);
						ilgenerator.Emit(OpCodes.Isinst, typeof(DBNull));
						ilgenerator.Emit(OpCodes.Brtrue_S, label2);
						Type underlyingType = Nullable.GetUnderlyingType(memberType);
						Type type2 = (underlyingType != null && underlyingType.IsEnum) ? underlyingType : memberType;
						if (type2.IsEnum)
						{
							Type underlyingType2 = Enum.GetUnderlyingType(type2);
							if (fieldType == typeof(string))
							{
								if (num2 == -1)
								{
									num2 = ilgenerator.DeclareLocal(typeof(string)).LocalIndex;
								}
								ilgenerator.Emit(OpCodes.Castclass, typeof(string));
								SqlMapper.StoreLocal(ilgenerator, num2);
								ilgenerator.Emit(OpCodes.Ldtoken, type2);
								ilgenerator.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), null);
								SqlMapper.LoadLocal(ilgenerator, num2);
								ilgenerator.Emit(OpCodes.Ldc_I4_1);
								ilgenerator.EmitCall(OpCodes.Call, SqlMapper.enumParse, null);
								ilgenerator.Emit(OpCodes.Unbox_Any, type2);
							}
							else
							{
								SqlMapper.FlexibleConvertBoxedFromHeadOfStack(ilgenerator, fieldType, type2, underlyingType2);
							}
							if (underlyingType != null)
							{
								ilgenerator.Emit(OpCodes.Newobj, memberType.GetConstructor(new Type[]
								{
									underlyingType
								}));
							}
						}
						else if (memberType.FullName == "System.Data.Linq.Binary")
						{
							ilgenerator.Emit(OpCodes.Unbox_Any, typeof(byte[]));
							ilgenerator.Emit(OpCodes.Newobj, memberType.GetConstructor(new Type[]
							{
								typeof(byte[])
							}));
						}
						else
						{
							TypeCode typeCode = Type.GetTypeCode(fieldType);
							TypeCode typeCode2 = Type.GetTypeCode(type2);
							bool flag3;
							if ((flag3 = SqlMapper.typeHandlers.ContainsKey(type2)) || fieldType == type2 || typeCode == typeCode2 || typeCode == Type.GetTypeCode(underlyingType))
							{
								if (flag3)
								{
									ilgenerator.EmitCall(OpCodes.Call, typeof(SqlMapper.TypeHandlerCache<>).MakeGenericType(new Type[]
									{
										type2
									}).GetMethod("Parse"), null);
								}
								else
								{
									ilgenerator.Emit(OpCodes.Unbox_Any, type2);
								}
							}
							else
							{
								SqlMapper.FlexibleConvertBoxedFromHeadOfStack(ilgenerator, fieldType, underlyingType ?? type2, null);
								if (underlyingType != null)
								{
									ilgenerator.Emit(OpCodes.Newobj, type2.GetConstructor(new Type[]
									{
										underlyingType
									}));
								}
							}
						}
					}
					if (specializedConstructor == null)
					{
						if (memberMap.Property != null)
						{
							if (type.IsValueType)
							{
								ilgenerator.Emit(OpCodes.Call, DefaultTypeMap.GetPropertySetter(memberMap.Property, type));
							}
							else
							{
								ilgenerator.Emit(OpCodes.Callvirt, DefaultTypeMap.GetPropertySetter(memberMap.Property, type));
							}
						}
						else
						{
							ilgenerator.Emit(OpCodes.Stfld, memberMap.Field);
						}
					}
					ilgenerator.Emit(OpCodes.Br_S, label3);
					ilgenerator.MarkLabel(label2);
					if (specializedConstructor != null)
					{
						ilgenerator.Emit(OpCodes.Pop);
						if (memberMap.MemberType.IsValueType)
						{
							int localIndex2 = ilgenerator.DeclareLocal(memberMap.MemberType).LocalIndex;
							SqlMapper.LoadLocalAddress(ilgenerator, localIndex2);
							ilgenerator.Emit(OpCodes.Initobj, memberMap.MemberType);
							SqlMapper.LoadLocal(ilgenerator, localIndex2);
						}
						else
						{
							ilgenerator.Emit(OpCodes.Ldnull);
						}
					}
					else
					{
						ilgenerator.Emit(OpCodes.Pop);
						ilgenerator.Emit(OpCodes.Pop);
					}
					if (flag2 && returnNullIfFirstMissing)
					{
						ilgenerator.Emit(OpCodes.Pop);
						ilgenerator.Emit(OpCodes.Ldnull);
						ilgenerator.Emit(OpCodes.Stloc_1);
						ilgenerator.Emit(OpCodes.Br, label);
					}
					ilgenerator.MarkLabel(label3);
				}
				flag2 = false;
				num++;
			}
			if (type.IsValueType)
			{
				ilgenerator.Emit(OpCodes.Pop);
			}
			else
			{
				if (specializedConstructor != null)
				{
					ilgenerator.Emit(OpCodes.Newobj, specializedConstructor);
				}
				ilgenerator.Emit(OpCodes.Stloc_1);
				if (flag)
				{
					ilgenerator.Emit(OpCodes.Ldloc_1);
					ilgenerator.EmitCall(OpCodes.Callvirt, typeof(ISupportInitialize).GetMethod("EndInit"), null);
				}
			}
			ilgenerator.MarkLabel(label);
			ilgenerator.BeginCatchBlock(typeof(Exception));
			ilgenerator.Emit(OpCodes.Ldloc_0);
			ilgenerator.Emit(OpCodes.Ldarg_0);
			SqlMapper.LoadLocal(ilgenerator, localIndex);
			ilgenerator.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod("ThrowDataException"), null);
			ilgenerator.EndExceptionBlock();
			ilgenerator.Emit(OpCodes.Ldloc_1);
			if (type.IsValueType)
			{
				ilgenerator.Emit(OpCodes.Box, type);
			}
			ilgenerator.Emit(OpCodes.Ret);
			return (Func<IDataReader, object>)dynamicMethod.CreateDelegate(typeof(Func<IDataReader, object>));
		}
		private static void FlexibleConvertBoxedFromHeadOfStack(ILGenerator il, Type from, Type to, Type via)
		{
			if (from == (via ?? to))
			{
				il.Emit(OpCodes.Unbox_Any, to);
				return;
			}
			MethodInfo @operator;
			if ((@operator = SqlMapper.GetOperator(from, to)) != null)
			{
				il.Emit(OpCodes.Unbox_Any, from);
				il.Emit(OpCodes.Call, @operator);
				return;
			}
			bool flag = false;
			OpCode opcode = default(OpCode);
			TypeCode typeCode = Type.GetTypeCode(from);
			if (typeCode == TypeCode.Boolean || typeCode - TypeCode.SByte <= 9)
			{
				flag = true;
				switch (Type.GetTypeCode(via ?? to))
				{
				case TypeCode.Boolean:
				case TypeCode.Int32:
					opcode = OpCodes.Conv_Ovf_I4;
					goto IL_FE;
				case TypeCode.SByte:
					opcode = OpCodes.Conv_Ovf_I1;
					goto IL_FE;
				case TypeCode.Byte:
					opcode = OpCodes.Conv_Ovf_I1_Un;
					goto IL_FE;
				case TypeCode.Int16:
					opcode = OpCodes.Conv_Ovf_I2;
					goto IL_FE;
				case TypeCode.UInt16:
					opcode = OpCodes.Conv_Ovf_I2_Un;
					goto IL_FE;
				case TypeCode.UInt32:
					opcode = OpCodes.Conv_Ovf_I4_Un;
					goto IL_FE;
				case TypeCode.Int64:
					opcode = OpCodes.Conv_Ovf_I8;
					goto IL_FE;
				case TypeCode.UInt64:
					opcode = OpCodes.Conv_Ovf_I8_Un;
					goto IL_FE;
				case TypeCode.Single:
					opcode = OpCodes.Conv_R4;
					goto IL_FE;
				case TypeCode.Double:
					opcode = OpCodes.Conv_R8;
					goto IL_FE;
				}
				flag = false;
			}
			IL_FE:
			if (flag)
			{
				il.Emit(OpCodes.Unbox_Any, from);
				il.Emit(opcode);
				if (to == typeof(bool))
				{
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Ceq);
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Ceq);
					return;
				}
			}
			else
			{
				il.Emit(OpCodes.Ldtoken, via ?? to);
				il.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), null);
				il.EmitCall(OpCodes.Call, typeof(Convert).GetMethod("ChangeType", new Type[]
				{
					typeof(object),
					typeof(Type)
				}), null);
				il.Emit(OpCodes.Unbox_Any, to);
			}
		}
		private static MethodInfo GetOperator(Type from, Type to)
		{
			if (to == null)
			{
				return null;
			}
			MethodInfo[] methods;
			MethodInfo result;
			MethodInfo[] methods2;
			if ((result = SqlMapper.ResolveOperator(methods = from.GetMethods(BindingFlags.Static | BindingFlags.Public), from, to, "op_Implicit")) == null && (result = SqlMapper.ResolveOperator(methods2 = to.GetMethods(BindingFlags.Static | BindingFlags.Public), from, to, "op_Implicit")) == null)
			{
				result = (SqlMapper.ResolveOperator(methods, from, to, "op_Explicit") ?? SqlMapper.ResolveOperator(methods2, from, to, "op_Explicit"));
			}
			return result;
		}
		private static MethodInfo ResolveOperator(MethodInfo[] methods, Type from, Type to, string name)
		{
			for (int i = 0; i < methods.Length; i++)
			{
				if (!(methods[i].Name != name) && !(methods[i].ReturnType != to))
				{
					ParameterInfo[] parameters = methods[i].GetParameters();
					if (parameters.Length == 1 && !(parameters[0].ParameterType != from))
					{
						return methods[i];
					}
				}
			}
			return null;
		}
        
		private static void LoadLocal(ILGenerator il, int index)
		{
			if (index < 0 || index >= 32767)
			{
				throw new ArgumentNullException("index");
			}
			switch (index)
			{
			case 0:
				il.Emit(OpCodes.Ldloc_0);
				return;
			case 1:
				il.Emit(OpCodes.Ldloc_1);
				return;
			case 2:
				il.Emit(OpCodes.Ldloc_2);
				return;
			case 3:
				il.Emit(OpCodes.Ldloc_3);
				return;
			default:
				if (index <= 255)
				{
					il.Emit(OpCodes.Ldloc_S, (byte)index);
					return;
				}
				il.Emit(OpCodes.Ldloc, (short)index);
				return;
			}
		}
		private static void StoreLocal(ILGenerator il, int index)
		{
			if (index < 0 || index >= 32767)
			{
				throw new ArgumentNullException("index");
			}
			switch (index)
			{
			case 0:
				il.Emit(OpCodes.Stloc_0);
				return;
			case 1:
				il.Emit(OpCodes.Stloc_1);
				return;
			case 2:
				il.Emit(OpCodes.Stloc_2);
				return;
			case 3:
				il.Emit(OpCodes.Stloc_3);
				return;
			default:
				if (index <= 255)
				{
					il.Emit(OpCodes.Stloc_S, (byte)index);
					return;
				}
				il.Emit(OpCodes.Stloc, (short)index);
				return;
			}
		}
        
		private static void LoadLocalAddress(ILGenerator il, int index)
		{
			if (index < 0 || index >= 32767)
			{
				throw new ArgumentNullException("index");
			}
			if (index <= 255)
			{
				il.Emit(OpCodes.Ldloca_S, (byte)index);
				return;
			}
			il.Emit(OpCodes.Ldloca, (short)index);
		}
        
		[Obsolete("Intended for internal use only")]
		public static void ThrowDataException(Exception ex, int index, IDataReader reader, object value)
		{
			Exception ex3;
			try
			{
				string arg = "(n/a)";
				string arg2 = "(n/a)";
				if (reader != null && index >= 0 && index < reader.FieldCount)
				{
					arg = reader.GetName(index);
					try
					{
						if (value == null || value is DBNull)
						{
							arg2 = "<null>";
						}
						else
						{
							arg2 = Convert.ToString(value) + " - " + Type.GetTypeCode(value.GetType());
						}
					}
					catch (Exception ex2)
					{
						arg2 = ex2.Message;
					}
				}
				ex3 = new DataException(string.Format("Error parsing column {0} ({1}={2})", index, arg, arg2), ex);
			}
			catch
			{
				ex3 = new DataException(ex.Message, ex);
			}
			throw ex3;
		}
		private static void EmitInt32(ILGenerator il, int value)
		{
			switch (value)
			{
			case -1:
				il.Emit(OpCodes.Ldc_I4_M1);
				return;
			case 0:
				il.Emit(OpCodes.Ldc_I4_0);
				return;
			case 1:
				il.Emit(OpCodes.Ldc_I4_1);
				return;
			case 2:
				il.Emit(OpCodes.Ldc_I4_2);
				return;
			case 3:
				il.Emit(OpCodes.Ldc_I4_3);
				return;
			case 4:
				il.Emit(OpCodes.Ldc_I4_4);
				return;
			case 5:
				il.Emit(OpCodes.Ldc_I4_5);
				return;
			case 6:
				il.Emit(OpCodes.Ldc_I4_6);
				return;
			case 7:
				il.Emit(OpCodes.Ldc_I4_7);
				return;
			case 8:
				il.Emit(OpCodes.Ldc_I4_8);
				return;
			default:
				if (value >= -128 && value <= 127)
				{
					il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
					return;
				}
				il.Emit(OpCodes.Ldc_I4, value);
				return;
			}
		}
        
		public static IEqualityComparer<string> ConnectionStringComparer
		{
			get
			{
				return SqlMapper.connectionStringComparer;
			}
			set
			{
				SqlMapper.connectionStringComparer = (value ?? StringComparer.Ordinal);
			}
		}
		public static SqlMapper.ICustomQueryParameter AsTableValuedParameter(this DataTable table, string typeName = null)
		{
			return new TableValuedParameter(table, typeName);
		}
		public static void SetTypeName(this DataTable table, string typeName)
		{
			if (table != null)
			{
				if (string.IsNullOrEmpty(typeName))
				{
					table.ExtendedProperties.Remove("dapper:TypeName");
					return;
				}
				table.ExtendedProperties["dapper:TypeName"] = typeName;
			}
		}
		public static string GetTypeName(this DataTable table)
		{
			if (table != null)
			{
				return table.ExtendedProperties["dapper:TypeName"] as string;
			}
			return null;
		}
		private static StringBuilder GetStringBuilder()
		{
			StringBuilder stringBuilder = SqlMapper.perThreadStringBuilderCache;
			if (stringBuilder != null)
			{
				SqlMapper.perThreadStringBuilderCache = null;
				stringBuilder.Length = 0;
				return stringBuilder;
			}
			return new StringBuilder();
		}
        
		private static string __ToStringRecycle(this StringBuilder obj)
		{
			if (obj == null)
			{
				return "";
			}
			string result = obj.ToString();
			if (SqlMapper.perThreadStringBuilderCache == null)
			{
				SqlMapper.perThreadStringBuilderCache = obj;
			}
			return result;
		}
		private static readonly ConcurrentDictionary<SqlMapper.Identity, SqlMapper.CacheInfo> _queryCache = new ConcurrentDictionary<SqlMapper.Identity, SqlMapper.CacheInfo>();
		private const int COLLECT_PER_ITEMS = 1000;
		private const int COLLECT_HIT_COUNT_MIN = 0;
		private static int collect;
		private static Dictionary<Type, DbType> typeMap;
		private static Dictionary<Type, SqlMapper.ITypeHandler> typeHandlers = new Dictionary<Type, SqlMapper.ITypeHandler>();
		internal const string LinqBinary = "System.Data.Linq.Binary";
		private static readonly Regex smellsLikeOleDb = new Regex("(?<![a-z0-9@_])[?@:](?![a-z0-9@_])", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex literalTokens = new Regex("(?<![a-z0-9_])\\{=([a-z0-9_]+)\\}", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex pseudoPositional = new Regex("\\?([a-z_][a-z0-9_]*)\\?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		internal static readonly MethodInfo format = typeof(SqlMapper).GetMethod("Format", BindingFlags.Static | BindingFlags.Public);

		private static readonly Dictionary<TypeCode, MethodInfo> toStrings = new Type[]
		{
			typeof(bool),
			typeof(sbyte),
			typeof(byte),
			typeof(ushort),
			typeof(short),
			typeof(uint),
			typeof(int),
			typeof(ulong),
			typeof(long),
			typeof(float),
			typeof(double),
			typeof(decimal)
		}.ToDictionary((Type x) => Type.GetTypeCode(x), (Type x) => x.GetMethod("ToString", BindingFlags.Instance | BindingFlags.Public, null, new Type[]
		{
			typeof(IFormatProvider)
		}, null));
        
		private static readonly MethodInfo StringReplace = typeof(string).GetMethod("Replace", BindingFlags.Instance | BindingFlags.Public, null, new Type[]
		{
			typeof(string),
			typeof(string)
		}, null);
        
		private static readonly MethodInfo InvariantCulture = typeof(CultureInfo).GetProperty("InvariantCulture", BindingFlags.Static | BindingFlags.Public).GetGetMethod();
        
		private static readonly MethodInfo enumParse = typeof(Enum).GetMethod("Parse", new Type[]
		{
			typeof(Type),
			typeof(string),
			typeof(bool)
		});
        
		private static readonly MethodInfo getItem = (from p in typeof(IDataRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public)
		where p.GetIndexParameters().Any<ParameterInfo>() && p.GetIndexParameters()[0].ParameterType == typeof(int)
		select p.GetGetMethod()).First<MethodInfo>();

		private static readonly Hashtable _typeMaps = new Hashtable();

		private const string DataTableTypeNameKey = "dapper:TypeName";

		private static IEqualityComparer<string> connectionStringComparer = StringComparer.Ordinal;

		[ThreadStatic]
		private static StringBuilder perThreadStringBuilderCache;

		public interface IDynamicParameters
		{
			void AddParameters(IDbCommand command, SqlMapper.Identity identity);
		}
        
		public interface IParameterLookup : SqlMapper.IDynamicParameters
		{
			object this[string name]
			{
				get;
			}
		}
        
		public interface IParameterCallbacks : SqlMapper.IDynamicParameters
		{
			void OnCompleted();
		}
        
		[AssemblyNeutral]
		public interface ICustomQueryParameter
		{
			void AddParameter(IDbCommand command, string name);
		}
        
		[AssemblyNeutral]
		public interface ITypeHandler
		{
			void SetValue(IDbDataParameter parameter, object value);
            
			object Parse(Type destinationType, object value);
		}
        
		public class UdtTypeHandler : SqlMapper.ITypeHandler
		{
			public UdtTypeHandler(string udtTypeName)
			{
				if (string.IsNullOrEmpty(udtTypeName))
				{
					throw new ArgumentException("Cannot be null or empty", udtTypeName);
				}
				this.udtTypeName = udtTypeName;
			}
            
			object SqlMapper.ITypeHandler.Parse(Type destinationType, object value)
			{
				if (!(value is DBNull))
				{
					return value;
				}
				return null;
			}
			void SqlMapper.ITypeHandler.SetValue(IDbDataParameter parameter, object value)
			{
				parameter.Value = (value ?? DBNull.Value);
				if (parameter is SqlParameter)
				{
					((SqlParameter)parameter).UdtTypeName = this.udtTypeName;
				}
			}
			private readonly string udtTypeName;
		}
        
		public abstract class TypeHandler<T> : SqlMapper.ITypeHandler
		{
			public abstract void SetValue(IDbDataParameter parameter, T value);
			public abstract T Parse(object value);
			void SqlMapper.ITypeHandler.SetValue(IDbDataParameter parameter, object value)
			{
				if (value is DBNull)
				{
					parameter.Value = value;
					return;
				}
				this.SetValue(parameter, (T)((object)value));
			}
            
			object SqlMapper.ITypeHandler.Parse(Type destinationType, object value)
			{
				return this.Parse(value);
			}
		}
		public interface ITypeMap
		{
			ConstructorInfo FindConstructor(string[] names, Type[] types);
			ConstructorInfo FindExplicitConstructor();
			SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName);
			SqlMapper.IMemberMap GetMember(string columnName);
		}
        
		public interface IMemberMap
		{
			string ColumnName { get; }
			Type MemberType { get; }
			PropertyInfo Property { get; }
			FieldInfo Field { get; }
			ParameterInfo Parameter { get; }
		}
		internal class Link<TKey, TValue> where TKey : class
		{
			public static bool TryGet(SqlMapper.Link<TKey, TValue> link, TKey key, out TValue value)
			{
				while (link != null)
				{
					if (key == link.Key)
					{
						value = link.Value;
						return true;
					}
					link = link.Tail;
				}
				value = default(TValue);
				return false;
			}
			public static bool TryAdd(ref SqlMapper.Link<TKey, TValue> head, TKey key, ref TValue value)
			{
				TValue tvalue;
				for (;;)
				{
					SqlMapper.Link<TKey, TValue> link = Interlocked.CompareExchange<SqlMapper.Link<TKey, TValue>>(ref head, null, null);
					if (SqlMapper.Link<TKey, TValue>.TryGet(link, key, out tvalue))
					{
						break;
					}
					SqlMapper.Link<TKey, TValue> value2 = new SqlMapper.Link<TKey, TValue>(key, value, link);
					if (Interlocked.CompareExchange<SqlMapper.Link<TKey, TValue>>(ref head, value2, link) == link)
					{
						return true;
					}
				}
				value = tvalue;
				return false;
			}
			private Link(TKey key, TValue value, SqlMapper.Link<TKey, TValue> tail)
			{
				this.Key = key;
				this.Value = value;
				this.Tail = tail;
			}
			public TKey Key { get; private set; }
			public TValue Value { get; private set; }
			public SqlMapper.Link<TKey, TValue> Tail { get; private set; }
		}
		private class CacheInfo
		{
			public SqlMapper.DeserializerState Deserializer { get; set; }
			public Func<IDataReader, object>[] OtherDeserializers { get; set; }
			public Action<IDbCommand, object> ParamReader { get; set; }
			public int GetHitCount()
			{
				return Interlocked.CompareExchange(ref this.hitCount, 0, 0);
			}
            
			public void RecordHit()
			{
				Interlocked.Increment(ref this.hitCount);
			}
            
			private int hitCount;
		}
		private struct DeserializerState
		{
			public DeserializerState(int hash, Func<IDataReader, object> func)
			{
				this.Hash = hash;
				this.Func = func;
			}
			public readonly int Hash;
			public readonly Func<IDataReader, object> Func;
		}
        
		[Obsolete("Not intended for direct usage", false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static class TypeHandlerCache<T>
		{
			[Obsolete("Not intended for direct usage", true)]
			public static T Parse(object value)
			{
				return (T)((object)SqlMapper.TypeHandlerCache<T>.handler.Parse(typeof(T), value));
			}
			[Obsolete("Not intended for direct usage", true)]
			public static void SetValue(IDbDataParameter parameter, object value)
			{
				SqlMapper.TypeHandlerCache<T>.handler.SetValue(parameter, value);
			}
            
			internal static void SetHandler(SqlMapper.ITypeHandler handler)
			{
				SqlMapper.TypeHandlerCache<T>.handler = handler;
			}
            
			private static SqlMapper.ITypeHandler handler;
		}
        
		public class Identity : IEquatable<SqlMapper.Identity>
		{
			internal SqlMapper.Identity ForGrid(Type primaryType, int gridIndex)
			{
				return new SqlMapper.Identity(this.sql, this.commandType, this.connectionString, primaryType, this.parametersType, null, gridIndex);
			}
			internal SqlMapper.Identity ForGrid(Type primaryType, Type[] otherTypes, int gridIndex)
			{
				return new SqlMapper.Identity(this.sql, this.commandType, this.connectionString, primaryType, this.parametersType, otherTypes, gridIndex);
			}
            
			public SqlMapper.Identity ForDynamicParameters(Type type)
			{
				return new SqlMapper.Identity(this.sql, this.commandType, this.connectionString, this.type, type, null, -1);
			}
			internal Identity(string sql, CommandType? commandType, IDbConnection connection, Type type, Type parametersType, Type[] otherTypes) : this(sql, commandType, connection.ConnectionString, type, parametersType, otherTypes, 0)
			{
			}
			private Identity(string sql, CommandType? commandType, string connectionString, Type type, Type parametersType, Type[] otherTypes, int gridIndex)
			{
				this.sql = sql;
				this.commandType = commandType;
				this.connectionString = connectionString;
				this.type = type;
				this.parametersType = parametersType;
				this.gridIndex = gridIndex;
				this.hashCode = 17;
				this.hashCode = this.hashCode * 23 + commandType.GetHashCode();
				this.hashCode = this.hashCode * 23 + gridIndex.GetHashCode();
				this.hashCode = this.hashCode * 23 + ((sql == null) ? 0 : sql.GetHashCode());
				this.hashCode = this.hashCode * 23 + ((type == null) ? 0 : type.GetHashCode());
				if (otherTypes != null)
				{
					foreach (Type type2 in otherTypes)
					{
						this.hashCode = this.hashCode * 23 + ((type2 == null) ? 0 : type2.GetHashCode());
					}
				}
				this.hashCode = this.hashCode * 23 + ((connectionString == null) ? 0 : SqlMapper.connectionStringComparer.GetHashCode(connectionString));
				this.hashCode = this.hashCode * 23 + ((parametersType == null) ? 0 : parametersType.GetHashCode());
			}
			public override bool Equals(object obj)
			{
				return this.Equals(obj as SqlMapper.Identity);
			}
			public override int GetHashCode()
			{
				return this.hashCode;
			}
            
			public bool Equals(SqlMapper.Identity other)
			{
				return other != null && this.gridIndex == other.gridIndex && this.type == other.type && this.sql == other.sql && this.commandType == other.commandType && SqlMapper.connectionStringComparer.Equals(this.connectionString, other.connectionString) && this.parametersType == other.parametersType;
			}
            
			public readonly string sql;
            
			public readonly CommandType? commandType;
            
			public readonly int hashCode;
            
			public readonly int gridIndex;
            
			public readonly Type type;
            
			public readonly string connectionString;
            
			public readonly Type parametersType;
		}
		private class DontMap
		{
		}
        
		private sealed class DapperTable
		{
			internal string[] FieldNames
			{
				get
				{
					return this.fieldNames;
				}
			}
			public DapperTable(string[] fieldNames)
			{
				if (fieldNames == null)
				{
					throw new ArgumentNullException("fieldNames");
				}
				this.fieldNames = fieldNames;
				this.fieldNameLookup = new Dictionary<string, int>(fieldNames.Length, StringComparer.Ordinal);
				for (int i = fieldNames.Length - 1; i >= 0; i--)
				{
					string text = fieldNames[i];
					if (text != null)
					{
						this.fieldNameLookup[text] = i;
					}
				}
			}
			internal int IndexOfName(string name)
			{
				int result;
				if (name == null || !this.fieldNameLookup.TryGetValue(name, out result))
				{
					return -1;
				}
				return result;
			}
            
			internal int AddField(string name)
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				if (this.fieldNameLookup.ContainsKey(name))
				{
					throw new InvalidOperationException("Field already exists: " + name);
				}
				int num = this.fieldNames.Length;
				Array.Resize<string>(ref this.fieldNames, num + 1);
				this.fieldNames[num] = name;
				this.fieldNameLookup[name] = num;
				return num;
			}
            
			internal bool FieldExists(string key)
			{
				return key != null && this.fieldNameLookup.ContainsKey(key);
			}
            
			public int FieldCount
			{
				get
				{
					return this.fieldNames.Length;
				}
			}
            
			private string[] fieldNames;
			private readonly Dictionary<string, int> fieldNameLookup;
		}
		private sealed class DapperRowMetaObject : DynamicMetaObject
		{
			public DapperRowMetaObject(Expression expression, BindingRestrictions restrictions) : base(expression, restrictions)
			{
			}
			public DapperRowMetaObject(Expression expression, BindingRestrictions restrictions, object value) : base(expression, restrictions, value)
			{
			}
			private DynamicMetaObject CallMethod(MethodInfo method, Expression[] parameters)
			{
				return new DynamicMetaObject(Expression.Call(Expression.Convert(base.Expression, base.LimitType), method, parameters), BindingRestrictions.GetTypeRestriction(base.Expression, base.LimitType));
			}
			public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
			{
				Expression[] parameters = new Expression[]
				{
					Expression.Constant(binder.Name)
				};
				return this.CallMethod(SqlMapper.DapperRowMetaObject.getValueMethod, parameters);
			}
            
			public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
			{
				Expression[] parameters = new Expression[]
				{
					Expression.Constant(binder.Name)
				};
				return this.CallMethod(SqlMapper.DapperRowMetaObject.getValueMethod, parameters);
			}
            
			public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
			{
				Expression[] parameters = new Expression[]
				{
					Expression.Constant(binder.Name),
					value.Expression
				};
				return this.CallMethod(SqlMapper.DapperRowMetaObject.setValueMethod, parameters);
			}
            
			private static readonly MethodInfo getValueMethod = typeof(IDictionary<string, object>).GetProperty("Item").GetGetMethod();

			private static readonly MethodInfo setValueMethod = typeof(SqlMapper.DapperRow).GetMethod("SetValue", new Type[]
			{
				typeof(string),
				typeof(object)
			});
		}
        
		private sealed class DapperRow : IDynamicMetaObjectProvider, IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
		{
			public DapperRow(SqlMapper.DapperTable table, object[] values)
			{
				if (table == null)
				{
					throw new ArgumentNullException("table");
				}
				if (values == null)
				{
					throw new ArgumentNullException("values");
				}
				this.table = table;
				this.values = values;
			}
            
			int ICollection<KeyValuePair<string, object>>.Count
			{
				get
				{
					int num = 0;
					for (int i = 0; i < this.values.Length; i++)
					{
						if (!(this.values[i] is SqlMapper.DapperRow.DeadValue))
						{
							num++;
						}
					}
					return num;
				}
			}
			public bool TryGetValue(string name, out object value)
			{
				int num = this.table.IndexOfName(name);
				if (num < 0)
				{
					value = null;
					return false;
				}
				value = ((num < this.values.Length) ? this.values[num] : null);
				if (value is SqlMapper.DapperRow.DeadValue)
				{
					value = null;
					return false;
				}
				return true;
			}
            
			public override string ToString()
			{
				StringBuilder stringBuilder = SqlMapper.GetStringBuilder().Append("{DapperRow");
				foreach (KeyValuePair<string, object> keyValuePair in this)
				{
					bool value = keyValuePair.Value != null;
					stringBuilder.Append(", ").Append(keyValuePair.Key);
					if (value)
					{
						stringBuilder.Append(" = '").Append(keyValuePair.Value).Append('\'');
					}
					else
					{
						stringBuilder.Append(" = NULL");
					}
				}
				return stringBuilder.Append('}').__ToStringRecycle();
			}
            
			DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
			{
				return new SqlMapper.DapperRowMetaObject(parameter, BindingRestrictions.Empty, this);
			}
            
			public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
			{
				string[] names = this.table.FieldNames;
				int num;
				for (int i = 0; i < names.Length; i = num + 1)
				{
					object obj = (i < this.values.Length) ? this.values[i] : null;
					if (!(obj is SqlMapper.DapperRow.DeadValue))
					{
						yield return new KeyValuePair<string, object>(names[i], obj);
					}
					num = i;
				}
				yield break;
			}
            
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
            
			void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
			{
				((IDictionary<string, object>)this).Add(item.Key, item.Value);
			}
            
			void ICollection<KeyValuePair<string, object>>.Clear()
			{
				for (int i = 0; i < this.values.Length; i++)
				{
					this.values[i] = SqlMapper.DapperRow.DeadValue.Default;
				}
			}
            
			bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
			{
				object objA;
				return this.TryGetValue(item.Key, out objA) && object.Equals(objA, item.Value);
			}
			void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
			{
				foreach (KeyValuePair<string, object> keyValuePair in this)
				{
					array[arrayIndex++] = keyValuePair;
				}
			}
            
			bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
			{
				return ((IDictionary<string, object>)this).Remove(item.Key);
			}
            
			bool ICollection<KeyValuePair<string, object>>.IsReadOnly
			{
				get
				{
					return false;
				}
			}
			bool IDictionary<string, object>.ContainsKey(string key)
			{
				int num = this.table.IndexOfName(key);
				return num >= 0 && num < this.values.Length && !(this.values[num] is SqlMapper.DapperRow.DeadValue);
			}
			void IDictionary<string, object>.Add(string key, object value)
			{
				this.SetValue(key, value, true);
			}
            
			bool IDictionary<string, object>.Remove(string key)
			{
				int num = this.table.IndexOfName(key);
				if (num < 0 || num >= this.values.Length || this.values[num] is SqlMapper.DapperRow.DeadValue)
				{
					return false;
				}
				this.values[num] = SqlMapper.DapperRow.DeadValue.Default;
				return true;
			}
            
			object IDictionary<string, object>.this[string key]
			{
				get
				{
					object result;
					this.TryGetValue(key, out result);
					return result;
				}
				set
				{
					this.SetValue(key, value, false);
				}
			}
            
			public object SetValue(string key, object value)
			{
				return this.SetValue(key, value, false);
			}
			private object SetValue(string key, object value, bool isAdd)
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				int num = this.table.IndexOfName(key);
				if (num < 0)
				{
					num = this.table.AddField(key);
				}
				else if (isAdd && num < this.values.Length && !(this.values[num] is SqlMapper.DapperRow.DeadValue))
				{
					throw new ArgumentException("An item with the same key has already been added", "key");
				}
				int num2 = this.values.Length;
				if (num2 <= num)
				{
					Array.Resize<object>(ref this.values, this.table.FieldCount);
					for (int i = num2; i < this.values.Length; i++)
					{
						this.values[i] = SqlMapper.DapperRow.DeadValue.Default;
					}
				}
				this.values[num] = value;
				return value;
			}
            
			ICollection<string> IDictionary<string, object>.Keys
			{
				get
				{
					return (from kv in this
					select kv.Key).ToArray<string>();
				}
			}
            
			ICollection<object> IDictionary<string, object>.Values
			{
				get
				{
					return (from kv in this
					select kv.Value).ToArray<object>();
				}
			}
            
			private readonly SqlMapper.DapperTable table;
			private object[] values;
			private sealed class DeadValue
			{
				private DeadValue()
				{
				}
                
				public static readonly SqlMapper.DapperRow.DeadValue Default = new SqlMapper.DapperRow.DeadValue();
			}
		}
		internal struct LiteralToken
		{
			public string Token
			{
				get
				{
					return this.token;
				}
			}
            
			public string Member
			{
				get
				{
					return this.member;
				}
			}
			internal LiteralToken(string token, string member)
			{
				this.token = token;
				this.member = member;
			}
            
			private readonly string token;
			private readonly string member;
			internal static readonly IList<SqlMapper.LiteralToken> None = new SqlMapper.LiteralToken[0];
		}

        public class GridReader : IDisposable
        {
            private IDataReader reader;
            private IDbCommand command;
            private SqlMapper.Identity identity;
            private int gridIndex;
            private int readCount;
            private bool consumed;
            private SqlMapper.IParameterCallbacks callbacks;

            internal GridReader(IDbCommand command, IDataReader reader, SqlMapper.Identity identity, SqlMapper.IParameterCallbacks callbacks)
            {
                this.command = command;
                this.reader = reader;
                this.identity = identity;
                this.callbacks = callbacks;
            }

            public IEnumerable<object> Read(bool buffered = true)
            {
                return this.ReadImpl<object>(typeof(SqlMapper.DapperRow), buffered);
            }

            public IEnumerable<T> Read<T>(bool buffered = true)
            {
                return this.ReadImpl<T>(typeof(T), buffered);
            }

            public IEnumerable<object> Read(Type type, bool buffered = true)
            {
                if (type == (Type)null)
                    throw new ArgumentNullException(nameof(type));
                return this.ReadImpl<object>(type, buffered);
            }

            private IEnumerable<T> ReadImpl<T>(Type type, bool buffered)
            {
                if (this.reader == null)
                    throw new ObjectDisposedException(this.GetType().FullName, "The reader has been disposed; this can happen after all data has been consumed");
                if (this.consumed)
                    throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");
                SqlMapper.Identity identity = this.identity.ForGrid(type, this.gridIndex);
                SqlMapper.CacheInfo cacheInfo = SqlMapper.GetCacheInfo(identity, (object)null, true);
                SqlMapper.DeserializerState deserializerState = cacheInfo.Deserializer;
                int columnHash = SqlMapper.GetColumnHash(this.reader);
                if (deserializerState.Func == null || deserializerState.Hash != columnHash)
                {
                    deserializerState = new SqlMapper.DeserializerState(columnHash, SqlMapper.GetDeserializer(type, this.reader, 0, -1, false));
                    cacheInfo.Deserializer = deserializerState;
                }
                this.consumed = true;
                IEnumerable<T> source = this.ReadDeferred<T>(this.gridIndex, deserializerState.Func, identity);
                if (!buffered)
                    return source;
                return (IEnumerable<T>)source.ToList<T>();
            }

            private IEnumerable<TReturn> MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Delegate func, string splitOn)
            {
                SqlMapper.Identity identity = this.identity.ForGrid(typeof(TReturn), new Type[7]
                {
          typeof (TFirst),
          typeof (TSecond),
          typeof (TThird),
          typeof (TFourth),
          typeof (TFifth),
          typeof (TSixth),
          typeof (TSeventh)
                }, this.gridIndex);
                try
                {
                    foreach (TReturn @return in ((IDbConnection)null).MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(new CommandDefinition(), func, splitOn, this.reader, identity, false))
                        yield return @return;
                }
                finally
                {
                    this.NextResult();
                }
            }

            public IEnumerable<TReturn> Read<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                IEnumerable<TReturn> source = this.MultiReadInternal<TFirst, TSecond, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, TReturn>((Delegate)func, splitOn);
                if (!buffered)
                    return source;
                return (IEnumerable<TReturn>)source.ToList<TReturn>();
            }

            public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                IEnumerable<TReturn> source = this.MultiReadInternal<TFirst, TSecond, TThird, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, TReturn>((Delegate)func, splitOn);
                if (!buffered)
                    return source;
                return (IEnumerable<TReturn>)source.ToList<TReturn>();
            }

            public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                IEnumerable<TReturn> source = this.MultiReadInternal<TFirst, TSecond, TThird, TFourth, SqlMapper.DontMap, SqlMapper.DontMap, SqlMapper.DontMap, TReturn>((Delegate)func, splitOn);
                if (!buffered)
                    return source;
                return (IEnumerable<TReturn>)source.ToList<TReturn>();
            }

            public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                IEnumerable<TReturn> source = this.MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, SqlMapper.DontMap, SqlMapper.DontMap, TReturn>((Delegate)func, splitOn);
                if (!buffered)
                    return source;
                return (IEnumerable<TReturn>)source.ToList<TReturn>();
            }

            public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                IEnumerable<TReturn> source = this.MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, SqlMapper.DontMap, TReturn>((Delegate)func, splitOn);
                if (!buffered)
                    return source;
                return (IEnumerable<TReturn>)source.ToList<TReturn>();
            }

            public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                IEnumerable<TReturn> source = this.MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>((Delegate)func, splitOn);
                if (!buffered)
                    return source;
                return (IEnumerable<TReturn>)source.ToList<TReturn>();
            }

            private IEnumerable<T> ReadDeferred<T>(int index, Func<IDataReader, object> deserializer, SqlMapper.Identity typedIdentity)
            {
                try
                {
                    while (index == this.gridIndex && this.reader.Read())
                        yield return (T)deserializer(this.reader);
                }
                finally
                {
                    if (index == this.gridIndex)
                        this.NextResult();
                }
            }

            public bool IsConsumed
            {
                get
                {
                    return this.consumed;
                }
            }

            private void NextResult()
            {
                if (this.reader.NextResult())
                {
                    ++this.readCount;
                    ++this.gridIndex;
                    this.consumed = false;
                }
                else
                {
                    this.reader.Dispose();
                    this.reader = (IDataReader)null;
                    if (this.callbacks != null)
                        this.callbacks.OnCompleted();
                    this.Dispose();
                }
            }

            public void Dispose()
            {
                if (this.reader != null)
                {
                    if (!this.reader.IsClosed && this.command != null)
                        this.command.Cancel();
                    this.reader.Dispose();
                    this.reader = (IDataReader)null;
                }
                if (this.command == null)
                    return;
                this.command.Dispose();
                this.command = (IDbCommand)null;
            }
        }
    }
}
