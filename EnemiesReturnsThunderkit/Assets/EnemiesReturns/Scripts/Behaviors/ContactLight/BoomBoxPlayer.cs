using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.ContactLight
{
    public class BoomBoxPlayer : NetworkBehaviour
    {
        public LoopSoundDef[] musicTracks;

        public float reuseDelay = 2f;

        private LoopSoundDef currentTrack;

        private GenericInteraction genericInteraction;

        [SyncVar(hook = nameof(OnTrackChanged))]
        private int currentTrackIndex;

        private float refreshTimer;

        private bool waitingForRefresh;

        private void Awake()
        {
            genericInteraction = GetComponent<GenericInteraction>();
        }

        private void Start()
        {
            if(musicTracks.Length == 0)
            {
                this.enabled = false;
            }

            currentTrackIndex = 0; // always start at 0 so less to sync on awake
            SwitchTracks(currentTrackIndex);
        }

        public void PickRandomTrack(Interactor interactor)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!this.enabled)
            {
                return;
            }

            var nextTrack = UnityEngine.Random.Range(0, musicTracks.Length);
            currentTrackIndex = nextTrack;
            SwitchTracks(currentTrackIndex);

            refreshTimer = reuseDelay;
            waitingForRefresh = true;
            genericInteraction.SetInteractabilityConditionsNotMet();
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if(waitingForRefresh)
            {
                refreshTimer -= Time.fixedDeltaTime;
                if(refreshTimer <= 0f)
                {
                    genericInteraction.SetInteractabilityAvailable();
                    waitingForRefresh = false;
                }
            }
        }

        private void OnTrackChanged(int newValue)
        {
            if (!this.enabled)
            {
                return;
            }

            SwitchTracks(currentTrackIndex);
        }

        private void SwitchTracks(int nextTrack)
        {
            if (currentTrack)
            {
                Util.PlaySound(currentTrack.stopSoundName, gameObject);
            }

            if(nextTrack >= musicTracks.Length)
            {
                Log.Warning($"BoomBox tried to switch to track {nextTrack} above max array lenght {musicTracks.Length}. Doing nothing...");
                return;
            }

            var newTrack = musicTracks[nextTrack];
            if (!newTrack)
            {
                Log.Warning($"LoopSoundDef at index {nextTrack} is null. Doing nothing...");
                return;
            }

            Util.PlaySound(newTrack.startSoundName, gameObject);

            currentTrack = newTrack;
        }

    }
}
