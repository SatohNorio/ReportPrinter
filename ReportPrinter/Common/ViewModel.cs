using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.CompilerServices;

namespace ReportPrinter
{
	/// <summary>
	/// ViewModelの基底クラスを定義します。
	/// </summary>
	/// <remarks>
	/// このクラスは下記サイトのものをコピペして改造しました。
	/// かずきのBlog@hatena
	/// http://okazuki.hatenablog.com/entry/20100223/1266897125
	/// </remarks>
	public abstract class ViewModel : DependencyObject, INotifyPropertyChanged, IDisposable
	{
		// ------------------------------------------------------------------------------------------------------------
		#region コンストラクタ

		/// <summary>
		/// Okaden.Common.Wpf.ViewModel クラスの新しいインスタンスを初期化します。
		/// </summary>
		/// <remarks>
		/// このクラスは継承して使用します。直接インスタンスが作成できないようにコンストラクタは protected で定義します。
		/// </remarks>
		protected ViewModel()
		{
			var props = GetType().GetProperties();
			this.FPropertyList.AddRange(props);
		}

		#endregion コンストラクタ
		// ------------------------------------------------------------------------------------------------------------
		#region 初期化処理

		/// <summary>
		/// ViewModel の初期化処理を取得します。
		/// </summary>
		public Action InitializeAction => () => this.Initialize();

		/// <summary>
		/// ViewModelの初期化処理を行います。実際の処理は派生クラスで定義します。
		/// </summary>
		public void Initialize()
		{
			this.OnInitialize();
		}

		/// <summary>
		/// ViewModelの初期化処理で実際に行う処理を定義します。
		/// </summary>
		protected virtual void OnInitialize()
		{
		}

		#endregion 初期化処理
		// ------------------------------------------------------------------------------------------------------------
		#region プロパティ変更通知イベント

