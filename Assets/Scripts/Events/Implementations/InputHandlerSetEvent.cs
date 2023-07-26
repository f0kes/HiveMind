using Characters;

namespace Events.Implementations
{
	public struct InputHandlerSetData
	{
		public readonly Character OldCharacter;
		public readonly Character NewCharacter;
		public InputHandlerSetData(Character oldCharacter, Character newCharacter)
		{
			OldCharacter = oldCharacter;
			NewCharacter = newCharacter;
		}
	}
	public abstract class InputHandlerSetEvent : GameEvent<InputHandlerSetData>
	{
	}

}