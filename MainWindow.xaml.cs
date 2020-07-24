using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DBView
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private MainViewModel ViewModel => (MainViewModel)DataContext;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			ViewModel.ConnectionString = Properties.Settings.Default.ConnectionString;

			if (Properties.Settings.Default.SavedConnectionStrings != null)
			{
				foreach (var str in Properties.Settings.Default.SavedConnectionStrings)
				{
					if (!ViewModel.SavedConnectionStrings.Contains(str))
					{
						ViewModel.SavedConnectionStrings.Add(str);
					}
				}
			}
		}

		private void MainWindow_OnClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.ConnectionString = ViewModel.ConnectionString;

			Properties.Settings.Default.SavedConnectionStrings = new StringCollection();
			Properties.Settings.Default.SavedConnectionStrings.AddRange(ViewModel.SavedConnectionStrings.ToArray());

			Properties.Settings.Default.Save();
		}

		private void DataGrid_OnCopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
		{
			var grid = (DataGrid) sender;
			var currentCell = e.ClipboardRowContent[grid.CurrentCell.Column.DisplayIndex];
			e.ClipboardRowContent.Clear();
			e.ClipboardRowContent.Add(currentCell);
		}
	}
}
