using RoR2;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Enemy
{
    public class ChargingExplosion : BaseChargingExplosion
    {
        public override float duration => EnemiesReturns.Configuration.Ifrit.PillarExplosionChargeDuration.Value;

        private TemporaryOverlayInstance temporaryOverlay;

        public override void OnEnter()
        {
            base.OnEnter();
            var modelTransform = GetModelTransform();
            if (modelLocator)
            {
                var characterModel = modelTransform.GetComponent<CharacterModel>();
                if (characterModel)
                {
                    temporaryOverlay = TemporaryOverlayManager.AddOverlay(base.gameObject);
                    temporaryOverlay.duration = duration;
                    temporaryOverlay.originalMaterial = CharacterModel.fullCritMaterial;
                    temporaryOverlay.AddToCharacterModel(characterModel);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextState(new Enemy.FireExplosion());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (temporaryOverlay != null)
            {
                temporaryOverlay.Destroy();
                temporaryOverlay = null;
            }
        }
    }
}
