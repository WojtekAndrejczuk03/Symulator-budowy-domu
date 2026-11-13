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

## 2026-11-13 — Projekt Unity, kontroler FPP, siatka i GridCursor

- **Cel:** projekt Unity w repo, chodzenie w pierwszej osobie, siatka 1 × 1 m na działce 50 × 50 m, podświetlenie kratki i krawędzi, na które patrzy gracz.
- **Narzędzie:** Claude Code.
- **Kluczowe prompty:** sklonowanie repo i przeczytanie `CLAUDE.md` i GDD, „mam projekt zrobiony”, akceptacja planu z rozmiarem działki 50 × 50 m.
- **Co AI zrobiło dobrze:** przeniosło szablon Unity do repo i posprzątało pliki przykładowe; rozdzieliło czystą matematykę siatki (`GridMath`, osobny `.asmdef`, testy EditMode) od komponentów sceny; zamiast ręcznie edytować plik `.unity` napisało skrypt edytora `HouseBuilder → Utwórz scenę testową`, który sam buduje scenę.
- **Co trzeba było poprawić ręcznie:** push na GitHuba wymagał jednorazowego logowania w przeglądarce. Unity przy pierwszym otwarciu poprosiło o aktualizację materiałów URP (zaakceptowane). Jednorazowy skrypt Unity Hub (`HubForceResolve.cs`) nie usunął się sam, więc trzeba go było skasować.
- **Czas:** około 1 h.
- **Commit:** `feat: kontroler FPP, siatka działki i GridCursor`.
