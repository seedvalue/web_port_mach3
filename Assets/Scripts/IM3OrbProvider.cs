using System.Collections.Generic;

public interface IM3OrbProvider
{
	M3Orb GenerateOrb(int column);

	void StartGeneration();

	void Save(List<int> saveGeneratedOrbs);

	bool Load(List<int> saveGeneratedOrbs);
}
