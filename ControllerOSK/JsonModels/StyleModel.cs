namespace ControllerOSK.JsonModels {
	public class StyleModel {
		public StyleModel SetDefaults(){
			if (RootSize				 == null) RootSize = 512;
			if (CircleDistanceFromCenter == null) CircleDistanceFromCenter = 167;
			if (CircleSize				 == null) CircleSize = 80;
			if (ActiveCircleSize		 == null) ActiveCircleSize = CircleSize;
			if (CirclePadding			 == null) CirclePadding = 10;
			if (ActiveCirclePadding		 == null) ActiveCirclePadding = CirclePadding;
			if (NormalTextStyle			 == null) NormalTextStyle = new TextStyle();
			NormalTextStyle.SetDefaults();
			if (ActiveTextStyle			 == null) ActiveTextStyle = NormalTextStyle;
			ActiveTextStyle.SetDefaults();
			return this;
		}

		public string BackgroundImage { get; set; }
		public string CursorImage { get; set; }
		public double? RootSize{ get; set; }
		public double? CircleDistanceFromCenter { get; set; }
		public double? CircleSize { get; set; }
		public double? CirclePadding { get; set; }
		public double? ActiveCircleSize { get; set; }
		public double? ActiveCirclePadding { get; set; }

		public TextStyle NormalTextStyle { get; set; }
		public TextStyle ActiveTextStyle { get; set; }

		public class TextStyle {
			public string FontFace { get; set; }
			public double? FontSize { get; set; }
			public string Color { get; set; }
			public bool Bold { get; set; }

			public TextStyle SetDefaults(){
				if (FontFace		== null) FontFace = "Segeo UI";
				if (FontSize		== null) FontSize = 16;
				if (Color			== null) Color = "White";
				return this;
			}
		}
	}
}
