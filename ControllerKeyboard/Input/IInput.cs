namespace ControllerKeyboard.Input {
	public interface IInput{
		event System.Action<IInput> KeyChange;

		Vector2 BlockPos { get; set; }
		Vector2 CharPos { get; set; }

		bool ChangeCase { get; set; }
		bool ChangeSymbols { get; set; }

		bool MoveLeft { get; set; }
		bool MoveRight { get; set; }
		bool Delete { get; set; }
		bool Space { get; set; }

		bool Close { get; set; }
		bool Show { get; set; }
	}
}
