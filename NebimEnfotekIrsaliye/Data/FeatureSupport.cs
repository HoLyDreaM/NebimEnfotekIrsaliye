using System;
using System.Data;

namespace NebimEnfotekIrsaliye.Data
{
	public class FeatureSupport
	{
		public static FeatureSupport Get(IDbConnection connection)
		{
			if (string.Equals((connection == null) ? null : connection.GetType().Name, "npgsqlconnection", StringComparison.InvariantCultureIgnoreCase))
			{
				return FeatureSupport.postgres;
			}
			return FeatureSupport.@default;
		}
		private FeatureSupport(bool arrays)
		{
			this.Arrays = arrays;
		}
		public bool Arrays { get; private set; }
		private static readonly FeatureSupport @default = new FeatureSupport(false);
		private static readonly FeatureSupport postgres = new FeatureSupport(true);
	}
}
