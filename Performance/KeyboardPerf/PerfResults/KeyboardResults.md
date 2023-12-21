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
**Description:** Changes for this version.

## **Stats**:
BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
12th Gen Intel Core i9-12900HK, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.100
[Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2 [AttachedDebugger]
DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


~~~~
