using System;
using System.Data;

namespace NebimEnfotekIrsaliye.Data
{
	public interface IWrappedDataReader : IDataReader, IDisposable, IDataRecord
	{
		IDataReader Reader { get; }
		IDbCommand Command { get; }
	}
}
