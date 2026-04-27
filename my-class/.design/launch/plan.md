# Dist Launch Scripts For `my-class`

## Summary
Add launch scripts directly in `my-class/dist`. They will run the existing published app at `my-class/dist/app/MyClass.exe`, bind it to port `5555`, replace localhost with the machine’s WiFi/local IP, and open the browser with a class query like `?c=demo`.

## Key Changes
- Add `my-class/dist/run.ps1`.
- Add `my-class/dist/run.bat`.
- `run.ps1` behavior:
  - Uses `app\MyClass.exe`.
  - Uses `app` as the working directory.
  - Defaults to port `5555`.
  - Accepts `-ClassCode`, default `demo`.
  - Builds the browser URL as `http://<wifi-local-ip>:5555/?c=<ClassCode>`.
  - Sets `ASPNETCORE_URLS=http://<wifi-local-ip>:5555`.
  - Starts the app and opens the browser.
  - Stores a PID file in `my-class/dist` and stops only the previously launched app on the next run.
  - Fails clearly if `app\MyClass.exe` is missing or port `5555` is already used by another process.

- `run.bat` behavior:
  - Lives beside `run.ps1`.
  - Calls PowerShell with execution policy bypass.
  - Passes the first BAT argument as `-ClassCode`.
  - Defaults to `demo` when no argument is supplied.

## Script Interface
From `my-class/dist`:

```bat
run.bat
```

opens:

```text
http://<wifi-local-ip>:5555/?c=demo
```

Custom class code:

```bat
run.bat math101
```

opens:

```text
http://<wifi-local-ip>:5555/?c=math101
```

PowerShell directly:

```powershell
.\run.ps1 -ClassCode demo
```

## Test Plan
- Run `my-class/dist/run.bat`.
- Confirm browser opens at `http://<wifi-local-ip>:5555/?c=demo`.
- Confirm the app process is `my-class/dist/app/MyClass.exe`.
- Run `my-class/dist/run.bat otherclass` and confirm the browser opens with `?c=otherclass`.
- Run the script twice and confirm the previous launched app instance is replaced cleanly.
- Confirm quiz assets still load from the existing `my-class/dist/.assets` folder through the existing `../.assets/quiz` appsetting.

## Assumptions
- No publish step.
- No asset copying.
- No app code or appsettings changes.
- Scripts go in `my-class/dist`, not `my-class/src`.
