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
- **Building rotations** — Apartment 2–4 and Grocery Backdoor knock-tested OK.
- **Artificial businesses** — off by default; enable via `MoreNPCs_Manager` → `ArtificialBusinessesEnabled` in MelonPreferences.
- **Fannso ↔ Nico** connection removed.
- **Cuke machine pile-up** — Evan keeps vending; Lila / Trisha / Nico use bench or extended stays instead.
- **Rusty Sump** — schedule shifted off Manhole Mike's pit / balcony overlap.
- **Long House Side Door** — `Sidedoor` root + `StaticDoor_Long_House_Side_Door` under Long House.
- **Performance** — removed idle cartel watcher; cached BuildingSetup types/source door; slowed building retry; NPCUnlockWatcher scans only tracked role NPCs (not full `NPC.All` every 0.25s).

## Backlog / tooling

## Review queue — Docks batch

`WithConnectionsById` is **only** for in-game unlock requirements (who the player needs rapport with), not for tooling or review order.

- [x] **Harper Lin** (`HarperLin.cs`) — Downtown customer, home **Apartment Building 3**, connections Jennifer + Rhea + Tessa; charcoal blouse + skirt
- [x] **Sloane Reyes** (`SloaneReyes.cs`) — female dealer, Docks / Red Docks Shipping Container 2 (edgier / open vest / tattoos)
- [x] **Rusty Sump** (`RustySump.cs`) — sewer loop; unlocks: `brack_silt`, `nadia_rim`, `manhole_mike`; schedule offset from Mike's pit
- [x] **Brack Silt** (`BrackSilt.cs`) — no connection prerequisites (empty)
- [x] **Dewey Koontz** (`DeweyKoontz.cs`) — no connection prerequisites (empty)
- [x] **Nadia Rim** (`NadiaRim.cs`) — no connection prerequisites (empty)
- [x] **Pike Mulch** (`PikeMulch.cs`) — no connection prerequisites (empty)
- [x] **Grunk** (`Grunk.cs`) — no connection prerequisites (empty)
- [ ] **Blake Drift** (`BlakeDrift.cs`) — Gasmart-strip oddball (Finch-adjacent vibe, not the dealer); unlocks: `billy_kramer`, `maya_webb`, `henry_mitchell` *(leave as-is for now)*
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

## Tomorrow (2026-07-02)

### Police vehicles (Sergeant Grey motor pool)
- [ ] **Siren prompt** — Verify headlights prompt hides and **Sirens / R** shows on shitbox + bruiser (InputPrompt disabled on clone so vanilla refresh cannot overwrite).
- [ ] **Checkpoint bypass** — Drive through curfew checkpoints in both cars: gates should open when you enter the detection zone, no vehicle search, no lag spike.
- [ ] **Curfew bypass** — Confirm cops ignore you during hard curfew in both cars.
- [ ] **Shitbox trunk** — Trunk interaction still broken after spawn/load; find what disables `TrunkInteraction` post-spawn and patch it.
- [ ] **Lightbar** — Confirm R toggles `PoliceLight` on SUV/Lightbar for both vehicles.

### Distributor (Richard Roe / anonymous SMS)
- [ ] **Trawler position** — Y = **-8** offshore; confirm visible from docks without clipping.
- [ ] **Pier ladder** — Functional ladder cloned at pier coords; test climb down to locker.
- [ ] **Pier locker** — Deposit flow, quest completion, card payout, SMS follow-ups.
- [ ] **Deal scheduling** — Sleep roll, deadline expiry, pity timer, premium vs bulk orders.
- [ ] **Tune pier locker position** if interact point feels wrong.

### Sergeant Grey SMS
- [ ] Confirm final copy: shitbox $25k / bruiser $30k / nevermind, card-only, no relationship gate.
- [ ] Test spawn + charge + no double-refund on spawn exception.

### Broader backlog
- [ ] **Suburbia** — +17 customers still needed toward 30.
- [ ] **Uptown** — +16 customers still needed toward 30.
- [ ] **Supervisor/manager unlock** — Verify Dominic, Silas, Thomas flows across saves.
- [ ] **Performance** — Profile if lag remains outside police cars (102 NPC schedules, Harmony hot paths).

---

## P1 — High (current work)

- [x] **Fix building rotations** — Apt 2–4 + Grocery Backdoor verified in-game.
- [x] **Downtown NPCs** — Roster / schedules / homes treated as **done** for now (30 customers + 2 dealers in-region; polish later if needed).

### Suggested order to continue

1. Finish **Suburbia** (+17 customers) and **Uptown** (+16 customers).
2. Work through remaining **review queue** (Sal Russo, Blake Drift when ready).
3. **P2** when P1 feels stable (distributor design, supervisor unlock verification).

## P2 — Medium (newer features)

- [x] **Distributor (Grant Ellison)** — Implemented; needs in-game testing. See **Distributor design** below.
- [ ] **Supervisor and manager unlocking** — Verify and tighten unlock flows (dealer thresholds, Taco Ticklers, SMS) for Dominic, Silas, and Thomas across saves and region settings.

### Distributor design (implemented — test in game)

**Fantasy:** After unlock, the distributor is never a regular customer/dealer — he's a **moving bulk buyer**. Each morning there's a roll that he texts you; if it hits, he names a **one-day meet spot**, buys product at **wholesale** (below street / customer price but above cost), then **leaves** (boat at Docks, truck in the slums).

**Unlock:** 15 recruited dealers (`NPCUnlockWatcher`); one-time intro SMS on unlock.

**Sleep hook:** `DistributorRunScheduler` on `TimeManager.OnDayPass`. Only runs if Grant is unlocked.

**Weekly orders (two types, rolled each in-game week):**

| Type | Chance | Quantity | Pricing |
|------|--------|----------|---------|
| **Bulk wholesale** | ~65% | Cocaine baseline **100–500**; weed **×1.5**, meth **×1.2**, shrooms **×1.3** | First X units **60%**, overflow **40%** |
| **Premium properties** | ~35% | **10–50** units | **Full market price** — must match 2 required effects + avoid 1 forbidden |

Example bulk: 225 marijuana (150–750 range). Example premium: *25 marijuana (Smelly + Explosive, no Sneaky) @ full price*.

**Wholesale sell flow (v1):** Approach Grant at venue → dialogue → transfer order. **v2:** load physical crate (see design note).

**NPC:** `GrantEllison` (`grant_ellison`) — pale, scrawny Uptown logistics coordinator; Lydia-esque tone.

**Still open:**
- Crate / HandoverScreen pickup flow (design below)
- Regenerate `npc_data.js` for Grant

## P3 — Polish / later

- [ ] **Officer Marcus & Sergeant Grey (police polish)** — Police-specific dialogue (including bribing where appropriate), police car behavior / presence, other cop-adjacent systems for Suburbia / station context.

## Performance notes

If lag persists after the watcher/building fixes, next suspects are **102 physical NPC schedules**, supervisor coroutine chains, and **Harmony patches** on hot paths — profile in-game before adding more per-frame work.
