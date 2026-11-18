# Contents

| Item | Subtype | Notes |
|---|---|---|
| Block (`SmallMissileLauncher`) | `IronDomeX` | "Iron Dome X", missile silo; fires up (block front); ammo through the conveyor at the base; model `Models\IronDomeX\IronDomeX.mwm` |
| Weapon definition | `IronDomeX_Weapon` | Used by the block |
| Ammo, magazine, blueprint | `IronDomeXMissile` | "Míssil Iron Dome X": same stats as the classic `IDMissile` (speed 100 to 300, acceleration 600, max trajectory 3000, radius 1, damage multiplier 4, mass 65); only the model `IronDomeX_Missile.mwm` differs |

All definitions are in `Mod/Data/IronDomeX.sbc`. Models are in `Mod/Models/IronDomeX/` (silo, display missile
`IronDomeX_Fake.mwm`, missile), textures in `Mod/Textures/`.

## Script: `Mod/Data/Scripts/IronDomeX/IronDomeXFake.cs`

Visuals only; the vanilla launcher logic aims and fires.

- `IronDomeXFake` (game logic on the `IronDomeX` block): a fake display missile sits in the silo. It disappears when the
  block fires (hidden 2.5 s) and rises back in 1.5 s. Runs on clients only (not on a dedicated server).
- `IronDomeXSession` (session component): hooks `OnMissileAdded` on clients to react to launches from the block.

## Relationship to the brain

**The mod requires the brain.** The block is registered in the brain's Iron Dome logic, so it works as an anti-missile
launcher ([../../missil-misterplace/docs/architecture.md](../../../missil-misterplace/docs/architecture.md)):

- Block `IronDomeX` is in the `[MyEntityComponentDescriptor]` of the brain's `MissileLauncher.cs`, which every 10 frames
  scans for enemy missiles within 3000 m (the `MissileTurret.missilesSubtypeIdWhitelist` ammo; aquatic missiles only once
  surfaced) and fires the block with `ShootOnce`.
- Ammo `IronDomeXMissile` is in `GuidedMissileSystem.InterceptorAmmoSubtypes`: it steers toward the chosen enemy missile
  with the faster turn limit and never targets grids or players.
- The ammo's stats stay here and match the classic `IDMissile` (see the table), so the 3000 m scan matches the 3000 m
  trajectory. Block-level settings still differ from the classic launcher: 120 rpm, one shot per burst, and a different
  blueprint cost (the classic one is 480 rpm, 19-shot burst). If the user wants those equal too, change `IronDomeX_Weapon`.
- A **Range** slider (100 to 3000 m, default 3000) is in the block's terminal; the brain adds it, so it exists only while
  the brain is loaded. It sets how far the block scans for enemy missiles, and idle power scales with it.
- The visuals script does not call the brain and is unchanged: it reacts to the shots the brain's logic triggers.
- Without the brain the block is a plain manual launcher. Nothing warns about it at runtime yet (LauncherX has such a
  warning; this mod does not).
- The Iron Dome turret/launcher of IronDomeClassic do **not** treat `IronDomeXMissile` as a target (it is not in the
  whitelist).

## Not set up yet
- No `metadata.mod`, no Workshop item (dev or final), no `BlockCategories.sbc` entry.
- `AntennaIronDome` now has its own repo ([../../AntennaIronDome](../../AntennaIronDome/CLAUDE.md)); `M61IronDomeX` still sits only in the game's Mods folder.
