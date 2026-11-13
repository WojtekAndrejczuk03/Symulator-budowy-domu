# Symulator budowy domu

<!-- TODO: GIF z budowania domu (np. docs/media/demo.gif) -->

Sandbox w pierwszej osobie, w którym projektujesz i budujesz dom: wyznaczasz fundament, stawiasz ściany, okna i drzwi, dokładasz piętro i dach, a potem chodzisz po gotowym budynku.

Projekt zaliczeniowy z przedmiotu „Tworzenie gier z AI” (Collegium Da Vinci, Poznań), tworzony z pomocą narzędzi AI.

> **Status:** w trakcie tworzenia (MVP planowane na 20.12.2026).

## Funkcje

- [ ] Budowanie na siatce w widoku z pierwszej osoby
- [ ] Fundament z prostokątów: kształty L, T, U, taras; linie pomocnicze i wymiary
- [ ] Ściany zewnętrzne generowane z obrysu, ściany wewnętrzne stawiane ręcznie
- [ ] Okna i drzwi w kilku wariantach
- [ ] Dwie kondygnacje i schody
- [ ] Dach płaski i dwuspadowy
- [ ] Cofnij / ponów
- [ ] Zapis i wczytanie budowy

## Sterowanie

| Klawisz | Akcja |
| --- | --- |
| WASD, mysz, Spacja, Shift | ruch, rozglądanie, skok, sprint |
| B | tryb spacer / budowa |
| 1–7 | narzędzie: fundament, ściana, okno, drzwi, schody, dach, gumka |
| LPM / PPM | postaw / anuluj |
| R, Q / E | obrót, wariant elementu |
| PgUp / PgDn | parter / piętro |
| Ctrl+Z / Ctrl+Y | cofnij / ponów |
| F5 / F9 | zapis / wczytanie |

## Pobierz i zagraj

Gotowe buildy na Windows: [Releases](../../releases).

## Uruchomienie projektu

1. Unity 6 LTS (wersja w `ProjectSettings/ProjectVersion.txt`).
2. `git lfs install`, a potem `git clone` tego repozytorium.
3. Paczki z Unity Asset Store nie są w repo (licencja nie pozwala ich publikować). Zaimportuj je do `Assets/ThirdParty/`:
   - <!-- TODO: lista paczek z linkami -->

## Architektura

Dane domu (`BuildingData`) są jedynym źródłem prawdy. Narzędzia tworzą komendy (`ICommand` z `Do`/`Undo`), komendy zmieniają dane, a widok (`BuildingView`) odbudowuje obiekty sceny po zdarzeniu `BuildingChanged`. Dzięki temu cofanie, zapis i wczytanie wynikają wprost z modelu danych, a logika jest testowana testami EditMode. Szczegóły: [docs/GDD.md](docs/GDD.md).

## Jak powstała gra

Kod powstawał w parze z Claude Code (AI w konsoli), a część grafik (tekstury, ikony) z generatorów obrazów. Przebieg pracy, prompty i to, co trzeba było poprawiać ręcznie, opisuje [docs/AI_LOG.md](docs/AI_LOG.md).

## Licencje

- Kod: [MIT](LICENSE).
- Grafiki wygenerowane przez AI i assety CC0: <!-- TODO: lista źródeł -->
- Paczki z Unity Asset Store: na licencji Asset Store EULA, poza repozytorium.

## Autor

Wojtek Andrejczuk — [GitHub](https://github.com/WojtekAndrejczuk03)
