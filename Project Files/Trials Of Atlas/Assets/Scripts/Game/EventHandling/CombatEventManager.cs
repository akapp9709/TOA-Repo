using System;

public class CombatEventManager
{
    public Action AttackStart;
    
    public Action LastAttackStart;
    public Action LastAttackEnd;

    public Action DashStart;
    public Action DashEnd;

    public enum CombatEventType{
        AttackStart,
        LastAttackStart,
        LastAttackEnd,
        DashStart,
        DashEnd
    }
}
