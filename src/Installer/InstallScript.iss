#define PluginBuildDir SourcePath + "..\bin\"
#define PrototyperFilesDir SourcePath + "..\PrototyperFiles\"
#define BepInEx32Dir SourcePath + "BepInEx32"
#define BepInEx64Dir SourcePath + "BepInEx64"
#define BepInExIl2cpp64Dir SourcePath + "BepInExIl2cpp64"
#define AppVersion GetVersionNumbersString(PluginBuildDir + "LoveMachine.Core\LoveMachine.Core.dll")

; We have a lot of plugins, so we just find them all and put them in here
; This way the script will handle new plugins by itself and we can forget about it
#dim Plugins[100]
#define PluginCount
#define PrototyperPluginStartIndex

#define GetPluginId(Index) Plugins[Index]
#define GetPluginInfoIni(Index) SourcePath + "..\" + GetPluginId(Index) + "\PluginInfo.ini"
#define GetGameNameEN(Index) ReadIni(GetPluginInfoIni(Index), GetPluginId(Index), "NameEN")
#define GetGameNameJP(Index) ReadIni(GetPluginInfoIni(Index), GetPluginId(Index), "NameJP")
#define GetGameRegSubKey(Index) ReadIni(GetPluginInfoIni(Index), GetPluginId(Index), "RegSubKey")
#define GetGameRegName(Index) ReadIni(GetPluginInfoIni(Index), GetPluginId(Index), "RegName")
#define GetGameArchitecture(Index) ReadIni(GetPluginInfoIni(Index), GetPluginId(Index), "Architecture")
#define GetExecutableName(Index) ReadIni(GetPluginInfoIni(Index), GetPluginId(Index), "ExecutableName")

#define GetPrototyperCfg(Index) PrototyperFilesDir + GetPluginId(Index) + "\Sauceke.LoveMachinePrototyper.cfg"
#define GetPrototyperGameArchitecture(Index) ReadIni(GetPrototyperCfg(Index), "Prototyper Settings", "Game Build Architecture") 
#define GetPrototyperExecutableName(Index) ReadIni(GetPrototyperCfg(Index), "Prototyper Settings", "Game Process Name")

#define I 0
#define FindHandle
#define FindResult

#sub AddPluginEntry
    #define PluginName FindGetFileName(FindHandle)
    #if (PluginName != ".") && (PluginName != "..") && Pos("LoveMachine.Core", PluginName) != 1
        #expr Plugins[I] = PluginName
        #expr I = I + 1
    #endif
#endsub

; Get all plugins from the build via file search
#for {FindHandle = FindResult = FindFirst(PluginBuildDir + "LoveMachine.*", faDirectory); \
    FindResult; \
    FindResult = FindNext(FindHandle)} \
        AddPluginEntry
#if FindHandle
    #expr PrototyperPluginStartIndex = I
    #expr FindClose(FindHandle)
#endif

; Get prototyper plugins
#for {FindHandle = FindResult = FindFirst(PrototyperFilesDir + "*", faDirectory); \
    FindResult; \
    FindResult = FindNext(FindHandle)} \
        AddPluginEntry
#if FindHandle
    #expr PluginCount = I
    #expr FindClose(FindHandle)
#endif

