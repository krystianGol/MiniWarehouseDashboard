# Mini Warehouse Dashboard 📦

A modern, cross-platform inventory management application built with **.NET MAUI**. This dashboard provides real-time visualization of warehouse stock, advanced filtering, and CSV data import capabilities.

## ✨ Features

*   **Real-time Dashboard**:
    *   Visual overview of total products in stock.
    *   **Interactive Chart**: Bar chart visualizing top products by quantity (DevExpress Charts).
    *   **Data Grid**: Detailed inventory list with sorting and columns (DevExpress DataGrid).

*   **Advanced Search & Filter**:
    *   🔍 **Product Search**: Filter by product name.
    *   📂 **Category Filter**: Filter by product category.
    *   🔢 **Quantity Range**: Filter by minimum and maximum stock levels.
    *   ⚠️ **Low Stock Alert**: Quickly identify items running low.

*   **Data Management**:
    *   **CSV Import**: Easily import inventory data from CSV files.
    *   **Scrollable Layout**: Optimized for tablets and desktop views with a responsive, side-by-side layout.

## 🛠️ Tech Stack

*   **Framework**: .NET MAUI (Multi-platform App UI)
*   **Language**: C# 12, XAML
*   **UI Components**: DevExpress MAUI Controls (Charts, DataGrid)
*   **Pattern**: MVVM (Model-View-ViewModel)

## 🚀 Getting Started

### Prerequisites
*   .NET 9.0 or later SDK
*   Visual Studio 2022 (with MAUI workload) OR VS Code with C# Dev Kit
*   Android Emulator / iOS Simulator / MacCatalyst

### Installation

1.  **Clone the repository**
    ```bash
    git clone https://github.com/YOUR_USERNAME/MiniWarehouseDashboard.git
    cd MiniWarehouseDashboard
    ```

2.  **Restore dependencies**
    ```bash
    dotnet restore
    ```

3.  **Build and Run**
    *   Select your target framework (Android, iOS, macOS, Windows) and run.
    ```bash
    dotnet build -t:Run -f net9.0-android
    ```

## 📁 Project Structure

*   `Views/`: UI pages (MainPage.xaml)
*   `ViewModels/`: Business logic and state management
*   `Services/`: Data handling (CSV parsing, File I/O)
*   `Models/`: Data models (WarehouseItem)
*   `Converters/`: Value converters for XAML bindings

## 📸 Some Screenshots
<img width="2126" height="1168" alt="image" src="https://github.com/user-attachments/assets/2096c798-8578-41b3-8fb1-f790866fea40" />
<br/><br/>
<img width="2114" height="926" alt="image" src="https://github.com/user-attachments/assets/7cc3575e-a63e-4b24-884a-e96449d4558c" />
<br/><br/>
<img width="2120" height="1166" alt="image" src="https://github.com/user-attachments/assets/e56c8119-39ea-471f-b9fe-90dadfb1e614" />
<br/><br/>
<img width="2116" height="1326" alt="image" src="https://github.com/user-attachments/assets/6af3f070-8a87-47d7-b3f6-b08143a34d69" />
<br/><br/>
<img width="2118" height="1164" alt="image" src="https://github.com/user-attachments/assets/daf73bef-cb89-460c-9b5f-6f10eeee4946" />
<br/><br/>
<img width="2116" height="1160" alt="image" src="https://github.com/user-attachments/assets/ee185548-e7c9-4dc3-a891-b703779eb246" />




