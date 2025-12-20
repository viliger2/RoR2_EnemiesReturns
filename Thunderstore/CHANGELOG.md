<details>
<summary>0.7.19</summary>

* Fixed parameterized strings not being filled for unsupported languages.
* Added Collective elite displays.
* Sand Crab
  * Fixed Sand Crab attachments floating in the air when it is being grabbed by Drifter.
* Colossus
  * Expanded awooga body list.
    * _This expands the dong._
* Judgement
  * Fixed NRE spam if players somehow went over the normal wave count.
  * Added sanitization of bodies' names when creating Anointed skin configs.
    * _This might result in some of the configs changing, I apologize for the inconvenience._
</details>
<details>
<summary>0.7.18</summary>

* Updated Simplified Chinese translation by [锅巴](https://space.bilibili.com/10832951?spm_id_from=333.1007.0.0).
* Lynx Tribe
  * Fixed being able to escape Storm just by using movement keys.
* Judgement
  * Lowered music volume to roughly match vanilla.
    * _I am so, so tired of this shit. I'll do sound effect pass some time in the future._
</details>
<details>
<summary>0.7.17</summary>

* 1.4.1 update.
* ReviveAPI is no longer a dependency.
* Added more cooking recipes.
* Swift
  * Fixed NRE when Swift prepares to dive and has no target.
* Ifrit
  * Fixed NRE when target without inventory is killed while killer has Infernal Lantern.
* Judgement
  * Fixed NRE on starting new run right after entering Judgement.
</details>
<details>
<summary>0.7.16</summary>

* Allowed boss items to be temporary. Fixed them not working if player somehow managed to make them temporary prior to this.
* Added cooking recipes.
* Lynx Shrine
  * Lynx Shrine now uses PurchaseInteraction for its activation.
    * _This is purely vanilla compatibility change, so any vanilla or modded item\behavior that checks for PurchaseInteraction.isShrine can work._
  * Shrine no longer collides with camera.
* Ifrit
  * Infernal Lantern
    * No longer triggers on elite kills. Old behavior can be restored in the config.
	* Can now chain proc from itself.
	  * _Technically it could before, it just wasn't intended._
	* Now spawns on Umbra kills.
	* Per item scaling lowered to 800% (was 1000%).
	  * _Main intention of these changes to lower chances of pillar just clearing stages by itself, chain-procing over and over again from elites (even if it wasn't intended and it still can happen now due to dots which do not save proc chain information)._
* Judgement
  * Fixed boss ignoring certain equipment if it was in non-active slot from new red item.
</details>
<details>
<summary>0.7.15</summary>

* Colossus
  * Another fix for Rock Clap rocks being incorrectly scaled.
</details>
<details>
<summary>0.7.14</summary>

* Colossus
  * Redid Rock Clap because it somehow broke with AC.
  * Increased homing speed on rocks to 40 (was 15).
    * _Number might look big, but for me it seems to be in line with how it was before AC, so I am not sure what happened._
* Judgement
  * Fixed Rotating Laser visuals not matching hitbox.
    * _I would like to also thank Randy to somehow fixing Arraign's Utility in his second phase, he shouldn't do like 3 laps around the arena anymore if you are far enough._
</details>
<details>
<summary>0.7.13</summary>

* Added support for Alloyed Collective stages.
* Colossus
  * Fixed Colossus' clap rocks being gigantic if WolfFixes is installed. This also removes RestoreGrandparentRock as dependency.
* Fixed mod not loading if AdvancedPrediction was installed.
</details>
<details>
<summary>0.7.12</summary>

* Made bosses ungrabbable (like base game).
* Mechanical Spider
  * Fixed Mechanical Spiders becoming zombies if killed when in vehicle seat (Drop Drone, Drifter's bag, etc).
* EliteReworks 2.0 compatability.
* Judgement
  * Added Commando and Bandit Anointed skins.
    * _This is not something that will happen regularly or even more at all. However we are open to submissions if someone wants to contribute to the mod._
</details>
<details>
<summary>0.7.11</summary>

* Updated for AC.
* Lynx Archer
  * Fixed Archer breaking his neck when firing from his bow.
* Sand Crab
  * Fixed Sand Crab going into T-pose animation while jumping.
* Swift
  * Extended Dive hitbox forward and made it slightly larger.
* Mechanical spiders
  * Integrated [Engi Mechanical Spider Skill](https://thunderstore.io/package/Moffein/Engi_Mechanical_Spider_Skill/) into the mod.
    * _Skill mod was made with a lot of jank, so we decided to integrate it, making it smoother and giving players more options on how to configure it. Comes with unlock condition related to Mechanical Spiders._
  * Made friendly Mechanical Spiders immune to fall damage.
  * Friendly Mechanical Spiders now make less sounds.
* Ifrit
  * Pillars from both Ifrit and players no longer leave a corpse.
* Archer Bug
  * Fixed spawning being broken on clients.
* Judgement
  * Fixed NRE when using equipment during stage transition.
  * Fixed issues when selecting monsters for waves that lobby has no entitlement for.
  * Made custom P3 music enabled by default.
    * _If Gearbox doesn't care about music style why should I?_
* Oops, I dropped my monster condom that I use for my magnum dong.
</details>
<details>
<summary>0.7.10</summary>

* Updated ru localization.
* Updated es-ES localization.
* Lynx Archer
  * Fixed NRE on stage transition if Archer is firing an arrow.
* Sand Crab
  * Increased mass to 300 (was 250).
    * _This is mainly so Loader cannot pull it with alt secondary._
  * Buffed bubbles health to 45+14(was 35+10).
  * Lowered range of footstep sounds.  
* Judgement
  * Fixed certain equipment not dropping on death.
  * Known issue: Expedition 0.1.2 breaks the pedastal if you pick up the Geolocator item. Next update to that mod will fix it. For now, just disable the item.
</details>
<details>
<summary>0.7.9</summary>

* Judgement
  * Fixed boss ignoring certain equipment.
</details>
<details>
<summary>0.7.8</summary>

* Added Simplified Chinese translation by [锅巴](https://space.bilibili.com/10832951?spm_id_from=333.1007.0.0).
</details>
<details>
<summary>0.7.7</summary>

* General polish to animation transitions for all monsters.
* Sand Crab
  * Additional visuals for grassy and sulfur variants.
  * Snip
    * Lowered damage to 3.75 (was 5).
  * Fire Bubbles
    * Lowered time between bubbles to 0.325 (was 0.45).
	* Optimized bubble mesh.
	* Added small white sphere inside of a bubble to improve visibility.
* Judgement
  * Fixed objectives going bananas on clients.
    * _There is a chance that objective might get stuck as uncompleted but this is better than objectives constantly being added and removed for like 20 seconds after wave has been started._
  * Lowered elite damage multiplier to 5 (was 6).
  * Phase 2 spawns
    * Increased spawn timer to 8-10 (was 3-5).
	* Lowered credit multiplier to 0.5 (was 0.75).
	  * _This just means that there will be less monsters spawning during phase 2._
  * Added fixed spawns for players.
  * Added ability to enter Judgement through void.
</details>
<details>
<summary>0.7.6</summary>

* I am 99% sure I've released 0.7.5 without functioning networking, so I am just gonna do another release just to be sure.
* Added soft dependency to [MoreStats](https://thunderstore.io/package/RiskOfBrainrot/MoreStats/)
  * _MoreStats breaks Lynx Shaman healing debuff hook, making it so debuff doesn't actually reduce healing. So I've decided to add soft dependency to it, using it if it is present. HOWEVER, current version of MoreStats (1.2.5) has broken hook for healing modification (lol) so, at the time of 0.7.6 release, if you have MoreStats installed you wouldn't get working healing debuff either way._
* Sand Crab
  * Set global death proc chance for bubbles to 0.
    * _This means that bubbles no longer proc on death effects. Value can be changed in the config._
</details>
<details>
<summary>0.7.5</summary>

* Added new Medium-tier monster: Sand Crab.
* Swift:
  * Fixed death animation not being played on clients.
  * Buffed movement speed to 10 (was 7).
  * Nerfed Dive speed coefficient to 6.3 (was 9).
    * _Dive is still the same, just adjusted to new movement speed so it is not faster than before._
* Colossus:
  * Min stage completion changed to 1 (was 0).
  * Laser Barrage projectile damage coefficient buffed to 1 (was 0.5).
    * _Laser Barrage was rather weak, but on stage 1 (and Colossus on stage 1 in general) with new buff it would rip survivors to shreds, so we decided to up minimum stage completion. It doesn't make that huge of a difference since in order to see Colossus on stage 1 you would need to pump those mountain shrines._
* Mechanical Spider:
  * Restored elite ramp display on purchasable drone.
  * Fixed purchasable drone spawning infinitely and throwing errors if interactable is spawned from elite that can't spawn normally. 
* Judgement:
  * Fixed NPC allies attacking enemies during cinematic phases.
  * Improved pedestal visibility.
  * Added ability to create Anointed skins from any skin.
    * _There is a new separate section in Judgement config dedicated to skin creation. Each survivor gets an entry where you can specify which skin will be used as source for Anointed skin. Do not expect 100% compatability with modded skins, especially ones with dynamic bones, but vanilla skins and more modest custom skins should work without issues._
</details>
<details>
<summary>0.6.6</summary>

* Fixed disabling Judgement breaks the game.
</details>
<details>
<summary>0.6.5</summary>

* Did another sound effect volume pass.
  * _Changing volume curve from last update caused a lot of sounds becoming way louder than before. I've adjusted almost all of them, so hopefully it should be in line with vanilla. Judgement sounds are mostly left the same, since sound cues are important for that encounter._
* Another attempt to fix network desync on Lynx Shrine item.
</details>
<details>
<summary>0.6.4</summary>

* Switched sound effects volume curve from Linear to Sine.
  * _With this change sounds should follow the same volume curve as vanilla sounds. This will bring zero changes to people who play at 100% sound effect volume and will increase loudness for everyone else. Please let me know if some sounds become too loud, since I play at 100% volume and would not notice the difference._
* Ifrit
  * Flame Charge
    * Buffed flame damage to 0.625 (was 0.4).
	* Nerfed body damage to 1.875 (was 2.5).
	* Nerfed turn speed to 160 (was 200).
	  * _Flame Charge was a bit too difficult to dodge without movement speed items and damage balancing was too skewed toward body contact damage, while flame was dealing basically nothing._
* Archer Bug
  * Movement pause duration when strafing around target changed to 1 second (was 0.75).
  * Attack projectile spread changed to 25 degrees between projectiles (was 20).
* Lynx Tribe
  * Totem
    * Now has invulnerability for the duration of animation when summoned from Shaman.
    * Now Storm applies DoT to targets that were grabbed but weren't thrown away before Storm disappears.
  * Shrine
    * Added item info on ping.
  * Shaman
    * Projectile now has lifetime expire effect.  
  * Scout
    * Buffed movement speed to 13ms (was 12).
    * Slightly increased attack sound volume.	
  * Hunter
    * Increased pre attack sound volume.  
* Mechanical spiders
  * Fixed variants not working.
    * _According to my git history it wasn't working from the very start but I distinctly remember it working? Dunno what happened here._  
* Swift
  * Fixed Swift spawning on ground nodes.
    * _I told you about the nodes bro!_
  * Dive
    * No longer adjusts movement vector during it.
    * Now has predictive aiming.
	* Slightly extended hitbox forward.
  * Slight animation and visual improvements.
  * Added Rallypoint Delta specific variant.
    * _Swifts were blending into walls on Rallypoint Delta. With how difficult post processing is on that stage to work with we made a very garish Swift variant that will not blend with the walls, however it will look terrible outside of Rallypoint, so I do not recommend putting it anywhere else. While adding snow variant, similar to golems, is an option, the issue is that this is a flying enemy and you will be looking at it from below 99% of the time, basically not seeing the snow at all._
* Judgement
  * Item selection now shows item count.	
</details>
<details>
<summary>0.6.3</summary>

* Added new Small-tier monster: Swift.
* Added appropriate monsters to Gilded Coast, Meridian and Artifact Reliquary (all variants).
* Archer Bug
  * Scaled size down by roughly 10%.
  * Added new Archer Bug variant courtesy of Synodii.
    * It spawns on Sundered Grove and Siren's Call.
* Ifrit
  * Pillar now has post processing and its light drops shadows. Pillars spawned by players are unaffected by these changes.
* Judgement
  * Added chat messages on item selection.
* Party time!
  * _It has been a year since first version of EnemiesReturns has been released. We would like to thank everyone who downloaded and played the mod, gave feedback and talked about it, it means a lot to us. To commemorate the occasion we added party mode to all added enemies that occurs every year on 4th of August. We hope to bring you more content in the upcoming year._
  * <img src="https://files.catbox.moe/64y9xf.jpg" alt="Thank you!">
</details>
<details>
<summary>0.5.21</summary>

* Judgement
  * Fixed game freeze if player enters with 20 stacks of Eulogy Zero.
  * Added Stone Titan to enemy blacklist.
  * There aren't any more rains down in Africa.
</details>
<details>
<summary>0.5.20</summary>

* Lynx Tribe
  * Fixed potential game freeze if summoned tribesmen failed to spawn. 
* Judgement
  * Fixed Engineer turrets dropping equipment.
  * Implemented clientside collision checks for some attacks.
    * _This is somewhat experimental since I haven't had a good chance to test networking with real delay and on proper modpack, but now rotating laser and chasing laser attacks should use clientside hit detection. If you have any network issues after this change - please report them._
</details>
<details>
<summary>0.5.19</summary>

* Added support for more Snowtime stages.
* Lynx Tribe
  * Redid item display sync on Lynx Shrine, so item should now be visible on high latency connections.
  * Fixed Lynx Totem having negative hitboxes on being spawned from Shaman.
* Archer Bugs
  * Fixed JitterBones (Malachite elite body shaking).
  * Added option to disable Archer Bugs.
  * Fixed Archer Bugs doing nothing after spawning.
  * Fixed Archer Bugs spawning on Ground nodes instead of Air nodes.
  * Fixed yaw animation.
  * Optimizations to wing textures.
* Colossus
  * Small animation improvements.
  * Fixed Colossus playing charge particles on spawn and not on actual attack charge.  
* Judgement
  * Max enemy count on waves now scales with player count.
  * Fixed Promethean Teleporter sometimes not spawning despite requirements being fulfilled.
  * Adjustments to fix pathfinding issues and potential cheese spots.
  * Animation improvements.
  * Elites now have additional gameplay effects.
  * Added additional checks to King leaving a trophy. 
  * Fixed chat message playing that King left a trophy despite it not being the case.
</details>
<details>
<summary>0.5.18</summary>

* Fixed some language tokens.
</details>
<details>
<summary>0.5.17</summary>

* Added new Small-tier monster: Archer Bug.
* Lynx Tribe
  * Totem
    * Sex: Yes
    * Storm
      * Poison duration lowered to 4 (was 5).
	  * Poison damage increased to 1.25 (was 0.8).
	  * Grab range increased to 6 (was 3).
	  * Grab strength increased to 6 (was 5).
	  * Storm immunity duration lowered to 4 (was 8).
	  * Storm now lives for 5 seconds after Totem death.
	  * Now zeroes your jump count, so you can no longer escape it with enough Feathers. Can be disabled in the config.
	* Groundpound
      * Cooldown lowered to 5 (was 10).
      * Damage increased to 3.2 (was 3).
    * Summon Tribe
      * Summon count increased to 5 (was 4).
      * Summon cap lowered to 10 (was 12).
      * Summon max range increased to 8 (was 4).
  * Shaman
    * Animation improvements.
    * Push back range increased to 11 (was 6).
  * Hunter
    * Movement speed increased to 10 (was 7).
    * Stab damage lowered to 2.5 (was 3).	
  * _Lynx Tribe as a whole felt a bit weak, this should buff them up a bit without going overboard._
  * Shrine
    * Added additional config values.
	* Shrine now becomes dithered when close to camera.
	* Reverted shrine to dropping only one item. Shrine no longer uses multiplayer scaling for spawned enemies. Multiplayer scaling + per player items can be enabled in the config.
    * Increased visibility of timer.
	* Spawned enemies are now marked with an indicator, similar to one used for bosses.
	* There are now chat messages on initial interaction, success and failure.
  * Lynx Fetish
    * Spawns are now disabled in bazaar. This can be reverted in the config.  
  * You can now set stages that Hunter, Archer and Scout can spawn on.
* Colosus
  * Rock Clap
    * Fixed skill not firing at high attack speeds.
  * Stomp
    * Removed minimum target distance activation requirement.
  * Colossal Fist
    * Restored fade in and out effect that was "broken" since SoTS.
      * _Thank Randy._	
* Mechanical Spider
  * Projectile speed lowered to 80 (was 100).
  * Fixed spiders not dancing on victories.	  
* Fixed Twisted elites displays not working since memop update.
* Fixed NREs when mod enemies die by void death.
* Judgement.
</details>
<details>
<summary>0.4.13</summary>

* Fixed for 1.3.9 update.
* There is probably some additional stuff here, but it would be difficult to trace what is here and what is not so everything will be made into a changelog next content update.

</details>
<details>
<summary>0.4.12</summary>

* Added Spanish translation by Manuerth.
* Config files are now generated via reflection.
  * _This is not something end users should care about, but some config files might be wiped. I am sorry for the inconvenience._
* Mechanical spider
  * Added option for drones to use initial stage price coefficient, disabled by default.
  * Removed attack speed scaling on "charge attack" state.
    * _Basically, if mechanical spider still has a target nearby instead of going through the full cycle of the skill (open hatch, charge attack, fire two times, close hatch) it goes back to "charge attack" state and cycles so until target is either dead or gets far enough for skill to end. This resulted in spiders turning the game into bullethell if you were to encounter SS2 Storm on a stage where they are present. By removing attack speed scaling we make them more manageable during storms specifically, while having zero effect on balance normally._
* Lynx Tribe
  * Friendly tribesmen summoned by Lynx Fetish now ignore team member limit.
  * Lynx Shrine now drops item for each player in game instead of only one.
* Ifrit
  * Fixed Flame Charge's body attack using wrong hitbox.
    * _Attack should be a lot more manageable now and easier to dodge either via movement abilities or circle strafing._

</details>
<details>
<summary>0.4.11</summary>

* Ifrit
  * Patched up some holes in Ifrit's face. Thanks [FORCED_REASSEMBLY](https://thunderstore.io/package/Forced_Reassembly/)!
  * Added landing sound.
  * Infernal Lantern
    * Summoned pillars are now limited to 2. This can be changed in the config.
  * Summon Pillars
    * Added material overlay similar to Ocular HUD.
	* Extended sound range Ifrit's roar on pillar summon to 250m (was 180m).
	  * _These changes are intended to provide better visibility to pillars, a bit too many times I've seen people just die to pillars they didn't know were there._
  * Charge
    * Now pushes targets left or right from Ifrit, depending on their position relative to Ifrit's centre.   
	* Lowered turn speed to 200 (was 300). Now uses a separate config value.
  * Hellzone
    * Now uses current AI target's position if initial bullseye search fails to find a target.
* Lynx Totem
  * Added to Artifact of Origin.
  * Added landing sound.
  * Lynx Fetish
    * Fixed an issue where Lynx tribesmen summoned by the item would always provide buffs to player team instead of team that summoned them.
  * Summon Storms
    * Fixed potential NullReferenceException on storm grabbing the target.
	* Behavior changed: now always summons one storm on AI's current target or if it doesn't have a target, then at player that is closest to it. Old behavior can be enabled in the config.
	* Storms now die when Totem dies.
  * Groundpound
    * Scalled VFX down to match hitbox more closely.  
* Spitter
  * Patched up some holes in Spitter's guts. Thanks [FORCED_REASSEMBLY](https://thunderstore.io/package/Forced_Reassembly/)!
* Colossus
  * Added landing sound.
  * Laser Barrage
    * Colossus now slowly moves forward and actively aims at his current target instead of standing still and sweeping from left to right (or was it right to left?).
	* Duration extended to 10s (was 5s).
</details>
<details>
<summary>0.4.10</summary>

* Lynx Fetish
  * Fixed potential ArgumentOutOfRangeException.

</details>
<details>
<summary>0.4.9</summary>

* Added new Small-tier monster: Lynx Shaman.
* Added new Boss-tier monster: Lynx Totem.
* Fixed an issue where Infernal Lantern wouldn't work after stage transition until player picks up a new item.
* Fixed an issue where Infernal Lantern proc type wouldn't work.
* Implemented DamageSource for all enemies.
  * _This means that all enemies can benefit from items and effects that check if damage was made with specific ability, assuming the enemy has said ability of course._
* Colossal fist
  * Added downwards force, so flying enemies are slammed into the ground on projectile impact.
* Colossus
  * Config to spawn additional enemies on Clap post-loop is now enabled by default.
    * _This and some things with Lynx Tribe are made to incentivise looping while having zero effect on players who prefer one loop gameplay._
* Mechanical Spider
  * Broken spider now has new visual effects and sounds. 

</details>
<details>
<summary>0.3.10</summary>

* Fixed a potential ArgumentOutOfRangeException on dealing damage if Colossal Fist is disabled.

</details>
<details>
<summary>0.3.9</summary>

* Fixed bodies mass being inconsistent (you will no longer throw Spitter around as Loader with just basic attacks).
* Update to new game patch.
	* _New system for DamageSource is not implemented yet. Will be implemented with next batch of enemies, so mod's enemies could take advantage of items that trigger of specific abilities or DoTs._
* Mechanical Spider
	* Fixed Spider trying to attack owner when there are no enemies nearby.
	* Some additional visual polish.

</details>
<details>
<summary>0.3.8</summary>

* Major rewrite of how body and master prefabs are created.
	* _While this is something that users should not care or even know about, I've decided to push the update before next batch of enemies, mainly so all the issues can be ironed out. There shouldn't be any major differences between this and previous versions, but if you notice something has changed, please let me know. ~~Yes, I am essentially pushing out untested mod so you can beta test for free.~~_
* Implemented ItemRelationshipProvider, so now current and all future boss item will be converted into Newly Hatched Zoea.
* Mechanical Spider:
	* Drone AI improvement, mainly now it shoots at enemies as it tries to pathfind back to its owner.

</details>

<details>
<summary>0.3.7</summary>

* Mechanical Spider:
	* Drone Spider now takes lava damage every second instead of the usual 0.2 seconds.
	* Base damage nerfed to 12(+2.4) from 15(+3).
	* Fixed a potential NRE on Spider's death.
	* Dual Shot:
		* Charge duration nerfed to 0.75 seconds from 0.5 seconds.
			* _While I don't entirely agree with this and base damage nerf, the fact that director can just spawn 5 spiders around you and they all relentlessly shoot you can lead to losing a run right on the spot. Something had to be done, I think with this spiders will be less of a nuisance._
</details>
<details>
<summary>0.3.6</summary>

* ProcTypeAPI support.
	* _This just means that Colossal Fist and Infernal Lantern can't proc themselves._
</details>
<details>
<summary>0.3.5</summary>

* Additional assets optimization.
	* _This time it is sounds. Mod now should be about 2 mbs lighter compared to previous version._
* Mechanical Spider:
	* Added elite + Spare Drone Parts displays.
	* Fixed drone not getting UseAmbientLevel item.
		* _For those who don't know - basically it meant drone spiders were not leveling up during stage together with the rest of the survivors. Their stats would adjust in next stage tho, so they weren't stuck on the level you bought then._
	* Added 10 DamageBoost items to drone, which boosts drone's damage by 100% of normal. Value can be configured in the config.
	* Double Shot:
		* Exposed charge duration to config.
		* New projectile visuals. Projectile behaviour is unchanged.
	* Spawn sound is limited to two instances globally.
		* _Since the director has tendency to spawn spiders in groups of at least 3 or more, spawn sounds can get layered on top of each other and result in very grating experience. Instead of replacing or adjusting it, I decided to just limit it, since the sound itself comes straight from RoRR, so I don't want to touch it for legacy reasons._
</details>
<details>
<summary>0.3.4</summary>

* Updated pt-BR translation.
* Mechanical Spider:
	* Added 20 BoostHP items to drone, which boosts drone health by 200% of normal. Value can be configured in the config.
	* Added [RiskyMod](https://thunderstore.io/package/Risky_Lives/RiskyMod/) minion items to drone.
	* Drone now persists through stages. 
	* Lowered chance of drone interactable spawn to 2% (was 20%).
	* Slightly lowered volume of gun charging sound.
	* Added "new" sounds and visuals to drone. Lowered drone sound range.
	* Drone spiders now always spawn drone interactable on death.
	* Improved drone's AI, now it shouldn't shoot at player as its first shot.
</details>
<details>
<summary>0.3.3</summary>

* Added new Small-tier monster: Mechanical Spider.
* Split configuration files. Now each enemy has their own configuration file. This obviously results in config wipe.
* Added [AdvancedPrediction](https://thunderstore.io/package/score/AdvancedPrediction/) support.
* Added [Risky_Artifacts](https://thunderstore.io/package/Moffein/Risky_Artifacts/)' Artifact of Origin support. Ifrit is marked as Tier 2 boss, Colossus is marked as Tier 3 (Colossus is disabled by default).
* Added 3D Spatialization for sound effects and lowered attenuation range to 180m (was 200m).
	* _This should hopefully improve sound effects, making them less of a "100 volume at 90m range, 0 at 100m range"._
* Ifrit:
	* Restored jittery animation on having Malachite elite aspect.
	* Fixed fire particles becoming golden blocks on having Gilded elite aspect.
	* Fixed impact (or landing) animation.
* Colossus:
	* Fixed impact (or landing) animation.

</details>
<details>
<summary>0.2.7</summary>

* Made some asset optimizations.
	* _The mod is now 3mbs lighter and should consume less video memory._
* Fixed nullref spam on void death for Ifrit and Colossus.
</details>
<details>
<summary>0.2.6</summary>

* Exposed stage list to the config. If enemy has multiple variants, each variant gets its own config.
* Ifrit:
	* Added new spawning animation.
	* Made Ifrit about 15% larger. Size increase comes with adjusted hitboxes for his Flame Charge, they are scalled proportionally.
		* _Some people wanted him Colossus big, since he is about the size of Colossus in 1 and Returns. This is not happening, but with this I think we can reach a compromise._
	* Lowered jump power.
		* _No more silly airborne Ifrit for you._
	* Fixed Infernal Lantern's icon appearing huge when scraping.
	* Added dynamic bones to tail.
	* Fixed Hellzone sometimes spawning at 0.0.0 when Ifrit has no target while using the ability.
	* Added Gephyrophobia as possible stage.
</details>
<details>
<summary>0.2.5 (Visual Polish Update)</summary>

* Colossus:
	* Polished visuals.
	* Added SoTS elite displays.
	* Removed Abyssal Depths from possible stages, added Shattered Abodes and Disturbed Impact instead.
	* Laser Barrage:
		* Now has particles to indicate that Colossus is charging the attack and spotlight to indicate where he is firing.
	* Rock Clap:
		* Added option (disabled by default) to spawn monsters post loop on skill use. Non-elite Colossus will spawn one Golem, Fire Colossus will spawn 6 Wisps, Overloading will spawn 6 Jellyfishes, other elites will spawn 2 Golems. Spawned monsters inherit elite equipment but not do not get elite stat boosts.
* Spitter:
	* Polished visuals.
	* Added SoTS elite displays.	
	* Added Helminth Hatchery as possible stage. Replaces Mini Mushroom (can be disabled in the config).
	* Restored Bite effect that was broken post-SoTS.
* Ifrit:
	* Polished visuals.
	* Increased volume of some sounds so they are more distinct.
	* Hellzone:
		* Reworked. Now Ifrit fires fireball under his feet, that will spawn a volcano near targeted player. Behaves similarly to Stone Titan's fist attack.
		* As a result of rework volcano radius is nerfed to 9m (was 12m), number of rock shoots lowered to 3 (was 4).
	* Summon Pillar:
		* Now destroying the pillar makes it explode and deal damage to all monsters without dealing damage to players. It will play a distinct sound effect and play animation of fireball hitting the ground if players manage to do that.
		* Lowered pillar health to 585(+176) (was 720(+216)).
	* Flame Charge:
		* Now has new animation and sound effects on stomping.	
</details>
<details>
<summary>0.2.4</summary>

* Colossus:
	* Director credits increased from 1000 to 1150.
		* _This was actually made last patch but I forgot to add it to changelog. He is a bit too strong to appear early on, but should still be there if you just keep that Shrine of Mountain streak going._
* Ifrit:
	* Added missing Simulacrum stages and Void Cell as possible spawn stage.
	* Fixed NRE with Elder Lemurian flame attack.
* Spitter:
	* Restored Sulfur Pools spawning.
</details>
<details>
<summary>0.2.3</summary>

* Added new Champion-tier (or boss) monster: Ifrit.
* Restored DirectorAPI dependency and added DeployableAPI dependency.
	* _DirectorAPI dependency restores enemy spawns on custom stages. It wasn't working since SoTS._
* Fixed potential issue with boss drops in other mods due to code-created ScriptableObjects not having names.
* Added option to disable each instance of content separately.
	* _This means you can have boss items without bosses and vice versa. But obviously if you have boss item without a boss that means it can only spawn either via printers or Artifact of Command._
* Colossus:
	* Fixed Laser Barrage explosion damage being scaled of Laser Barrage Explosion radius config instead of correct config.
</details>
<details>
<summary>0.1.13</summary>

* Fixed Colossus not spawning outside of family events.
* Fixed Colossus' Laser Barrage not doing full head spin on high attack speed.
* Added soft dependency on RestoreGrandparentRock.
</details>
<details>
<summary>0.1.12 (The Moon actually fell)  </summary>

* SoTS update.
* Removed R2API dependencies.
* I would like to take a moment and tell Randy that he can go fuck himself.
</details>
<details>
<summary>0.1.11 (Dawn of the Final Day, 24 Hours Remain)  </summary>

* Added Colossus to Fogbound Lagoon.
* Some general polish.
</details>
<details>
<summary>0.1.10 (Dawn of the Second Day, 48 Hours Remain)  </summary>

* Colossus:
	* Added config option to destroy model after death (turned off by default).
	* Lowered director cost to 1000 (from 1200). Lowered minimum stage completion to 0 (from 3).
		* _With these changes he should become even more frequent, and with 1000\0 on director it is very possible to encounter Colossus on first stage with Shrines of the Mountain._
	* Laser Barrage's head pitch is lowered to 0.05 (from 0.75). Spread is lowered to 0.15 (from 0.18). Prep time lowered to 4.5 seconds (from 5.5).
		* _Now you can't just hide under his feet, you have to get around him or run far away. Well, not as far as it used to be._
	* Colossal Fist now deals 500% damage (was 400%) with 8% proc chance (was 10%).
		* _This is mostly performance related. DPS is still the same._
	* Castle variant visual polish.
</details>
<details>
<summary>0.1.9 </summary>

* Fixed an issue where loading the game with mod's unsupported language would hang the game at 100%.
* Spitter:
	* A big thanks to [rob](https://thunderstore.io/package/rob_gaming/) for a new set of animations!
	* Spitter now slows down on charging spit to 70% of movement speed and comes to a complete stop on spit release.
	* Charged Spit now has a sound cue on charging the attack.
	* Added 2 second cooldown on Bite.
		* _He was quite relentless in melee range when the intention was that it would be somewhat of a last resort for him. This should calm him down._
* Colossus:
	* Laser Barrage explosion radius increased from 5 to 10.
		* _You could just stand in front of him and not get hit, ever. It was never my intention to make this attack hard to dodge, but it should serve it intended purpose - create a hell zone in front of him that you need to get away from ASAP. Hopefully this will make it closer to that intent._
	* Lowered director cost to 1200.
	* Added Void Fields and Abyssal Depths as possible spawn stage.
		* _I've got a lot of reports that people just don't see him in runs. Lowering director cost and adding him to the only stage 4 he can kinda fit in (he can't go into the cave) should hopefully make him less rare. Void Fields should probably be behind config tho._
	* Fixed head disappearing under certain angles.
	* Added Sky Meadows variant. 
	* Added Castle variant (don't get your hopes up).
	* Fixed eye glow and light not going out on death. 
	* Fixed idle animation blending with death animation.
	* Fixed footstep effects not playing. 
	* Now you should really be careful when Colossus steps and falls to the ground.
	* hehe boner
</details>
<details>
<summary>0.1.8 </summary>

* Added new Champion-tier (or boss) monster: Colossus.
* Spitter:
	* Split projectiles no longer collide with bodies.
		* _It is done to remove "shotgun effect" where in some situations, depending on target's and Spitter's elevation all 4 projectiles will hit the same target, resulting in massive, unintended damage._
* Added pt-BR translation by [Kauzok](https://github.com/Kauzok)
</details>

<details>
<summary>0.0.12 </summary>

* Spitter:
	* Fixed log book display and text.
	* Fixed log book text showing up in chat on Spitter's monster log pickup.
	* Fixed Spitter not being stunnable, frozable, etc.
	* Adjusted sounds' volume and attenuation to roughly match vanilla.
	* Made projectile and DoT zone decal more orange-ish.
	* Slight adjustments to elite colors so hopefully Overloading and Glacial are easier to differentiate.
	* Added monster to Void Cells, Abyssal Depths (Simulacrum) and Bulwark's Ambry.
</details>

<details>
<summary>0.0.11 </summary>

* Initial release
</details>
