﻿using System;
using System.Data;

namespace NebimEnfotekIrsaliye_SYS.Data
{
	internal sealed class DataTableHandler : SqlMapper.ITypeHandler
	{
		public object Parse(Type destinationType, object value)
		{
			throw new NotImplementedException();
		}
		public void SetValue(IDbDataParameter parameter, object value)
		{
			TableValuedParameter.Set(parameter, value as DataTable, null);
		}
	}
}
