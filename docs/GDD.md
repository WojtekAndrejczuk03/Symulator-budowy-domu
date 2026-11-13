# Symulator budowy domu — zarys projektu (Unity, FPP)

Wersja z 13 listopada 2026 · Wojtek Andrejczuk

## Wizja i założenia

Sandbox w pierwszej osobie: gracz wyznacza fundament na płaskiej działce, stawia ściany, okna i drzwi, dokłada piętro i dach, a potem chodzi po gotowym domu. Projekt robi jedna osoba w około 8 tygodni. Ma wylądować w publicznym repo na GitHubie jako pozycja do portfolio.

AI jest narzędziem do tworzenia gry, nie mechaniką w grze: kod powstaje z pomocą Claude Code w konsoli, część grafik z generatorów obrazów.

**Założenia, które upraszczają projekt:**

- Wszystko na siatce, ściany tylko pod kątem prostym.
- Fundament wyznacza gracz: prostokąty składane w dowolny kształt (L, T, U, taras). Bez symulacji lania betonu.
- Maksymalnie 2 kondygnacje.
- Tylko bryła budynku plus proste elementy działki (płot). Bez mebli i wnętrz.
- Płaska działka, bez fizyki i statyki, bez ekonomii i NPC.
- Ściany składane z modułów o szerokości jednej kratki. Okno lub drzwi to po prostu inny moduł, więc nie trzeba wycinać otworów w siatce (CSG).

## Zakres funkcji

MVP to wersja, która musi działać do końca tygodnia 5. Rozszerzenia robisz dopiero po niej, w kolejności od góry.

**MVP (musi być):**

- [ ] Kontroler FPP: chodzenie, skok, sprint, celownik na środku ekranu.
- [ ] Siatka 1 m × 1 m na działce, podświetlenie kratki, na którą patrzy gracz.
- [ ] Fundament: rysowanie prostokątów (dodaj i odejmij), linie pomocnicze i wymiary w metrach podczas rysowania.
- [ ] Ściany zewnętrzne generowane automatycznie z obrysu fundamentu (z możliwością usunięcia).
- [ ] Ściany wewnętrzne stawiane ręcznie po krawędziach kratek.
- [ ] Okna i drzwi wstawiane w moduł ściany (2–3 rodzaje każdego).
- [ ] Drugie piętro: strop z obrysu, przełączanie aktywnej kondygnacji, schody jako gotowy element.
- [ ] Dach: płaski dla dowolnego kształtu i dwuspadowy dla prostokąta.
- [ ] Podgląd przed postawieniem (zielony = OK, czerwony = kolizja), usuwanie, cofnij i ponów (undo/redo).
- [ ] Zapis i wczytanie budowy do pliku JSON.
- [ ] Menu główne i pauza.

**Rozszerzenia (jeśli starczy czasu):**

1. Płot i furtka stawiane wzdłuż linii siatki.
2. Materiały: kolor i tekstura ścian, rodzaj dachówki, malowane pędzlem na module.
3. Widok planu z góry (przełączany klawiszem) do wygodnego rysowania fundamentu.
4. Taras i balkon jako wydzielona część fundamentu bez ścian.
5. Dach dwuspadowy na kształtach L i T (osobny dach na każdy prostokąt).
6. Otoczenie: chodnik, trawa, drzewa, poręcze balkonu.
7. Cykl dnia i nocy, zrzut ekranu z poziomu gry.

**Poza zakresem:** ściany pod kątem, więcej niż 2 piętra, nierówny teren, meble i wnętrza, fizyka konstrukcji, ekonomia, NPC.

## Sterowanie i tryby budowania

Gracz ma dwa tryby: **spacer** (sam ruch, bez budowania) i **budowa** (pasek narzędzi na dole ekranu, siatka widoczna). Każde narzędzie działa tak samo: celownik wskazuje kratkę lub krawędź, LPM zaczyna lub stawia, PPM anuluje.

