echo '<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear/>
    <add key="NuGet" value="https://api.nuget.org/v3/index.json" />
    <add key="identity-server-rk-software" value="https://nuget.pkg.github.com/rk-software-systems/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <identity-server-rk-software>
      <add key="Username" value="dockerbuild" />
      <add key="ClearTextPassword" value="'"$1"'" />
    </identity-server-rk-software>
  </packageSourceCredentials>
</configuration>' >> "NuGet.config"