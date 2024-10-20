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
