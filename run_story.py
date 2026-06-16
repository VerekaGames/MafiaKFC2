#!/usr/bin/env python3
"""Konsolowy runner historii — ta sama logika co GameManager.cs."""

import json
import sys
from pathlib import Path

STORY_PATH = Path(__file__).parent / "Unity/Assets/Resources/story.json"
START_NODE_ID = 1


def load_story(path: Path) -> dict[int, dict]:
    data = json.loads(path.read_text(encoding="utf-8"))
    nodes = data.get("nodes") or []
    if not nodes:
        raise SystemExit("Blad: JSON historii jest pusty lub ma zly format.")

    node_dict: dict[int, dict] = {}
    for node in nodes:
        node_id = node["id"]
        if node_id in node_dict:
            print(f"Ostrzezenie: zduplikowane ID wezla {node_id}, pomijam duplikat.")
            continue
        node_dict[node_id] = node
    return node_dict


def print_history(choices: list[str]) -> None:
    if not choices:
        print("\nSciezka: brak wyborow.")
        return
    print("\nSciezka:")
    for choice in choices:
        print(f"  - {choice}")


def play(node_dict: dict[int, dict], start_id: int = START_NODE_ID) -> None:
    choices: list[str] = []
    current_id = start_id

    while True:
        node = node_dict.get(current_id)
        if node is None:
            print(f"Blad scenariusza: brak wezla o ID {current_id}.")
            sys.exit(1)

        print("\n" + "=" * 60)
        print(node["text"])
        print_history(choices)

        if node.get("isEnding"):
            again = input("\nZagraj ponownie? [t/n]: ").strip().lower()
            if again in ("t", "tak", "y", "yes"):
                choices.clear()
                current_id = start_id
                continue
            print("\nDo zobaczenia!")
            return

        a_text = node.get("choiceAText", "")
        b_text = node.get("choiceBText", "")
        print(f"\n  [A] {a_text}")
        print(f"  [B] {b_text}")

        while True:
            pick = input("\nWybierz A lub B: ").strip().upper()
            if pick == "A":
                choices.append(f"A: {a_text}")
                current_id = node["choiceANodeId"]
                break
            if pick == "B":
                choices.append(f"B: {b_text}")
                current_id = node["choiceBNodeId"]
                break
            print("Nieprawidlowy wybor — wpisz A lub B.")


def main() -> None:
    if not STORY_PATH.exists():
        raise SystemExit(f"Blad: nie znaleziono {STORY_PATH}")

    print("Mafia Friz x KFC 2 — tryb konsolowy")
    print(f"Zaladowano: {STORY_PATH.name}")
    node_dict = load_story(STORY_PATH)
    play(node_dict)


if __name__ == "__main__":
    main()
