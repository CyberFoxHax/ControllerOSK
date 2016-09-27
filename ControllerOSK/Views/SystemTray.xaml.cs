using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ControllerOSK.Views {
	public partial class SystemTray {
		public SystemTray() {
			InitializeComponent();
			StylePicker.SubmenuOpened += StylePickerOnSubmenuOpened;
		}

		private static readonly List<MenuItem> SkinItems = new List<MenuItem>();

		private void StylePickerOnSubmenuOpened(object sender, RoutedEventArgs routedEventArgs){
			foreach (var skinItem in SkinItems)
				skinItem.Click -= SkinMenuItem_OnClick;

			StylePicker.Items.Clear();
			SkinItems.Clear();

			var result = System.IO.Directory.GetDirectories(Environment.CurrentDirectory + "\\Skins");

			foreach (var item in result.Select(s => new MenuItem{
				Header = System.IO.Path.GetFileName(s),
				DataContext = s
			})){
				item.Click += SkinMenuItem_OnClick;
				StylePicker.Items.Add(item);
				SkinItems.Add(item);
			}
		}

		private void SkinMenuItem_OnClick(object sender, RoutedEventArgs routedEventArgs){
			var menuItem = sender as MenuItem;
			if (menuItem == null) return;

			var skinFolder = menuItem.DataContext as string;
			if (skinFolder == null) return;

			if (SkinPick != null) SkinPick(skinFolder);
		}

		public event Action XInputClick;
		public event Action KeyboardHookClick;
		public event Action ShowHideClick;
		public event Action ExitClick;
		public event Action<string> SkinPick;

		private void XInput_OnClick(object sender, RoutedEventArgs e){
			if (XInputClick != null) XInputClick();
		}

		private void KeyboardHook_OnClick(object sender, RoutedEventArgs e){
			if (KeyboardHookClick != null) KeyboardHookClick();
		}

		private void ShowHide_OnClick(object sender, RoutedEventArgs e){
			if (ShowHideClick != null) ShowHideClick();
		}

		private void Exit_OnClick(object sender, RoutedEventArgs e){
			if (ExitClick != null) ExitClick();
		}
	}
}
