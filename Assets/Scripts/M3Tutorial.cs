using System.Collections.Generic;
using UnityEngine;
using Utils;

public class M3Tutorial : MonoBehaviour
{
	public bool itemsActive = true;

	public int mobCooldownOverride = -1;

	public int mobIndex = -1;

	public int itemCooldownOverride = -1;

	public int itemIndex = -1;

	private List<M3TutorialStep> steps = new List<M3TutorialStep>();

	private int currentStep = -1;

	public M3TutorialStep CurrentStep => (!Helpers.IntInRange(currentStep, 0, steps.Count)) ? null : steps[currentStep];

	public M3TutorialActivity CurrentActivity => (CurrentStep != null) ? CurrentStep.tutorialActivity : M3TutorialActivity.None;

	public void Init()
	{
		steps.Clear();
		steps.AddRange(GetComponentsInChildren<M3TutorialStep>());
		for (int i = 0; i < steps.Count; i++)
		{
			steps[i].Init();
		}
		currentStep = 0;
	}

	public bool UpdateAfterActivity(M3TutorialActivity finishedActivity)
	{
		if (finishedActivity == M3TutorialActivity.None)
		{
			CurrentStep.EnterStep(this);
			return true;
		}
		if (finishedActivity == CurrentActivity)
		{
			CurrentStep.ExitStep(this);
			currentStep++;
			if ((bool)CurrentStep)
			{
				CurrentStep.EnterStep(this);
			}
			return true;
		}
		return false;
	}

	public void Save(M3SaveTutorial saveTutorial)
	{
		saveTutorial.step = currentStep;
	}

	public bool Load(M3SaveTutorial saveTutorial)
	{
		currentStep = saveTutorial.step;
		return true;
	}
}
