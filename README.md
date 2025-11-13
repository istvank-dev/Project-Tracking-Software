React with ASP.NET Core (.NET 10)

To reset the SQLite db:
1. Delete `Migration` folder and `app.db` file.
2. Open Package Manager Console (Tools -> NuGer Package Manager -> Package Manager Console).
3. In the PMC select the default project as "ProjectTrackingSoftware.Server"
4. Run `Add-Migration InitialSetup` command.
5. Run `Update-Database` command.
