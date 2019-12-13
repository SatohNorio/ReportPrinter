using Reactive.Bindings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Printing;
using System.Text;

namespace ReportPrinter
{
	/// <summary>
	/// 帳票印刷画面の内部処理を定義します。
	/// </summary>
	public class ReportPrinterViewModel : ViewModel
	{
		#region コンストラクタ

		/// <summary>
		/// 新しいオブジェクトを生成します。
		/// </summary>
		public ReportPrinterViewModel()
		{
			// 印刷処理を定義
			this.FPrintAction = new Action(() =>
			{
				using (var queue = this.FServer.GetPrintQueue(this.SelectedPrinter.Value))
				{
					var writer = PrintQueue.CreateXpsDocumentWriter(queue);
					if (!String.IsNullOrWhiteSpace(this.SelectedFilePath.Value))
					{
						writer.Write(this.SelectedFilePath.Value);
					}
				}
			});

			// インストールされているプリンタの一覧
			this.FPrinterCollection = PrinterSettings.InstalledPrinters;

			// 初期値に既定のプリンタを設定
			var defaultPrinter = this.FServer.DefaultPrintQueue;
			foreach (string name in this.FPrinterCollection)
			{
				if (name == defaultPrinter.Name)
				{
					this.SelectedPrinter = new ReactiveProperty<string>(name);
					break;
				}
			}

			// ファイルパスを初期化
			this.SelectedFilePath = new ReactiveProperty<string>();

			// ReactiveProperty のライセンステキスト設定
			using (var reader = new StreamReader("LICENSE.txt"))
			{
				this.License = new ReactiveProperty<string>(reader.ReadToEnd());
			}
		}

		#endregion コンストラクタ

		/// <summary>
		/// ローカルのプリントサーバー（プリンタがインストールされているコンピューター）を管理します。
		/// </summary>
		private readonly LocalPrintServer FServer = new LocalPrintServer();

		/// <summary>
		/// 印刷処理を管理します。
		/// </summary>
		private readonly Action FPrintAction;

		/// <summary>
		/// 印刷処理を取得します。
		/// </summary>
		public Action PrintAction => this.FPrintAction;

		/// <summary>
		/// プリンタ名の一覧を管理します。
		/// </summary>
		private readonly ICollection FPrinterCollection;

		/// <summary>
		/// プリンタ名の一覧を取得します。
		/// </summary>
		public ICollection PrinterCollection => this.FPrinterCollection;

		/// <summary>
		/// 現在選択されているプリンタ名を取得または設定します。
		/// </summary>
		public ReactiveProperty<string> SelectedPrinter { get; }

		/// <summary>
		/// 印刷するファイルのパスを取得または設定します。
		/// </summary>
		public ReactiveProperty<string> SelectedFilePath { get; }

		/// <summary>
		/// ReactiveProperty のライセンス表示を取得します。
		/// </summary>
		public IReadOnlyReactiveProperty<string> License { get; }
	}
}
