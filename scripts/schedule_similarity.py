#!/usr/bin/env python3
"""
Extract ordered schedule tuples from NPCs/**/*.cs for rough similarity review.
Run from project root: python scripts/schedule_similarity.py

Outputs JSON lines: npc_id, action_sequence (building names / WalkTo / Sit / etc.)
Use for manual Jaccard-style comparison against the ~40% similarity guideline.
"""
import re
from pathlib import Path

PROJECT_ROOT = Path(__file__).resolve().parent.parent
NPCS = PROJECT_ROOT / "NPCs"


def extract_schedule(path: Path):
    text = path.read_text(encoding="utf-8", errors="replace")
    if "sealed class" not in text or " NPC" not in text:
        return None
    m = re.search(r'WithIdentity\s*\(\s*"([^"]+)"', text)
    if not m:
        return None
    npc_id = m.group(1)
    msched = re.search(r"\.WithSchedule\s*\(\s*plan\s*=>", text)
    if not msched:
        return None
    body_start = msched.end()
    depth = 1
    i = body_start
    while i < len(text) and depth > 0:
        if text[i] == "{":
            depth += 1
        elif text[i] == "}":
            depth -= 1
        i += 1
    block = text[body_start : i - 1]

    actions: list[str] = []
    for line in block.splitlines():
        line = line.strip()
        if "StayInBuildingSpec" in line and "BuildingName" in line:
            bm = re.search(r'BuildingName\s*=\s*"([^"]+)"', line)
            if bm:
                actions.append(f"Stay:{bm.group(1)}")
        elif "StayInBuilding(" in line and "plan.StayInBuilding" in line:
            actions.append("Stay:typed")
        elif "WalkToSpec" in line:
            actions.append("WalkTo")
        elif "SitSpec" in line:
            actions.append("Sit")
        elif "LocationDialogue" in line or "LocationDialogueSpec" in line:
            actions.append("LocationDialogue")
        elif "UseATM" in line or "UseATMSpec" in line:
            actions.append("ATM")
        elif "UseVendingMachine" in line or "UseVendingMachineSpec" in line:
            actions.append("Vending")
        elif "EnsureDealSignal" in line:
            actions.append("DealSignal")

    return {"file": str(path.relative_to(PROJECT_ROOT)), "id": npc_id, "actions": actions}


def main() -> None:
    rows: list[dict] = []
    for f in sorted(NPCS.rglob("*.cs")):
        row = extract_schedule(f)
        if row and row["actions"]:
            rows.append(row)
    out = PROJECT_ROOT / "scripts" / "schedule_fingerprints.json"
    import json

    out.write_text(json.dumps(rows, indent=2), encoding="utf-8")
    print(f"Wrote {len(rows)} NPC fingerprints to {out}")


if __name__ == "__main__":
    main()
