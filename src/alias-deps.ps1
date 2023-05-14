cd $PSScriptRoot
dotnet tool install --global Alias
$plugins = Get-ChildItem -Name -Path ./bin/ | Where-Object { $_ -notlike "LoveMachine.Core*" }
foreach ($plugin in $plugins) {
	if (Test-Path "./bin/$plugin/BepInEx/plugins/*/LoveMachine.Core.IL2CPP.dll") {
		continue
	}
	assemblyalias --target-directory "./bin/$plugin/BepInEx/plugins" --suffix _Of_Love --assemblies-to-alias "LitJSON;SuperSocket.ClientEngine;WebSocket4Net"
}
