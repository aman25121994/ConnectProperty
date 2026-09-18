# Property Connect

Internal property-listing app for LAN deployment via IIS. Anonymous users browse
and search listings and see seller contact info; a single admin account creates
and deletes listings (with photo upload).

Built with ASP.NET Core MVC (.NET 8), Entity Framework Core, and SQLite. No
external CSS/JS dependencies — everything needed to render the UI ships in the
project, so it works correctly on a LAN with no internet access.

## 1. Prerequisites (dev machine)

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## 2. Run it locally

```bash
cd PropertyConnect
dotnet restore
dotnet run
```

Browse to the URL shown in the console (e.g. `http://localhost:5000`). The
SQLite database (`propertyconnect.db`) and a few sample listings are created
automatically on first run.

**Default admin login:** username `admin`, password `ChangeMe123!`
Change this before deploying — see step 5.

## 3. Where data is stored

The SQLite database and uploaded photos are **not** stored inside the app's
own folder — they live in a separate `data` directory so they survive
redeploys on hosts that wipe the app folder each time you publish:

- **On Azure App Service**: automatically uses Azure's persistent storage
  (`%HOME%\data`), which is untouched by redeploys.
- **Everywhere else** (local dev, your own IIS server): uses a `data` folder
  created next to the running app.

No configuration needed — this is detected automatically at startup
(`Data/StoragePaths.cs`).

## 4. Publish for your own IIS server

```bash
dotnet publish -c Release -o C:\inetpub\wwwroot\PropertyConnect
```

This produces a self-contained publish folder including `web.config`, which
tells IIS to route requests to the ASP.NET Core Module.

## 5. IIS setup (Windows Server / Workstation)

1. Install the **.NET 8 Hosting Bundle** on the IIS machine (this installs the
   ASP.NET Core Module for IIS — required even though the SDK isn't needed on
   the server). Restart IIS afterward (`net stop was /y && net start w3svc`).
2. In IIS Manager, create a new **Application Pool**:
   - .NET CLR version: **No Managed Code**
   - Start mode: **AlwaysRunning**
3. Create a new **Site** (or Application) pointing at the publish folder
   (e.g. `C:\inetpub\wwwroot\PropertyConnect`), bound to **Port 80**, using
   the app pool from step 2.
4. **Authentication:** this app handles its own login with a cookie — it does
   not use IIS Windows Authentication. In IIS Manager → the site →
   **Authentication**, make sure:
   - **Anonymous Authentication** = Enabled
   - **Windows Authentication** = Disabled
   (If your server has Windows Authentication enabled by default for other
   sites/apps, be sure it's off for this one specifically.)
5. **Permissions:** grant the app pool identity (`IIS AppPool\<YourPoolName>`,
   or `IIS_IUSRS` if using a shared pool) **Modify** permission on the
   publish folder (so it can create the `data` folder next to it).
6. **Firewall:** open inbound TCP port 80 if it isn't already:
   ```powershell
   New-NetFirewallRule -DisplayName "Property Connect (HTTP)" -Direction Inbound -Protocol TCP -LocalPort 80 -Action Allow
   ```
7. Browse to `http://<server-ip>/` from another machine on the LAN to confirm.

## 6. Deploy to Azure App Service (free tier)

1. Create a free account at https://portal.azure.com if you don't have one.
2. **Create a resource → Web App**:
   - Publish: Code
   - Runtime stack: **.NET 8**
   - Pricing plan: **F1 (Free)**
3. Install the [Azure CLI](https://aka.ms/installazurecliwindows), then from
   the project folder:
   ```bash
   az login
   cd PropertyConnect
   az webapp up --name <your-app-name> --resource-group <your-resource-group> --runtime "DOTNETCORE:8.0"
   ```
4. Your site is live at `https://<your-app-name>.azurewebsites.net` with free
   HTTPS. The database and uploaded photos will persist across redeploys —
   no extra setup needed, `StoragePaths.cs` detects Azure automatically.

## 7. Change the admin password

The admin credential lives in `appsettings.json` under `AdminAccount`, stored
as a SHA-256 hash (never plaintext). To set a new password, compute its hash
and paste it in:

**PowerShell (on the server or your dev machine):**
```powershell
$password = "YourNewPassword"
$hash = [System.BitConverter]::ToString(
    [System.Security.Cryptography.SHA256]::Create().ComputeHash(
        [System.Text.Encoding]::UTF8.GetBytes($password)
    )
).Replace("-", "").ToLower()
Write-Output $hash
```

Copy the printed hash into `appsettings.json`:
```json
"AdminAccount": {
  "Username": "admin",
  "PasswordHash": "<paste hash here>"
}
```
Restart the app pool after changing it.

## 8. Project layout

```
PropertyConnect/
  Controllers/      HomeController (public browse/search/details),
                     AccountController (admin login/logout),
                     PropertiesController (admin create/manage/delete)
  Models/            Property, PropertyFormViewModel, LoginViewModel
  Data/              ApplicationDbContext (EF Core + SQLite),
                     DbInitializer (seed data),
                     StoragePaths (persistent storage location — see step 3)
  Views/             Razor views, styled via wwwroot/css/site.css
  data/              SQLite database + uploaded photos (created automatically,
                     lives outside the app folder — see step 3)
```

Data is auto-provisioned with `EnsureCreated()` — no EF migrations needed. If
you change the `Property` model later, delete `propertyconnect.db` to have it
rebuilt (you'll lose existing listings), or switch to EF migrations.
