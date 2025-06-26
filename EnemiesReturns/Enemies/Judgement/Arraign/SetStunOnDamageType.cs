using EntityStates;
using R2API;
using RoR2;
using UnityEngine.Networking;
using static R2API.DamageAPI;

namespace EnemiesReturns.Enemies.Judgement.Arraign
{
    public class SetStunOnDamageType : NetworkBehaviour, IOnTakeDamageServerReceiver
    {
        public EntityStateMachine targetStateMachine;

        public EntityStateMachine[] idleStateMachines;

        public SerializableEntityStateType stunState;

        public float stunDuration;

        public bool applyBuff;

        public BuffDef buff;

        public float buffDuration = -1f;

        public ModdedDamageType moddedDamateType => Content.DamageTypes.EndGameBossWeapon;

        private bool hasEffectiveAuthority = true;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            UpdateAuthority();
        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();
            UpdateAuthority();
        }

        private void UpdateAuthority()
        {
            hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
        }

        private void Start()
        {
            UpdateAuthority();
        }

        public void OnTakeDamageServer(DamageReport damageReport)
        {
            if (!targetStateMachine)
            {
                return;
            }

            if (damageReport.damageInfo == null || !damageReport.victimBody)
            {
                return;
            }

            var damageType = damageReport.damageInfo.damageType;
            if (damageType.HasModdedDamageType(moddedDamateType) && (damageType.damageSource & DamageSource.Equipment) == DamageSource.Equipment)
            {
                SetStun(stunDuration);
                if (applyBuff)
                {
                    if (buffDuration < 0)
                    {
                        damageReport.victimBody.AddBuff(buff);
                    }
                    else
                    {
                        damageReport.victimBody.AddTimedBuff(buff, buffDuration);
                    }
                }
            }
        }

        public void SetStun(float duration)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (hasEffectiveAuthority)
            {
                SetStunInternal(duration);
            }
            else
            {
                RpcSetStun(duration);
            }
        }

        private void SetStunInternal(float duration)
        {
            if (targetStateMachine)
            {
                var newStunState = EntityStateCatalog.InstantiateState(ref stunState);
                (newStunState as StunState).stunDuration = duration;
                targetStateMachine.SetInterruptState(newStunState, InterruptPriority.Stun);
            }
            foreach (var machine in idleStateMachines)
            {
                machine.SetNextStateToMain();
            }
        }

        [ClientRpc]
        private void RpcSetStun(float duration)
        {
            if (hasEffectiveAuthority)
            {
                SetStunInternal(duration);
            }
        }
    }
}
