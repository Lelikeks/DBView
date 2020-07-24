using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DBView
{
	internal class DbManager
	{
		internal static IEnumerable<Table> GetTables(string connectionString)
		{
			using (var cn = new SqlConnection(connectionString))
			using (var cm = cn.CreateCommand())
			{
				cm.CommandText = "select t.object_id, t.name, s.name from sys.tables t join sys.schemas s on t.schema_id = s.schema_id order by t.name";

				cn.Open();
				using (var dr = cm.ExecuteReader())
				{
					while (dr.Read())
					{
						yield return new Table
						{
							Id = dr.GetInt32(0),
							Name = dr.GetString(1),
							Schema = dr.GetString(2)
						};
					}
				}
			}
		}

		internal static IEnumerable<Column> GetColumns(string connectionString, int tableId)
		{
			using (var cn = new SqlConnection(connectionString))
			using (var cm = cn.CreateCommand())
			{
				cm.CommandText = "select c.name, c.is_nullable, t.name from sys.columns c join sys.types t on c.user_type_id = t.user_type_id where c.object_id = @object_id order by c.name";
				cm.Parameters.AddWithValue("object_id", tableId);

				cn.Open();
				using (var dr = cm.ExecuteReader())
				{
					while (dr.Read())
					{
						yield return new Column
						{
							Name = dr.GetString(0),
							Nullable = dr.GetBoolean(1),
							DataType = dr.GetString(2)
						};
					}
				}
			}
		}

		internal static IEnumerable<ColumnValue> GetColumnValues(string connectionString, Table table, string column)
		{
			using (var cn = new SqlConnection(connectionString))
			using (var cm = cn.CreateCommand())
			{
				cm.CommandText = $"select [{column}], count(*) from [{table.Schema}].[{table.Name}] group by [{column}] order by 2 desc";

				cn.Open();
				using (var dr = cm.ExecuteReader())
				{
					while (dr.Read())
					{
						yield return new ColumnValue
						{
							Value = dr.IsDBNull(0) ? Constants.NullText : dr.GetValue(0),
							Count = dr.GetInt32(1)
						};
					}
				}
			}
		}

		internal static DataTable GetData(string connectionString, Table table, string column, object value)
		{
			SqlDataAdapter da;
			var dt = new DataTable();

			if (column == null)
			{
				var sql = $"select * from [{table.Schema}].[{table.Name}]";
				da = new SqlDataAdapter(sql, connectionString);
			}
			else
			{
				var filter = value as string == Constants.NullText ? "is null" : "= @value";
				var sql = $"select * from [{table.Schema}].[{table.Name}] where [{column}] " + filter;
				da = new SqlDataAdapter(sql, connectionString);

				if (value != null)
				{
					da.SelectCommand.Parameters.AddWithValue("value", value);
				}
			}

			da.Fill(dt);

			return dt;
		}
	}
}
