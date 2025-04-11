param (
    [string] $env
)

# 先安裝 EFCOrePowerTools CLI
dotnet tool install --global ErikEJ.EFCorePowerTools.Cli --version 9.1.807

# 指定設定檔
$appsettingsFile = ""
switch ($env) {
    "Development" { $appsettingsFile = "appsettings.Development.json" }
    default { $appsettingsFile = "appsettings.json" }
}

# 取得設定檔
$appsettings = Get-Content -Path ".\Nastice.GoogleAuthenticateLab.Api\${appsettingsFile}" -Raw | ConvertFrom-Json

# 取得連線字串
$connectionString = $appsettings.ConnectionStrings.SqlServer

# 如果連線字串為空，則直接離開
if ([string]::IsNullOrEmpty($connectionString)) {
    Write-Host "提供的設定檔案 ${appsettingsFile} 找不到 SqlServer 連線字串！請先設定 ConnectionString.SqlServer。" -ForegroundColor Red
    exit 1;
}

# 移動到 Data 專案
cd "./Nastice.GoogleAuthenticateLab.Data"

# 執行 Reverse Engineering
efcpt $connectionString mssql

# 回到上層資料夾
cd ..