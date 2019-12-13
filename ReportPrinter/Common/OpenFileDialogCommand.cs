using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace ReportPrinter
{
	/// <summary>
	/// ファイル選択ダイアログを表示してファイルを選択するコマンドを定義します。
	/// </summary>
	public class OpenFileDialogCommand : CommandBase
	{
		#region コンストラクタ

		/// <summary>
		/// 新しいインスタンスを生成します。
		/// </summary>
		public OpenFileDialogCommand()
		{

		}

		#endregion コンストラクタ

		#region 抽象メソッドの実装

		/// <summary>
		/// 派生クラスで実装された場合、System.Windows.Freezable 派生クラスの新しいインスタンスを作成します。
		/// </summary>
		/// <returns>生成した新しいインスタンスを返します。</returns>
		protected override Freezable CreateInstanceCore() => new OpenFileDialogCommand();

		/// <summary>
		/// コマンドが実行可能かどうか判定します。
		/// </summary>
		/// <param name="parameter">コマンドで使用するパラメータを指定します。</param>
		/// <returns>コマンドが実行可能ならば true を返します。</returns>
		protected override bool OnCanExecute(object parameter) => true;

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		/// <param name="parameter">コマンドで使用するパラメータを指定します。</param>
		protected override void OnExecute(object parameter)
		{
			var path = Assembly.GetEntryAssembly().Location;
			var curPath = Path.GetDirectoryName(path);

			var dlg = new OpenFileDialog
			{
				Title = this.Title,
				Filter = this.Filter,
				InitialDirectory = curPath,
			};
			if (dlg.ShowDialog() == true)
			{
				this.FileName = dlg.FileName;
			}
		}

		#endregion 抽象メソッドの実装

		#region Title

		/// <summary>
		/// ダイアログに表示するタイトル を取得または設定します。
		/// </summary>
		public string Title
		{
			get => (string)this.GetValue(TitleProperty);
			set => this.SetValue(TitleProperty, value);
		}

		/// <summary>
		/// ダイアログに表示するタイトル を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(OpenFileDialogCommand), new PropertyMetadata(String.Empty));

		#endregion Title

		#region Filter

		/// <summary>
		/// ダイアログに設定するフィルタ を取得または設定します。
		/// </summary>
		public string Filter
		{
			get => (string)this.GetValue(FilterProperty);
			set => this.SetValue(FilterProperty, value);
		}

		/// <summary>
		/// ダイアログに設定するフィルタ を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register("Filter", typeof(string), typeof(OpenFileDialogCommand), new PropertyMetadata(String.Empty));

		#endregion Filter

		#region FileName

		/// <summary>
		/// ダイアログから取得したファイル名 を取得または設定します。
		/// </summary>
		public string FileName
		{
			get => (string)this.GetValue(FileNameProperty);
			set => this.SetValue(FileNameProperty, value);
		}

		/// <summary>
		/// ダイアログから取得したファイル名 を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty FileNameProperty =
			DependencyProperty.Register("FileName", typeof(string), typeof(OpenFileDialogCommand), new PropertyMetadata(String.Empty));

		#endregion FileName
	}
}
