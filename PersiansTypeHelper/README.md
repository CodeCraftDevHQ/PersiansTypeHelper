# PersianTypeHelper

A lightweight Windows background tool for typing and displaying RTL Persian/Arabic text correctly in applications and Unity-based games.

PersianTypeHelper was originally created because typing Persian text in games such as Genshin Impact was difficult due to the way some Unity applications handle right-to-left text. The goal of this project is to provide a simple way to process Persian/Arabic text and send it where it is needed using customizable global hotkeys.

## ✨ Features

- 🇮🇷 **Persian and Arabic RTL text reshaping**
- ⌨️ **Customizable global hotkeys**
- 🔢 **Support for Persian, Arabic, and English numbers**
- 📝 **Optional Persian/Arabic character and text processing**
- 🖥️ **Runs as a Windows background application**
- 📌 **Pin-to-top window mode**
- 🌙 **Dark / Light dynamic theme (with system auto-detection)**
- 🎨 **Custom application title bar**
- 🔔 **System tray integration**
- 🚀 **Startup notification**
- ⚙️ **Configurable hotkey settings**
- 🔢 **Character limit configuration**
- 🕹️ **Designed to work with Unity-based applications and games**

---

## 🎮 Why was it created?

The original motivation for PersianTypeHelper came from a simple problem:

> I wanted to type Persian text in Genshin Impact, but RTL text was not handled correctly.

Instead of manually fixing the text every time, I decided to build a small Windows tool that could process Persian/Arabic text and make it easier to use through global hotkeys.

Although Genshin Impact was one of the main reasons for creating the project, PersianTypeHelper is not limitedث to Genshin Impact and can be useful with other applications that have similar RTL text handling problems.

---

## 🧩 How it works

PersianTypeHelper processes Persian /Arabic text before sending or using it.
The project includes a custom `PersianReshaper` implementation that handles Arabic/Persian character forms and reshapes text so that RTL text can be displayed more appropriately in environments that do not handle it correctly.

The text processing system also supports:
- **Persian digits:** `۰۱۲۳۴۵۶۷۸۹`
- **Arabic digits:** `٠١٢٣٤٥٦٧٨٩`
- **English digits:** `0123456789`
- Arabic/Persian characters & Harakat
- Common RTL symbols and punctuation

---

## ⌨️ Hotkeys & UI Features

The application uses global keyboard shortcuts to make text processing accessible without constantly switching between applications. Hotkeys can be configured through the application's settings.

- **System Tray:** PersianTypeHelper runs in the background without occupying the taskbar.
- **Themes:** Supports dynamic Dark / Light mode switching, saving preferences automatically to `settings.json`.
- **Custom Title Bar:** Modern borderless UI with dedicated Pin and Close controls.

---

## 🛠️ Technologies

- **Language:** C#
- **Framework:** .NET / Windows Forms
- **APIs:** Native Windows APIs & Global Hotkey Hooks
- **Text Processing:** Custom Unicode Reshaper

---

## 📁 Project Structure

```text
PersianTypeHelper/
│
├── PersianTypeHelper/
│   ├── HotkeyCaptureForm.cs
│   ├── HotkeySettings.cs
│   ├── HotkeyWindow.cs
│   ├── InputForm.cs
│   ├── NativeMethods.cs
│   ├── PersianReshaper.cs
│   ├── Program.cs
│   ├── Theme.cs
│   ├── TrayContext.cs
│   └── app.ico
│
├── .gitignore
├── .gitattributes
└── PersianTypeHelper.slnx
