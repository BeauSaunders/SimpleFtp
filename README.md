# SimpleFtp

A lightweight, reusable C# FTP library with async support.  
Designed for quick integration into any project that needs reliable FTP operations.

---

## Features

- Configure FTP credentials  
- Retrieve directory lists asynchronously  
- Test connection health  
- Fully async / await compatible  

---

## API Overview

Below is a clear breakdown of each public function, including purpose, parameters, return values, and examples.

---

### `SetCredentials(string host, string username, string password)`

**Purpose**  
Configure the FTP connection details that all other methods will rely on.

**Parameters**  
- `host` – FTP server URL or IP address  
- `username` – Login username  
- `password` – Login password  

**Example**  
```csharp
FtpClient.SetCredentials("ftp.example.com", "user", "pass");
```

---

### `Task<List<string>> GetDirectoriesAsync(string path)`

**Purpose**
Retrieve a list of directories from a specified remote FTP path.

**Parameters**
	•	`path` – The remote directory to query (e.g., /assets/)

**Returns**
	•	`Task<List<string>>` – A list of directory names

**Example**
```csharp
var dirs = await FtpClient.GetDirectoriesAsync("/server/");
foreach (var d in dirs)
    Console.WriteLine(d);
```
