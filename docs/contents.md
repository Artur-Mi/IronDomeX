# Contents

| Item | Subtype | Notes |
|---|---|---|
| Block (`SmallMissileLauncher`) | `IronDomeX` | "Iron Dome X", missile silo; fires up (block front); ammo through the conveyor at the base; model `Models\IronDomeX\IronDomeX.mwm` |
| Weapon definition | `IronDomeX_Weapon` | Used by the block |
| Ammo, magazine, blueprint | `IronDomeXMissile` | "Míssil Iron Dome X": speed 5 to 200, explosion radius 4; model `IronDomeX_Missile.mwm` |

All definitions are in `Mod/Data/IronDomeX.sbc`. Models are in `Mod/Models/IronDomeX/` (silo, display missile
`IronDomeX_Fake.mwm`, missile), textures in `Mod/Textures/`.

## Script: `Mod/Data/Scripts/IronDomeX/IronDomeXFake.cs`

Visuals only; the vanilla launcher logic aims and fires.

- `IronDomeXFake` (game logic on the `IronDomeX` block): a fake display missile sits in the silo. It disappears when the
  block fires (hidden 2.5 s) and rises back in 1.5 s. Runs on clients only (not on a dedicated server).
- `IronDomeXSession` (session component): hooks `OnMissileAdded` on clients to react to launches from the block.

## Relationship to the brain

The script does not call the brain. Despite its name, this mod does not use the brain's Iron Dome interceptor logic
(`LargeMissileLauncherIronDome`, ammo `IDMissile`): the brain looks ammo up by subtype name
([../../missil-misterplace/docs/architecture.md](../../missil-misterplace/docs/architecture.md)), and `IronDomeXMissile`
is not on its list. Phase 4 of [../../docs/modpack-plan.md](../../docs/modpack-plan.md) covers connecting them.

## Not set up yet
- No `metadata.mod`, no Workshop item (dev or final), no `BlockCategories.sbc` entry.
- `AntennaIronDome` and `M61IronDomeX` also sit in the game's Mods folder, but are not part of this workspace.
