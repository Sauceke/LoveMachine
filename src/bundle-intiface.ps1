cd $PSScriptRoot
$intiface_url = "https://github.com/intiface/intiface-engine/releases/download/v1.4.0/intiface-engine-win-x64-Release.zip"
curl -L $intiface_url -o ./intiface.zip
Expand-Archive -Path ./intiface.zip -DestinationPath ./intiface
$plugins = Get-ChildItem -Name -Path ./bin/ | Where-Object { $_ -notlike "LoveMachine.Core*" }
foreach ($plugin in $plugins) {
	$dest_path = (Get-ChildItem "./bin/$plugin/BepInEx/plugins/").FullName
	copy ./intiface $dest_path
}
