using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class MetaPlayerView : MetaView
{
	[Header("Stats")]
	public Text hpText;

	public Text defText;

	public Text rcvText;

	public Text melText;

	public Text rngText;

	public Text hrmText;

	public Text prmText;

	public Text vdText;

	public Text attackText;

	public CanvasGroup attackStatsGroup;

	private bool statsVisible;

	[Header("Level/Exp")]
	public Text levelText;

	public Text expText;

	public Image expBar;

	[Header("Other")]
	public Text playerNameText;

	[Header("Behaviour")]
	public Color positiveColor = Color.green;

	public Color negativeColor = Color.red;

	public new MetaPlayer GetObject()
	{
		return base.GetObject() as MetaPlayer;
	}

	protected override void OnInteract()
	{
		base.OnInteract();
		if (!statsVisible)
		{
			ShowAttackStats();
		}
		else
		{
			HideAttackStats();
		}
	}

	private void ShowAttackStats(bool instant = false, bool forceSave = true)
	{
		if ((bool)attackStatsGroup)
		{
			if (instant)
			{
				statsVisible = true;
				attackStatsGroup.alpha = 1f;
			}
			else
			{
				statsVisible = true;
				UITweenerEx.fadeIn(attackStatsGroup, 0.25f);
			}
			if (forceSave)
			{
				SingletonComponent<Meta, ArmoryComponent>.Instance.showAttacStats = true;
				Singleton<Meta>.Instance.RequestSave();
			}
		}
	}

	private void HideAttackStats(bool instant = false, bool forceSave = true)
	{
		if ((bool)attackStatsGroup)
		{
			if (instant)
			{
				statsVisible = false;
				attackStatsGroup.alpha = 0f;
			}
			else
			{
				statsVisible = false;
				UITweenerEx.fadeOut(attackStatsGroup, 0.25f);
			}
			if (forceSave)
			{
				SingletonComponent<Meta, ArmoryComponent>.Instance.showAttacStats = false;
				Singleton<Meta>.Instance.RequestSave();
			}
		}
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		if (SingletonComponent<Meta, ArmoryComponent>.Instance.showAttacStats)
		{
			ShowAttackStats(instant: true, forceSave: false);
		}
		else
		{
			HideAttackStats(instant: true, forceSave: false);
		}
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaPlayer @object = GetObject();
		Stats stats = @object.stats;
		Stats statsDeltaForSelectedItem = @object.statsDeltaForSelectedItem;
		Helpers.SetText(hpText, StatToString(stats.HP, statsDeltaForSelectedItem.HP));
		Helpers.SetText(defText, StatToString(stats.Def, statsDeltaForSelectedItem.Def));
		Helpers.SetText(rcvText, StatToString(stats.Rcv, statsDeltaForSelectedItem.Rcv));
		Helpers.SetText(melText, StatToString(stats.Mel, statsDeltaForSelectedItem.Mel));
		Helpers.SetText(rngText, StatToString(stats.Rng, statsDeltaForSelectedItem.Rng));
		Helpers.SetText(hrmText, StatToString(stats.Hrm, statsDeltaForSelectedItem.Hrm));
		Helpers.SetText(prmText, StatToString(stats.Prm, statsDeltaForSelectedItem.Prm));
		Helpers.SetText(vdText, StatToString(stats.Vd, statsDeltaForSelectedItem.Vd));
		Helpers.SetText(attackText, StatToString(stats.totalAttack, statsDeltaForSelectedItem.totalAttack));
		Helpers.SetText(levelText, @object.level.ToString());
		if (@object.levelExpLimit > 0)
		{
			Helpers.SetText(expText, "Exp " + @object.levelExp.ToString() + "/" + @object.levelExpLimit);
			if ((bool)expBar)
			{
				expBar.fillAmount = (float)@object.levelExp / (float)@object.levelExpLimit;
			}
		}
		else
		{
			Helpers.SetText(expText, "Max level reached");
			if ((bool)expBar)
			{
				expBar.fillAmount = 1f;
			}
		}
		Helpers.SetText(playerNameText, @object.playerName);
	}

	public static void RichColorBegin(StringBuilder sb, Color32 color)
	{
		sb.Append("<color=#");
		sb.Append(color.r.ToString("X2"));
		sb.Append(color.g.ToString("X2"));
		sb.Append(color.b.ToString("X2"));
		sb.Append(color.a.ToString("X2"));
		sb.Append(">");
	}

	public static void RichColorEnd(StringBuilder sb)
	{
		sb.Append("</color>");
	}

	private string StatToString(int value, int diff)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(value);
		if (diff != 0)
		{
			stringBuilder.AppendLine();
			if (diff > 0)
			{
				RichColorBegin(stringBuilder, positiveColor);
				stringBuilder.Append("+");
				stringBuilder.Append(diff);
				RichColorEnd(stringBuilder);
			}
			else
			{
				RichColorBegin(stringBuilder, negativeColor);
				stringBuilder.Append(diff);
				RichColorEnd(stringBuilder);
			}
		}
		return stringBuilder.ToString();
	}

	protected override void Awake()
	{
		initialMetaObject = MetaPlayer.local;
		base.Awake();
	}

	public IEnumerator ExpSequencePre(int diff)
	{
		MetaPlayer obj = GetObject();
		int start = obj.levelExp;
		int end = start + diff;
		float a = 0.25f;
		float b = 0.75f;
		float duration = Mathf.Lerp(a, b, Mathf.Clamp01(Mathf.Sqrt((float)diff * 1f - 1f) / 10f));
		return CoroutineHelper.AnimateInTime(duration, delegate(float t)
		{
			int num = (int)Mathf.Lerp(start, end, t);
			Helpers.SetText(expText, "Exp " + num.ToString() + "/" + obj.levelExpLimit);
			if ((bool)expBar)
			{
				expBar.fillAmount = Mathf.Min((float)obj.levelExp / (float)obj.levelExpLimit, 1f);
			}
		});
	}
}