[Setup]
AppName=LoveMachine
AppPublisher=Sauceke
AppPublisherURL=sauceke.github.io
AppVersion={#AppVersion}
DefaultDirName={localappdata}\LoveMachine
DefaultGroupName=LoveMachine
UninstallDisplayIcon={app}\Inno_Setup_Project.exe
Compression=lzma2
SolidCompression=yes
OutputDir=bin
OutputBaseFilename=LoveMachineInstaller
WizardStyle=classic
DisableDirPage=yes
DisableWelcomePage=no
PrivilegesRequired=lowest
SetupLogging=yes

[Languages]
Name: "en"; MessagesFile: "EN.isl,compiler:Default.isl"
Name: "jp"; MessagesFile: "compiler:Languages/Japanese.isl,JP.isl"

[Files]
; BepInEx files
#sub BepInExFileEntry
    Source: "{#BepInEx32Dir}\*"; DestDir: {code:GetDir|{#I}}; \
        Flags: recursesubdirs onlyifdoesntexist; \
        Check: IsDirSelected({#I}) and IsBuildType({#I}, 'x86')
    Source: "{#BepInEx64Dir}\*"; DestDir: {code:GetDir|{#I}}; \
        Flags: recursesubdirs onlyifdoesntexist; \
        Check: IsDirSelected({#I}) and IsBuildType({#I}, 'x64')
    Source: "{#BepInExIl2cpp64Dir}\*"; DestDir: {code:GetDir|{#I}}; \
        Flags: recursesubdirs onlyifdoesntexist; \
        Check: IsDirSelected({#I}) and IsBuildType({#I}, 'il2cpp-x64')
#endsub
#if DirExists(BepInEx32Dir) && DirExists(BepInEx64Dir)
    #for {I = 0; I < PluginCount; I++} BepInExFileEntry
#endif

; LoveMachine files
#sub PluginFileEntry
    Source: "{#PluginBuildDir}{#GetPluginId(I)}\*"; DestDir: {code:GetDir|{#I}}; \
        Flags: recursesubdirs ignoreversion; \
        Check: IsDirSelected({#I})
    Source: "..\{#GetPluginId(I)}\tweaks\*"; DestDir: {code:GetDir|{#I}}; \
        Flags: recursesubdirs ignoreversion skipifsourcedoesntexist onlyifdoesntexist; \
        Check: IsDirSelected({#I})
#endsub
#for {I = 0; I < PrototyperPluginStartIndex; I++} PluginFileEntry

; Prototyper files
# sub PrototyperPluginEntry
    Source: "{#PrototyperFilesDir}{#GetPluginId(I)}\*"; DestDir: "{code:GetDir|{#I}}\BepInEx\config"; \
        Flags: recursesubdirs ignoreversion; \
        Check: IsDirSelected({#I})
    Source: "{#PluginBuildDir}LoveMachinePrototyper\*"; DestDir: "{code:GetDir|{#I}}"; \
        Flags: recursesubdirs ignoreversion; \
        Check: IsDirSelected({#I}) and not IsBuildType({#I}, 'il2cpp-x64')
    Source: "{#PluginBuildDir}LoveMachinePrototyper.IL2CPP\*"; DestDir: "{code:GetDir|{#I}}"; \
        Flags: recursesubdirs ignoreversion; \
        Check: IsDirSelected({#I}) and IsBuildType({#I}, 'il2cpp-x64')
#endsub
#for {I = PrototyperPluginStartIndex; I < PluginCount; I++} PrototyperPluginEntry

[Icons]
Name: "{group}\Inno_Setup_Project"; Filename: "{app}\Inno_Setup_Project.exe"

[Code]
const
    PluginCount = {#PluginCount};
    PrototyperPluginStartIndex = {#PrototyperPluginStartIndex};
    Spacing = 8;
var
    GameDirs: array[0..{#PluginCount - 1}] of String;
    PathEdit: TEdit;
    TitleComboBox: TComboBox;
    PathList: TListBox;

// The ID of the plugin at the given index (e. g. 'LoveMachine.KK')
function GetPluginId(Index: Integer): String;
begin
    case Index of
        #sub IdMapping
            {#I}: Result := '{#GetPluginId(I)}';
        #endsub
        #for {I = 0; I < PluginCount; I++} IdMapping
    end;
end;

function GetGameNameEN(Index: Integer): String;
begin
    case Index of
        #sub EngNameMapping
            {#I}: Result := '{#GetGameNameEN(I)}';
        #endsub
        #for {I = 0; I < PluginCount; I++} EngNameMapping
    end;
end;

function GetGameNameJP(Index: Integer): String;
begin
    case Index of
        #sub JpNameMapping
            {#I}: Result := '{#GetGameNameJP(I)}';
        #endsub
        #for {I = 0; I < PluginCount; I++} JpNameMapping
    end;
end;

// The human-readable name of the game at the given index
function GetGameName(Index: Integer): String;
begin
    if Index >= PrototyperPluginStartIndex then
    begin
        Result := GetPluginId(Index);
        exit;
    end;
    Result := GetGameNameEN(Index);
    if ActiveLanguage = 'jp' then
        Result := GetGameNameJP(Index);
    // this shouldn't happen, but whatever
    if Result = '' then
        Result := GetPluginId(Index);
end;

function GetGameArchitecture(Index: Integer): String;
begin
    case Index of
        #sub ArchitectureMapping
            {#I}: Result := '{#GetGameArchitecture(I)}';
        #endsub
        #for {I = 0; I < PrototyperPluginStartIndex; I++} ArchitectureMapping
        #sub PrototyperArchitectureMapping
            {#I}: Result := '{#GetPrototyperGameArchitecture(I)}';
        #endsub
        #for {I = PrototyperPluginStartIndex; I < PluginCount; I++} PrototyperArchitectureMapping
    end;
end;

function GetExecutableName(Index: Integer): String;
begin
    case Index of
        #sub ExeNameMapping
            {#I}: Result := '{#GetExecutableName(I)}';
        #endsub
        #for {I = 0; I < PrototyperPluginStartIndex; I++} ExeNameMapping
        #sub PrototyperExeNameMapping
            {#I}: Result := '{#GetPrototyperExecutableName(I)}';
        #endsub
        #for {I = PrototyperPluginStartIndex; I < PluginCount; I++} PrototyperExeNameMapping
    end;
end;

// Tries to guess the root directory of the game at the given index
function GuessGamePath(Index: Integer): String;
begin
    case Index of
        #sub PathMapping
            {#I}: RegQueryStringValue(HKCU, '{#GetGameRegSubKey(I)}', '{#GetGameRegName(I)}', Result);
        #endsub
        #for {I = 0; I < PrototyperPluginStartIndex; I++} PathMapping
    end;
    if not DirExists(Result) then
        Result := ''
end;

function GuessPluginIndex(GameDir: String): Integer;
var
    Index: Integer;
begin
    Result := -1;
    for Index := 0 to PluginCount - 1 do
    begin
        if FileExists(AddBackslash(GameDir) + GetExecutableName(Index) + '.exe') then
        begin
            Result := Index;
            break;
        end;
    end;
end;

function GetDir(Index: String): String;
begin
    Result := GameDirs[StrToInt(Index)];
end;

function IsDirSelected(Index: Integer): Boolean;
begin
    Result := GetDir(IntToStr(Index)) <> '';
end;

function IsIntifaceInstalled(): Boolean;
begin
    Result := DirExists(AddBackslash(ExpandConstant('{commonpf32}')) + 'IntifaceCentral')
        or DirExists(AddBackslash(ExpandConstant('{userappdata}')) + 'IntifaceCentral');
end;

function IsBuildType(Index: Integer; Architecture: String): Boolean;
begin
    Result := (GetGameArchitecture(Index) = Architecture);
end;

function GetPreviousDataKey(Index: Integer): String;
begin
    Result := 'GameDir.' + GetPluginId(Index);
end;

procedure Warn(Message: String; Interactive: Boolean);
begin
    if Interactive then
        MsgBox(Message, mbError, MB_OK);
end;

function Ask(Message: String; Interactive: Boolean): Boolean;
begin
    Result := Interactive and (MsgBox(Message, mbConfirmation, MB_YESNO) = IDYES);
end;

function ValidateGameDir(Path: String; Interactive: Boolean): Boolean;
var
    FindRec: TFindRec;
begin
    Result := FindFirst(AddBackslash(Path) + '*_Data', FindRec);
    if not Result then
        Warn(FmtMessage(CustomMessage('NotAGameDir'), [Path]), Interactive);
    FindClose(FindRec);
end;

procedure RemoveGameDir(GameDir: String);
var
    Index: Integer;
begin
    for Index := 0 to PluginCount - 1 do
    begin
        if GameDirs[Index] = GameDir then
            GameDirs[Index] := '';
    end;
    PathList.Items.Delete(PathList.Items.IndexOf(GameDir));
end;

function AddGameDir(GameDir: String; PluginIndex: Integer; Interactive: Boolean): Boolean;
var
    Index: Integer;
begin
    Result := False;
    if not ValidateGameDir(GameDir, Interactive) then
        exit;
    if PluginIndex < 0 then
    begin
        Warn(CustomMessage('MissingTitle'), Interactive);
        exit;
    end;
    if GameDirs[PluginIndex] <> '' then
    begin
        if Ask(FmtMessage(CustomMessage('ConflictingPaths'), [GetGameName(PluginIndex)]), Interactive) then
            RemoveGameDir(GameDirs[PluginIndex])
        else
            exit;
    end;
    for Index := 0 to PluginCount - 1 do
    begin
        if GameDirs[Index] <> GameDir then
            continue;
        if Ask(FmtMessage(CustomMessage('ConflictingTitles'), [GetGameName(Index)]), Interactive) then
            RemoveGameDir(GameDirs[Index])
        else
            exit;
    end;
    GameDirs[PluginIndex] := GameDir;
    PathList.Items.Add(GameDir);
    Result := True;
end;

procedure PopulateGameListPage;
var
    Index: Integer;
    GameDir: String;
begin
    for Index := 0 to PluginCount - 1 do
    begin
        TitleComboBox.Items.Add(GetGameName(Index));
        GameDir := GetPreviousData(GetPreviousDataKey(Index), GuessGamePath(Index));
        AddGameDir(GameDir, Index, False);
    end;
end;

procedure OnPathChanged(Sender: TObject);
begin
    TitleComboBox.ItemIndex := GuessPluginIndex(PathEdit.Text);
end;

procedure OnBrowseClick(Sender: TObject);
var
    Path: String;
begin
    Path := PathEdit.Text;
    if Path = '' then
        Path := ExpandConstant('{sd}');
    if BrowseForFolder(SetupMessage(msgBrowseDialogLabel), Path, False) then
    begin
        ValidateGameDir(Path, True); // allow bad path for easier correction
        PathEdit.Text := Path;
        OnPathChanged(Sender);
    end;
end;

procedure OnAddClick(Sender: TObject);
begin
    if AddGameDir(PathEdit.Text, TitleComboBox.ItemIndex, True) then
    begin
        PathEdit.Text := '';
        TitleComboBox.ItemIndex := -1;
    end;
end;

procedure OnRemoveClick(Sender: TObject);
begin
    if PathList.ItemIndex >= 0 then
        RemoveGameDir(PathList.Items[PathList.ItemIndex]);
end;

function OnGameListPageNextButtonClick(Sender: TWizardPage): Boolean;
begin
    if PathList.Items.Count = 0 then
    begin
        Warn(CustomMessage('EmptyGameList'), True);
        Result := False;
        exit;
    end;
    if (PathEdit.Text <> '') and not Ask(CustomMessage('GamePending'), True) then
    begin
        Result := False;
        exit;
    end;
    Result := True;
end;

procedure AddGameListPage;
var
    GameListPage: TWizardPage;
    BrowseBtn: TButton;
    AddBtn: TButton;
    RemoveBtn: TButton;
    AddLabel: TLabel;
    ListLabel: TLabel;
begin
    GameListPage := CreateCustomPage(wpSelectDir,
        CustomMessage('GameListTitle'),
        CustomMessage('GameListDesc'));
    GameListPage.OnNextButtonClick := @OnGameListPageNextButtonClick;
    PathEdit := TEdit.Create(WizardForm);
    PathEdit.Parent := GameListPage.Surface;
    PathEdit.Left := 0;
    PathEdit.Top := 0;
    PathEdit.OnChange := @OnPathChanged;
    BrowseBtn := TButton.Create(WizardForm);
    BrowseBtn.Parent := GameListPage.Surface;
    BrowseBtn.Left := GameListPage.Surface.Width - BrowseBtn.Width;
    BrowseBtn.Top := 0;
    BrowseBtn.Height := PathEdit.Height;
    BrowseBtn.Caption := SetupMessage(msgButtonBrowse);
    BrowseBtn.OnClick := @OnBrowseClick;
    PathEdit.Width := BrowseBtn.Left - PathEdit.Left - Spacing;
    TitleComboBox := TComboBox.Create(WizardForm);
    TitleComboBox.Parent := GameListPage.Surface;
    TitleComboBox.Left := 0;
    TitleComboBox.Top := PathEdit.Height + Spacing;
    TitleComboBox.Width := GameListPage.Surface.Width;
    TitleComboBox.Text := CustomMessage('TitlePlaceholder');
    AddBtn := TButton.Create(WizardForm);
    AddBtn.Parent := GameListPage.Surface;
    AddBtn.Left := 0;
    AddBtn.Top := TitleComboBox.Top + TitleComboBox.Height + Spacing;
    AddBtn.Height := BrowseBtn.Height;
    AddBtn.Caption := CustomMessage('AddBtn');
    AddBtn.OnClick := @OnAddClick;
    ListLabel := TLabel.Create(WizardForm);
    ListLabel.Parent := GameListPage.Surface;
    ListLabel.Left := 0;
    ListLabel.Top := AddBtn.Top + AddBtn.Height + Spacing;
    ListLabel.Caption := CustomMessage('PathListLabel');
    PathList := TListBox.Create(WizardForm);
    PathList.Parent := GameListPage.Surface;
    PathList.Left := 0;
    PathList.Top := ListLabel.Top + ListLabel.Height;
    PathList.Width := GameListPage.Surface.Width;
    PathList.Height := GameListPage.Surface.Height - BrowseBtn.Height - PathList.Top - Spacing;
    PathList.MultiSelect := False;
    RemoveBtn := TButton.Create(WizardForm);
    RemoveBtn.Parent := GameListPage.Surface;
    RemoveBtn.Left := 0;
    RemoveBtn.Top := PathList.Top + PathList.Height + spacing;
    RemoveBtn.Height := BrowseBtn.Height;
    RemoveBtn.Caption := CustomMessage('RemoveBtn');
    RemoveBtn.OnClick := @OnRemoveClick;
end;

procedure CheckIntiface;
var
    ErrorCode: Integer;
begin
    if IsIntifaceInstalled() then
        exit;
    if not Ask(CustomMessage('InstallIntiface'), True) then
        exit;
    if not ShellExec('open', 'https://intiface.com/central/', '', '', SW_SHOW, ewNoWait, ErrorCode) then
        Warn(SysErrorMessage(ErrorCode), True);
end;

procedure InitializeWizard;
begin
    CheckIntiface;
    AddGameListPage;
    PopulateGameListPage;
end;

procedure RegisterPreviousData(PreviousDataKey: Integer);
var
    Index: Integer;
begin
    for Index := 0 to PluginCount - 1 do
    begin
        if DirExists(GameDirs[Index]) then
            SetPreviousData(PreviousDataKey, GetPreviousDataKey(Index), GameDirs[Index]);
    end;
end;
