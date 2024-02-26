using System;

namespace NebimEnfotekIrsaliye_SYS.Data
{
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
	public sealed class ExplicitConstructorAttribute : Attribute
	{
	}
}
