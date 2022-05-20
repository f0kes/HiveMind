using DefaultNamespace.AI;
using AI.ComplexBehaviours;
using AI.SteeringBehaviours;

namespace AI.ConcreteAI
{
	public class ProtectorAI : CharacterAi
	{
		protected override void PopulateBehaviours()
		{
			base.PopulateBehaviours();
			//Behaviours.Add(new ComplexBehaviour(new FriendPointGetter(2f), new AttractBehaviour(), 2));
		}
	}
}