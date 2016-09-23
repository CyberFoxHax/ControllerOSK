using System.Linq;
using System.Windows.Controls;

// 
// http://stackoverflow.com/a/35692688/1148434
// 

namespace ControllerOSK.Controls{
	public class RadioMenuItem : MenuItem {
		public string GroupName { get; set; }
		protected override void OnClick() {
			var ic = Parent as ItemsControl;
			if (null != ic) {
				var rmi = ic.Items.OfType<RadioMenuItem>().FirstOrDefault(i =>
					i.GroupName == GroupName && i.IsChecked);
				if (null != rmi)
					rmi.IsChecked = false;
			}
			base.OnClick();
			IsChecked = true;
		}
	}
}