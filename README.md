# IronDomeX

Missile silo block ("Iron Dome X") for the MisterWeapon modpack (Space Engineers 1). Has a script of its own for
visuals. It **requires** the brain mod ([missil-misterplace](../../missil-misterplace/README.md)), which runs the Iron
Dome interception logic for the block; see [docs/contents.md](docs/contents.md). Load the matching brain (dev with dev,
final with final) and set it as a Required item when publishing.

| Target | Local folder in `<SE data>/Mods` (Windows: `%APPDATA%\SpaceEngineers\Mods`) | Workshop ID |
|---|---|---|
| dev   | `IronDomeX-dev` | none yet (created by the first dev Publish) |
| final | `IronDomeX`     | none yet (not published) |

## Quick start
```powershell
.\deploy.ps1 dev            # link Mods/IronDomeX-dev -> Mod/ (live edits)
.\deploy.ps1 final -DryRun  # preview a release
.\deploy.ps1 final          # needs a clean git tree; copies a snapshot, tags release/<date>
```
Then publish from the game's Mods screen. Never load dev and final in the same world.
Deploy details, and what to do about IDs of unpublished targets:
[../missil-misterplace/docs/deployment.md](../../missil-misterplace/docs/deployment.md).

## Layout
- `Mod/`: everything that ships (Data, Models, Textures)
- `deploy.json` / `deploy.ps1`: deploy targets and script
- `docs/`: documentation

## Docs
- [Contents](docs/contents.md)
