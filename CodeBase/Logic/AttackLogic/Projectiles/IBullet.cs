using System;
using Assets.CodeBase.Infrastructure.Data;

namespace Assets.CodeBase.Logic.AttackLogic.Projectiles
{
    public interface IBullet
    {
        public event Action Destroyed;
        public Team OwnerTeam { get; set; }
    }
}