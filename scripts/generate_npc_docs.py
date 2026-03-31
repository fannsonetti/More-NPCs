#!/usr/bin/env python3
"""
Extracts NPC metadata from More-NPCs C# source files and generates npc_data.js.
Run from project root: python scripts/generate_npc_docs.py
"""
import re
import os
import json
from pathlib import Path

PROJECT_ROOT = Path(__file__).resolve().parent.parent
NPCS_DIR = PROJECT_ROOT / "NPCs"
OUTPUT_JS = PROJECT_ROOT / "npc_data.js"
SUPERVISOR_COLLECTOR_FILE = PROJECT_ROOT / "Supervisor" / "SupervisorEarningsCollector.cs"
MANAGER_CHAIN_FILE = PROJECT_ROOT / "Manager" / "ManagerLaunderingChain.cs"

# Day enum to string
DAY_MAP = {
    "Monday": "Mon", "Tuesday": "Tue", "Wednesday": "Wed", "Thursday": "Thu",
    "Friday": "Fri", "Saturday": "Sat", "Sunday": "Sun",
    "Day.Monday": "Mon", "Day.Tuesday": "Tue", "Day.Wednesday": "Wed",
    "Day.Thursday": "Thu", "Day.Friday": "Fri", "Day.Saturday": "Sat", "Day.Sunday": "Sun",
}


def _read_text(path: Path) -> str:
    try:
        return path.read_text(encoding="utf-8", errors="replace")
    except Exception:
        return ""


def _extract_percent_from_multiplier(path: Path, pattern: str, default_percent: float) -> float:
    text = _read_text(path)
    m = re.search(pattern, text)
    if not m:
        return default_percent
    return float(m.group(1)) * 100.0


SUPERVISOR_CUT_PERCENT = _extract_percent_from_multiplier(
    SUPERVISOR_COLLECTOR_FILE,
    r'cut\s*=\s*raw\s*\*\s*([\d.]+)f?\s*;',
    10.0
)

MANAGER_CUT_PERCENT = _extract_percent_from_multiplier(
    MANAGER_CHAIN_FILE,
    r'LaunderCutPercent\s*=\s*([\d.]+)f?\s*;',
    10.0
)

