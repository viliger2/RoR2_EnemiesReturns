using EnemiesReturns.Projectiles;
using R2API;
using RoR2;
using RoR2.Projectile;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.SandCrab
{
    public class SandCrabStuff
    {
        public GameObject CreateSnipEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBiteTrail.prefab").WaitForCompletion().InstantiateClone("SpitterBiteEffect", false);

            var particleSystem = clonedEffect.GetComponentInChildren<ParticleSystem>();
            var main = particleSystem.main;
            main.startRotationX = new ParticleSystem.MinMaxCurve(0f, 0f);
            main.startRotationY = new ParticleSystem.MinMaxCurve(140f, 140f);

            particleSystem.gameObject.transform.localScale = new Vector3(2f, 2f, 2f);

            return clonedEffect;
        }
    }
}
