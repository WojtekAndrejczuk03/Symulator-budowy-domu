# Dziennik pracy z AI

Jak AI było używane przy tworzeniu gry. Jeden wpis na każdą funkcję lub większą zmianę.

**Narzędzia:** Claude Code (kod), [nazwa generatora] (tekstury, ikony, grafika tytułowa).

---

## Szablon wpisu

### RRRR-MM-DD — [nazwa funkcji]

- **Cel:** co miało powstać.
- **Narzędzie:** Claude Code / generator grafiki.
- **Kluczowe prompty:** 1–3 najważniejsze, skrócone.
- **Co AI zrobiło dobrze:**
- **Co trzeba było poprawić ręcznie:** błędy, halucynacje API, poprawki w edytorze.
- **Czas:** około X h.
- **Commit:** `hash` lub link.

---

## 2026-11-13 — Zarys projektu i konfiguracja repo

- **Cel:** wybór tematu gry, zakres MVP, architektura, harmonogram, pliki startowe repo.
- **Narzędzie:** Claude (czat).
- **Kluczowe prompty:** „zaproponuj gry do zrobienia w Unity w 2 miesiące”, „ciężko będzie zrobić symulator budowania domu?”, odpowiedzi na pytania o zakres.
- **Co AI zrobiło dobrze:** podział na MVP i rozszerzenia, uproszczenie ścian do modułów na siatce, ostrzeżenie o licencjach Asset Store.
- **Co trzeba było poprawić ręcznie:**
- **Efekt:** `docs/GDD.md`, `CLAUDE.md`, `.gitignore`, `.gitattributes`, `LICENSE`, `README.md`.
