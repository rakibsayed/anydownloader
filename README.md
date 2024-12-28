# AnyDownloader

A lightweight and extensible downloader application for managing multiple downloads with a focus on simplicity, performance, and flexibility.

---

## 🚧 Status: In Progress

This project is actively under development. Core functionalities are being implemented, with more features planned for the future.

---

## Features

### Implemented
- **DownloadManager**: Manage multiple downloads with asynchronous functionality.
  - `StartDownloadAsync`: Download files from a URL to a specified destination.
- **HttpDownloader**: Handles HTTP-based file downloads with:
  - Support for valid URLs.
  - Validation for invalid URLs.
- **Unit Tests**:
  - `DownloadManager` tests:
    - Verify `StartDownloadAsync` functionality.
    - Placeholder tests for `PauseDownload` and `ResumeDownload`.
  - `HttpDownloader` tests:
    - File downloads for valid URLs.
    - Exception handling for invalid URLs.

### Planned
- Pause and resume functionality for downloads.
- User Interface (UI) for managing downloads using WPF.
- Support for additional protocols (e.g., FTP, SFTP).
- Exporting logs and statistics of downloads.

---

## 🛠 Technologies

- **C#**: Core functionality.
- **WPF**: UI (planned).
- **Xunit**: Unit testing framework.
- **NSubstitute**: Mocking library for tests.

---

## 📋 Getting Started

### Clone the Repository
```bash
git clone https://github.com/your-username/AnyDownloader.git
cd AnyDownloader
