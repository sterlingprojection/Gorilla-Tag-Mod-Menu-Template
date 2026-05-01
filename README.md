# Testplate Mod Template

A high-performance, modular mod menu template for Gorilla Tag. This template is designed for developers who want a clean, comment-free codebase with advanced UI capabilities and robust networking utilities.

## Key Features

*   **Advanced UI System**: A fully functional tabbed menu with support for toggles, buttons, and real-time controllable sliders.
*   **Controllable Sliders**: Smooth, drag-based slider interaction for precise value adjustments (e.g., Fly Speed).
*   **Reflection-Based Utilities**: Robust `VRRigHelper` and `VRRigExtensions` that use reflection to ensure compatibility across different SDK versions.
*   **Modular Feature Registry**: Easily add or remove features and tabs through a centralized `FeatureRegistry`.
*   **Clean Codebase**: Stripped of all comments for a streamlined, professional look.

## Getting Started

### Prerequisites

*   Visual Studio 2022 or later.
*   Gorilla Tag installed on your PC.
*   Latest BepInEx and Gorilla Tag SDK.

### Installation

1.  Clone the repository or download the source code.
2.  Open `Testplate.sln` in Visual Studio.
3.  Update the project references to point to your local Gorilla Tag `Managed` folder.
4.  Build the project (Ctrl+Shift+B).
5.  Copy the resulting `Testplate.dll` from the `bin/Debug` or `bin/Release` folder to your `Gorilla Tag/BepInEx/plugins` directory.

## Usage

### Adding a New Feature

1.  Create a new class in the `Features` folder that inherits from `FeatureBase`.
2.  Implement your logic in the `OnUpdate`, `OnEnable`, and `OnDisable` methods.
3.  Register your feature in `Menu/FeatureRegistry.cs` within the `Initialize` method.

### Customizing the UI

The menu layout is managed in `FeatureRegistry.cs`. You can create new `MenuTab` objects and add `MenuButton` instances to them.

```csharp
var myTab = new MenuTab("My Tab");
myTab.AddButton(new MenuButton("My Feature", () => { /* Logic */ }));
Tabs.Add(myTab);
```

## Credits

*   **Template Developer**: Sterling
*   **Utilities Reference**: Seralyth-Menu (RigUtilities & VRRigExtensions)

## License

This project is licensed under the MIT License - see the LICENSE file for details.
