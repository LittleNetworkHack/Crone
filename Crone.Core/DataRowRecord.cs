using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public record DataRowRecord : CoreComponent
	{
		#region Properties

		public DataRow Row { get; protected set; }
		public DataTable Table => Row?.Table;
		public DataColumnCollection Columns => Table?.Columns;

		#endregion Properties

		#region Constructors

		public DataRowRecord(DataRow row) => Row = row;

		#endregion Constructors

		#region Get/Set Core

		protected override bool GetValueCore(int index, out object value)
		{
			value = null;
			if (Columns == null)
				return false;

			DataColumn column = Columns.Count.IndexInRange(index) ? Columns[index] : null;
			if (column == null)
				return false;

			value = Row[column];
			return true;
		}
		protected override bool SetValueCore(int index, object value)
		{
			if (Columns == null)
				return false;

			DataColumn column = Columns.Count.IndexInRange(index) ? Columns[index] : null;
			if (column == null)
				return false;

			var defaultValue = column.AllowDBNull ? DBNull.Value : ObjectActivator.GetDefaultOrNull(column.DataType);
			Row[column] = ValueConverter.ConvertTo(value, column.DataType, defaultValue);
			return true;
		}

		protected override bool GetValueCore(string name, out object value)
		{
			value = null;
			if (Columns == null)
				return false;

			DataColumn column = Columns[name];
			if (column == null)
				return false;

			value = Row[column];
			return true;
		}
		protected override bool SetValueCore(string name, object value)
		{
			if (Columns == null)
				return false;

			DataColumn column = Columns[name];
			if (column == null)
				return false;

			var defaultValue = column.AllowDBNull ? DBNull.Value : ObjectActivator.GetDefaultOrNull(column.DataType);
			Row[column] = ValueConverter.ConvertTo(value, column.DataType, defaultValue);
			return true;
		}

		#endregion Get/Set Core
	}
}