		/// <summary>
		/// プロパティが変更されたことを通知するイベントハンドラを定義します。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// プロパティ変更イベントを発生させる
		/// </summary>
		/// <remarks>
		/// このメソッドをプロパティ値が変更された時にコールして下さい。<br/>
		/// プロパティ値が変更されたことをクライアント (通常はバインド元クライアント) に通知するためのイベントを発生させます。<br/>
		/// CallerMemberName の属性を使用すると、 NotifyPropertyChanged のメソッドへの呼び出しは文字列引数として<br/>
		/// プロパティ名を指定する必要はありません。そのプロパティ名になります。
		/// </remarks>
		/// <param name="propertyName">プロパティ名(デフォルト=そのプロパティ名)</param>
		public virtual void OnPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null)
			{
				var prop = this.FPropertyList.Where(p => p.Name == propertyName).SingleOrDefault();
				if (prop != null)
				{
					var dispatcher = Application.Current.Dispatcher;
					if (dispatcher.CheckAccess())
					{
						handler(this, new PropertyChangedEventArgs(prop.Name));
					}
					else
					{
						dispatcher.InvokeAsync(() => handler(this, new PropertyChangedEventArgs(prop.Name)));
					}
				}
			}
		}

		/// <summary>
		/// プロパティが変更されたときに発生します。
		/// </summary>
		/// <param name="names">イベントに渡すプロパティ名の配列を宣言します。</param>
		protected virtual void OnPropertyChanged(params string[] names)
		{
			if (names == null)
			{
				var nm = MethodBase.GetCurrentMethod().GetParameters()[0].Name;
				throw new ArgumentNullException(nm);
			}

			var h = this.PropertyChanged;
			if (h != null)
			{
				// プロパティのコレクションを作成
				var props = this.FPropertyList;
				//this.CheckPropertyName(names);
				foreach (var name in names)
				{
					// 存在するプロパティについてのみ変更を通知する
					var prop = props.Where(p => p.Name == name).SingleOrDefault();
					if (prop != null)
					{
						h(this, new PropertyChangedEventArgs(name));
					}
				}
			}
		}

		/// <summary>
		/// プロパティの一覧を管理します。
		/// </summary>
		private List<PropertyInfo> FPropertyList = new List<PropertyInfo>();

		/// <summary>
		/// デバッグ時に引数となるプロパティ名が設定されているかどうか確認し、設定されていなければ例外を投げます。
		/// </summary>
		/// <param name="names">確認するプロパティ名の配列を宣言します。</param>
		[Conditional("DEBUG")]
		private void CheckPropertyName(params string[] names)
		{
			var props = GetType().GetProperties();
			foreach (var name in names)
			{
				var prop = props.Where(p => p.Name == name).SingleOrDefault();
				if (prop == null)
				{
					Trace.WriteLine("存在しないプロパティ:" + name);
					//throw new ArgumentException(name);
				}
			}
		}

		/// <summary>
		/// よくわからない。。。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyExpression"></param>
		protected void OnPropertyChanged<T>(params Expression<Func<T>>[] propertyExpression)
		{
			OnPropertyChanged(
				propertyExpression.Select(ex => ((MemberExpression)ex.Body).Member.Name).ToArray());
		}

		#endregion プロパティ変更通知イベント
		// ------------------------------------------------------------------------------------------------------------
		#region IDisposable Support

		/// <summary>
		/// リソースが既に解放されていればtrueを保持します。
		/// </summary>
		/// <remarks>
		/// 重複して解放処理が行われないようにするために使用します。
		/// </remarks>
		private bool disposedValue = false; // 重複する呼び出しを検出するには

		/// <summary>
		/// オブジェクトの破棄処理
		/// </summary>
		/// <param name="disposing">マネージオブジェクトを破棄する場合はtrueを指定します。</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// マネージ状態を破棄します (マネージ オブジェクト)。
				}

				// アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
				// 大きなフィールドを null に設定します。

				disposedValue = true;
			}
		}

		/// <summary>
		/// ファイナライザ
		/// </summary>
		/// <remarks>
		/// アンマネージリソースを解放します。
		/// </remarks>
		~ViewModel()
		{
			// このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
			Dispose(false);
		}

		/// <summary>
		/// オブジェクトの終了処理を行います。
		/// </summary>
		/// <remarks>
		/// このコードは、破棄可能なパターンを正しく実装できるように追加されました。
		/// </remarks>
		public void Dispose()
		{
			// このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
		// ------------------------------------------------------------------------------------------------------------
		//#region Freezable 抽象メソッド実装

		//protected override Freezable CreateInstanceCore()
		//{
		//	return
		//}

		//#endregion Freezable 抽象メソッド実装
		// ------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// プロパティが変更されている場合は更新して通知します。
		/// </summary>
		/// <typeparam name="T">プロパティの型を指定します。</typeparam>
		/// <param name="prop">プロパティの値を管理する変数を指定します。</param>
		/// <param name="value">変更する値を指定します。</param>
		/// <param name="propertyName">プロパティ名(デフォルト=そのプロパティ名)</param>
		/// <returns>プロパティが変更された場合は true を返します。</returns>
		protected bool UpdateProperty<T>(ref T prop, T value, [CallerMemberName] String propertyName = "")
		{
			var b = (prop == null || !prop.Equals(value));
			if (b)
			{
				prop = value;
				this.OnPropertyChanged(propertyName);
			}
			return b;
		}
	}

	/// <summary>
	/// Modelの参照を保持するViewModelの基底クラスを定義します。
	/// </summary>
	public abstract class ViewModel<TModel> : ViewModel where TModel : INotifyPropertyChanged
	{
		// ------------------------------------------------------------------------------------------------------------
		#region コンストラクタ

		/// <summary>
		/// Modelを使用して、 Okaden.Common.Wpf.ViewModel クラスの新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="model">データを管理するモデルを指定します。</param>
		/// <remarks>
		/// このクラスは継承して使用します。直接インスタンスが作成できないようにコンストラクタは protected で定義します。
		/// </remarks>
		protected ViewModel(TModel model)
		{
			this.FModel = model;
			this.Model.PropertyChanged += ModelPropertyChanged;

			// プロパティ名の一覧を作成する。
			var members = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var info in members)
			{
				this.FPropertyList.Add(info.Name);
			}
		}

		#endregion
		// ------------------------------------------------------------------------------------------------------------
		// ------------------------------------------------------------------------------------------------------------
		#region 終了処理

		/// <summary>
		/// オブジェクトの終了処理を行います。
		/// </summary>
		/// <param name="disposing">マネージオブジェクトを破棄する場合はtrueを指定します。</param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.Model.PropertyChanged -= ModelPropertyChanged;
		}

		#endregion
		// ------------------------------------------------------------------------------------------------------------
		// ------------------------------------------------------------------------------------------------------------
		#region プロパティ変更通知イベント

		/// <summary>
		/// モデルで発生したプロパティ変更通知イベントを処理します。
		/// </summary>
		/// <param name="sender">イベントを送信したオブジェクトを指定します。</param>
		/// <param name="e">イベント引数を指定します。</param>
		private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnModelPropertyChanged(e.PropertyName);
		}

		/// <summary>
		/// モデルで発生したプロパティ変更通知イベントをViewに通知します。
		/// </summary>
		/// <param name="propertyName">プロパティ名を指定します。</param>
		protected virtual void OnModelPropertyChanged(string propertyName)
		{
			// プロパティの変更をそのままViewに通知する。
			this.OnPropertyChanged(propertyName);
		}

		#endregion
		// ------------------------------------------------------------------------------------------------------------
		// ------------------------------------------------------------------------------------------------------------
		#region モデルプロパティ

		/// <summary>
		/// データを管理する TModel オブジェクトを管理します。
		/// </summary>
		private TModel FModel;

		/// <summary>
		/// データを管理する TModel オブジェクトを取得します。このプロパティは派生クラスから利用できるようにするために定義します。
		/// </summary>
		protected TModel Model
		{
			get
			{
				return this.FModel;
			}
		}

		#endregion
		// ------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// プロパティ名の一覧を管理します。
		/// </summary>
		private List<string> FPropertyList = new List<string>();

	}
}
