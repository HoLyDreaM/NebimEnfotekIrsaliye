using System;

namespace NebimEnfotekIrsaliye.Data
{
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
	public sealed class ExplicitConstructorAttribute : Attribute
	{
	}
}
