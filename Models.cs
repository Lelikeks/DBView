namespace DBView
{
	internal class Constants
	{
		public const string NullText = "<null>";
	}

	internal class Table
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Schema { get; set; }
	}

	internal class Column
	{
		public string Name { get; set; }
		public bool Nullable { get; set; }
		public string DataType { get; set; }
	}

	internal class ColumnValue
	{
		public object Value { get; set; }
		public int Count { get; set; }
	}
}
