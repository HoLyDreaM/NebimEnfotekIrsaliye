using System;

namespace NebimEnfotekIrsaliye.Data
{
	[Flags]
	public enum CommandFlags
	{
		None = 0,
		Buffered = 1,
		Pipelined = 2,
		NoCache = 4
	}
}
