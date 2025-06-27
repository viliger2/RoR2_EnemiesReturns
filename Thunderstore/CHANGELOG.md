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
