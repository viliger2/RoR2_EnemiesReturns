using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface ISfxLocator
    {
        protected class SfxLocatorParams
        {
            public string deathSound = "";
            public string barkSound = "";
            public string openSound = "";
            public string landingSound = "";
            public string fallDamageSound = "";
            public string aliveLoopStart = "";
            public string aliveLoopStop = "";
            public string sprintLoopStart = "";
            public string sprintLoopStop = "";
        }

        protected bool NeedToAddSfxLocator();

        protected SfxLocatorParams GetSfxLocatorParams();

        protected SfxLocator AddSfxLocator(GameObject bodyPrefab, SfxLocatorParams locatorParams)
        {
            SfxLocator sfxLocator = null;
            if (NeedToAddSfxLocator())
            {
                sfxLocator = bodyPrefab.GetOrAddComponent<SfxLocator>();
                sfxLocator.deathSound = locatorParams.deathSound;
                sfxLocator.barkSound = locatorParams.barkSound;
                sfxLocator.openSound = locatorParams.openSound;
                sfxLocator.landingSound = locatorParams.landingSound;
                sfxLocator.fallDamageSound = locatorParams.fallDamageSound;
                sfxLocator.aliveLoopStart = locatorParams.aliveLoopStart;
                sfxLocator.aliveLoopStop = locatorParams.aliveLoopStop;
                sfxLocator.sprintLoopStart = locatorParams.sprintLoopStart;
                sfxLocator.sprintLoopStop = locatorParams.sprintLoopStop;
            }
            return sfxLocator;
        }

    }
}
