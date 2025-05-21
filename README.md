# RoomScout

## Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Requirements](#requirements)
  - [Input Files](#input-files)
- [Running the Application](#running-the-application)
  - [Option 1: From Visual Studio](#option-1-from-visual-studio-recommended-for-debugging)
  - [Option 2: From Command Line](#option-2-from-command-line)
- [Supported Commands](#supported-commands)
  - [Availability Command](#availability-command)
  - [Search Command](#search-command)
- [Running Tests](#running-tests)
- [Design Notes & Assumptions](#design-notes--assumptions)
- [Technologies](#technologies)

## Overview

A C#/.NET console application for managing hotel room availability and search functionality based on a seed dataset. This application reads hotel and booking data from JSON files and supports two key commands:

- `Availability(...)`
- `Search(...)`

The application follows clean architecture principles. The business logic is isolated, testable, and decoupled from infrastructure concerns. Special care was taken to ensure the code is modular, readable, and easy to extend.

---

## Project Structure

```
Core/RoomScout/
|-- RoomScout.Business/         # Services, command handlers, interfaces
|-- RoomScout.DataAccess/       # Repositories, models, JSON seed data
|-- RoomScout.Presentation/     # CLI entry point, dispatcher and bootstrapper
|-- RoomScout.UnitTests/        # MSTest unit tests for services and commands
|-- RoomScout.sln               # Solution file
```

---

## Getting Started

### Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Any C# compatible IDE such as:
  - Visual Studio 2022 (recommended)
  - JetBrains Rider
  - Visual Studio Code (with C# extension)
- Alternatively, you can build and run the app using the .NET CLI

---

### Input Files

Ensure you have the following two JSON files (already included):

- `hotels.json`
- `bookings.json`

By default, these are located at:
```
RoomScout.DataAccess/SeedData/hotels.json
RoomScout.DataAccess/SeedData/bookings.json
```

---

## Running the Application

### Option 1: From Visual Studio (recommended for debugging)

Just press **F5** or run the **RoomScout.Presentation** project. Default input files will be loaded automatically.

---

### Option 2: From Command Line

Navigate to the project root and run:

```bash
dotnet restore
dotnet build
dotnet run --project RoomScout.Presentation --hotels RoomScout.DataAccess/SeedData/hotels.json --bookings RoomScout.DataAccess/SeedData/bookings.json
```

You can also provide custom input files using:

```bash
dotnet run --project RoomScout.Presentation --hotels path/to/hotels.json --bookings path/to/bookings.json
```

Example:

![image](https://github.com/user-attachments/assets/a8672c4a-705a-433a-9404-f91f264d2405)
![image-1](https://github.com/user-attachments/assets/4417538c-0b59-4362-94dd-3ad529810c38)

---

## Supported Commands

### Availability Command

```bash
Availability(H1, 20250901, SGL)
Availability(H1, 20250901-20250903, DBL)
```

- Returns the available room count for a specific date or date range.
- For ranges, returns the **minimum availability** across the range.

**Example Output:**
```
2
```

---

### Search Command

```bash
Search(H1, 30, SGL)
```

- Searches availability for the next N days, returning all available ranges.
- Output is a list of `(arrival-departure, availability)` tuples.

**Example Output:**
```
(20250901-20250902, 2), (20250902-20250903, 1), (20250905-20250907, 1), (20250909-20250931, 2)
```

---

## Running Tests

Run all unit tests using:

```bash
dotnet test
```

The tests cover edge cases and business logic in:

- `AvailabilityCommandHandler`
- `SearchCommandHandler`
- `AvailabilityService`
- `SearchService`

---

## Design Notes & Assumptions

- Uses the half-open interval `[arrival, departure)` for date logic.
- Room availability can be **negative** (overbooking is allowed).
- Validation and error messages are handled via simple `if` logic and console output for clarity.
- Command parsing tolerates **extra spaces** and **case differences**.

---

## Technologies

- C# 12 / .NET 8.0
- MSTest
- Newtonsoft.Json NuGet package
- 3-tier Clean Architecture (Presentation -> Business -> DataAccess)
