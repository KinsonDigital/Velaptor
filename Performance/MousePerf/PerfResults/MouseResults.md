## **Stats**:

BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
12th Gen Intel Core i9-12900HK, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.100
[Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
Job-VCWPZF : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

## **Velaptor Stats (Baseline)**
**Velaptor Version:** v1.0.0-preview.30  
**Description**: Baseline performance

| Method                 | MouseButton  | IsDown | Mean       | Error      | StdDev     | Median      | P95         | Allocated |
|----------------------- |------------- |------- |-----------:|-----------:|-----------:|------------:|------------:|----------:|
| MouseState.GetPosition | LeftButton   | False  | 73.2558 ns | 16.3650 ns | 44.5221 ns | 100.0000 ns | 100.0000 ns |     400 B |
| GetX                   | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| MouseState.GetPosition | LeftButton   | True   | 30.8081 ns | 13.9286 ns | 40.8501 ns |   0.0000 ns | 150.0000 ns |     400 B |
| GetX                   | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| MouseState.GetPosition | RightButton  | False  | 10.1010 ns | 10.3271 ns | 30.2876 ns |   0.0000 ns | 100.0000 ns |     112 B |
| GetX                   | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| MouseState.GetPosition | RightButton  | True   |  9.1837 ns |  9.9512 ns | 29.0280 ns |   0.0000 ns | 100.0000 ns |     400 B |
| GetX                   | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| MouseState.GetPosition | MiddleButton | False  | 67.3469 ns | 22.4368 ns | 65.4493 ns | 100.0000 ns | 200.0000 ns |     400 B |
| GetX                   | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| MouseState.GetPosition | MiddleButton | True   | 67.0000 ns | 22.6384 ns | 66.7499 ns | 100.0000 ns | 200.0000 ns |     400 B |
| GetX                   | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |

## **Velaptor Stats (Baseline)**
**Velaptor Version:** v1.0.0-preview.30  
**Description**: After converting the mouse state to a readonly struct

| Method                 | MouseButton  | IsDown | Mean       | Error      | StdDev     | Median      | P95         | Allocated |
|----------------------- |------------- |------- |-----------:|-----------:|-----------:|------------:|------------:|----------:|
| MouseState.GetPosition | LeftButton   | False  | 21.2121 ns | 14.0101 ns | 41.0891 ns |   0.0000 ns | 100.0000 ns |     400 B |
| GetX                   | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | LeftButton   | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| MouseState.GetPosition | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |     400 B |
| GetX                   | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | LeftButton   | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| MouseState.GetPosition | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |     400 B |
| GetX                   | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | RightButton  | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| MouseState.GetPosition | RightButton  | True   | 46.9697 ns | 14.5760 ns | 42.7488 ns |  50.0000 ns | 150.0000 ns |     400 B |
| GetX                   | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | RightButton  | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| MouseState.GetPosition | MiddleButton | False  | 74.7475 ns | 23.0245 ns | 67.5269 ns | 100.0000 ns | 200.0000 ns |     400 B |
| GetX                   | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | MiddleButton | False  |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| MouseState.GetPosition | MiddleButton | True   | 75.0000 ns | 23.7994 ns | 70.1729 ns | 100.0000 ns | 200.0000 ns |     400 B |
| GetX                   | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetY                   | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonDown           | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsButtonUp             | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonDown       | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsLeftButtonUp         | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonDown     | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsMiddleButtonUp       | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonDown      | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| IsRightButtonUp        | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetButtonState         | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| AnyButtonsDown         | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollWheelValue    | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetScrollDirection     | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
| GetState               | MiddleButton | True   |  0.0000 ns |  0.0000 ns |  0.0000 ns |   0.0000 ns |   0.0000 ns |         - |
