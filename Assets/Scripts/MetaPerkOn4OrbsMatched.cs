using System.Collections.Generic;

public class MetaPerkOn4OrbsMatched : MetaPerk, IPerkStructureMatchHandler
{
	public M3Orb orbType1;

	public float damageAmplifier = 1f;

	public void OnStructureMatch(M3Orb orbType, int orbCount, M3OrbStructure structure, List<M3AttackModifier> atkModifiers, List<M3DefenseModifier> defModifiers)
	{
		if (atkModifiers.Count == 0)
		{
			M3AttackModifier item = new M3AttackModifier(M3AttackModifierRange.Global, 1.1f, 1);
			atkModifiers.Add(item);
		}
		string str = string.Empty;
		switch (structure)
		{
		case M3OrbStructure.four:
			str = "Czwórka, ";
			break;
		case M3OrbStructure.row:
			str = "Wiersz, ";
			break;
		case M3OrbStructure.cross:
			str = "Krzyż, ";
			break;
		}
		str += M3Board.orbNames[(int)orbType];
	}
}
