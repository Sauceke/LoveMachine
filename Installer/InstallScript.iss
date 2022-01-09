#define PluginBuildDir SourcePath + "..\bin\"
#define AppVersion GetVersionNumbersString(PluginBuildDir + "LoveMachine.Core\LoveMachine.Core.dll")

#define PluginInfoIni SourcePath + "\PluginInfo.ini"
#define PluginInfoIniNameKey "Name"
#define PluginInfoIniRegSubKeyKey "RegSubKey"
#define PluginInfoIniRegNameKey "RegName"
#define PluginInfoIniArchitectureKey "RegName"

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
SetupLogging=yes

[Files]
#sub FileEntry
    Source: "{#PluginBuildDir}{#GetPluginId(I)}\*"; DestDir: {code:GetDir|{#I}}; Flags: recursesubdirs; Check: IsDirSelected({#I})
#endsub
#for {I = 0; I < PluginCount; I++} FileEntry

[Icons]
Name: "{group}\Inno_Setup_Project"; Filename: "{app}\Inno_Setup_Project.exe"

[Code]
#define PageSize 4
const
    PageSize = {#PageSize};
    PluginCount = {#PluginCount};
var
    { The directory prompts don't fit all in one page, so we need more pages }
    { This is way too many pages but whatever }
    DirPages: array[0..{#PluginCount}] of TInputDirWizardPage;
    DownloadPage: TDownloadWizardPage;
    Old_WizardForm_NextButton_OnClick: TNotifyEvent;

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

{ Tries to guess the root directory of the game at the given index }
function GuessGamePath(Index: Integer): String;
var
    GameExe: TFindRec;
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

function GetPreviousDataKey(Index: Integer): String;
begin
    Result := 'GameDir.' + GetPluginId(Index);
end;

procedure AddDirPrompts();
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
            DirPages[Page].Values[IndexInPage] := WizardDirValue;
    end;
    Old_WizardForm_NextButton_OnClick(Sender);
    for Index := 0 to PluginCount - 1 do
    begin
        GetPageAndIndex(Index, Page, IndexInPage);
        if DirPages[Page].Values[IndexInPage] = WizardDirValue then
            DirPages[Page].Values[IndexInPage] := '';
    end;
end;

procedure InitializeWizard;
begin
    AddDirPrompts();
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
