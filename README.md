# Travel Agency Management System

A Windows desktop application for managing trips, clients, bookings, and payments. Developed by Group 41 for CS312 — Modern Development Tools at Misr University for Science and Technology (MUST).

## Modules

| Module | Function |
| --- | --- |
| Login | Creates the first administrator on an empty database and signs in existing users. |
| Dashboard | Displays summary cards, booking trends, and monthly revenue. |
| Trips | Creates, edits, and cancels trips. |
| Clients | Creates, edits, and deactivates client records. |
| Bookings | Creates and cancels reservations and calculates totals from the trip and traveler count. |
| Payments | Records receipts and shows booking balances. |
| Reports | Generates date-range summaries and ranks popular destinations. |
| Users | Lets administrators manage user accounts, roles, and active status. |

Shared record screens use DataGridView, search, and filters. Dialogs handle save actions and display validation and operation errors.

## Technology and structure

The project uses C#, .NET 8 Windows Forms, GDI+ through System.Drawing, and MongoDB.Driver 3.13.0.

- [Solution](TravelAgencyManagementSystem.sln) — entry point for Visual Studio.
- [Application project](TravelAgencyManagementSystem/TravelAgencyManagementSystem.csproj) — Windows target and NuGet dependency.
- [Forms](TravelAgencyManagementSystem/Forms/) — login, navigation, dialogs, Designer files, and form resources.
- [UserControls](TravelAgencyManagementSystem/UserControls/) — feature screens, shared record controls, and custom charts.
- [Data](TravelAgencyManagementSystem/Data/) — MongoDB access, models, reservations, reports, and user services.
- [Styling](TravelAgencyManagementSystem/Styling/) — shared application theme.

## GDI+ visualization

The Dashboard shows bookings in a line chart with circular markers and monthly revenue in a bar chart. The login branding draws an illustration using translation and rotation. See the [implementation evidence](CS312_EVIDENCE.md) for drawing APIs and source references.

## Run locally

1. On Windows, install Visual Studio with the **.NET desktop development** workload and support for the .NET 8 Windows target. Install the .NET 8 Desktop Runtime if needed.
2. Install MongoDB Community Server locally. If installed as a Windows service, check it with **Get-Service -Name MongoDB**. Start a stopped service from elevated PowerShell with **Start-Service -Name MongoDB**.
3. Open **TravelAgencyManagementSystem.sln** and press **F5**. Visual Studio restores the NuGet dependency.
4. Select **Sign in**. On an empty database, the app prompts you to create the first administrator. Enter a username, email, and password of 12–256 characters, then sign in.

To build from the solution directory, run **dotnet build TravelAgencyManagementSystem.sln -c Release**.

## Database configuration and limitations

The application connects to **mongodb://localhost:27017** and uses **TravelAgencyDB**. The endpoint is set in [AgencyStore.cs](TravelAgencyManagementSystem/Data/AgencyStore.cs). The connection string carries no credentials. The first administrator is created at runtime. Passwords are stored as salted PBKDF2-SHA256 hashes.

The application assumes a trusted local MongoDB instance. Keep its listener on loopback. Local processes with access to an unauthenticated instance can bypass application roles. Standalone MongoDB operations spanning multiple collections can temporarily expose intermediate states after an interruption until application recovery runs.
