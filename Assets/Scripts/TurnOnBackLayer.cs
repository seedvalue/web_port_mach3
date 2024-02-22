using UnityEngine;

public class TurnOnBackLayer : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetLayerWeight(1, 1f);
		animator.SetLayerWeight(2, 1f);
		UnityEngine.Debug.Log("BackLayerTurnedOf");
	}
}
