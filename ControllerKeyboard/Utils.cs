using System.Windows;
using System.Runtime.InteropServices;
using System;

namespace ControllerKeyboard {
	public static class Utils {
		public static Window GetParentWindow(this FrameworkElement elm){
			while (elm.Parent != null && elm is Window == false)
				elm = (FrameworkElement) elm.Parent;
			return elm as Window;
		}

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
	}

}