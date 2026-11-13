# CLAUDE.md — Symulator budowy domu

Kontekst projektu dla Claude Code. Czytaj na początku każdej sesji. Pełny opis gry: `docs/GDD.md`.

## O projekcie

Jednoosobowy projekt w Unity na przedmiot „Tworzenie gier z AI” (CDV). Sandbox w pierwszej osobie: gracz wyznacza fundament na płaskiej działce, stawia ściany, okna, drzwi, piętro i dach, potem chodzi po domu. Repo jest publiczne i służy jako portfolio, więc kod ma być czytelny, a historia commitów porządna.

AI jest narzędziem do tworzenia gry (kod z Claude Code, część grafik z generatorów), nie mechaniką w grze.

## Założenia (nie łam ich bez pytania)

- Wszystko na siatce 1 m × 1 m, ściany tylko pod kątem prostym.
- Fundament = zbiór zajętych kratek (`HashSet<Vector2Int>`). Prostokąty dodają lub usuwają kratki.
- Ściana leży na krawędzi kratki; jedna krawędź = najwyżej jeden moduł (`Solid`, `Window`, `Door`). Bez CSG.
- Maksymalnie 2 kondygnacje, wysokość kondygnacji 3 m.
- Płaska działka, bez fizyki konstrukcji, bez ekonomii, bez NPC, bez mebli.

## Architektura

Dane domu są jedynym źródłem prawdy. Obiekty na scenie są tylko widokiem odbudowywanym z danych.

```
Gracz/GridCursor → narzędzie (IBuildTool) → komenda (ICommand) → CommandStack
  → BuildingData → zdarzenie BuildingChanged → BuildingView (odbudowa obiektów)
BuildingData ↔ SaveService (JSON)
```

Zasady:

1. **Dane zmieniają się wyłącznie przez komendy** (`ICommand` z `Do()` i `Undo()`) wykonywane przez `CommandStack`. Narzędzia i UI nigdy nie modyfikują `BuildingData` bezpośrednio.
2. Logika domeny (model, obrys, generowanie dachu, serializacja) to **czysty C# bez MonoBehaviour** w `Scripts/Model`, z osobnym `.asmdef`, pokryty testami EditMode.
3. `BuildingView` przebudowuje tylko zmienioną kondygnację, nie cały dom.
4. Zapis: Newtonsoft JSON, pole `version` w `BuildingData`.

## Stos

- Unity 6 LTS (dokładna wersja w `ProjectSettings/ProjectVersion.txt`), URP
- Input System (nowy), TextMeshPro
- `com.unity.nuget.newtonsoft-json`
- Unity Test Framework (EditMode)

## Struktura

```
Assets/
  _Project/
    Scripts/
      Model/       dane + czyste algorytmy (HouseBuilder.Model.asmdef)
      Commands/
      Building/    GridCursor, BuildToolController, narzędzia
      View/        BuildingView, meshe dachu
      Player/  UI/  Save/
    Prefabs/  Materials/  Scenes/
    Art/AI/        grafiki z generatorów AI
    Tests/EditMode/
  ThirdParty/      paczki z Asset Store — W .gitignore, NIE commitować
docs/
  GDD.md           opis gry
  AI_LOG.md        dziennik pracy z AI
```

## Konwencje kodu

- Namespace: `HouseBuilder.<Folder>` (np. `HouseBuilder.Model`, `HouseBuilder.Building`).
- PascalCase dla typów i metod, `_camelCase` dla pól prywatnych, `[SerializeField] private` zamiast pól publicznych.
- Jedna klasa na plik, nazwa pliku = nazwa klasy.
- Bez `FindObjectOfType` i `GameObject.Find` w pętli gry; zależności przez pola w inspektorze lub konstruktor.
- Bez magicznych liczb: rozmiar kratki, wysokość kondygnacji itp. w jednej klasie stałych lub ScriptableObject.
- Komentarze i nazwy w kodzie po angielsku, rozmowa i dokumentacja po polsku.

## Czego nie robić

- Nie edytuj plików `.unity`, `.prefab`, `.asset`, `.meta` ręcznie. Jeśli zmiana wymaga sceny lub prefaba, opisz krok po kroku, co ustawić w edytorze.
- Nie dotykaj `Library/`, `Temp/`, `Assets/ThirdParty/`.
- Nie dodawaj paczek do `Packages/manifest.json` bez zapytania.
- Nie wymyślaj API Unity. Jeśli nie masz pewności, czy metoda istnieje w Unity 6, powiedz to.

## Sposób pracy

1. Jedna funkcja na sesję. Najpierw krótki plan, implementacja dopiero po akceptacji.
2. Małe kroki, które da się sprawdzić w edytorze.
3. Do kodu w `Scripts/Model` pisz testy EditMode. Uruchamianie z konsoli (ścieżka do Unity zależy od instalacji, edytor musi być zamknięty):

   ```
   "<ścieżka do Unity>" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults wyniki.xml
   ```
4. Po skończonej funkcji zaproponuj wpis do `docs/AI_LOG.md` i opis commita.
5. Commity w stylu Conventional Commits po polsku: `feat: rysowanie fundamentu prostokątami`, `fix: …`, `refactor: …`, `docs: …`, `test: …`.

## Postęp (MVP do 20 grudnia 2026)

- [ ] Kontroler FPP, siatka, GridCursor
- [ ] Fundament (dodaj/odejmij prostokąt, linie pomocnicze, wymiary)
- [ ] Ściany zewnętrzne z obrysu, ściany wewnętrzne, gumka, undo/redo
- [ ] Okna i drzwi, drugie piętro, schody
- [ ] Dach płaski i dwuspadowy
- [ ] Zapis/wczytanie JSON, menu główne i pauza
