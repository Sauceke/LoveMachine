#define PluginBuildDir SourcePath + "..\bin\"
#define BepInEx32Dir SourcePath + "BepInEx32"
#define BepInEx64Dir SourcePath + "BepInEx64"
#define AppVersion GetVersionNumbersString(PluginBuildDir + "LoveMachine.Core\LoveMachine.Core.dll")

#define PluginInfoIni SourcePath + "\PluginInfo.ini"
#define PluginInfoIniNameKey "Name"
#define PluginInfoIniRegSubKeyKey "RegSubKey"
#define PluginInfoIniRegNameKey "RegName"
#define PluginInfoIniArchitectureKey "Architecture"

; We have a lot of plugins, so we just find them all and put them in here
; This way the script will handle new plugins by itself and we can forget about it
#dim Plugins[100]
#define PluginCount 0

#define GetPluginId(Index) Plugins[Index]
#define GetGameName(Index) ReadIni(PluginInfoIni, GetPluginId(Index), PluginInfoIniNameKey, GetPluginId(Index))
#define GetGameRegSubKey(Index) ReadIni(PluginInfoIni, GetPluginId(Index), PluginInfoIniRegSubKeyKey)
#define GetGameRegName(Index) ReadIni(PluginInfoIni, GetPluginId(Index), PluginInfoIniRegNameKey)
#define GetGameArchitecture(Index) ReadIni(PluginInfoIni, GetPluginId(Index), PluginInfoIniArchitectureKey)

#define I 0
#sub AddGameEntry
    #define PluginName FindGetFileName(FindHandle)
    #if PluginName != "LoveMachine.Core"
        #expr Plugins[I] = PluginName
        #expr I = I + 1
    #endif
#endsub

; Get all plugins from the build via file search
#define FindHandle
#define FindResult
#for {FindHandle = FindResult = FindFirst(PluginBuildDir + "LoveMachine*", faDirectory); \
    FindResult; \
    FindResult = FindNext(FindHandle)} \
        AddGameEntry
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
WizardStyle=modern
DisableDirPage=yes
DisableWelcomePage=no
InfoBeforeFile=Readme.rtf

