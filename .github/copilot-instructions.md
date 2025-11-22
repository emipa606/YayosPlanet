# GitHub Copilot Instructions for Yayo's Planet (Continued) Mod

## Mod Overview and Purpose

**Yayo's Planet (Continued)** is a mod for RimWorld that introduces seasonal catastrophic events. These events occur every 4th quarter of the in-game year, presenting unique survival challenges such as extreme weather conditions that players must prepare for and endure. Each year begins with players knowing the type of disaster for the upcoming fourth quarter. After the disaster period ends, temperatures return to normal before the cycle repeats.

## Key Features and Systems

- **Disasters**: Introduces three primary disasters:
  - *Ice Age*: Brings very cold temperatures.
  - *Inferno*: Leads to scorching hot temperatures.
  - *Sandstorm*: Involves numerous tornadoes. These tornadoes cannot penetrate walls or damage items indoors.

- **Temperature Control**: Offers adjustable temperature settings in the options menu for each disaster type.

- **Mod Recommendations**:
  - **Dubs Bad Hygiene**: Adds items for enhanced temperature control.
  - **Yayo's Nature**: Alters biome conditions every 60 days for more environmental variety.

## Coding Patterns and Conventions

- The mod is developed primarily using C#, following the .NET Framework 4.8.
- **Class Naming**: Each class corresponds to specific mod functionalities and follows a consistent naming pattern (e.g., `GameCondition_yyCold`).
- **Method Structure**: Typically private helper methods perform complex functionality within the main class methods.

## XML Integration

XML integration is often required for defining in-game elements such as:
- Events and events' descriptions.
- Configuration settings that can be adjusted by the player.

XML allows easy modification and extension of game content without needing deep programming knowledge.

## Harmony Patching

Harmony is leveraged to patch existing game methods and inject custom logic where needed. This ensures that the mod interacts seamlessly with the core game mechanics and potentially other mods, allowing:
- Modifications of existing game conditions to introduce or modify disaster behavior.
- Interaction with the game's storyteller and other systems.

## Suggestions for Copilot

1. **Harmony Patching**:
   - Suggest patches for parts of the game's API that handle weather or map conditions to efficiently inject new disaster behaviors.

2. **Event Scheduling**:
   - Recommend methods to schedule events dynamically based on the game's in-game time loops and seasons.

3. **Error Handling**:
   - Provide suggestions for logging and debugging techniques to handle and trace errors effectively, given the integration complexities.

4. **Performance Optimization**:
   - Offer tips on optimizing methods that might involve intensive computations, such as those managing the tornado's path or temperature simulations.

5. **User Configuration**:
   - Propose methods for managing user settings, potentially through XML definition files or advanced settings management in C# to enhance user configurability and experiences.

By using the guidelines provided in this document, you can effectively utilize Copilot to assist you in working with the Yayo's Planet (Continued) mod codebase. This includes introducing further features, optimizing existing code, or adapting the mod to new use cases.
