using AI.ComplexBehaviours;
using AI.SteeringBehaviours;
using DefaultNamespace.AI;

namespace AI.ConcreteAI
{
	public class BillyBonkaAI : CharacterAi
	{
		protected override void PopulateBehaviours()
		{
			base.PopulateBehaviours();
			Behaviours.Add(new ComplexBehaviour(new EnemyPointGetter(3f, true), new AttractBehaviour(), 5f));
			Behaviours.Add(new ComplexBehaviour(new EnemyPointGetter(3f), new StrafeBehaviour(), 4f));
		}
	}
}