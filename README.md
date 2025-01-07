# **AnyDownloader**

A lightweight and extensible downloader application for managing multiple downloads with a focus on simplicity, performance, and flexibility.

---

## 🚧 Status: **Actively Developed**

The project is under continuous development. New features, enhancements, and bug fixes are being implemented regularly.

---

## **Features**

### **Implemented**
- **Core Enhancements**:
  - **Download Manager**:
    - Manages multiple download tasks concurrently with cancellation, pause, and resume support. *(Note: Pause/Resume functionality is not yet implemented in the UI.)*
    - `StartDownloadAsync`: Starts a download task with progress tracking and automatic file path resolution.
    - `PauseDownload` and `ResumeDownload`: Added functionality for pausing and resuming downloads.
  - **HTTP Downloader**:
    - Validates URLs and resolves file paths using HTTP headers.
    - Tracks progress, download speed, and estimated time remaining.
    - Handles errors gracefully with robust temporary file cleanup.
- **Shared Utilities**:
  - **FileSizeFormatter**: Converts file sizes into human-readable formats (e.g., MB, GB).
  - **File System Abstraction**:
    - Provides a testable interface for file-related operations such as creation, deletion, and renaming.
- **Models**:
  - **DownloadProgress**:
    - Tracks metadata such as file size, downloaded bytes, speed, and estimated time remaining.
    - *(Note: Only percentage tracking is implemented in the UI.)*
  - **DownloadTask**:
    - Represents an individual download with metadata and cancellation support.
- **Presentation Layer**:
  - **DownloaderPage**:
    - Interactive WPF UI for initiating and managing downloads.
    - Includes progress visualization, validation messages, and a spinner animation.
  - **MainWindow**:
    - Dynamically loads the `DownloaderPage` as its main content.
- **Logging**:
  - Integrated **Serilog** for structured logging to console, debug output, and file.
  - Application-wide logging provider for consistent log messages.
- **Unit Tests**:
  - **DownloadManager Tests**:
    - Verified `StartDownloadAsync` with mocked file download and progress tracking.
    - Tested invalid URL scenarios.
  - **HttpDownloader Tests**:
    - Resolved file paths correctly using HTTP headers.
    - Ensured temporary file cleanup on failures.
    - Simulated HTTP responses using custom `HttpMessageHandler`.

---

### **Planned**
- Advanced UI improvements (e.g., progress bars, batch download management).
- Support for additional protocols (e.g., FTP, SFTP).
- Improved error notifications in the UI.
- Configuration options for user-defined download directories.
- Exporting logs and download statistics.
- Automatic retries for failed downloads.

---

## 🛠 **Technologies**

- **C#**: Core functionality and logic.
- **WPF (Windows Presentation Foundation)**: Modern and responsive UI.
- **Xunit**: Unit testing framework.
- **NSubstitute**: Mocking library for testing.
- **Serilog**: Comprehensive logging framework.

---

## 📋 **Getting Started**

### Clone the Repository
```bash
git clone https://github.com/your-username/AnyDownloader.git
cd AnyDownloader
