using System.Collections.Generic;

public interface IPerkComboLevelHandler
{
	void OnComboLevel(int comboLevel, List<M3AttackModifier> atkModifiers, List<M3DefenseModifier> defModifiers);
}
