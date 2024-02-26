using System;
using System.Data;

namespace NebimEnfotekIrsaliye_SYS.Data
{
	public interface IWrappedDataReader : IDataReader, IDisposable, IDataRecord
	{
		IDataReader Reader { get; }
		IDbCommand Command { get; }
	}
}
