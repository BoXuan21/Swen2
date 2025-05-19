# Tour Planner Frontend

## Backend Connection Setup

To connect the frontend to your backend database:

1. **Configure the backend URL**:
   - Open `App.config` and update the `ApiBaseUrl` value to match your backend URL
   - The default is `https://localhost:7022/api` which is the default for ASP.NET Core projects in Visual Studio

2. **Ensure the backend is running**:
   - Make sure your backend project is running before starting the frontend
   - You can set up multiple-project startup in Visual Studio:
     - Right-click on the solution in Solution Explorer
     - Select "Set StartUp Projects..."
     - Choose "Multiple startup projects"
     - Set both your backend and frontend projects to "Start"

3. **Verify database connection**:
   - Make sure your backend has correct database connection string
   - The connection string in App.config is for reference only (used by the backend)

## Troubleshooting Connection Issues

If you see the error "Zielcomputer verweigert Verbindung" (Target computer refuses connection):

1. Verify that your backend is running
2. Check that the port number in `ApiBaseUrl` matches the port your backend is using
   - Look in your backend project's `launchSettings.json` file for the correct port
3. Ensure no firewall is blocking the connection
4. Try using `localhost` instead of `127.0.0.1` or vice versa

## Running in Offline Mode

The application can run in "offline mode" with mock data if the backend is unavailable. This is automatic and requires no configuration. 