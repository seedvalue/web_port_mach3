using System.Collections.Generic;

public interface IPerkStructureMatchHandler
{
	void OnStructureMatch(M3Orb orbType, int orbCount, M3OrbStructure structure, List<M3AttackModifier> atkModifiers, List<M3DefenseModifier> defModifiers);
}
