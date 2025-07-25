﻿Congratulations, 'EF Core Power Tools' has now generated a DbContext and Entity classes for you. 

You need to configure your app now - here are some hints:

### NuGet packages:

Add these NuGet packages to your .csproj based on your selections:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.3" />
</ItemGroup>
```

### ASP.NET Core:

1. Register your DbContext class in your "Program.cs" file.

    ```csharp
    builder.Services.AddDbContext<MssqlContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    ```

2. Add "ConnectionStrings" to your configuration file (secrets.json, appsettings.Development.json or appsettings.json).

    ```json
    {
        "ConnectionStrings": {
            "DefaultConnection": "data source=localhost;initial catalog=test_env;trust server certificate=True;command timeout=300"
        }
    }
    ```

### Thank you!

Thank you for using this free tool! Have a look at [the wiki](https://github.com/ErikEJ/EFCorePowerTools/wiki/Reverse-Engineering) 
to learn more about all the advanced features

You can create issues, questions and suggestions [on GitHub](https://github.com/ErikEJ/EFCorePowerTools/issues)

If you like my free tool, I would be very grateful for a rating or review 
on [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools&ssr=false#review-details) 
or even a [one-time or monthly sponsorship](https://github.com/sponsors/ErikEJ?frequency=one-time&sponsor=ErikEJ)
