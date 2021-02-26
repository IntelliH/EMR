Copy C:\inetpub\wwwroot\webapp\Web.Docker.config C:\inetpub\wwwroot\webapp\Web.config 
@PowerShell "(GC C:\inetpub\wwwroot\webapp\Web.config)|%%{$_ -Replace '#DB_HOST#',$env:DB_HOST}|SC C:\inetpub\wwwroot\webapp\Web.config"
@PowerShell "(GC C:\inetpub\wwwroot\webapp\Web.config)|%%{$_ -Replace '#DB_USER#',$env:DB_USER}|SC C:\inetpub\wwwroot\webapp\Web.config"
@PowerShell "(GC C:\inetpub\wwwroot\webapp\Web.config)|%%{$_ -Replace '#DB_PASSWORD#',$env:DB_PASSWORD}|SC C:\inetpub\wwwroot\webapp\Web.config"
@PowerShell "(GC C:\inetpub\wwwroot\webapp\Web.config)|%%{$_ -Replace '#DB_NAME#',$env:DB_NAME}|SC C:\inetpub\wwwroot\webapp\Web.config"
@PowerShell "Import-Module WebAdministration; Remove-Website -Name 'Default Web Site'; New-Website -Name 'webapp' -Port 80 -PhysicalPath 'C:\inetpub\wwwroot\webapp'; Start-Website -Name webapp; Get-ChildItem -Path IIS:\Sites"

C:\ServiceMonitor.exe w3svc
