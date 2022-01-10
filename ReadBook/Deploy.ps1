Start-Deploy -ComputerName 182.43.196.20 -WebSiteName ReadBook -WebSitePort 8060 -ScriptBlock { 
    param($o) dotnet publish -o $o -c "Release" --no-self-contained -v m --nologo /p:EnvironmentName=Production 
} -OutputPath .\bin\publish\