# Aspire Demo Project

This repository is a demo project created to complement the DevWare GmbH article on the possibilities of Microsoft Aspire. It showcases how to use Aspire for modern .NET application development, including service orchestration, dashboard usage, and integration with C# and JavaScript components.

## Features

- Example .NET services using Aspire
- Integration with the Aspire dashboard
- Sample endpoints and service orchestration
- Demonstrates best practices for local development

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Aspire Dashboard](https://learn.microsoft.com/en-us/dotnet/aspire/overview/)
- Git

## Setup

1. **Clone the repository:**
   ```sh
   git clone https://github.com/your-org/your-repo.git
   cd your-repo
   ```

2. **Restore .NET dependencies:**
   ```sh
   dotnet restore
   ```

3. **(Optional) Install JavaScript dependencies:**
   ```sh
   cd path/to/js/project
   npm install
   ```

4. **Run the solution:**
   ```sh
   dotnet run --project path/to/your/aspire/project
   ```

5. **Access the Aspire dashboard:**
    - The dashboard will be available at `http://localhost:your-dashboard-port`

## Usage

- Explore the endpoints and services via the dashboard.
- Modify and extend the solution as needed for your own experiments.

---