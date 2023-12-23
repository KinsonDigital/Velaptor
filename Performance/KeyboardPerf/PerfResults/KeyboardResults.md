## **Velaptor Stats (Baseline)**
**Velaptor Version:** v1.0.0-preview.30  
**Description**: Baseline before changes for v1.0.0-preview.31

## **Stats**:
BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
12th Gen Intel Core i9-12900HK, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.100
[Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2 [AttachedDebugger]
DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


| Method                                  | Mean     | Error     | StdDev    | Median   | Gen0   | Gen1   | Allocated |
|---------------------------------------- |---------:|----------:|----------:|---------:|-------:|-------:|----------:|
| KeyboardState.IsKeyDown                 | 1.965 us | 0.0495 us | 0.1459 us | 2.002 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsKeyUp                   | 1.971 us | 0.0389 us | 0.0785 us | 1.984 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.SetKeyState               | 2.042 us | 0.0397 us | 0.0641 us | 2.054 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.AnyKeysDown               | 2.164 us | 0.0584 us | 0.1704 us | 2.194 us | 0.6733 | 0.0153 |   8.25 KB |
| KeyboardState.KeyToChar                 | 1.980 us | 0.0582 us | 0.1661 us | 2.019 us | 0.6733 | 0.0153 |   8.25 KB |
| KeyboardState.GetDownKeys               | 2.136 us | 0.0427 us | 0.1162 us | 2.190 us | 0.6714 | 0.0114 |   8.23 KB |
| KeyboardState.GetKeyStates              | 2.017 us | 0.0683 us | 0.1958 us | 2.048 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.AnyAltKeysDown            | 2.054 us | 0.1041 us | 0.3053 us | 2.155 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.AnyCtrlKeysDown           | 2.100 us | 0.0478 us | 0.1341 us | 2.126 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.AnyShiftKeysDown          | 2.571 us | 0.3406 us | 1.0043 us | 2.007 us | 0.6676 | 0.0076 |    8.2 KB |
| KeyboardState.AnyNumpadNumberKeysDown   | 2.061 us | 0.0474 us | 0.1398 us | 2.055 us | 0.6714 | 0.0095 |   8.23 KB |
| KeyboardState.AnyStandardNumberKeysDown | 1.985 us | 0.0714 us | 0.2061 us | 2.036 us | 0.6714 | 0.0114 |   8.23 KB |
| KeyboardState.IsLeftAltKeyDown          | 1.684 us | 0.1207 us | 0.3559 us | 1.887 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsLeftCtrlKeyDown         | 1.969 us | 0.0560 us | 0.1634 us | 1.986 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsLeftShiftKeyDown        | 1.940 us | 0.0394 us | 0.1156 us | 1.974 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsRightAltKeyDown         | 1.894 us | 0.0889 us | 0.2606 us | 1.958 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsRightCtrlKeyDown        | 1.966 us | 0.0477 us | 0.1376 us | 1.991 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsRightShiftKeyDown       | 1.933 us | 0.0551 us | 0.1626 us | 1.974 us | 0.6695 | 0.0095 |    8.2 KB |
| Keyboard.GetState                       | 3.230 us | 0.0642 us | 0.0713 us | 3.255 us | 0.6695 | 0.0095 |    8.2 KB |

---

## **Velaptor Stats (Test 2)**
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

## **Velaptor Stats (Test 3)**
**Velaptor Version:** v1.0.0-preview.31  
**Description:** Changes from test 2 is that KeyboardKeyGroups uses frozen dictionaries and immutable lists.

## **Stats**:
BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
12th Gen Intel Core i9-12900HK, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.100
[Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2 [AttachedDebugger]
DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
