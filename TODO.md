# More-NPCs — todo list

**Priority:** **P1** = finish current / in-progress work. **P2** = newer or larger features. **P3** = polish when time allows.

## Regional customer targets (30 total customers per region)

**Rule:** Count only **customer** NPCs (`EnsureCustomer`, not dealers). Do **not** count supervisors (Dominic, Silas), manager (Thomas), or dealers toward the 30.

**Base game customer counts (vanilla):** Northtown 14 · Westville 10 · Downtown 11 · Docks 11 · Suburbia 10 · Uptown 10.

**Mod customer counts (toward 30 = base + mod):**

| Region | Base | Mod customers | Current total | **Still need** |
|--------|-----:|--------------:|----------------:|---------------:|
| Northtown | 14 | 16 | 30 | **0** |
| Westville | 10 | 20 | 30 | **0** |
| Downtown | 11 | 19 | 30 | **0** |
| Docks | 11 | 19 | 30 | **0** |
| Suburbia | 10 | 3 | 13 | **17** |
| Uptown | 10 | 4 | 14 | **16** |

**Dealers (12 total, not counted above):** 2 per region.

| Region | Mod dealers |
|--------|-------------|
| Northtown | Fannso Netti, Iris Samwell |
| Westville | Elliot Vaughn, Maxine Junefield |
| Downtown | Luis Navarro, Brooke Walsh |
| Docks | Sloane Reyes, Frozen Finch |
| Suburbia | Sergeant Grey, Rick Torres |
| Uptown | Hayes Denberg, Esteban Cordova |

---

## Done / parked

- Relationship merge workaround in More-NPCs was **removed** on purpose (S1API issue).

## Backlog / tooling

## Review queue — Docks batch

`WithConnectionsById` is **only** for in-game unlock requirements (who the player needs rapport with), not for tooling or review order.

- [x] **Harper Lin** (`HarperLin.cs`) — Downtown customer, home **Apartment Building 3**, connections Jennifer + Rhea + Tessa; charcoal blouse + skirt
- [x] **Sloane Reyes** (`SloaneReyes.cs`) — female dealer, Docks / Red Docks Shipping Container 2 (edgier / open vest / tattoos)
- [ ] **Rusty Sump** (`RustySump.cs`) — sewer walk loop; unlocks: `brack_silt`, `nadia_rim`, `manhole_mike`
- [ ] **Brack Silt** (`BrackSilt.cs`) — no connection prerequisites (empty)
- [ ] **Dewey Koontz** (`DeweyKoontz.cs`) — no connection prerequisites (empty)
- [ ] **Nadia Rim** (`NadiaRim.cs`) — no connection prerequisites (empty)
- [ ] **Pike Mulch** (`PikeMulch.cs`) — no connection prerequisites (empty)
- [ ] **Grunk** (`Grunk.cs`) — no connection prerequisites (empty)
- [ ] **Blake Drift** (`BlakeDrift.cs`) — Gasmart-strip oddball (Finch-adjacent vibe, not the dealer); unlocks: `billy_kramer`, `maya_webb`, `henry_mitchell`
- [x] **Frozen Finch** (`FrozenFinch.cs`) — dealer Gasmart Freezer; unlocks: `maya_webb`, `blake_drift`
- [ ] **Sal Russo** (`SalRusso.cs`) — unlocks: `finn_murphy`, `kelly_reynolds`
- [x] **Brooke Walsh** (`BrookeWalsh.cs`) — female dealer, Apartment Building 2 (cleaner / collar jacket / tired eyes)
- [x] **Luis Navarro** (`LuisNavarro.cs`) — grocery backdoor dealer
- [x] **Rick Torres** (`RickTorres.cs`) — **Suburbia** / **Long House Side Door**, no connections *(moved from Docks)* 
- [x] **Orlando Castillo** (`OrlandoCastillo.cs`)
- [x] **Tessa Ward** (`TessaWard.cs`)
- [x] **Wade Humphrey** (`WadeHumphrey.cs`)
- [x] **Faith Donovan** (`FaithDonovan.cs`)
- [x] **Ethan Vance** (`EthanVance.cs`) — renamed from Uriah (not Evan; distinct from Evan Rowland)
- [x] **Nina Cho** (`NinaCho.cs`) — V-neck, East Asian palette / double top knot
- [x] **Calder Wren** (`CalderWren.cs`) — overnight Storage Warehouse Elevator
- [x] **Damon Rusk** (`DamonRusk.cs`) — overnight Storage Warehouse Elevator
- [x] **Maris Eldridge** (`MarisEldridge.cs`) — connections: Damon + Calder
- [x] **Juniper Lyre** (`JuniperLyre.cs`) — cap color tweak

## P1 — High (current work)

- [ ] **Fix building rotations** — Align custom enterables / doors / spawned building objects with intended world orientation where they’re wrong.
  - **Current:** **Apartment Building 2** and **Grocery Backdoor** use world rotation **0,0,0** (`DoorWorldEulerIdentity`). **Apartment Building 3–4** use `DefaultDoorWorldEuler` (Y≈180°). **PPGrave** and docks containers keep their tuned angles. Tweak per row in `Utils/BuildingSetup.cs` as needed.
- [x] **Downtown NPCs** — Roster / schedules / homes treated as **done** for now (30 customers + 2 dealers in-region; polish later if needed).

### Suggested order to continue

1. Load Hyland Point and **knock-test** Apartment 2–4, Grocery Backdoor, and any other custom door you care about; tweak `BuildingSetup` if one site needs a custom euler.
2. Work through the **review queue** (tick boxes when an NPC looks right in-game).
3. ~~**Downtown roster**~~ — parked; focus next region (e.g. Docks / Suburbia / Uptown) or building rotations.
4. **P2** when P1 feels stable (distributor design, supervisor unlock verification).

## P2 — Medium (newer features)

- [ ] **Distributor (new NPC type)** — Whole new archetype (alongside supervisor, manager, dealer, customer). Role: **roughly double money output** from the pipeline you attach them to (exact mechanic TBD). Design, S1API integration, unlock/progression, dialogue, first NPC.
- [ ] **Supervisor and manager unlocking** — Verify and tighten unlock flows (dealer thresholds, Taco Ticklers, SMS, lock enforcement) for Dominic, Silas, and Thomas across saves and region settings.

## P3 — Polish / later

- [ ] **Officer Marcus & Sergeant Grey (police polish)** — Police-specific dialogue (including bribing where appropriate), police car behavior / presence, other cop-adjacent systems for Suburbia / station context.
