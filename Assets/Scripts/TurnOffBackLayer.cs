using UnityEngine;

public class TurnOffBackLayer : StateMachineBehaviour
{
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetLayerWeight(1, 0f);
		animator.SetLayerWeight(2, 0f);
		UnityEngine.Debug.Log("BackLayerTurnedOf");
	}
}
