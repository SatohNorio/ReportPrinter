using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ReportPrinter
{
	/// <summary>
	/// 任意の処理を実行するコマンドを定義します。
	/// </summary>
	public class DelegateCommand : CommandBase
	{
		// ------------------------------------------------------------------------------------------------------------
		#region コンストラクタ

		/// <summary>
		/// DelegateCommand クラスの新しいインスタンスを作成します。
		/// </summary>
		public DelegateCommand() : this(null, () => true)
		{
		}

		/// <summary>
		/// DelegateCommand クラスの新しいインスタンスを作成します。
		/// </summary>
		/// <param name="execute">実行する処理を指定します。</param>
		public DelegateCommand(Action execute) : this(execute, () => true)
		{
		}

		/// <summary>
		/// DelegateCommand クラスの新しいインスタンスを作成します。
		/// </summary>
		/// <param name="execute">実行する処理を指定します。</param>
		/// <param name="canExecute">コマンドが実行可能か判定する処理を指定します。</param>
		public DelegateCommand(Action execute, Func<bool> canExecute)
		{
			this.ExecutingAction = execute;
			this.CanExecuteAction = canExecute;
		}

		#endregion コンストラクタ
		// ------------------------------------------------------------------------------------------------------------
		#region プロパティ

		/// <summary>
		/// メソッドが定義されているデータコンテキスト を取得または設定します。
		/// </summary>
		public object DataContext
		{
			get { return (object)GetValue(DataContextProperty); }
			set { SetValue(DataContextProperty, value); }
		}

		/// <summary>
		/// メソッドが定義されているデータコンテキスト を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty DataContextProperty =
			DependencyProperty.Register("DataContext", typeof(object), typeof(DelegateCommand), new PropertyMetadata(null));

		/// <summary>
		/// 実行するアクション名 を取得または設定します。
		/// </summary>
		public string ActionName
		{
			get { return (string)GetValue(ActionNameProperty); }
			set { SetValue(ActionNameProperty, value); }
		}

		/// <summary>
		/// 実行するアクション名 を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty ActionNameProperty =
			DependencyProperty.Register("ActionName", typeof(string), typeof(DelegateCommand), new PropertyMetadata(null));

		/// <summary>
		/// 実行するアクション を取得または設定します。
		/// </summary>
		public Action ExecutingAction
		{
			get { return (Action)GetValue(ExecutingActionProperty); }
			set { SetValue(ExecutingActionProperty, value); }
		}

		/// <summary>
		/// 実行するアクション を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty ExecutingActionProperty =
			DependencyProperty.Register("ExecutingAction", typeof(Action), typeof(DelegateCommand), new PropertyMetadata(null));

		/// <summary>
		/// コマンドを実行してい良いか判断するアクション名 を取得または設定します。
		/// </summary>
		public string CanExecuteActionName
		{
			get { return (string)GetValue(CanExecuteActionNameProperty); }
			set { SetValue(CanExecuteActionNameProperty, value); }
		}

		/// <summary>
		/// コマンドを実行してい良いか判断するアクション名 を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty CanExecuteActionNameProperty =
			DependencyProperty.Register("CanExecuteActionName", typeof(string), typeof(DelegateCommand), new PropertyMetadata(null));

		/// <summary>
		/// コマンドを実行してい良いか判断するアクション を取得または設定します。
		/// </summary>
		public Func<bool> CanExecuteAction { get; set; }

		#endregion プロパティ
		// ------------------------------------------------------------------------------------------------------------
		#region CommandBase 抽象クラスの実装

		/// <summary>
		/// コマンドが実行可能かどうか判定します。
		/// </summary>
		/// <param name="parameter">コマンドで使用するパラメータを指定します。</param>
		/// <returns>コマンドが実行可能ならば true を返します。</returns>
		protected override bool OnCanExecute(object parameter)
		{
			if (this.DataContext != null)
			{
				var dc = this.DataContext;

				// バインドされたメソッドがあればそちらを実行可否判定アクションとして実行
				if (!string.IsNullOrEmpty(this.CanExecuteActionName))
				{
					var mt = dc.GetType().GetMethod(this.CanExecuteActionName, BindingFlags.Public | BindingFlags.Instance);
					if (mt != null)
					{
						if (parameter != null)
						{
							return (bool)mt.Invoke(dc, new object[] { parameter });
						}
						else
						{
							return (bool)mt.Invoke(dc, null);
						}
					}
				}
			}
			return this.CanExecuteAction.Invoke();
		}

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		/// <param name="parameter">コマンドで使用するパラメータを指定します。</param>
		protected override void OnExecute(object parameter)
		{
			if (this.DataContext != null)
			{
				var dc = this.DataContext;

				// バインドされたメソッドがあればそちらを実行アクションとして実行
				if (!string.IsNullOrEmpty(this.ActionName))
				{
					var mt = dc.GetType().GetMethod(this.ActionName, BindingFlags.Public | BindingFlags.Instance);
					if (mt != null)
					{
						if (parameter != null)
						{
							mt.Invoke(dc, new object[] { parameter });
						}
						else
						{
							mt.Invoke(dc, null);
						}
						return;
					}
				}
			}
			this.ExecutingAction?.Invoke();
		}

		/// <summary>
		/// 派生クラスで実装された場合、System.Windows.Freezable 派生クラスの新しいインスタンスを作成します。
		/// </summary>
		/// <returns>生成した新しいインスタンスを返します。</returns>
		protected override Freezable CreateInstanceCore()
		{
			return new DelegateCommand();
		}

		#endregion CommandBase 抽象クラスの実装
		// ------------------------------------------------------------------------------------------------------------
	}

	/// <summary>
	/// パラメータ付きの任意の処理を実行するコマンドを定義します。
	/// </summary>
	/// <typeparam name="T">コマンドに渡すパラメータの型を指定します。</typeparam>
	public class DelegateCommand<T> : CommandBase
	{
		#region コンストラクタ

		/// <summary>
		/// DelegateCommand クラスの新しいインスタンスを作成します。
		/// </summary>
		public DelegateCommand() : this(null, (p) => true)
		{
		}

		/// <summary>
		/// DelegateCommand クラスの新しいインスタンスを作成します。
		/// </summary>
		/// <param name="execute">実行する処理を指定します。</param>
		public DelegateCommand(Action<T> execute) : this(execute, (p) => true)
		{
		}

		/// <summary>
		/// DelegateCommand クラスの新しいインスタンスを作成します。
		/// </summary>
		/// <param name="execute">実行する処理を指定します。</param>
		/// <param name="canExecute">コマンドが実行可能か判定する処理を指定します。</param>
		public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
		{
			this.ExecutingAction = execute;
			this.CanExecuteAction = canExecute;
		}

		#endregion コンストラクタ

		#region プロパティ

		/// <summary>
		/// メソッドが定義されているデータコンテキスト を取得または設定します。
		/// </summary>
		public object DataContext
		{
			get { return (object)GetValue(DataContextProperty); }
			set { SetValue(DataContextProperty, value); }
		}

		/// <summary>
		/// メソッドが定義されているデータコンテキスト を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty DataContextProperty =
			DependencyProperty.Register("DataContext", typeof(object), typeof(DelegateCommand<T>), new PropertyMetadata(null));

		/// <summary>
		/// 実行するアクション名 を取得または設定します。
		/// </summary>
		public string ActionName
		{
			get { return (string)GetValue(ActionNameProperty); }
			set { SetValue(ActionNameProperty, value); }
		}

		/// <summary>
		/// 実行するアクション名 を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty ActionNameProperty =
			DependencyProperty.Register("ActionName", typeof(string), typeof(DelegateCommand<T>), new PropertyMetadata(null));

		/// <summary>
		/// 実行するアクション を取得または設定します。
		/// </summary>
		public Action<T> ExecutingAction
		{
			get { return (Action<T>)GetValue(ExecutingActionProperty); }
			set { SetValue(ExecutingActionProperty, value); }
		}

		/// <summary>
		/// 実行するアクション を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty ExecutingActionProperty =
			DependencyProperty.Register("ExecutingAction", typeof(Action<T>), typeof(DelegateCommand<T>), new PropertyMetadata(null));

		/// <summary>
		/// コマンドを実行してい良いか判断するアクション名 を取得または設定します。
		/// </summary>
		public string CanExecuteActionName
		{
			get { return (string)GetValue(CanExecuteActionNameProperty); }
			set { SetValue(CanExecuteActionNameProperty, value); }
		}

		/// <summary>
		/// コマンドを実行してい良いか判断するアクション名 を管理する依存関係プロパティを定義します。
		/// </summary>
		public static readonly DependencyProperty CanExecuteActionNameProperty =
			DependencyProperty.Register("CanExecuteActionName", typeof(string), typeof(DelegateCommand<T>), new PropertyMetadata(null));

		/// <summary>
		/// コマンドを実行してい良いか判断するアクション を取得または設定します。
		/// </summary>
		public Func<T, bool> CanExecuteAction { get; set; }

		#endregion プロパティ

		// ------------------------------------------------------------------------------------------------------------

		#region CommandBase 抽象クラスの実装

		/// <summary>
		/// コマンドが実行可能かどうか判定します。
		/// </summary>
		/// <param name="parameter">コマンドで使用するパラメータを指定します。</param>
		/// <returns>コマンドが実行可能ならば true を返します。</returns>
		protected override bool OnCanExecute(object parameter)
		{
			var p = default(T);
			if (parameter != null)
			{
				p = (T)parameter;
			}
			if (this.DataContext != null)
			{
				var dc = this.DataContext;

				// バインドされたメソッドがあればそちらを実行可否判定アクションとして実行
				if (!string.IsNullOrEmpty(this.CanExecuteActionName))
				{
					var mt = dc.GetType().GetMethod(this.CanExecuteActionName, BindingFlags.Public | BindingFlags.Instance);
					if (mt != null)
					{
						return (bool)mt.Invoke(dc, new object[] { p });
					}
				}
			}
			return this.CanExecuteAction(p);
		}

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		/// <param name="parameter">コマンドで使用するパラメータを指定します。</param>
		protected override void OnExecute(object parameter)
		{
			var p = default(T);
			if (parameter != null)
			{
				p = (T)parameter;
			}
			if (this.DataContext != null)
			{
				var dc = this.DataContext;

				// バインドされたメソッドがあればそちらを実行アクションとして実行
				if (!string.IsNullOrEmpty(this.ActionName))
				{
					var mt = dc.GetType().GetMethod(this.ActionName, BindingFlags.Public | BindingFlags.Instance);
					if (mt != null)
					{
						mt.Invoke(dc, new object[] { p });
						return;
					}
				}
			}
			this.ExecutingAction?.Invoke(p);
		}

		/// <summary>
		/// 派生クラスで実装された場合、System.Windows.Freezable 派生クラスの新しいインスタンスを作成します。
		/// </summary>
		/// <returns>生成した新しいインスタンスを返します。</returns>
		protected override Freezable CreateInstanceCore()
		{
			return new DelegateCommand<T>();
		}

		#endregion CommandBase 抽象クラスの実装

		// ------------------------------------------------------------------------------------------------------------
	}
}
