# 设置服务端根路径（请确保路径正确）
$projectRoot = "D:\Programing\C#\FMS\FMS.Server"
$outputZip = "D:\Programing\C#\FMS\FMS_Server_Export.zip"

# 需要包含的所有关键源代码文件/文件夹
$include = @(
    "Controllers\*.cs",         # 控制器：RevenueController.cs
    "Data\*.cs",                # DbContext、DbContextFactory
    "Models\*.cs",              # RevenueRecord.cs 等模型类
    "Migrations\*.cs",          # EF Core 迁移记录（结构同步）
    "Program.cs",               # 应用入口
    "appsettings.json",         # 数据库连接配置
    "appsettings.Development.json",
    "FMS.Server.csproj"         # 项目文件（确保依赖清晰）
)

# 收集完整路径
$fullPaths = $include | ForEach-Object { Join-Path $projectRoot $_ }

# 执行压缩
Compress-Archive -Path $fullPaths -DestinationPath $outputZip -Force

Write-Host " 打包完成: $outputZip"