using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerKeyboard {
	public static class Utils {
		public static System.Windows.Window GetParentWindow(this System.Windows.FrameworkElement elm){
			while (elm.Parent != null && elm is System.Windows.Window == false)
				elm = (System.Windows.FrameworkElement) elm.Parent;
			return elm as System.Windows.Window;
		}
	}
}
