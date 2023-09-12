cd $PSScriptRoot
dotnet tool install --global Alias
$plugins = Get-ChildItem -Name -Path ./bin/ | Where-Object { $_ -notlike "LoveMachine.Core*" }
foreach ($plugin in $plugins) {
	if (Test-Path "./bin/$plugin/BepInEx/plugins/*/LoveMachine.Core.IL2CPP.dll") {
		# don't alias il2cpp plugins
		continue
	}
	$suffix = "_Of_Love"
	$path = "./bin/$plugin/BepInEx/plugins"
	# find all dependencies to alias
	$dlls = Get-ChildItem $path -Filter "*.dll" -Recurse
	$dll_names = $dlls -Replace "\.dll$",""
	$deps = $dll_names | Where { $_ -NotLike "LoveMachine.*" -And $_ -NotLike "*$suffix" }
	$deps_str = $deps -Join ";"
	assemblyalias --target-directory $path --suffix $suffix --assemblies-to-alias $deps_str 
}