| Klawisz | Akcja |
| --- | --- |
| WASD, mysz, Spacja, Shift | ruch, rozglądanie, skok, sprint |
| B | przełącz spacer / budowa |
| 1–7 | narzędzie: fundament, ściana, okno, drzwi, schody, dach, gumka |
| LPM | klik = początek prostokąta lub ściany, drugi klik = koniec |
| PPM / Esc | anuluj bieżącą operację |
| R | obróć element (drzwi, schody, kierunek dachu) |
| Q / E | wariant elementu (np. inny typ okna) |
| PgUp / PgDn | aktywna kondygnacja: parter / piętro |
| Ctrl+Z / Ctrl+Y | cofnij / ponów |
| F5 / F9 | szybki zapis / wczytanie |
| Tab | widok planu z góry (rozszerzenie) |

Fundament rysuje się dwoma kliknięciami (dwa przeciwległe narożniki). Między kliknięciami widać prostokąt, linie pomocnicze przedłużające istniejące krawędzie i wymiary w metrach. Ctrl+LPM odejmuje prostokąt, np. żeby wyciąć wnękę albo oddzielić taras.

## Architektura techniczna

Najważniejsza zasada: **dane domu są jedynym źródłem prawdy, a obiekty na scenie tylko się z nich odbudowują.** Dzięki temu cofanie, zapis i wczytanie są prawie za darmo, a logikę da się testować bez uruchamiania gry.

```
Gracz i kursor ──► Narzędzia (IBuildTool) ──► CommandStack (Do / Undo)
                                                    │ zmienia dane
                                                    ▼
                         SaveService (JSON) ◄──► BuildingData (jedyne źródło prawdy)
                                                    │ zdarzenie BuildingChanged
                                                    ▼
                                             BuildingView (odbudowa obiektów sceny)
                                                    │
                                                    └──► gracz widzi efekt
```

Kliknięcie narzędzia tworzy komendę, komenda zmienia dane, a widok odbudowuje się po zdarzeniu. Cofanie to po prostu `Undo()` ostatniej komendy.

**Stos:** Unity 6 (najnowsza wersja LTS), URP, nowy Input System, TextMeshPro, Newtonsoft JSON (`com.unity.nuget.newtonsoft-json`), Unity Test Framework.

