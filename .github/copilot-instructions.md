# GitHub Copilot Instructions for RimWorld Mod Project

## Mod Overview and Purpose
This project enhances the gameplay of RimWorld by introducing dynamic planet-based events that influence the game environment. The mod is built for the .NET Framework 4.8 and integrates seamlessly with existing game mechanics through custom C# classes and XML configurations.

## Key Features and Systems
- **Dynamic Weather Events**: Includes unique weather conditions like extreme cold, heat, and tornadoes that impact gameplay.
- **Environmental Effects**: The mod simulates realistic environmental dangers, adding depth and challenge to the player experience.
- **Custom Storyteller Integrations**: Adjusts threat levels and event frequencies dynamically through custom storytellers.

## Coding Patterns and Conventions
- **Class Organization**: The mod utilizes well-organized classes that extend RimWorld's base classes such as `MapComponent`, `GameCondition`, `SkyOverlay`, and `ThingWithComps`.
- **Consistent Naming**: Follows C# naming conventions for class and method names, e.g., `GameCondition_yyCold` and `Spawn`.
- **Single Responsibility Principle**: Each class and method serves a specific purpose to maintain code readability and ease of maintenance.

## XML Integration
While the current summary does not specify XML files, it is assumed that XML is used for configuring and defining mod settings, events, and storyteller behaviors in RimWorld. Future modifications to XML files should:
- Define new game conditions and event parameters.
- Adjust probability settings for dynamic events in coordination with C# logic.
- Ensure integration with Harmony patches for consistent behavior across different game versions.

## Harmony Patching
- **HarmonyPatches Class**: Utilizes the Harmony library to patch game methods for extending or altering native game behavior without modifying the base game code.
- **Patch Methods**: Use descriptive method names and annotations to maintain clear documentation of each patch's purpose and scope.

## Suggestions for Copilot
1. **Class Definitions**: When creating new classes, ensure they extend the most suitable base class (e.g., `GameCondition` or `MapComponent`) to integrate properly.
2. **Event Handling**: Implement robust error handling and logging at key points in dynamic event classes (e.g., `yyTornado`) to troubleshoot issues faster.
3. **XML Files**: If generating XML configuration files, align properties with corresponding C# class attributes for consistency.
4. **Conditions and Utility**: Develop utility methods in `util` and `util2` for common calculations or checks, promoting code reuse and simplicity.
5. **Performance Optimization**: Suggest improvements, such as caching frequently accessed data or optimizing loop operations, especially in performance-critical classes like weather effects or threat evaluations.

By adhering to these guidelines, Copilot can assist developers in extending this mod further while maintaining a high standard of code quality and gameplay integration.
