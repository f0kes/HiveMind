using AI.ComplexBehaviours;
using AI.SteeringBehaviours;
using DefaultNamespace.AI;

namespace AI.ConcreteAI
{
	public class SniperAI : CharacterAi
	{
		protected override void PopulateBehaviours()
		{
			base.PopulateBehaviours();
			Behaviours.Add(new ComplexBehaviour(new BehindTankPointGetter(5f), new AttractBehaviour(), 8f));
		}
	}
}