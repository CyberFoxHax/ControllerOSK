namespace ControllerOSK.Input{
	public struct InputState {
		public Vector2 BlockPos;
		public Vector2 CharPos;
		public bool ChangeCase;
		public bool ChangeSymbols;
		public bool MoveLeft;
		public bool MoveRight;
		public bool Delete;
		public bool Space;
		public bool Return;
		public bool OpenClose;

		public InputState(IInput input) {
			BlockPos = input.BlockPos;
			CharPos = input.CharPos;
			ChangeCase = input.ChangeCase;
			ChangeSymbols = input.ChangeSymbols;
			MoveLeft = input.MoveLeft;
			MoveRight = input.MoveRight;
			Delete = input.Delete;
			Space = input.Space;
			Return = input.Return;
			OpenClose = input.OpenClose;
		}

		public static bool operator ==(InputState a, InputState b) {
			return
				a.BlockPos == b.BlockPos &&
				a.CharPos == b.CharPos &&
				a.ChangeCase == b.ChangeCase &&
				a.ChangeSymbols == b.ChangeSymbols &&
				a.MoveLeft == b.MoveLeft &&
				a.MoveRight == b.MoveRight &&
				a.Delete == b.Delete &&
				a.Space == b.Space &&
				a.Return == b.Return &&
				a.OpenClose == b.OpenClose;
		}

		public static bool operator !=(InputState a, InputState b) {
			return !(a == b);
		}

		private bool Equals(InputState other) {
			return this == other;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			return obj is InputState && Equals((InputState)obj);
		}

		public override int GetHashCode() {
			unchecked {
				var hashCode = BlockPos.GetHashCode();
				hashCode = (hashCode * 397) ^ CharPos.GetHashCode();
				hashCode = (hashCode * 397) ^ ChangeCase.GetHashCode();
				hashCode = (hashCode * 397) ^ ChangeSymbols.GetHashCode();
				hashCode = (hashCode * 397) ^ MoveLeft.GetHashCode();
				hashCode = (hashCode * 397) ^ MoveRight.GetHashCode();
				hashCode = (hashCode * 397) ^ Delete.GetHashCode();
				hashCode = (hashCode * 397) ^ Space.GetHashCode();
				hashCode = (hashCode * 397) ^ Return.GetHashCode();
				hashCode = (hashCode * 397) ^ OpenClose.GetHashCode();
				return hashCode;
			}
		}
	}
}