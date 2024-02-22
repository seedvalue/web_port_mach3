using System;

[Serializable]
public class M3SaveMob
{
	public int hp;

	public int cooldown;

	public M3SaveMob(int mobHP, int mobCooldown)
	{
		hp = mobHP;
		cooldown = mobCooldown;
	}

	public M3SaveMob()
	{
	}
}
