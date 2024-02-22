using System;
using System.Collections;
using System.Reflection;

[Serializable]
public class MetaTutorialRequirement
{
	public MetaObject target;

	public string property;

	public string value;

	public CompareType compareType;

	private PropertyInfo targetProperty;

	private object targetValue;

	public bool completed
	{
		get
		{
			switch (compareType)
			{
			case CompareType.Equal:
				return Compare() == 0;
			case CompareType.NotEqual:
				return Compare() != 0;
			case CompareType.Greater:
				return Compare() > 0;
			case CompareType.GreaterOrEqual:
				return Compare() >= 0;
			case CompareType.Less:
				return Compare() < 0;
			case CompareType.LessOrEqual:
				return Compare() <= 0;
			default:
				return false;
			}
		}
	}

	public bool Validate()
	{
		try
		{
			targetProperty = target.GetType().GetProperty(property, BindingFlags.Instance | BindingFlags.Public);
			if (targetProperty.PropertyType.IsEnum)
			{
				targetValue = Enum.Parse(targetProperty.PropertyType, value);
			}
			else
			{
				targetValue = Convert.ChangeType(value, targetProperty.PropertyType);
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	private int Compare()
	{
		if (!Validate())
		{
			throw new Exception("Requirement is not valid");
		}
		return Comparer.Default.Compare(targetProperty.GetValue(target, null), targetValue);
	}
}
