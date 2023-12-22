## **Velaptor Stats**
**Velaptor Version:** v1.0.0-preview.30  
**Description**: Baseline before changes for v1.0.0-preview.31

## **Stats**:
BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
12th Gen Intel Core i9-12900HK, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.100
[Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2 [AttachedDebugger]
DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


| Method                                  | Mean       | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|---------------------------------------- |-----------:|----------:|----------:|-------:|-------:|----------:|
| KeyboardState.IsKeyDown                 |   3.548 ns | 0.0597 ns | 0.0499 ns |      - |      - |         - |
| KeyboardState.IsKeyUp                   |   3.757 ns | 0.0813 ns | 0.0968 ns |      - |      - |         - |
| KeyboardState.SetKeyState               |   3.823 ns | 0.0655 ns | 0.0511 ns |      - |      - |         - |
| KeyboardState.AnyKeysDown               | 140.306 ns | 1.4158 ns | 1.3244 ns | 0.0038 |      - |      48 B |
| KeyboardState.KeyToChar                 |  33.482 ns | 0.3216 ns | 0.2850 ns | 0.0038 |      - |      48 B |
| KeyboardState.GetDownKeys               |  96.916 ns | 1.3483 ns | 1.2612 ns | 0.0025 |      - |      32 B |
| KeyboardState.GetKeyStates              |   1.292 ns | 0.0286 ns | 0.0267 ns |      - |      - |         - |
| KeyboardState.AnyAltKeysDown            |   5.628 ns | 0.0779 ns | 0.0728 ns |      - |      - |         - |
| KeyboardState.AnyCtrlKeysDown           |   5.691 ns | 0.0624 ns | 0.0553 ns |      - |      - |         - |
| KeyboardState.AnyShiftKeysDown          |   5.592 ns | 0.0675 ns | 0.0599 ns |      - |      - |         - |
| KeyboardState.AnyNumpadNumberKeysDown   |  39.268 ns | 0.2728 ns | 0.2552 ns | 0.0025 |      - |      32 B |
| KeyboardState.AnyStandardNumberKeysDown |  37.958 ns | 0.2148 ns | 0.2009 ns | 0.0025 |      - |      32 B |
| KeyboardState.IsLeftAltKeyDown          |   2.822 ns | 0.0334 ns | 0.0279 ns |      - |      - |         - |
| KeyboardState.IsLeftCtrlKeyDown         |   2.879 ns | 0.0728 ns | 0.0681 ns |      - |      - |         - |
| KeyboardState.IsLeftShiftKeyDown        |   2.816 ns | 0.0224 ns | 0.0210 ns |      - |      - |         - |
| KeyboardState.IsRightAltKeyDown         |   2.857 ns | 0.0510 ns | 0.0477 ns |      - |      - |         - |
| KeyboardState.IsRightCtrlKeyDown        |   2.822 ns | 0.0429 ns | 0.0380 ns |      - |      - |         - |
| KeyboardState.IsRightShiftKeyDown       |   2.824 ns | 0.0525 ns | 0.0492 ns |      - |      - |         - |
| Keyboard.GetState                       |   1.674 us | 0.0268 us | 0.0237 us | 0.6695 | 0.0095 |    8.2 KB |

---

## **Velaptor Stats**


**Velaptor Version:** v1.0.0-preview.31  
**Description:** Changes for this version without KeyboardKeyGroups frozen dictionaries.

## **Stats**:
BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
12th Gen Intel Core i9-12900HK, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.100
[Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2 [AttachedDebugger]
DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


| Method                                  | Mean       | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|---------------------------------------- |-----------:|----------:|----------:|-------:|-------:|----------:|
| KeyboardState.IsKeyDown                 |  0.0223 ns | 0.0209 ns | 0.0185 ns |      - |      - |         - |
| KeyboardState.IsKeyUp                   |  0.0462 ns | 0.0179 ns | 0.0159 ns |      - |      - |         - |
| KeyboardState.SetKeyState               | 86.5721 ns | 1.7339 ns | 2.7501 ns | 0.2193 |      - |    2752 B |
| KeyboardState.SetKeyState               |   3.306 ns | 0.0565 ns | 0.0529 ns |      - |      - |         - |
| KeyboardState.AnyKeysDown               |  1.5269 ns | 0.0499 ns | 0.0467 ns |      - |      - |         - |
| KeyboardState.KeyToChar                 |  5.1001 ns | 0.0689 ns | 0.0645 ns |      - |      - |         - |
| KeyboardState.GetDownKeys               |  1.5725 ns | 0.0368 ns | 0.0344 ns |      - |      - |         - |
| KeyboardState.GetKeyStates              |  0.0557 ns | 0.0218 ns | 0.0204 ns |      - |      - |         - |
| KeyboardState.AnyAltKeysDown            |  0.7100 ns | 0.0393 ns | 0.0667 ns |      - |      - |         - |
| KeyboardState.AnyCtrlKeysDown           |  0.6955 ns | 0.0379 ns | 0.0437 ns |      - |      - |         - |
| KeyboardState.AnyShiftKeysDown          |  0.6806 ns | 0.0323 ns | 0.0286 ns |      - |      - |         - |
| KeyboardState.AnyNumpadNumberKeysDown   | 19.8618 ns | 0.2548 ns | 0.2384 ns | 0.0051 |      - |      64 B |
| KeyboardState.AnyStandardNumberKeysDown | 17.9653 ns | 0.3300 ns | 0.3087 ns | 0.0051 |      - |      64 B |
| KeyboardState.IsLeftAltKeyDown          |  0.0405 ns | 0.0168 ns | 0.0157 ns |      - |      - |         - |
| KeyboardState.IsLeftCtrlKeyDown         |  0.0350 ns | 0.0185 ns | 0.0173 ns |      - |      - |         - |
| KeyboardState.IsLeftShiftKeyDown        |  0.3593 ns | 0.0216 ns | 0.0202 ns |      - |      - |         - |
| KeyboardState.IsRightAltKeyDown         |  0.0370 ns | 0.0170 ns | 0.0159 ns |      - |      - |         - |
| KeyboardState.IsRightCtrlKeyDown        |  0.0276 ns | 0.0079 ns | 0.0070 ns |      - |      - |         - |
| KeyboardState.IsRightShiftKeyDown       |  0.0312 ns | 0.0169 ns | 0.0158 ns |      - |      - |         - |
| Keyboard.GetState                       |   848.2 ns |  16.41 ns |  15.35 ns | 0.3748 | 0.0029 |   4.59 KB |
---
