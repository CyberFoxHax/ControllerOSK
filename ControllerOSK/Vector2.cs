namespace ControllerOSK{
	public struct Vector2{
		public Vector2(float x, float y)
			: this() {
			X = x;
			Y = y;
		}

		public float X;
		public float Y;

		public static bool operator ==(Vector2 a, Vector2 b){
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(Vector2 a, Vector2 b){
			return !(a == b);
		}
	}
}