using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ReportPrinter
{
	public class ShowWindowCommand : CommandBase
	{
		#region 抽象メソッドの実装

		/// <summary>
		/// 派生クラスで実装された場合、System.Windows.Freezable 派生クラスの新しいインスタンスを作成します。
		/// </summary>
		/// <returns>生成した新しいインスタンスを返します。</returns>
		protected override Freezable CreateInstanceCore() => new ShowWindowCommand();

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
			if (this.TargetType != null)
			{
				if (Activator.CreateInstance(this.TargetType) is Window w)
				{
					if (this.Owner != null)
					{
						w.Owner = this.Owner;
						w.DataContext = this.Owner.DataContext;
						w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
					}
					w.Show();
				}
			}
		}

		#endregion 抽象メソッドの実装

		#region Owner

		/// <summary>
		/// 親ウィンドウ を取得または設定します。
		/// </summary>
		public Window Owner
		{
			get => (Window)this.GetValue(OwnerProperty);
			set => this.SetValue(OwnerProperty, value);
		}

		/// <summary>
		/// 親ウィンドウ を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty OwnerProperty =
			DependencyProperty.Register("Owner", typeof(Window), typeof(ShowWindowCommand), new PropertyMetadata(null));

		#endregion Owner

		#region TargetType

		/// <summary>
		/// 表示するウィンドウの型 を取得または設定します。
		/// </summary>
		public Type TargetType
		{
			get => (Type)this.GetValue(TargetTypeProperty);
			set => this.SetValue(TargetTypeProperty, value);
		}

		/// <summary>
		/// 表示するウィンドウの型 を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty TargetTypeProperty =
			DependencyProperty.Register("TargetType", typeof(Type), typeof(ShowWindowCommand), new PropertyMetadata(null));

		#endregion TargetType
	}
}