[Files]
; BepInEx files
#sub BepInExFileEntry
    Source: "{#BepInEx32Dir}\*"; DestDir: {code:GetDir|{#I}}; Flags: recursesubdirs; Check: ShouldInstallBepInEx({#I}, 'x86')
    Source: "{#BepInEx64Dir}\*"; DestDir: {code:GetDir|{#I}}; Flags: recursesubdirs; Check: ShouldInstallBepInEx({#I}, 'x64')
#endsub
#if DirExists(BepInEx32Dir) && DirExists(BepInEx64Dir)
    #for {I = 0; I < PluginCount; I++} BepInExFileEntry
#endif

; LoveMachine files
#sub PluginFileEntry
    Source: "{#PluginBuildDir}{#GetPluginId(I)}\*"; DestDir: {code:GetDir|{#I}}; Flags: recursesubdirs ignoreversion; Check: IsDirSelected({#I})
#endsub
#for {I = 0; I < PluginCount; I++} PluginFileEntry

[Icons]
Name: "{group}\Inno_Setup_Project"; Filename: "{app}\Inno_Setup_Project.exe"

[Code]
const
    PageSize = 4;
    PluginCount = {#PluginCount};
var
    { The directory prompts don't fit all in one page, so we need more pages }
    { This is way too many pages but whatever }
    DirPages: array[0..{#PluginCount}] of TInputDirWizardPage;
    Old_WizardForm_NextButton_OnClick: TNotifyEvent;
    PlaceholderDir: String;

{ The ID of the plugin at the given index (e. g. 'LoveMachine.KK') }
function GetPluginId(Index: Integer): String;
begin
    case Index of
        #sub IdMapping
            {#I}: Result := '{#GetPluginId(I)}';
        #endsub
        #for {I = 0; I < PluginCount; I++} IdMapping
    end;
end;

{ The human-readable name of the game at the given index }
function GetGameName(Index: Integer): String;
begin
    case Index of
        #sub NameMapping
            {#I}: Result := '{#GetGameName(I)}';
        #endsub
        #for {I = 0; I < PluginCount; I++} NameMapping
    end;
end;

function GetGameArchitecture(Index: Integer): String;
begin
    case Index of
        #sub ArchitectureMapping
            {#I}: Result := '{#GetGameArchitecture(I)}';
        #endsub
        #for {I = 0; I < PluginCount; I++} ArchitectureMapping
    end;
end;

{ Tries to guess the root directory of the game at the given index }
function GuessGamePath(Index: Integer): String;
begin
    case Index of
        #sub PathMapping
            {#I}: RegQueryStringValue(HKCU, '{#GetGameRegSubKey(I)}', '{#GetGameRegName(I)}', Result);
        #endsub
        #for {I = 0; I < PluginCount; I++} PathMapping
    end;
    if not DirExists(Result) then
        Result := ''
end;

{ Tells us where on which page the install dir box for the given index is located }
procedure GetPageAndIndex(Index: Integer; out Page: Integer; out IndexInPage: Integer);
begin
    Page := Index / PageSize;
    IndexInPage := Index mod PageSize;
end;

function GetDir(Index: String): String;
var
    Page: Integer;
    IndexInPage: Integer;
begin
    GetPageAndIndex(StrToInt(Index), Page, IndexInPage);
    Result := DirPages[Page].Values[IndexInPage];
end;

function IsDirSelected(Index: Integer): Boolean;
begin
    Result := GetDir(IntToStr(Index)) <> '';
end;

function ShouldInstallBepInEx(Index: Integer; Architecture: String): Boolean;
var
    BepInExConfigDir: String;
begin
    BepInExConfigDir := AddBackslash(GetDir(IntToStr(Index))) + 'BepInEx\config';
    Result := (not DirExists(BepInExConfigDir)) and (GetGameArchitecture(Index) = Architecture);
end;

function GetPreviousDataKey(Index: Integer): String;
begin
    Result := 'GameDir.' + GetPluginId(Index);
end;

function ValidateGameDir(Path: String): Boolean;
var
    FindRec: TFindRec;
    WarningMsg: String;
begin
    Result := True;
    if (not FindFirst(AddBackslash(Path) + '*_Data', FindRec)) and (Path <> PlaceholderDir) then
    begin
        WarningMsg := Format('Path %s does not appear to be a valid game directory.', [Path]);
        MsgBox(WarningMsg, mbError, MB_OK);
        Result := False;
    end;
end;

function OnDirPageNextClick(Page: TWizardPage): Boolean;
var
    DirPage: TInputDirWizardPage;
    IndexInPage: Integer;
begin
    Result := True;
    DirPage := Page as TInputDirWizardPage;
    try
        for IndexInPage := 0 to PageSize - 1 do
        begin
            if not ValidateGameDir(DirPage.Values[IndexInPage]) then
            begin
                Result := False;
                break;
            end;
        end;
    except
        { there is no way to get the length of TInputDirWizardPage.Values }
        { so just go for an out of bounds error and fucking swallow it }
    end;
end;

procedure AddDirPrompts;
var
    Index: Integer;
    Page: Integer;
    IndexInPage: Integer;
    PrevPageID: Integer;
begin
    for Index := 0 to PluginCount - 1 do
    begin
        GetPageAndIndex(Index, Page, IndexInPage);
        if Page = 0 then
            PrevPageID := wpSelectDir
        else
            PrevPageID := DirPages[Page - 1].ID;
        if IndexInPage = 0 then
            DirPages[Page] := CreateInputDirPage(PrevPageID,
                'Select Destinations - Page ' + IntToStr(Page + 1),
                'Select the game folder for each of your games. '
                    + 'Leave blank for games you don''t have.',
                '', False, '');
        DirPages[Page].Add(GetGameName(Index));
        DirPages[Page].Values[IndexInPage] :=
            GetPreviousData(GetPreviousDataKey(Index), GuessGamePath(Index));
        DirPages[Page].OnNextButtonClick := @OnDirPageNextClick;
    end;
end;

{ based on https://stackoverflow.com/a/31706698 }
procedure New_WizardForm_NextButton_OnClick(Sender: TObject);
var
    Index: Integer;
    Page: Integer;
    IndexInPage: Integer;
begin
    for Index := 0 to PluginCount - 1 do
    begin
        GetPageAndIndex(Index, Page, IndexInPage);
        if DirPages[Page].Values[IndexInPage] = '' then
            { Force value to pass validation }
            DirPages[Page].Values[IndexInPage] := PlaceholderDir;
    end;
    Old_WizardForm_NextButton_OnClick(Sender);
    for Index := 0 to PluginCount - 1 do
    begin
        GetPageAndIndex(Index, Page, IndexInPage);
        if DirPages[Page].Values[IndexInPage] = PlaceholderDir then
            DirPages[Page].Values[IndexInPage] := '';
    end;
end;

procedure CheckIntiface;
var
    ErrorCode: Integer;
begin
    if not DirExists(AddBackslash(ExpandConstant('{localappdata}')) + 'IntifaceDesktop') then
        if MsgBox('LoveMachine requires Intiface to be installed. Install it now?', mbConfirmation, MB_YESNO) = IDYES then
            if not ShellExec('open', 'https://intiface.com/desktop/', '', '', SW_SHOW, ewNoWait, ErrorCode) then
                MsgBox(SysErrorMessage(ErrorCode), mbError, MB_OK);
end;

procedure InitializeWizard;
begin
    PlaceholderDir := AddBackslash(ExpandConstant('{sd}')) + '$Recycle.Bin';
    CheckIntiface;
    AddDirPrompts;
    Old_WizardForm_NextButton_OnClick := WizardForm.NextButton.OnClick;
    WizardForm.NextButton.OnClick := @New_WizardForm_NextButton_OnClick;
end;

procedure RegisterPreviousData(PreviousDataKey: Integer);
var
    Index: Integer;
    Page: Integer;
    IndexInPage: Integer;
    DirPath: String;
begin
    for Index := 0 to PluginCount - 1 do
    begin
        GetPageAndIndex(Index, Page, IndexInPage);
        DirPath := DirPages[Page].Values[IndexInPage];
        if DirExists(DirPath) then
            SetPreviousData(PreviousDataKey, GetPreviousDataKey(Index), DirPath); 
    end;
end;
