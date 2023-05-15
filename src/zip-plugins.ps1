cd $PSScriptRoot
mkdir assets
$plugins = Get-ChildItem -Name -Path bin/ | Where-Object { $_ -notlike "LoveMachine.Core*" }
foreach ($plugin in $plugins) {
	$info = "./$plugin/PluginInfo.ini"
	$gameName = Get-Content -Path $info | Where-Object { $_ -match "NameEN = *" }
	$gameName = $gameName.Substring($gameName.IndexOf("= ") + 2)
	$gameName = $gameName.Replace(" ", "_")
	$gameName = $gameName -Replace "[^a-zA-Z0-9_]", ""
	$gameName = $gameName.Replace("__", "_")
	Compress-Archive -Path "./bin/$plugin/BepInEx" -DestinationPath "./assets/LoveMachine_for_${gameName}.zip"
}
