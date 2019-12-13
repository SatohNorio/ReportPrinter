using System;
using System.Windows;
using System.Windows.Input;

namespace ReportPrinter
{
	/// <summary>
	/// コマンドの基底クラスを定義します。
	/// </summary>
	public abstract class CommandBase : Freezable, ICommand
	{
		#region ICommand インターフェースの実装

		/// <summary>
		/// コマンド実行可能状態が変更する可能性がある場合に発生するイベントを管理します。
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// コマンドが実行可能かどうか判定します。
		/// </summary>
		/// <param name="parameter">コマンドで使用するパラメータを指定します。</param>
		/// <returns>コマンドが実行可能ならば true を返します。</returns>
		public bool CanExecute(object parameter)
		{
			return this.OnCanExecute(parameter);
		}

		/// <summary>
		/// コマンドが実行可能かどうか判定します。
		/// </summary>
		/// <param name="parameter">コマンドで使用するパラメータを指定します。</param>
		/// <returns>コマンドが実行可能ならば true を返します。</returns>
		protected abstract bool OnCanExecute(object parameter);

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		/// <param name="parameter">コマンドで使用するパラメータを指定します。</param>
		public void Execute(object parameter)
		{
			this.OnExecute(parameter);
		}

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		/// <param name="parameter">コマンドで使用するパラメータを指定します。</param>
		protected abstract void OnExecute(object parameter);

		#endregion ICommand インターフェースの実装

		/// <summary>
		/// CanExecuteChanged イベントを発生させます。
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			this.OnCanExecuteChanged(new EventArgs());
		}

		/// <summary>
		/// CanExecuteChanged イベントを発生させます。
		/// </summary>
		/// <param name="e">イベント引数を指定します。</param>
		protected void OnCanExecuteChanged(EventArgs e)
		{
			var act = this.CanExecuteChanged;
			if (act != null)
			{
				act(this, e);
			}
		}
	}
}