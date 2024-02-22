using UnityEngine;
using UnityEngine.UI;

public class M3StatusBar : MonoBehaviour
{
	public Text textDef;

	public Text textRcv;

	public Image star3;

	public Image star2;

	public Text stepCounter;

	private M3Player player;

	private MetaStage metaStage;

	private M3BattleCounter battleCounter;

	private int numStepsStars3 = 1;

	private int numStepsStars2 = 2;

	private int numSteps = 1;

	public int StageResult => (numSteps <= numStepsStars3) ? 3 : ((numSteps > numStepsStars2) ? 1 : 2);

	public void Init(MetaStage stage)
	{
		player = UnityEngine.Object.FindObjectOfType<M3Player>();
		metaStage = stage;
		if ((bool)metaStage)
		{
			numStepsStars2 = metaStage.star2;
			numStepsStars3 = metaStage.star3;
		}
		battleCounter = GetComponentInChildren<M3BattleCounter>();
		if ((bool)battleCounter)
		{
			battleCounter.Init(metaStage.battles.Count);
		}
		UpdateState();
	}

	public void NextMove()
	{
		numSteps++;
		UpdateState();
	}

	public void NextBattle()
	{
		if ((bool)battleCounter)
		{
			battleCounter.NextBattle();
		}
	}

	private void UpdateState()
	{
		if ((bool)textDef)
		{
			textDef.text = "Def: " + player.Defense.ToString();
		}
		if ((bool)textRcv)
		{
			textRcv.text = "Rcv: " + player.Recovery.ToString();
		}
		if ((bool)star2)
		{
			star2.enabled = (numSteps <= numStepsStars2);
		}
		if ((bool)star3)
		{
			star3.enabled = (numSteps <= numStepsStars3);
		}
		if ((bool)stepCounter)
		{
			if (numSteps <= numStepsStars3)
			{
				stepCounter.text = numSteps.ToString() + "/" + numStepsStars3.ToString();
			}
			else if (numSteps <= numStepsStars2)
			{
				stepCounter.text = numSteps.ToString() + "/" + numStepsStars2.ToString();
			}
			else
			{
				stepCounter.text = numSteps.ToString() + "/-";
			}
		}
	}
}
