using DefaultNamespace;

namespace Events.Implementations
{
	public struct CharacterSwappedData
	{
		public readonly Entity OldCharacter;
		public readonly Entity NewCharacter;
		public CharacterSwappedData(Entity oldCharacter, Entity newCharacter)
		{
			OldCharacter = oldCharacter;
			NewCharacter = newCharacter;
		}
	}
	public abstract class CharacterSwappedEvent : GameEvent<CharacterSwappedData>
	{
	}
}