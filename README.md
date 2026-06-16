# Mafia Friz x KFC 2

Projekt zawiera prototyp gry decyzyjnej pod Unity oparty o historie typu "Mafia".

## Struktura

- `Unity/Assets/Scripts/StoryData.cs` - klasy danych do JSON
- `Unity/Assets/Scripts/GameManager.cs` - logika gry i UI
- `Unity/Assets/Resources/story.json` - drzewo fabuly

## Jak podlaczyc w Unity

1. Utworz nowy projekt Unity (2D lub 3D).
2. Skopiuj katalog `Unity/Assets` do swojego projektu Unity.
3. W scenie dodaj obiekt `GameManager` i przypnij skrypt `GameManager`.
4. Podlacz referencje w Inspectorze:
   - `storyText` (TextMeshProUGUI)
   - `historyText` (opcjonalnie, TextMeshProUGUI)
   - `buttonA` + `buttonAText`
   - `buttonB` + `buttonBText`
5. Upewnij sie, ze masz TextMeshPro w projekcie.
6. Uruchom scene.

## Ulepszenia w stosunku do bazowej wersji

- walidacja pliku JSON i czytelne komunikaty bledow
- wykrywanie zduplikowanych ID wezlow
- restart gry po zakonczeniu jednym przyciskiem
- historia wyborow wyswietlana w UI
- oddzielenie danych (`StoryData`) od logiki (`GameManager`)