**Model danych (czysty C#, bez MonoBehaviour):**

```csharp
class BuildingData  { int version; List<FloorData> floors; List<FenceData> fences; }
class FloorData     { int level; float height = 3f; HashSet<Vector2Int> cells;
                      List<WallData> walls; List<RoofData> roofs; List<StairsData> stairs; }
class WallData      { EdgeId edge; WallModule module; int variant; bool exterior; }
struct EdgeId       { Vector2Int cell; EdgeDir dir; }   // dir: North lub East
enum WallModule     { Solid, Window, Door }
```

Fundament to zbiór zajętych kratek. Dodanie prostokąta dodaje kratki, odjęcie je usuwa, więc kształty L, T czy U wychodzą same. Obrys to krawędzie między kratką zajętą a pustą: z nich powstają ściany zewnętrzne. Każda krawędź siatki mieści najwyżej jeden moduł ściany.

**Systemy:**

- `GridCursor`: raycast ze środka kamery na płaszczyznę aktywnej kondygnacji, zwraca kratkę i najbliższą krawędź.
- `BuildToolController` + narzędzia (`FoundationTool`, `WallTool`, `OpeningTool`, `StairsTool`, `RoofTool`, `EraseTool`): maszyna stanów, każde narzędzie implementuje `IBuildTool` (podgląd, potwierdzenie, anulowanie).
- `CommandStack`: wzorzec Command. Każda zmiana to obiekt z `Do()` i `Undo()`; narzędzia nigdy nie zmieniają danych bezpośrednio.
- `OutlineService`, `RoofGenerator`: czyste algorytmy (obrys z kratek, dach z obrysu), pokryte testami.
- `BuildingView`: słucha zdarzenia `BuildingChanged` i odbudowuje obiekty danej kondygnacji z prefabów modułów.
- `SaveService`: zapis i odczyt `BuildingData` do JSON w `Application.persistentDataPath`, z polem `version` na przyszłe zmiany formatu.
- `PlayerController`, `HUD`, `MainMenu`.

**Struktura folderów:**

```
Assets/
  _Project/
    Scripts/
      Model/        dane + czyste algorytmy (osobny .asmdef)
      Commands/
      Building/     GridCursor, narzędzia
      View/         BuildingView, generowanie meshów dachu
      Player/  UI/  Save/
    Prefabs/  Materials/  Scenes/
    Art/AI/         tekstury i ikony z generatorów
    Tests/EditMode/
  ThirdParty/       paczki z Asset Store (poza repo, patrz niżej)
```

## AI w procesie tworzenia

Claude Code pisze kod, ale nie widzi edytora Unity: ty uruchamiasz grę, sprawdzasz efekt i wklejasz błędy z konsoli. Dlatego praca idzie małymi krokami, a logika siedzi w czystym C#, który da się testować bez klikania.

**Kod (Claude Code w konsoli):**

1. W katalogu głównym repo trzymaj `CLAUDE.md`: opis projektu, założenia z tego dokumentu, struktura folderów, konwencje kodu i zasada „zmiany tylko przez komendy”. Claude czyta go na starcie każdej sesji.
2. Jedna funkcja = jedna sesja: najpierw poproś o plan, potem implementację, potem test w edytorze, na końcu commit.
3. Do algorytmów (obrys, dach, serializacja) proś o testy EditMode. Claude może je sam odpalić z konsoli: `Unity -batchmode -projectPath . -runTests -testPlatform EditMode -testResults wyniki.xml` (ścieżka do Unity zależy od instalacji).
4. Sceny, prefaby i ustawienia komponentów ustawiasz ręcznie w edytorze. Pliki `.unity` i `.prefab` lepiej nie edytować przez AI, bo łatwo je zepsuć.
5. Gdy coś nie działa: wklej cały błąd z konsoli i opisz, co widać na ekranie. Gdy Claude kręci się w kółko, cofnij zmiany w gicie i zacznij od mniejszego kroku.

**Grafika (generatory obrazów):**

- Bezszwowe tekstury: cegła, tynk, drewno, dachówka, kostka brukowa, trawa. W prompcie „seamless, tileable, top-down, flat lighting”.
- Skybox, ikony narzędzi na pasek, logo i grafika tytułowa do README.
- Przed użyciem sprawdź regulamin generatora, czy pozwala na publikację grafik.

**Dokumentowanie (ważne na zaliczenie i do portfolio):** plik `docs/AI_LOG.md`, wpis na każdą funkcję: co zleciłeś, jakie prompty działały, co AI zrobiło źle i co poprawiłeś ręcznie. W README krótka sekcja „Jak powstała gra” z linkiem do tego pliku.

## Repozytorium GitHub i licencje assetów

Paczek z Asset Store nie wrzucaj do publicznego repo. Standardowa licencja Asset Store nie pozwala na redystrybucję, a publiczne repozytorium jest traktowane jak redystrybucja ([dyskusja na forum Unity](https://discussions.unity.com/t/what-kind-of-license-can-be-used-to-protect-assets-purchased-from-the-asset-store-in-a-public-repo/860104)). Dotyczy to także darmowych paczek.

**Jak to rozwiązać:**

- Paczki z Asset Store importuj do `Assets/ThirdParty/` i dodaj ten folder do `.gitignore`. W README wypisz, które paczki trzeba pobrać, żeby otworzyć projekt.
- Do repo commituj tylko assety, które wolno publikować: własne, wygenerowane przez AI (zgodnie z regulaminem generatora) oraz na licencji CC0 (np. Kenney, Quaternius, Poly Haven).
- Grywalny build (.zip na Windows) wrzucaj do GitHub Releases. Gotowa gra w buildzie może zawierać assety ze sklepu, więc ktoś z portfolio uruchomi ją bez otwierania projektu.

**Konfiguracja repo:**

- `.gitignore` z szablonu Unity (bez `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Builds/`).
- Git LFS dla plików `.png`, `.psd`, `.fbx`, `.wav`, `.mp3`, żeby repo nie puchło.
- W edytorze: Asset Serialization = Force Text, Version Control = Visible Meta Files.
- Licencja MIT dla kodu, w README osobna informacja o licencjach grafik.
- Małe commity z czytelnymi opisami (np. `feat: rysowanie fundamentu prostokątami`). Na portfolio historia commitów też się liczy.

**README pod portfolio:** GIF z budowania domu na samej górze, lista funkcji, sterowanie, link do Releases, krótki opis architektury, sekcja „Jak powstała gra” (praca z AI), licencje.

## Harmonogram 8 tygodni

MVP zamyka się w tygodniu 5, przed świętami. Tygodnie 6–8 to rozszerzenia, dopracowanie i oddanie.

| Tydzień | Daty | Cel | Zakres |
| --- | --- | --- | --- |
| 1 | 16–22 lis | Start projektu | Repo, Unity + URP, CLAUDE.md, kontroler FPP, siatka i kursor na kratce |
| 2 | 23–29 lis | Fundament | Model danych, rysowanie i odejmowanie prostokątów, linie pomocnicze, wymiary |
| 3 | 30 lis – 6 gru | Ściany | Ściany zewnętrzne z obrysu, wewnętrzne ręcznie, gumka, cofnij i ponów |
| 4 | 7–13 gru | Okna, drzwi i piętro | Moduły otworów, strop, przełączanie kondygnacji, schody |
| 5 | 14–20 gru | Dach, zapis i menu | Dach płaski i dwuspadowy, zapis i wczytanie JSON, menu, test całości |
| **—** | **20 gru** | **MVP gotowe** | |
| 6 | 21–27 gru | Rozszerzenia (święta) | Płot, materiały i tekstury z AI, pasek narzędzi z ikonami |
| 7 | 28 gru – 3 sty | Dopracowanie (święta) | Oświetlenie, dźwięki, poprawki błędów, wydajność, pierwszy build |
| 8 | 4–10 sty | Bufor i oddanie | README z GIF-em, Release na GitHubie, AI_LOG, prezentacja |

Tygodnie 6 i 7 przypadają na święta, dlatego wszystko, co niezbędne, kończy się przed nimi. Jeśli MVP się spóźni, rozszerzenia wypadają w całości.

## Ryzyka i plan awaryjny

Największe ryzyko to dach i precyzja budowania w pierwszej osobie. Oba mają proste wyjścia awaryjne.

| Ryzyko | Objaw | Plan awaryjny |
| --- | --- | --- |
| Dach na kształtach L/T | geometria się nie zgadza, dziury w narożnikach | dach płaski dla złożonych kształtów, dwuspadowy tylko na prostokąt |
| Budowanie z FPP jest niewygodne | trudno trafić w kratkę z daleka | latanie w trybie budowy albo widok planu z góry (Tab) |
| Rozrost zakresu | w tygodniu 5 MVP nie działa | wszystkie rozszerzenia wypadają, tydzień 6 idzie na dokończenie MVP |
| AI wymyśla nieistniejące API Unity | błędy kompilacji, kręcenie się w kółko | mniejsze kroki, wklejanie dokumentacji, cofnięcie w gicie |
| Wydajność przy dużym domu | spadki FPS przy odbudowie widoku | odbudowa tylko zmienionej kondygnacji, łączenie meshów |
| Święta (tygodnie 6–7) | mniej czasu na pracę | MVP zamknięte przed 20 grudnia, po świętach tylko dopracowanie |
| Licencje assetów | paczka ze sklepu trafia do repo | folder `ThirdParty/` w `.gitignore` od pierwszego commita |
