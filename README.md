# Fannso's MoreNPCs

`MoreNPCs` is a `Schedule I` mod that adds a large set of custom NPCs and supporting role systems.

## What The Mod Adds

- 66 custom NPCs generated from the source data
- Customers, dealers, supervisors, and a manager role
- Searchable NPC documentation generated from `NPCs/*.cs`
- Support for `Mono`, `Il2cpp`, and `CrossCompat` builds

## Project Info

The source is public so people can see how the mod works, learn from it, and follow development.

Reuse, credit, redistribution, and attribution terms are in `LICENSE.md`.

Permission/contact:

- `Fannsonetti` on Nexus Mods
- `fannso` on Discord

## Website

This repo includes a static NPC database site powered by:

- `index.html`
- `npcs.html`
- `npc_data.js`

`npc_data.js` is generated from source by:

```bash
python scripts/generate_npc_docs.py
```

A GitHub Pages workflow is included so the site can automatically redeploy on each push.

## Development Notes

- Target framework: `netstandard2.1`
- Build configurations: `Mono`, `Il2cpp`, `CrossCompat`
- Local `S1API` usage is configured in `MoreNPCs.csproj`

## No Affiliation

This project is an unofficial fan-made mod and is not affiliated with or endorsed by the game developer or publisher.