def parse_file(path):
    """Parse a single NPC C# file and return extracted metadata or None."""
    try:
        text = path.read_text(encoding="utf-8", errors="replace")
    except Exception as e:
        print(f"Warning: Could not read {path}: {e}")
        return None

    # Must be a sealed class extending NPC
    if "sealed class" not in text or " NPC" not in text:
        return None

    out = {"file": path.name, "type": "customer"}

    # Description intentionally omitted from generated DB output.
    out["description"] = ""

    # WithIdentity("id", "FirstName", "LastName")
    identity = re.search(r'WithIdentity\s*\(\s*"([^"]+)"\s*,\s*"([^"]*)"\s*,\s*"([^"]*)"\s*\)', text)
    if identity:
        id_, first, last = identity.groups()
        out["id"] = id_
        out["firstName"] = first
        out["lastName"] = last
        out["name"] = f"{first} {last}".strip() or id_.replace("_", " ").title()
    else:
        return None

    # IsDealer
    out["isDealer"] = "IsDealer => true" in text or "override bool IsDealer => true" in text

    # Role inference
    out["isManager"] = (
        "Manager.ManagerDialogue.SetupFor(this)" in text
        or "ManagerLaunderingChain.Initialize(this)" in text
        or ('WithIdentity("thomas_ashford"' in text)
    )
    out["isSupervisor"] = (
        "SupervisorDialogue.SetupFor(this" in text
        or "SupervisorActivityChain.Initialize(this" in text
        or out.get("id") in {"silas_mercer", "dominic_cross"}
    )

    if out["isManager"]:
        out["type"] = "manager"
    elif out["isDealer"]:
        out["type"] = "dealer"
    elif out["isSupervisor"]:
        out["type"] = "supervisor"
    else:
        out["type"] = "customer"

    # Region
    region = re.search(r'Region\s*=\s*(?:S1API\.Map\.)?Region\.(\w+)', text)
    if region:
        out["region"] = region.group(1)
    else:
        out["region"] = "Unknown"

    # Dealer-specific
    if out["isDealer"]:
        cut = re.search(r'WithCut\s*\(\s*([\d.]+)f?\s*\)', text)
        out["cut"] = float(cut.group(1)) * 100 if cut else None
        fee = re.search(r'WithSigningFee\s*\(\s*([\d.]+)f?\s*\)', text)
        out["signingFee"] = int(float(fee.group(1))) if fee else None
        home = re.search(r'Building\.Get<(\w+)>', text)
        if home:
            out["home"] = home.group(1)
        else:
            home_by_name = re.search(r'Building\.GetByName\s*\(\s*"([^"]+)"\s*\)', text)
            out["home"] = home_by_name.group(1) if home_by_name else None

    # Supervisor-specific
    if out["isSupervisor"]:
        out["cut"] = SUPERVISOR_CUT_PERCENT

    # Manager-specific
    if out["isManager"]:
        out["cut"] = MANAGER_CUT_PERCENT

    # Customer-specific fields only.
    # Managers/supervisors use EnsureCustomer wrappers in code, but we intentionally
    # keep the DB output focused on their management role (no orders/week, standards, etc).
    if out["type"] == "customer" and "WithCustomerDefaults" in text:
        spend = re.search(r'WithSpending\s*\(\s*minWeekly:\s*([\d.]+)f?\s*,\s*maxWeekly:\s*([\d.]+)f?\s*\)', text)
        if spend:
            out["spending"] = [int(float(x)) for x in spend.groups()]
        else:
            spend = re.search(r'WithSpending\s*\(\s*([\d.]+)f?\s*,\s*([\d.]+)f?\s*\)', text)
            out["spending"] = [int(float(x)) for x in spend.groups()] if spend else [0, 0]

        orders = re.search(r'WithOrdersPerWeek\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)', text)
        out["orders"] = [int(orders.group(1)), int(orders.group(2))] if orders else [1, 4]

        time_m = re.search(r'WithOrderTime\s*\(\s*(\d+)\s*\)', text)
        if time_m:
            t = int(time_m.group(1))
            h, m = divmod(t, 100)
            out["time"] = f"{h:02d}:{m:02d}"
        else:
            out["time"] = "12:00"

        std = re.search(r'WithStandards\s*\(\s*CustomerStandard\.(\w+)\s*\)', text)
        out["standards"] = std.group(1) if std else "Moderate"
        # Map to display names
        std_display = {"Trash": "Trash", "VeryLow": "Very Low", "Low": "Low", "Moderate": "Moderate",
                       "High": "High", "Extreme": "Extreme"}
        out["standardsDisplay"] = std_display.get(out["standards"], out["standards"])

        day_m = re.search(r'WithPreferredOrderDay\s*\(\s*Day\.(\w+)\s*\)', text)
        out["day"] = DAY_MAP.get(f"Day.{day_m.group(1)}", "Mon") if day_m else None

        # Affinities: (DrugType.X, float)
        affs = re.findall(r'\(DrugType\.(\w+)\s*,\s*([-\d.]+)f?\s*\)', text)
        out["affinities"] = {k: int(float(v) * 100) for k, v in affs} if affs else {}

        # Properties
        props = re.findall(r'Property\.(\w+)', text)
        # Dedupe and filter to preferred
        seen = set()
        pref = []
        in_pref = False
        for line in text.split("\n"):
            if "WithPreferredProperties" in line:
                in_pref = True
            if in_pref and "Property." in line:
                for m in re.finditer(r'Property\.(\w+)', line):
                    if m.group(1) not in seen:
                        seen.add(m.group(1))
                        pref.append(m.group(1).replace("Glowie", "Glowing"))
            if in_pref and ")" in line and "WithPreferredProperties" not in line:
                break
        out["properties"] = pref if pref else []
        out["noProps"] = not pref

        police = re.search(r'WithCallPoliceChance\s*\(\s*([\d.]+)f?\s*\)', text)
        out["policeRisk"] = int(float(police.group(1)) * 100) if police else 0

        dep = re.search(r'WithDependence\s*\(\s*baseAddiction:\s*([\d.]+)f?\s*', text)
        out["dependence"] = float(dep.group(1)) if dep else 1

        conn = re.search(r'WithConnectionsById\s*\(([^)]*)\)', text)
        if conn:
            out["connections"] = re.findall(r'"([^"]+)"', conn.group(1))
        else:
            out["connections"] = []

        out["unlocked"] = out["connections"]  # For display
        out["avatar"] = "🧑"  # Default

    return out

def main():
    seen = set()
    npcs = []
    for p in sorted(NPCS_DIR.glob("*.cs")):
        if p.name in seen:
            continue
        seen.add(p.name)
        data = parse_file(p)
        if data:
            npcs.append(data)

    # Sort: supervisors, managers, dealers, then customers; then by name
    def order(n):
        t = {"supervisor": 0, "manager": 1, "dealer": 2, "customer": 3}.get(n["type"], 4)
        return (t, n["name"])
    npcs.sort(key=order)

    js_content = f"""// Auto-generated from NPCs/*.cs - run: python scripts/generate_npc_docs.py
// Do not edit manually.

const NPC_DATA = {json.dumps(npcs, indent=2)};
"""

    OUTPUT_JS.write_text(js_content, encoding="utf-8")
    print(f"Generated {OUTPUT_JS} with {len(npcs)} NPCs")

if __name__ == "__main__":
    main()
