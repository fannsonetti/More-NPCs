"""
Apply a stable per-NPC minute offset in [-30, +30] to schedule clock times only.
Durations (second arg after time in StayInBuilding, DurationMinutes, etc.) unchanged.
"""
from __future__ import annotations

import hashlib
import re
import sys
from pathlib import Path


def npc_offset_minutes(stem: str) -> int:
    h = int(hashlib.md5(stem.encode()).hexdigest()[:8], 16)
    return (h % 61) - 30


def token_str_to_minutes(s: str) -> int:
    t = s.strip()
    if not t.isdigit():
        raise ValueError(s)
    if len(t) == 1:
        return int(t)
    if len(t) == 2:
        return int(t[0]) * 60 + int(t[1])
    if len(t) == 3:
        return int(t[0]) * 60 + int(t[1:3])
    if len(t) == 4:
        return int(t[0:2]) * 60 + int(t[2:4])
    raise ValueError(s)


def minutes_to_token(m: int) -> str:
    m = m % 1440
    if m < 0:
        m += 1440
    hh = m // 60
    mm = m % 60
    return f"{hh:02d}{mm:02d}"


def shift_schedule_token(tok: str, delta: int) -> str:
    return minutes_to_token(token_str_to_minutes(tok) + delta)


def process_text(text: str, delta: int) -> tuple[str, int]:
    subs = 0

    # StayInBuilding( x, TIME, DURATION )
    def repl_sib(m: re.Match) -> str:
        nonlocal subs
        new_t = shift_schedule_token(m.group(2), delta)
        subs += 1
        return f"{m.group(1)}{new_t}{m.group(3)}{m.group(4)}{m.group(5)}"

    out = text
    out = re.sub(
        r"(StayInBuilding\([^,]+,\s*)(\d{3,4})(\s*,\s*)(\d+)(\s*\))",
        repl_sib,
        out,
    )
    # UseATM / UseVendingMachine
    def repl_use(m: re.Match) -> str:
        nonlocal subs
        new_t = shift_schedule_token(m.group(2), delta)
        subs += 1
        return f"{m.group(1)}{new_t}{m.group(3)}"

    out = re.sub(r"(UseATM\(\s*)(\d{3,4})(\s*\))", repl_use, out)
    out = re.sub(r"(UseVendingMachine\(\s*)(\d{3,4})(\s*\))", repl_use, out)
    # StartTime = TIME
    def repl_st(m: re.Match) -> str:
        nonlocal subs
        new_t = shift_schedule_token(m.group(2), delta)
        subs += 1
        return f"{m.group(1)}{new_t}{m.group(3)}"

    out = re.sub(r"(StartTime\s*=\s*)(\d{3,4})(\s*[,}])", repl_st, out)
    out = re.sub(
        r"(new Use(?:ATM|VendingMachine)Spec\s*\{\s*StartTime\s*=\s*)(\d{3,4})(\s*\})",
        repl_st,
        out,
    )
    # LocationDialogue( pos, TIME [, ...] ) or LocationDialogue( pos, TIME );
    out = re.sub(
        r"(LocationDialogue\([^,]+,\s*)(\d{3,4})(\s*,|\s*\))",
        repl_st,
        out,
    )
    return out, subs


def process_file(path: Path) -> tuple[str, int, int] | None:
    stem = path.stem
    delta = npc_offset_minutes(stem)
    text = path.read_text(encoding="utf-8")
    if "WithSchedule" not in text and "plan.EnsureDealSignal" not in text:
        return None
    new_text, n = process_text(text, delta)
    if n == 0:
        return stem, delta, 0
    path.write_text(new_text, encoding="utf-8")
    return stem, delta, n


def main() -> int:
    root = Path(__file__).resolve().parent.parent / "NPCs"
    if not root.is_dir():
        print("NPCs folder not found", file=sys.stderr)
        return 1
    total = 0
    for p in sorted(root.glob("*.cs")):
        r = process_file(p)
        if r and r[2] > 0:
            print(f"{r[0]}: offset {r[1]:+d}, {r[2]} time(s) shifted")
            total += r[2]
    print(f"Done. Total time tokens shifted: {total}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
