
public abstract class ActiveReward
{
    public ConsumeReward data;
    public int curLevel = 1;
    public int experience = 0;
}
[System.Serializable]
public class ActiveWeapon : ActiveReward
{
    public GunController controller;
}

[System.Serializable]
public class RewardInstance : ActiveReward
{
    public RewardInstance(ConsumeReward reward)
    {
        this.data = reward;
    }
}