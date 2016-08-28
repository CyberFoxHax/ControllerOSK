using System.Windows;

namespace ControllerKeyboard {
	public static class Utils {
		public static Window GetParentWindow(this FrameworkElement elm){
			while (elm.Parent != null && elm is Window == false)
				elm = (FrameworkElement) elm.Parent;
			return elm as Window;
		}
	}

}