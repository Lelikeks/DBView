using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace DBView
{
	internal class MainViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Command ShowCommand { get; set; }
		public ObservableCollection<Table> Tables { get; set; } = new ObservableCollection<Table>();
		public ObservableCollection<Column> Columns { get; set; } = new ObservableCollection<Column>();
		public ObservableCollection<ColumnValue> ColumnValues { get; set; } = new ObservableCollection<ColumnValue>();
		public ObservableCollection<string> SavedConnectionStrings { get; } = new ObservableCollection<string>();

		private DataTable _dataTable;
		public DataTable Data
		{
			get => _dataTable;
			set
			{
				_dataTable = value;
				OnPropertyChanged();
			}
		}

		public ColumnValue SelectedValue
		{
			set => Data = value == null ? null : DbManager.GetData(ConnectionString, SelectedTable, SelectedColumn.Name, value.Value);
		}

		private Table _selectedTable;
		public Table SelectedTable
		{
			get => _selectedTable;
			set
			{
				_selectedTable = value;
				Columns.Clear();

				if (value != null)
				{
					foreach (var column in DbManager.GetColumns(ConnectionString, value.Id))
					{
						Columns.Add(column);
					}
				}

				Data = value == null ? null : DbManager.GetData(ConnectionString, value, null, null);
			}
		}

		private Column _selectedColumn;
		public Column SelectedColumn
		{
			get => _selectedColumn;
			set
			{
				_selectedColumn = value;

				ColumnValues.Clear();
				if (value != null)
				{
					foreach (var columnValue in DbManager.GetColumnValues(ConnectionString, SelectedTable, value.Name))
					{
						ColumnValues.Add(columnValue);
					}
				}
			}
		}

		private string _connectionString;
		public string ConnectionString
		{
			get => _connectionString;
			set
			{
				_connectionString = value;
				OnPropertyChanged();

				if (!SavedConnectionStrings.Contains(_connectionString))
				{
					SavedConnectionStrings.Insert(0, _connectionString);
				}
			}
		}

		public MainViewModel()
		{
			ShowCommand = new Command(OnShow);
		}

		private void OnShow()
		{
			Tables.Clear();

			foreach (var table in DbManager.GetTables(ConnectionString))
			{
				Tables.Add(table);
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
