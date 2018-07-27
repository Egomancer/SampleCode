#define xADEF_ADEG_DBG

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Steamworks;
using Galaxy.Api;
using ADEF.ESD;
using ADEF.Log;


namespace ADEF {
	
	namespace ADEG {

		/// <summary>
		/// Game SteamWorks Demo
		/// </summary>
		public class ADEG : MonoBehaviour {
			public static float def_tickTimeMAXDefault = 0.100f;
			public static float def_clientTimePingMAXDefault = 0.500f;
			public static float def_clientTimeTimeoutMAXDefault = 2.000f;

			public static bool dbg_gameActionTimeoutActive = false;	// [DEFAULT] true
			public static bool dbg_commTimeoutActive = false;	// [DEFAULT] true


			private static ADEG instance_;
			public static ADEG instance {
				get {
					return instance_;
				}
			}

			public ADE_Platform platformMain;
			public ADE_Platform platformNetworking;
			public GameObject objADEG;
			public ADE ade;

			// app's name
			public const string def_ADEG_Name = "ADEG";
			// app's version (major/minor/build/revision)
			public const string def_ADEG_Version = "0.5.07.204";

			public ADEG_Project project;

			/// <summary>
			/// [SET] no human player, autocreates/autoconnects on MP if true, false otherwise
			/// </summary>
			public bool def_botMode;
			/// <summary>
			/// [SET] the bot client client tries to autocreate a MP lobby in the lobbies list if true, false otherwise
			/// </summary>
			public bool def_botModeLobbyAutoCreate;
			/// <summary>
			/// [SET][DBG] force P2P even for local communication
			/// </summary>
			public static bool def_DBG_LocalToP2PComm = false;	// [DEFAULT] false	// [!!!][004] set on true doesn't seem to work with GoG networking
			/// <summary>
			/// [SET] if true then a variable is locally set before receiving server's confirmation, false otherwise
			/// </summary>
			public bool def_clientUnconfirmedLocalEffect;

			public GameMenuState stateMenu;
			public GameMenuState stateMenuPrev;
			public GameMenuMPState stateMenuMP;

			/// <summary>
			/// [SET] the client tries to autojoin any present MP lobby in the lobbies list if true, false otherwise
			/// </summary>
			public bool def_lobbyAutoJoin;	// [DEFAULT] false
			/// <summary>
			/// [SET] if there are no lobbies to join the client tries to autocreate a MP lobby in the lobbies list if true, false otherwise
			/// </summary>
			public bool def_lobbyAutoCreate;	// [DEFAULT] false

			public Game_User gameUser;
			[NonSerialized]
			public Game_Manager gameManager_;
			public virtual Game_Manager gameManager {
				get {
					return gameManager_;
				}
			}
			[NonSerialized]
			public Game_Client gameClient_;
			public Game_Client gameClient {
				get {
					return gameClient_;
				}
			}
			public Game_Lobby gameLobby;
			[NonSerialized]
			public Game_Server gameServer_;
			public Game_Server gameServer {
				get {
					return gameServer_;
				}
			}
			[NonSerialized]
			public Game_Society gameSociety_;
			public virtual Game_Society gameSociety {
				get {
					return gameSociety_;
				}
			}

			/// <summary>
			/// [SET] shows version if true, false otherwise
			/// </summary>
			public bool def_labelVersion;
			public string labelVersion;
			private float labelVersionWidth_;

			public bool panelGameShow;
			public bool panelGameDetailsShow;

			public ADE_Platform inputPlatformMain;
			public ADE_Platform inputPlatformNetworking;
			public string inputPasswordCreate;
			public string inputGameName;
			public int inputGameMap;
			public string inputPasswordJoin;
			public GamePlayerState inputPlayerState;
			public int inputPlayerNumber;
			private string lobbyMessage_;

			public int panelOffset_;
			public int panelWidth_;
			public int panelHeight_;

			private Vector2 scrollFriendsPos_;
			private Vector2 scrollGroupsPos_;

			public Rect rectAux0;
			public Rect rectAux1;
			public Rect rectAux2;

			public virtual void DBG_Log() {
				ALog.LogJournal("<-- ADEG log -->");
				ALog.LogJournal("v0.0.01.01 -> added: the main ADEG class and initialized the ADE class");
				ALog.LogJournal("v0.0.01.02 -> added: the menu code and tthe ADEG_State enum");
				ALog.LogJournal("v0.0.02.03 -> added: the OnGUI_SinglePlayer() method");
				ALog.LogJournal("v0.0.03.04 -> added: the OnGUI_MultiPlayerLobbyList(), OnGUI_MultiPlayerLobbyList_ButtonLobbyJoin() and OnGUI_MultiPlayerLobbyList_ButtonLobbyCreate() methods");
				ALog.LogJournal("v0.0.04.05 -> added: the OnGUI_MultiPlayerLobby() method");
				ALog.LogJournal("v0.0.05.06 -> added: the OnGUI_MultiPlayer() method");
				ALog.LogJournal("v0.0.05.13 -> added: isolated all the display code sections from the ADE into the new ADE_DBG class");
				ALog.LogJournal("v0.0.06.14 -> added:"
					+ "\n   - modified the GameLoop() multiplayer code section to not reset the start 1 lobby data as soon as the lobby's owner client entered the multiplayer game"
					+ "\n   - modified the OnGUI_MultiPlayerLobby() code enabling the access to lobby's name and lobby's map controls for lobby's owner client only"
					+ "\n   - modified the OnGUI_MultiPlayerLobby() code removing the Back menu button (as the Leave button was already offering the same functionality)"
					+ "\n   - modified the OnGUI_MultiPlayerLobbyList(), OnGUI_MultiPlayerLobby() and OnGUI_Multi() methods' back/leave buttons code section to deactivate the activated ADE used classes"
				);
				ALog.LogJournal("v0.0.07.15 -> added:"
					+ "\n   - added the new Game_Player class and the new ADEG_PlayerState enum"
					+ "\n   - added the new Game_Lobby class"
					+ "\n   - added the new ADEG_DataType enum"
				);
				ALog.LogJournal("v0.0.08.16 -> added:"
					+ "\n   - added the new GameLobbyCreated() and GameLobbyJoined() delagates"
					+ "\n   - added the new GameLobbyDataInit() and GameLobbyDataGet() delegates"
					+ "\n   - added the new GameClientJoined() and GameClientLeft() delegates"
					+ "\n   - added the new GameClientDataInit() and GameClientDataGet() delegates"
					+ "\n   - added the new GameClientP2PDataReceived() delegate"
				);
				ALog.LogJournal("v0.0.09.17 -> added:"
					+ "\n   - moved the ADE_Client .state, .rand and .pos members into the Game_Player class"
				);
				ALog.LogJournal("v0.0.10.19 -> added:"
					+ "\n    - the new def_lobbyAutoJoin and all its related code"
				);
				ALog.LogJournal("v0.0.11.25 -> added:"
					+ "\n   - split the old ADEG_State enum into the new GameState and GameMPState enumns"
					+ "\n   - renamed the old ADEG_PlayerState enum into the new GamePlayerState enum"
					+ "\n   - renamed the old ADEG_DataType enum into the new GameDataType enum"
					+ "\n   - added the new def_standalone member"
					+ "\n   - added the new panelGameShow member"
					+ "\n   - added the new OnGUI_Menu_ButtonMultiplayer() method"
					+ "\n   - added the new ADEG_PROJECT define"
				);
				ALog.LogJournal("v0.0.12.31 -> added:"
					+ "\n   - modified the GCR .player and .players members into public"
					+ "\n   - modified the GameDataType enum adding the new DropshipDeploy member"
					+ "\n   - modified the .GameLoop() method GameMPState.Game code section to only transmit the initial token's position in the standalone mode"
					+ "\n   - generalized the GameClientP2PDataSend() method for the standalone mode"
					+ "\n   - added a new .GameClientP2PDataSend() method overload for the integrated mode"
					+ "\n   - modified the .GameClientP2PDataReceived() method for both the standalone and intergrated mode"
				);
				ALog.LogJournal("v0.1.00.54 -> added:"
					+ "\n   +++ project's whole code base overhaul (additions, modifications, streamlining and optimizations) (1)"
				);
				ALog.LogJournal("v0.2.03.69 -> added:"
					+ "\n   +++ project's whole code base overhaul (additions, modifications, streamlining and optimizations) (2)"
				);
				ALog.LogJournal("v0.2.03.71 -> added:"
					+ "\n   + project's whole code base overhaul (additions, modifications, streamlining and optimizations) (3)"
				);
				ALog.LogJournal("v0.2.03.72 -> added:"
					+ "\n   + project's whole code base overhaul (additions, modifications, streamlining and optimizations) (4)"
				);
				ALog.LogJournal("v0.2.03.73 -> added:"
					+ "\n   + project's whole code base overhaul (additions, modifications, streamlining and optimizations) (5)"
				);
				ALog.LogJournal("v0.2.03.74 -> added:"
					+ "\n   + project's whole code base overhaul (additions, modifications, streamlining and optimizations) (6)"
				);
				ALog.LogJournal("v0.2.03.75 -> added:"
					+ "\n   ++ project's whole code base overhaul (additions, modifications, streamlining and optimizations) (7)"
				);
				ALog.LogJournal("v0.2.03.76 -> added:"
					+ "\n   ++ project's whole code base overhaul (additions, modifications, streamlining and optimizations) (8)"
				);
				ALog.LogJournal("v0.3.00.93 -> added:"
					+ "\n   +++ decoupling Demo and GCR projects' code from ADEG class (1)"
				);
				ALog.LogJournal("v0.3.00.94 -> added:"
					+ "\n   + decoupling Demo and GCR projects' code from ADEG class (2)"
				);
				ALog.LogJournal("v0.3.01.95 -> added:"
					+ "\n   ++ decoupling Demo and GCR projects' code from ADEG class (3)"
				);
				ALog.LogJournal("v0.3.02.98 -> added:"
					+ "\n   ++ game stop code"
					+ "\n   ++ game reset code"
				);
				ALog.LogJournal("0.3.03.99 -> added:"
					+ "\n   + parameterized game load and game stop code"
				);
				ALog.LogJournal("0.3.04.101 -> added:"
					+ "\n   + queue to list loby messages code (1)"
				);
				ALog.LogJournal("0.3.05.104 -> added:"
					+ "\n   - gameplay mode game load parameter code"
				);
				ALog.LogJournal("0.4.00.117 -> added:"
					+ "\n   +++ decoupling Demo and GCR projects' code from ADEG class (4)"
					+ "\n   +++ decoupling Demo and GCR projects game managers' code from Game_Manager class (1)"
					+ "\n   +++ decoupling Demo and GCR projects game server' code from Game_Server class (1)"
					+ "\n   +++ decoupling Demo and GCR projects game clients' code from Game_Client class (1)"
					+ "\n   +++ adding Demo and GCR projects custom commands' code"
				);
				ALog.LogJournal("0.4.01.118 -> added:"
					+ "\n   ++ decoupling Demo and GCR projects' code from ADEG class (5)"
				);
				ALog.LogJournal("0.4.02.121 -> added:"
					+ "\n   ++ decoupling Demo and GCR projects' code from ADEG class (6)"
					+ "\n   +++ adding Demo and GCR projects extended commands' code"
					+ "\n   ++ extending the local P2P forcing debug code"
				);
				ALog.LogJournal("0.4.03.132 -> added:"
					+ "\n   +++ adding Demo and GCR projects' data commands and actions code"
				);
				ALog.LogJournal("0.5.00.164 -> added:"
					+ "\n   +++ ADEG class (digital distribution platform wrapper code)"
				);
				ALog.LogJournal("0.5.01.168 -> added:"
					+ "\n   ++ Steam and GoG crossplatform networking"
				);
				ALog.LogJournal("0.5.01.169 -> added:"
					+ "\n   - the ADE_Platform.Local related code"
					+ "\n   - deactiv ated the not used yet .Update(), .Update_(), .OnGUI() and .OnGUI_() empty methods"
				);
				ALog.LogJournal("0.5.02.172 -> added:"
					+ "\n   ++ the time sync code (1)"
				);
				ALog.LogJournal("0.5.02.173 -> added:"
					+ "\n   - modified the GameCommandType enum adding the .PlayerKickRequest, .PlayerKickResponse and .PlayerKick members"
					+ "\n   - modified the Game_Client.CommandNew() method adding code sections for the new GameCommandType members"
					+ "\n   - added the new PlayerKickRequest, PlayerKickResponse and PlayerKick classes"
					+ "\n   - modified the ADEG.OnGUI_MultiPlayerLobby_ButtonPlayerKick() and Game_Manager.PlayerKick() methods"
				);
				ALog.LogJournal("0.5.02.174 -> added:"
					+ "\n   - modified the Game_Manager.PlayerKick() into an IEnumerator"
					+ "\n   ++ modified the OnGUI_Menu_ButtonMultiplayer() method adding a networking platform and signed in check"
				);
				ALog.LogJournal("0.5.02.175 -> added:"
					+ "\n   - renamed the old .def_DBG_LocalToP2P into the new .def_DBG_LocalToP2PComm and added all the new SP game start/stop communication initialization/deinitialization code"
					+ "\n   - added the new .def_DBG_LocalToP2PLobby and added all the new SP game start/stop lobby initialization/deinitialization code"
					+ "\n   - modified the .OnApplicationQuit() method extending the call of Game_Manager.GameMPLeave() to all valid MP game states"
				);
				ALog.LogJournal("0.5.03.176 -> added:"
					+ "\n   - modified the Awake_() method setting the application to also run in background"
					+ "\n   - modified the OnGUI_MultiPlayerLobbyList() method changing the currently creating/joining a loby check from ADE_LList.lobbySelectedID's validity to ADE_LList.requestLobbyCreateRunning and ADE_LList.requestLobbyJoinRunning"
					+ "\n   - moved all ADE_LList.lobbySelectedID.Reset() calls into ADE_LList.ActiveSet() method"
				);
				ALog.LogJournal("0.5.04.177 -> added:"
					+ "\n   ++ networking communication error handling (1)"
				);
				ALog.LogJournal("0.5.05.186 -> added:"
					+ "\n   ++ private lobbies code"
					+ "\n   +++ (WIP) friends code (1)"
					+ "\n   + (WIP) app command line arguments code (1)"
				);
				ALog.LogJournal("0.5.05.187 -> added:"
					+ "\n   - modified the Awake_() method removing the setting the application to run in background call"
					+ "\n   - modified the OnApplicationQuit() method adding error handling code (game manager null test)"
				);
				ALog.LogJournal("0.5.06.188 -> added:"
					+ "\n   ++ the new ADEF.Debug class usage update (1)"
					+ "\n   - modified the Game_Command.GameStopRequest.Execute() and Game_Command.GameEndRequest.Execute() methods to stop the primary client last"
					+ "\n   - modified the Game_Manager.Game_Command.GameStop() method to also reset the .inputPasswordCreate and inputPasswordJoin variables"
				);
				ALog.LogJournal("0.5.06.191 -> added:"
					+ "\n   - the new .inputPlatformMain and .inputPlatformNetworking members"
					+ "\n   - modified the .Awake_() method to also initialize the newly added .inputPlatformMain and .inputPlatformNetworking members"
					+ "\n   - modified the GameMenuState enum adding the new Settings member"
					+ "\n   - added the new .OnGUI_Settings() method"
				);
				ALog.LogJournal("0.5.06.192 -> added:"
					+ "\n   - modified the .def_lobbyAutoJoin and .def_lobbyAutoCreate  members defaults initializations with false"
					+ "\n   - modified the .OnGUI_Settings() method to clarify that the newly modified platform main and networking values will only be loaded after next application's start (1)"
					+ "\n   - added the new .stateMenuPrev member and modified the .OnGUI_Friends() .and OnGUI_Settings() methods's back button to restore the previous menu state"
				);
				ALog.LogJournal("0.5.06.195 -> added:"
					+ "\n   - Game_Server new tickID related members and code"
					+ "\n   - Game_Server new .ExecuteCustomResponseSuccess() and ExecuteCustomResponseFailures() methods"
					+ "\n   + (WIP) lobby client left code (1)"
					+ "\n   ++ (WIP) keepalive (client/server ping request/response) code (1)"
				);
				ALog.LogJournal("0.5.06.196 -> added:"
					+ "\n   - modified the .OnGUI_Settings() method to clarify that the newly modified platform main and networking values will only be loaded after next application's start (2)"
				);
				ALog.LogJournal("0.5.07.201 -> added:"
					+ "\n   ++ communication timeout/keepalive code (GameManager.FixedUpdate_() and GameServer.FixedUpdate_() methods"
					+ "\n   ++ game action timeout code (GameManager.ActionsExecute() and Action.Execute() methods)"
					+ "\n      + modified the Game_Action class'es old timestamp parameter into the new timeGame parameter and added the new tickTime parameter"
					+ "\n   ++ modified the game synchronization code to prevent a problem resulting in delayed game start time for the other then the server clients"
					+ "\n      - modified the GameStartRequest.Execute() method updating the game state to the correct GameState.Ready one"
					+ "\n      - modified the GameStartResponse.Execute() method updating the server's game time to also consider the GameStart command travel time"
					+ "\n      - modified the GameStart class to contain the game start time as a parameter"
					+ "\n   - the new GameServer .timeLobbyReal and .timeGameReal parameters"
				);
				ALog.LogJournal("0.5.07.202 -> added:"
					+ "\n   - modified the PlayerLoginResponse.Execute() method's GCR hardcoded code section"
				);
				ALog.LogJournal("0.5.07.203 -> added:"
					+ "\n   - modified the Awake_() method's adding x32/x64 mode detection code"
				);
				ALog.LogJournal("0.5.07.206 -> added:"
					+ "\n   - (WIP) new ADEG code modifications (Razvan) (1)"
				);
				ALog.LogJournal("0.5.07.207 -> added:"
					+ "\n   - updated all ADE_Lobby.MessageAdd() and ADE_Lobby.MessageSend() method calls to the newly added formats"
					+ "\n   - modified the Game_Command:PlayerLeaveRequest.Execute() and Game_Command:PlayerKickRequest.Execute() methods making message's player identification text consistent"
					+ "\n   - modified the Game_Command:PlayerLoginRequest.Execute() method sending a server player joining the game message to all players"
				);
			}

		//	void Awake() {
		//		ALog.LogSys("   ===>   ADEG.Awake()");
		//		Awake_();
		//	}

			public void Awake_<TGame_Manager, TGame_Server, TGame_Client, TGame_Society> (ADE_Platform pPlatformMain, ADE_Platform pPlatformNetworking) where TGame_Manager : Game_Manager where TGame_Server : Game_Server where TGame_Client : Game_Client where TGame_Society : Game_Society {
				ALog.LogSys("   ===>   ADEG.Awake_()");
				instance_ = this;

				platformMain = inputPlatformMain = pPlatformMain;
				platformNetworking = inputPlatformNetworking = pPlatformNetworking;

				objADEG = GameObject.Find(def_ADEG_Name);
				if (objADEG != null) {
					DontDestroyOnLoad(objADEG);

					ade = objADEG.GetComponent<ADE>();
					if (ade == null) {
						ade = objADEG.AddComponent<ADE>();
						ade.Awake_(platformMain, platformNetworking);
					}
					ADE.def_ADE_Lobby_AutoJoin = false;
					ADE.def_ADE_Lobby_AutoCreate = false;
					ADE_Lobby.def_messagesNoMAX = 4;

					gameUser = new Game_User(ADE.clientLocal);
					gameManager_ = objADEG.AddComponent<TGame_Manager>();
					gameServer_ = objADEG.AddComponent<TGame_Server>();
					gameClient_ = objADEG.AddComponent<TGame_Client>();
					gameLobby = new Game_Lobby();
					gameSociety_ = objADEG.AddComponent<TGame_Society>();

					ADE.lList.LobbyCreateResponseCustom = gameServer.GameLobbyCreated;
					ADE.lList.LobbyJoinResponseCustom = gameLobby.GameLobbyJoined;
					ADE.lobby.DataInitCustom = gameLobby.GameLobbyDataInit;
					ADE.lobby.DataGetCustom = gameLobby.GameLobbyDataGet;
					ADE.lobby.ClientJoinedCustom = gameClient.LobbyClientJoined;
					ADE.lobby.ClientLeftCustom = gameClient.LobbyClientLeft;
					ADE.lobby.ClientDataInitCustom = gameClient.LobbyClientDataInit;
					ADE.lobby.ClientDataGetCustom = gameClient.LobbyClientDataGet;
					ADE.comm.P2PDataSendFailureCustom = gameClient.GameClientP2PDataSendFailure;
					ADE.comm.P2PDataReceivedCustom = gameClient.GameClientP2PDataReceived;
					ADE.comm.P2PDataReceivedSuccessCustom = gameClient.GameClientP2PDataReceivedSuccess;
					ADE.comm.P2PDataReceivedFailureCustom = gameClient.GameClientP2PDataReceivedFailure;
				}

				//DBG_Log();

				def_labelVersion = true;
				labelVersion = "ADEG v" + def_ADEG_Version;

				def_botMode = false;
				def_botModeLobbyAutoCreate = true;
				def_clientUnconfirmedLocalEffect = true;

				stateMenu = GameMenuState.MainMenu;
				ALogger.show = true;

				def_lobbyAutoJoin = ADE.def_ADE_Lobby_AutoJoin;
				def_lobbyAutoCreate = ADE.def_ADE_Lobby_AutoCreate;

				panelGameShow = true;
				panelGameDetailsShow = true;

				lobbyMessage_ = "";

				rectAux0 = new Rect();
				rectAux1 = new Rect();
				rectAux2 = new Rect();

			// x32/x64 app
				ALog.LogDebug("[INIT] - IntPtr.Size: " + IntPtr.Size + " -> " + ((IntPtr.Size == 8)?"x64":"x32") + " app");

			// command line arguments
				var v_args = (string[]) System.Environment.GetCommandLineArgs();
				ALog.LogDebug("[ARG] - args.Length: " + v_args.Length);
				for (int i=0; i<v_args.Length; i++) {
					ALog.LogDebug("[ARG] - args[" + i + "]: " + v_args[i]);
					if (v_args[i].Equals("+connect_lobby")) {
						var v_lobbyID = (ulong) 0;
						UInt64.TryParse(v_args[i +1], out v_lobbyID);
					}
				}
			}

			/* // [NUY]
			void Update() {
				Update_();
			}

			void OnGUI() {
				OnGUI_();
			}

			public void Update_() {
				//
			}

			public void OnGUI_() {
				//
			}
			*/

			public void OnApplicationQuit() {
				ALog.LogWarning("ADEG.OnApplicationQuit() call", DBGC.DBG);

				if (gameManager != null) {
					if ((gameManager.gameMode == GameMode.MultiPlayer) && (gameManager.gameState >= GameState.Lobby)) {
						StartCoroutine(gameManager.GameMPLeave());
					}
					else if ((gameManager.gameMode == GameMode.SinglePlayer) && ADE.def_DBG_LocalToP2PLobby) {
						// [!!!][004] - create a SP true lobby
					}
				}
			}


			public void OnGUI_Version() {
				if (labelVersionWidth_ == 0f) {
					var v_widthMin = (float) 0f;
					GUI.skin.GetStyle("label").CalcMinMaxWidth(new GUIContent(labelVersion), out v_widthMin, out labelVersionWidth_);
					labelVersionWidth_ += panelOffset_;
				}
				rectAux1.Set(rectAux0.width -labelVersionWidth_, rectAux0.height -19, labelVersionWidth_, 18);
				GUI.Label(rectAux1, labelVersion);
			}

			public void OnGUI_Menu_ButtonSingleplayer (int pLevelID) {
				inputGameName = "ADEG Demo SP Game";
				inputGameMap = UnityEngine.Random.Range(0, 10000);

				StartCoroutine(gameManager.GameSPStart(pLevelID));
			}

			private IEnumerator OnGUI_SinglePlayer_ButtonGameStop() {
				yield return StartCoroutine(gameManager.GameStop());
			}

			public void OnGUI_Menu_ButtonMultiplayer() {
				if (ade.connected) {
					ADE.lList.ActiveSet(true);

					stateMenu = GameMenuState.MultiPlayer;

					inputGameName = (((!ADE.def_ADE_Lobby_AutoJoin && !ADE.def_ADE_Lobby_AutoCreate) ? "" : "(a) ") + ((ADE.clientLocal != null) ? ADE.clientLocal.name + " " : "") + ADE.def_ADE_Lobby_Name);
					inputGameMap = UnityEngine.Random.Range(0, 10000);

					//if (!ADE.def_ADE_Lobby_AutoJoin && !ADE.def_ADE_Lobby_AutoCreate) {
					if (!def_lobbyAutoJoin) {
						gameUser.state = GamePlayerState.LookingForGame;
						stateMenuMP = GameMenuMPState.LobbyList;
					}
					else {
						if (!gameLobby.connecting) {
							if (ADE.lList.lobbiesNo > 0) {
								if (def_lobbyAutoJoin) {
									ADE.lList.lobbySelectedID = ADE.lList.LobbyValidFirstGet();
									if (ADE.lList.lobbySelectedID.valid) {
										StartCoroutine(OnGUI_MultiPlayerLobbyList_ButtonLobbyJoin(ADE.lList.lobbySelectedID));
									}
								}
							}
							else {
								if ((!def_botMode && def_lobbyAutoCreate) || (def_botMode && def_botModeLobbyAutoCreate)) {
									StartCoroutine(OnGUI_MultiPlayerLobbyList_ButtonLobbyCreate(GameplayMode.Deathmatch));
								}
							}
						}
					}
				}
			}

			public void OnGUI_MultiPlayerLobbyList() {
				rectAux1.Set(panelWidth_ +2*panelOffset_, panelOffset_, 2*panelWidth_ +panelOffset_, 2*panelHeight_+panelOffset_);
				GUILayout.BeginArea(rectAux1, GameMenuState.MultiPlayer.ToString() + " - " + GameMenuMPState.LobbyList.ToString(), GUI.skin.window);
				{
					rectAux2.Set(panelOffset_, 2*panelOffset_, rectAux1.width -2*panelOffset_, rectAux1.height - 3*panelOffset_);
					GUILayout.BeginArea(rectAux2, GUI.skin.box);
					{
						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							var v_lobbyID = (ADE_ID) null;
							GUILayout.Label("lobbies no: " + ADE.lList.lobbiesNo);
							for (int i=0; i<ADE.lList.lobbiesNo; i++) {
								v_lobbyID = (ADE_ID) ADE.lList.LobbyIDGet(i);
								if (v_lobbyID.valid && ADE.lList.LobbyDataGet(v_lobbyID, "ready").Equals("1")) {
									GUILayout.BeginHorizontal(GUI.skin.box);
									{
										if (GUILayout.Button(v_lobbyID.ID.ToString(), GUILayout.Width(200))) {
											ADE.lList.lobbySelectedID = v_lobbyID;
										}
										GUILayout.Space(panelOffset_);
										GUILayout.Label("server: " + (ADE.lList.LobbyDataGet(v_lobbyID, "locked").Equals("1") ? "+":"") + ADE.lList.LobbyDataGet(v_lobbyID, "serverID"), GUILayout.Width(200));
										GUILayout.Label("name: " + ADE.lList.LobbyDataGet(v_lobbyID, "name"), GUILayout.Width(200));
										GUILayout.Label("map: #" + ADE.lList.LobbyDataGet(v_lobbyID, "map"), GUILayout.Width(100));
										GUILayout.Label("clients: " + ADE.lList.LobbyClientsNoGet(v_lobbyID), GUILayout.Width(100));
									}
									GUILayout.EndHorizontal();
								}
							}
						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						if (!gameLobby.connecting) {
							GUILayout.BeginHorizontal(GUI.skin.box);
							{
								GUI.enabled = ((ADE.lList.lobbySelectedID.valid) && (!ADE.lList.LobbyDataGet(ADE.lList.lobbySelectedID, "locked").Equals("1") || !string.IsNullOrEmpty(inputPasswordJoin)));
								if (GUILayout.Button("Join", GUILayout.Width(120))) {
									StartCoroutine(OnGUI_MultiPlayerLobbyList_ButtonLobbyJoin(ADE.lList.lobbySelectedID, inputPasswordJoin));
								}
								GUI.enabled = true;
								if (ADE.lList.lobbySelectedID.valid) {
									GUILayout.Label(ADE.lList.LobbyDataGet(ADE.lList.lobbySelectedID, "name"));
									if (ADE.lList.LobbyDataGet(ADE.lList.lobbySelectedID, "locked").Equals("1")) {
										if (GUILayout.Button("password:", GUILayout.Width(80))) {
											inputPasswordJoin = "";
										}
										inputPasswordJoin = GUILayout.TextField(inputPasswordJoin, GUILayout.Width(120));
									}
								}
								else {
									GUILayout.Label("select a lobby to join");
								}

								GUILayout.FlexibleSpace();
							}
							GUILayout.EndHorizontal();

							GUILayout.BeginHorizontal(GUI.skin.box);
							{
								if (GUILayout.Button("Create", GUILayout.Width(120))) {
									StartCoroutine(OnGUI_MultiPlayerLobbyList_ButtonLobbyCreate(GameplayMode.Deathmatch, inputPasswordCreate));
								}
								if (GUILayout.Button("name:", GUILayout.Width(80))) {
									inputGameName = (((!ADE.def_ADE_Lobby_AutoJoin && !ADE.def_ADE_Lobby_AutoCreate) ? "" : "(a) ") + ADE.clientLocal.name + " " + ADE.def_ADE_Lobby_Name);
								}
								inputGameName = GUILayout.TextField(inputGameName, GUILayout.Width(200));
								if (GUILayout.Button("map: #", GUILayout.Width(80))) {
									inputGameMap = UnityEngine.Random.Range(0, 10000);
								}
								int.TryParse(GUILayout.TextField(inputGameMap.ToString("0000"), GUILayout.Width(80)), out inputGameMap);
								if (GUILayout.Button("password:", GUILayout.Width(80))) {
									inputPasswordCreate = "";
								}
								inputPasswordCreate = GUILayout.TextField(inputPasswordCreate, GUILayout.Width(120));
							}
							GUILayout.EndHorizontal();
						}
						else {
							GUILayout.Label((ADE.lList.requestLobbyCreateRunning) ? "Creating the <" + gameLobby.gameName + "> lobby.." : (ADE.lList.requestLobbyJoinRunning) ? "Joining the <" + ADE.lList.LobbyDataGet(ADE.lList.lobbySelectedID, "name") + "> lobby.." : "Waiting for the <" + (!string.IsNullOrEmpty(gameLobby.gameName) ? gameLobby.gameName : ADE.lList.LobbyDataGet(ADE.lobby.aID, "name")) + "> lobby's connection..");
						}

						GUILayout.Space(panelOffset_);

						GUI.enabled = (!gameLobby.connecting);
						if (GUILayout.Button("Back", GUILayout.Width(120))) {
							StartCoroutine(OnGUI_MultiPlayerLobbyList_ButtonLobbyListLeave());
						}
						GUI.enabled = true;
					}
					GUILayout.EndArea();
				}
				GUILayout.EndArea();
			}

			public IEnumerator OnGUI_MultiPlayerLobbyList_ButtonLobbyCreate (GameplayMode pGameplayMode, string pPassword = "") {
				gameLobby.connecting = true;

				yield return StartCoroutine(gameManager.GameMPCreate(pGameplayMode, pPassword));

				gameLobby.connecting = false;
			}

			public IEnumerator OnGUI_MultiPlayerLobbyList_ButtonLobbyJoin (ADE_ID pLobbyID, string pPassword = "") {
				gameLobby.connecting = true;

				yield return StartCoroutine(gameManager.GameMPJoin(pLobbyID, pPassword));

				gameLobby.connecting = false;
			}

			public IEnumerator OnGUI_MultiPlayerLobbyList_ButtonLobbyListLeave() {
				ADE.lList.ActiveSet(false);
				stateMenu = GameMenuState.MainMenu;
				stateMenuMP = GameMenuMPState.LobbyList;

				yield break;
			}

			public void OnGUI_MultiPlayerLobby() {
				rectAux1.Set(panelWidth_ +2*panelOffset_, panelOffset_, 2*panelWidth_ +panelOffset_, 2*panelHeight_+panelOffset_);
				GUILayout.BeginArea(rectAux1, GameMenuState.MultiPlayer.ToString() + " - " + GameMenuMPState.Lobby.ToString(), GUI.skin.window);
				{
					rectAux2.Set(panelOffset_, 2*panelOffset_, rectAux1.width -2*panelOffset_, rectAux1.height - 3*panelOffset_);
					GUILayout.BeginArea(rectAux2, GUI.skin.box);
					{
						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.Label("name: " + gameLobby.gameName + " (" + gameLobby.lobby.ID.ToString() + ")");
							GUILayout.Label("map: #" + gameLobby.gameMap);
							GUILayout.Label("owner: " + ((gameLobby.lobby.clientOwner != null) ? gameLobby.lobby.clientOwner.name + " (" + gameLobby.lobby.clientOwner.ID.ToString() + ")" : ""));
						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginHorizontal(GUI.skin.box);
						{
							GUILayout.Label("name: " + gameManager.gameName);
							GUILayout.Label("map: #" + gameManager.gameMap);
							if (gameLobby.lobby.ClientCheckIfOwnerLocal()) {
								if (GUILayout.Button("password:", GUILayout.Width(80))) {
									if (!inputPasswordCreate.Equals(gameLobby.password, StringComparison.InvariantCultureIgnoreCase)) {
										if (!string.IsNullOrEmpty(gameLobby.password) && string.IsNullOrEmpty(inputPasswordCreate)) {
											// password reset
											gameLobby.password = "";
										}
										else if (string.IsNullOrEmpty(gameLobby.password) && !string.IsNullOrEmpty(inputPasswordCreate)) {
											// password set
											gameLobby.password = inputPasswordCreate;
										}
										else if (!string.IsNullOrEmpty(inputPasswordCreate)) {
											// password change
											gameLobby.password = inputPasswordCreate;
										}
										ALog.LogDebug("[CSCOMM][0800] - Client->Server  ->  GameCommandType.ServerPasswordUpdateRequest");
										gameClient.CommandClientToServerSend(gameClient.clientServer, new Game_Command.ServerPasswordUpdateRequest(-1, 0, Time.time, gameUser.client, gameLobby.password));
									}
								}
								inputPasswordCreate = GUILayout.TextField(inputPasswordCreate, GUILayout.Width(120));
							}
						}
						GUILayout.EndHorizontal();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.Label("players: " + gameManager.players.Count + " (clients: " + gameLobby.lobby.clients.Count + ")");
							for (int i=0; i<gameManager.players.Count; i++) {
								GUILayout.BeginHorizontal(GUI.skin.box);
								{
									GUILayout.Label("Client ID: " + gameManager.players[i].client.ID, GUILayout.Width(200));
									GUILayout.Space(panelOffset_);
									GUILayout.Label("name: " + gameManager.players[i].name, GUILayout.Width(200));
									if (ADE.ClientCheckIfLocal(gameManager.players[i].client)) {
										GUILayout.Label("state: ", GUILayout.Width(50));
										if (GUILayout.Button(gameManager.players[i].state.ToString(), GUILayout.Width(120))) {
											inputPlayerState = ((gameManager.players[i].state == GamePlayerState.LobbySpectating) ? GamePlayerState.LobbySeated : (gameManager.players[i].state == GamePlayerState.LobbySeated) ? GamePlayerState.LobbyReady : GamePlayerState.LobbySpectating);
											gameManager.players[i].state = (def_clientUnconfirmedLocalEffect ? inputPlayerState : gameManager.players[i].state);
											ALog.LogDebug("[CSCOMM][0400] - Client->Server  ->  GameCommandType.PlayerStateUpdateRequest");
											gameClient.CommandClientToServerSend(gameClient.clientServer, new Game_Command.PlayerStateUpdateRequest(-1, ++gameClient.commandID, Time.time, gameUser.client, gameManager.players[i].ID, inputPlayerState));
										}
										GUILayout.Space(40);
									}
									else {
										GUILayout.Label("state: " + gameManager.players[i].state.ToString(), GUILayout.Width(210));
									}
									if (ADE.ClientCheckIfLocal(gameManager.players[i].client)) {
										if (GUILayout.Button("number: ", GUILayout.Width(80))) {
											inputPlayerNumber = UnityEngine.Random.Range(1, 101);
										}
										int.TryParse(GUILayout.TextField(((inputPlayerNumber != gameManager.players[i].number)?inputPlayerNumber:gameManager.players[i].number).ToString("00"), GUILayout.Width(50)), out inputPlayerNumber);
										if (inputPlayerNumber != gameManager.players[i].number) {
											gameManager.players[i].number = (def_clientUnconfirmedLocalEffect ? inputPlayerNumber : gameManager.players[i].number);
											ALog.LogDebug("[CSCOMM][0500] - Client->Server  ->  GameCommandType.PlayerDataUpdateRequest");
											gameClient.CommandClientToServerSend(gameClient.clientServer, new Game_Command.PlayerDataUpdateRequest(-1, ++gameClient.commandID, Time.time, gameUser.client, gameManager.players[i].ID, inputPlayerNumber));
										}
										GUILayout.Space(70);
									}
									else {
										GUILayout.Label("number: " + gameManager.players[i].number.ToString("00"), GUILayout.Width(80));
									}
									if (gameClient.CheckIfAdmin() && gameLobby.lobby.ClientCheckIfOwnerLocal()) {
										GUI.enabled = (gameManager.player != gameManager.players[i]);
										if (GUILayout.Button("kick", GUILayout.Width(50))) {
											StartCoroutine(OnGUI_MultiPlayerLobby_ButtonPlayerKick(gameManager.players[i]));
										}
										GUI.enabled = true;
									}
								}
								GUILayout.EndHorizontal();
							}
						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(5*panelOffset_));
							{
								for (int i=Math.Max(0, (ADE.lobby.logMessages.Count -ADE_Lobby.def_messagesToDisplayNo)); i<ADE.lobby.logMessages.Count; i++) {
									GUILayout.Label(ADE.lobby.logMessages[i]);
								}
							}
							GUILayout.EndVertical();

							GUILayout.Space(panelOffset_);

							GUILayout.BeginHorizontal();
							lobbyMessage_ = GUILayout.TextField(lobbyMessage_, 205, GUILayout.Width(200));
							GUI.enabled = (!string.IsNullOrEmpty(lobbyMessage_));
							if (GUILayout.Button("Send Message", GUILayout.Width(100))) {
								if (ADE.lobby.MessageSend(lobbyMessage_, ADE_LobbyMessageType.Message)) {
									lobbyMessage_ = "";
								}
							}
							GUI.enabled = true;
							GUILayout.EndHorizontal();

						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginHorizontal(GUI.skin.box);
						{
							GUI.enabled = gameManager.GameMPStartReady();
							if (GUILayout.Button("Start", GUILayout.Width(120))) {
								StartCoroutine(OnGUI_MultiPlayerLobby_ButtonGameStart(1));
							}
							GUI.enabled = true;
							GUI.enabled = ADE.lobby.ClientCheckIfOwnerLocal();
							if (GUILayout.Button("name: ", GUILayout.Width(120))) {
								inputGameName = (((!ADE.def_ADE_Lobby_AutoJoin && !ADE.def_ADE_Lobby_AutoCreate) ? "" : "(a) ") + ADE.clientLocal.name + " " + ADE.def_ADE_Lobby_Name);
							}
							GUI.enabled = true;
							GUI.enabled = ADE.lobby.ClientCheckIfOwnerLocal();
							inputGameName = GUILayout.TextField(((GUI.enabled && (inputGameName != gameManager.gameName))?inputGameName:gameManager.gameName), GUILayout.Width(120));
							if (GUI.enabled && (inputGameName != gameManager.gameName)) {
								//gameManager.gameName = inputGameName;
								ALog.LogDebug("[CSCOMM][0300] - Client->Server  ->  GameCommandType.GameDataUpdateRequest");
								gameClient.CommandClientToServerSend(gameClient.clientServer, new Game_Command.GameDataUpdateRequest(-1, ++gameClient.commandID, Time.time, gameUser.client, inputGameName, inputGameMap, new byte[0]));
							}
							GUI.enabled = true;
							GUI.enabled = ADE.lobby.ClientCheckIfOwnerLocal();
							if (GUILayout.Button("map: #", GUILayout.Width(120))) {
								inputGameMap = UnityEngine.Random.Range(0, 10000);
							}
							GUI.enabled = true;
							GUI.enabled = ADE.lobby.ClientCheckIfOwnerLocal();
							int.TryParse(GUILayout.TextField(((GUI.enabled && (inputGameMap != gameManager.gameMap))?inputGameMap:gameManager.gameMap).ToString("0000"), GUILayout.Width(120)), out inputGameMap);
							if (GUI.enabled && (inputGameMap != gameManager.gameMap)) {
								//gameManager.gameMap = inputGameMap;
								ALog.LogDebug("[CSCOMM][0300] - Client->Server  ->  GameCommandType.GameDataUpdateRequest");
								gameClient.CommandClientToServerSend(gameClient.clientServer, new Game_Command.GameDataUpdateRequest(-1, ++gameClient.commandID, Time.time, gameUser.client, inputGameName, inputGameMap, new byte[0]));
							}
							GUI.enabled = true;
						}
						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal(GUI.skin.box);
						{
							GUI.enabled = (gameManager.gameState == GameState.Lobby);
							if (GUILayout.Button("Leave", GUILayout.Width(120))) {
								StartCoroutine(OnGUI_MultiPlayerLobby_ButtonLobbyLeave());
							}
							GUI.enabled = true;
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndArea();
				}
				GUILayout.EndArea();
			}

			public IEnumerator OnGUI_MultiPlayerLobby_ButtonGameStart (int pLevelID) {
				yield return StartCoroutine(gameManager.GameMPStart(pLevelID));
			}

			public IEnumerator OnGUI_MultiPlayerLobby_ButtonLobbyLeave() {
				yield return StartCoroutine(gameManager.GameMPLeave());
			}

			public IEnumerator OnGUI_MultiPlayerLobby_ButtonPlayerKick (Game_Player pPlayer) {
				yield return StartCoroutine(gameManager.PlayerKick(pPlayer));
			}

			public void OnGUI_MultiPlayer() {
				rectAux1.Set(panelWidth_ +2*panelOffset_, panelOffset_, 2*panelWidth_ +panelOffset_, 2*panelHeight_+panelOffset_);
				GUILayout.BeginArea(rectAux1, GameMenuState.MultiPlayer.ToString() + " - " + GameMenuMPState.Game.ToString(), GUI.skin.window);
				{
					rectAux2.Set(panelOffset_, 2*panelOffset_, rectAux1.width -2*panelOffset_, rectAux1.height - 3*panelOffset_);
					GUILayout.BeginArea(rectAux2, GUI.skin.box);
					{
						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.Label("name: " + gameLobby.gameName + " (" + gameLobby.lobby.ID.ToString() + ")");
							GUILayout.Label("map: #" + gameLobby.gameMap);
							GUILayout.Label("owner: " + ((gameLobby.lobby.clientOwner != null) ? gameLobby.lobby.clientOwner.name + " (" + gameLobby.lobby.clientOwner.ID.ToString() + ")" : ""));
						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginHorizontal(GUI.skin.box);
						{
							GUILayout.Label("name: " + gameManager.gameName);
							GUILayout.Label("map: #" + gameManager.gameMap);
						}
						GUILayout.EndHorizontal();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.Label("players: " + gameManager.players.Count + " (clients: " + gameLobby.lobby.clients.Count + ")");
							for (int i=0; i<gameManager.players.Count; i++) {
								GUILayout.BeginHorizontal(GUI.skin.box);
								{
									GUILayout.Label("Client ID: " + gameManager.players[i].client.ID, GUILayout.Width(200));
									GUILayout.Space(panelOffset_);
									GUILayout.Label("name: " + gameManager.players[i].name, GUILayout.Width(200));
									GUILayout.Label("state: " + gameManager.players[i].state.ToString(), GUILayout.Width(200));
									GUILayout.Label("number: " + gameManager.players[i].number.ToString("00"), GUILayout.Width(200));
								}
								GUILayout.EndHorizontal();
							}
						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(5*panelOffset_));
							{
								for (int i=Math.Max(0, (ADE.lobby.logMessages.Count -ADE_Lobby.def_messagesToDisplayNo)); i<ADE.lobby.logMessages.Count; i++) {
									GUILayout.Label(ADE.lobby.logMessages[i]);
								}
							}
							GUILayout.EndVertical();

							GUILayout.Space(panelOffset_);

							GUILayout.BeginHorizontal();
							lobbyMessage_ = GUILayout.TextField(lobbyMessage_, 205, GUILayout.Width(200));
							GUI.enabled = (!string.IsNullOrEmpty(lobbyMessage_));
							if (GUILayout.Button("Send Message", GUILayout.Width(100))) {
								if (ADE.lobby.MessageSend(lobbyMessage_, ADE_LobbyMessageType.Message)) {
									lobbyMessage_ = "";
								}
							}
							GUI.enabled = true;
							GUILayout.EndHorizontal();

						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginHorizontal(GUI.skin.box);
						{
							GUI.enabled = ADE.lobby.ClientCheckIfOwnerLocal() && ((gameManager.gameState == GameState.Lobby) || (gameManager.gameState == GameState.Playing));
							if (GUILayout.Button("Stop", GUILayout.Width(120))) {
								StartCoroutine(OnGUI_MultiPlayer_ButtonGameStop());
							}
							GUI.enabled = true;
							GUI.enabled = ((gameManager.gameState == GameState.Lobby) || (gameManager.gameState == GameState.Playing));
							if (GUILayout.Button("Leave", GUILayout.Width(120))) {
								StartCoroutine(OnGUI_MultiPlayer_ButtonGameLeave());
							}
							GUI.enabled = true;
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndArea();
				}
				GUILayout.EndArea();
			}

			private IEnumerator OnGUI_MultiPlayer_ButtonGameLeave() {
				yield return StartCoroutine(gameManager.GameMPLeave());
			}

			private IEnumerator OnGUI_MultiPlayer_ButtonGameStop() {
				yield return StartCoroutine(gameManager.GameMPLeave());
			}

			public void OnGUI_Friends() {
				rectAux1.Set(panelWidth_ +2*panelOffset_, panelOffset_, 2*panelWidth_ +panelOffset_, 2*panelHeight_+panelOffset_);
				GUILayout.BeginArea(rectAux1, "Friends", GUI.skin.window);
				{
					rectAux2.Set(panelOffset_, 2*panelOffset_, rectAux1.width -2*panelOffset_, rectAux1.height - 3*panelOffset_);
					GUILayout.BeginArea(rectAux2, GUI.skin.box);
					{
						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.Label("Self");

							GUILayout.Space(panelOffset_);

							GUILayout.BeginHorizontal(GUI.skin.box);
							{
								GUILayout.Label("State: " + gameSociety.SocietyPersonSelfGetState().ToString());
								GUI.enabled = (gameSociety.SocietyPersonSelfGetState() == GamePersonState.Offline);
								if (GUILayout.Button("Login", GUILayout.Width(120))) {
									SteamFriends.ActivateGameOverlayToWebPage("http://www.google.com");
									SteamFriends.ActivateGameOverlay("Friends");
								}
								GUI.enabled = true;
								if (gameSociety.SocietyPersonSelfGetState() == GamePersonState.Offline) {
									GUILayout.Label("Please switch to Steam and login to Steam community to be able to see your friends");
								}
							}
							GUILayout.EndHorizontal();
						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.Label("Game Client");
							GUILayout.Space(panelOffset_);
						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.Label("Friends");
							GUILayout.Space(panelOffset_);

							if (gameSociety.SocietyPersonSelfGetState() != GamePersonState.Offline) {
								scrollFriendsPos_ = GUILayout.BeginScrollView(scrollFriendsPos_, GUI.skin.box);
								{
									var v_text = (string) null;
									for (int i=0; i<gameSociety.persons.Count; i++) {
										v_text = "Friend[" + i +"]  ->  name: " + gameSociety.persons[i].client.name + ", state: " + gameSociety.persons[i].state.ToString();
										if (gameSociety.persons[i].state == GamePersonState.Online) {
											if (gameSociety.persons[i].gameID != 0) {
												v_text += ", gameID: " + gameSociety.persons[i].gameID;
											}
											if (gameSociety.persons[i].lobbyID != 0) {
												v_text += ", lobbyID: " + gameSociety.persons[i].lobbyID;
											}
										}
										if (gameSociety.persons[i].state == GamePersonState.Online) {
											GUILayout.BeginHorizontal();
											{
												GUILayout.Label(v_text);
												if ((gameSociety.persons[i].gameID != 0) && (gameSociety.persons[i].lobbyID != 0)) {
													if (GUILayout.Button("Join", GUILayout.Width(120))) {
													}
												}
												GUILayout.FlexibleSpace();
											}
											GUILayout.EndHorizontal();
										}
									}
								}
								GUILayout.EndScrollView();

								scrollGroupsPos_ = GUILayout.BeginScrollView(scrollGroupsPos_, GUI.skin.box, GUILayout.Height(100));
								{
									for (int i=0; i<gameSociety.groups.Count; i++) {
										if (gameSociety.groups[i].groupId != -1) {
											GUILayout.Label("Group[" + i + "]  ->  groupId: " + gameSociety.groups[i].groupId + ", name: " + gameSociety.groups[i].name + ", members no: " + gameSociety.groups[i].personsNo);
										}
									}
								}
								GUILayout.EndScrollView();
							}
							else {
								GUILayout.Label("Not logged in to Steam Community");
							}
						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						if (GUILayout.Button("Back", GUILayout.Width(190))) {
							stateMenu = stateMenuPrev;
						}
						GUI.enabled = true;
					}
					GUILayout.EndArea();
				}
				GUILayout.EndArea();
			}

			public void OnGUI_Settings() {
				rectAux1.Set(panelWidth_ +2*panelOffset_, panelOffset_, 2*panelWidth_ +panelOffset_, 2*panelHeight_+panelOffset_);
				GUILayout.BeginArea(rectAux1, "Settings", GUI.skin.window);
				{
					rectAux2.Set(panelOffset_, 2*panelOffset_, rectAux1.width -2*panelOffset_, rectAux1.height - 3*panelOffset_);
					GUILayout.BeginArea(rectAux2, GUI.skin.box);
					{
						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(500));
						{						
							GUILayout.BeginHorizontal(GUI.skin.box);
							{
								GUILayout.FlexibleSpace();

								if (GUILayout.Button("PlatformMain: " + inputPlatformMain.ToString(), GUILayout.Width(190))) {
									inputPlatformMain = inputPlatformNetworking = (ADE_Platform) inputPlatformMain.NextDefaultSkip<ADE_Platform>();
									PlayerPrefs.SetInt("def_platformMain", (int)inputPlatformMain);
									PlayerPrefs.SetInt("def_platformNetworking", (int)inputPlatformMain);
								}

								GUILayout.FlexibleSpace();

								GUI.enabled = (inputPlatformMain == ADE_Platform.Steam);
								if (GUILayout.Button("PlatformNetworking: " + inputPlatformNetworking.ToString(), GUILayout.Width(190))) {
									inputPlatformNetworking = (ADE_Platform) ((inputPlatformNetworking == ADE_Platform.Steam) ? ADE_Platform.GoG : ADE_Platform.Steam);
									PlayerPrefs.SetInt("def_platformNetworking", (int)inputPlatformNetworking);
								}
								GUI.enabled = true;

								GUILayout.FlexibleSpace();
							}
							GUILayout.EndHorizontal();

							if ((inputPlatformMain != platformMain) || (inputPlatformNetworking != platformNetworking)) {
								GUILayout.BeginHorizontal(GUI.skin.box);
								{
									GUILayout.FlexibleSpace();
									GUILayout.Label("(Please restart the app to load the new platform settings)");
									GUILayout.FlexibleSpace();
								}
								GUILayout.EndHorizontal();
							}
						}
						GUILayout.EndVertical();

						GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(500));
						{
							GUILayout.FlexibleSpace();

							GUILayout.BeginVertical(GUI.skin.box);
							{
								if (GUILayout.Button("Auto Join: " + (def_lobbyAutoJoin ? "On" : "Off"), GUILayout.Width(190))) {
									def_lobbyAutoJoin = !def_lobbyAutoJoin;
								}
								if (GUILayout.Button("Bot Mode: " + (def_botMode ? "On" : "Off"), GUILayout.Width(190))) {
									def_botMode = !def_botMode;
								}
								if (GUILayout.Button("ALogger: " + (ALogger.show ? "On" : "Off"), GUILayout.Width(190))) {
									ALogger.show = !ALogger.show;
								}
							}
							GUILayout.EndVertical();

							GUILayout.FlexibleSpace();

							GUILayout.BeginVertical(GUI.skin.box);
							{
								if (GUILayout.Button("DBG GameActionTimeoutActive: " + (dbg_gameActionTimeoutActive ? "On" : "Off"), GUILayout.Width(230))) {
									dbg_gameActionTimeoutActive = !dbg_gameActionTimeoutActive;
								}
								if (GUILayout.Button("DBG CommTimeoutActive: " + (dbg_commTimeoutActive ? "On" : "Off"), GUILayout.Width(230))) {
									dbg_commTimeoutActive = !dbg_commTimeoutActive;
								}
							}
							GUILayout.EndVertical();

							GUILayout.FlexibleSpace();
						}
						GUILayout.EndHorizontal();

						GUILayout.Space(panelOffset_);

						if (GUILayout.Button("Back", GUILayout.Width(190))) {
							stateMenu = stateMenuPrev;
						}
						GUI.enabled = true;
					}
					GUILayout.EndArea();
				}
				GUILayout.EndArea();
			}

			public void OnGUI_GameMaster() {
				rectAux1.Set(panelWidth_ +2*panelOffset_, panelOffset_, 2*panelWidth_ +panelOffset_, 2*panelHeight_+panelOffset_);
				GUILayout.BeginArea(rectAux1, "Game Master - Client " + gameClient.clientType + (((platformMain == ADE_Platform.Local) && (platformNetworking == ADE_Platform.Local)) ? " [L]" : ((platformMain == ADE_Platform.Steam) && (platformNetworking == ADE_Platform.Steam)) ? " [S]" : ((platformMain== ADE_Platform.GoG) && (platformNetworking == ADE_Platform.GoG)) ? " [G]" : ((platformMain == ADE_Platform.Steam) && (platformNetworking == ADE_Platform.GoG)) ? " [SG]" : " [?!!]"), GUI.skin.window);
				{
					rectAux2.Set(panelOffset_, 2*panelOffset_, rectAux1.width -2*panelOffset_, rectAux1.height - 3*panelOffset_);
					GUILayout.BeginArea(rectAux2, GUI.skin.box);
					{
						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.Label("Game Manager");
							GUILayout.Space(panelOffset_);
							switch (gameManager.gameState) {
								case GameState.Active:
								case GameState.Lobby:
								case GameState.Loading:
								case GameState.Loaded:
								case GameState.Initializing:
								case GameState.Initialized:
								case GameState.Ready:
								case GameState.Playing: {
									GUILayout.BeginHorizontal();
									{
										GUILayout.Label("state: " + gameManager.gameState);
										GUILayout.Label("mode: " + gameManager.gameMode);
										GUILayout.Label("gpMode: " + gameManager.gameGPMode);
										GUILayout.Label("name: " + gameManager.gameName);
										GUILayout.Label("map: " + gameManager.gameMap);
									}
									GUILayout.EndHorizontal();
									var v_string = (string) "";
									for (int i=0; i<gameManager.players.Count; i++) {
										v_string += (((i==0) ? "" : ", ") + ((gameManager.players[i] == gameManager.player) ? "+" : "") + gameManager.players[i].ToString());
									}
									GUILayout.Label("players[" + gameManager.players.Count + "]: " + v_string);
									GUILayout.BeginHorizontal();
									{
										GUILayout.Label("timeLobby: " + gameManager.timeLobby.ToString("F2"));
										GUILayout.Label("timeGame: " + gameManager.timeGame.ToString("F2"));
										GUILayout.Label("tickID: " + gameManager.tickID);
										GUILayout.Label("tickTime: " + gameManager.tickTime.ToString("F2") + " / " + gameManager.tickTimeMAX.ToString("F2"));
										GUILayout.Label("Time.time: " + Time.time.ToString("F2"));
									}
									GUILayout.EndHorizontal();
								}
									break;
								case GameState.Inactive: {
									GUILayout.Label("state: " + gameManager.gameState);
								}
									break;
								case GameState.NONE: {
									GUILayout.Label("state: " + gameManager.gameState);
								}
									break;
							}
						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.Label("Game Client");
							GUILayout.Space(panelOffset_);
							switch (gameClient.state) {
							case GameClientState.Active: {
									GUILayout.Label("state: " + gameClient.state);
									if (gameClient.clients.Find(x => (x == gameClient.clientServer)) == null) {
										GUILayout.Label("clientServer: " + ((gameClient.clientServer!= null) ? gameClient.clientServer.ToString() : "<null>"));
									}
									var v_string = (string) "";
									for (int i=0; i<gameClient.clients.Count; i++) {
										v_string += (((i==0) ? "" : ", ") + ((gameClient.clients[i]== gameClient.clientServer) ? "+" : "") + gameClient.clients[i].ToString() + " (" + gameClient.clients[i].timeFromLastComm + ")");
									}
									GUILayout.Label("clients[" + gameClient.clients.Count + "]: " + v_string);
									//GUILayout.Label("clientType: " + gameClient.clientType);
									GUILayout.Label("commandID: " + gameClient.commandID);
								}
								break;
							case GameClientState.Inactive: {
									GUILayout.Label("state: " + gameClient.state);
								}
								break;
							case GameClientState.NONE: {
									GUILayout.Label("state: " + gameClient.state);
								}
								break;
							}
						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);

						GUILayout.BeginVertical(GUI.skin.box);
						{
							GUILayout.Label("Game Server");
							GUILayout.Space(panelOffset_);
							switch (gameServer.state) {
							case GameServerState.Active: {
								GUILayout.BeginHorizontal();
								{
									GUILayout.Label("state: " + gameServer.state);
									GUILayout.Label("gameState: " + gameServer.gameState);
									GUILayout.Label("gameMode: " + gameServer.gameMode);
									GUILayout.Label("gameGPMode: " + gameServer.gameGPMode);
								}
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal();
								{
									GUILayout.Label("gameName: " + gameServer.gameName);
									GUILayout.Label("gameMap: " + gameServer.gameMap);
									GUILayout.Label("password: " + gameServer.password);
								}
								GUILayout.EndHorizontal();
								if (gameServer.gsClients.Find(x => (x.client == gameServer.clientServer)) == null) {
									GUILayout.Label("clientServer: " + ((gameServer.clientServer != null) ? gameServer.clientServer.ToString() : "<null>"));
								}
								else {
									//GUILayout.Label("clientPrimary: " + ((gameServer.clientPrimary!= null) ? gameServer.clientPrimary.ToString() : "<null>"));
								}
								var v_string = (string) "";
								for (int i=0; i<gameServer.gsClients.Count; i++) {
									v_string += (((i==0) ? "" : ", ") + ((gameServer.gsClients[i].type == GameClientType.Primary) ? "+" : "") + gameServer.gsClients[i].client.ToString() + "(" + gameServer.gsClients[i].state + ")(" + gameServer.gsClients[i].gameState + ")(" + gameServer.gsClients[i].client.timeFromLastComm + ")");
								}
								GUILayout.Label("gsClients[" + gameServer.gsClients.Count + "]: " + v_string);
								v_string = (string) "";
								for (int i=0; i<gameServer.gsPlayers.Count; i++) {
									v_string += (((i==0) ? "" : ", ") + ((gameServer.gsPlayers[i].gsClient.type == GameClientType.Primary) ? "+" : "") + "[" + gameServer.gsPlayers[i].ID + "] " + gameServer.gsPlayers[i].name + " (" + gameServer.gsPlayers[i].gsClient.client.ID + ")(" + ((gameServer.gsPlayers[i].type == GamePlayerType.Human) ? "H" : (gameServer.gsPlayers[i].type == GamePlayerType.AI) ? "AI" : "N") + ")(" + gameServer.gsPlayers[i].playerState + ")");
								}
								GUILayout.Label("gsPlayers[" + gameServer.gsPlayers.Count + "]: " + v_string);
								GUILayout.BeginHorizontal();
								{
									GUILayout.Label("commandID: " + gameServer.commandID);
									GUILayout.Label("playerID: " + gameServer.playerID);
									GUILayout.Label("sideID: " + gameServer.sideID);
								}
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal();
								{
									GUILayout.Label("timeLobby: " + gameServer.timeLobby.ToString("F2"));
									GUILayout.Label("timeGame: " + gameServer.timeGame.ToString("F2"));
									GUILayout.Label("tickID: " + gameServer.tickID);
									GUILayout.Label("tickTime: " + gameServer.tickTime.ToString("F2") + " / " + gameServer.tickTimeMAX.ToString("F2"));
									GUILayout.Label("Time.time: " + Time.time.ToString("F2"));
								}
								GUILayout.EndHorizontal();
							}
								break;
							case GameServerState.Inactive: {
								GUILayout.Label("state: " + gameServer.state);
							}
								break;
							case GameServerState.NONE: {
								GUILayout.Label("state: " + gameServer.state);
							}
								break;
							}
						}
						GUILayout.EndVertical();

						GUILayout.Space(panelOffset_);
					}
					GUILayout.EndArea();
				}
				GUILayout.EndArea();
			}
		}

		/// <summary>
		/// [ENUM] ADEG projects
		/// </summary>
		[DefaultValue(NONE)]
		public enum ADEG_Project {
			[Description("NONE")]
			NONE = -1,
			[Description("Demo")]
			Demo = 0,
			[Description("GCR")]
			GCR = 1,
		}

		/// <summary>
		/// [ENUM] game states
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameMenuState {
			[Description("NONE")]
			NONE = -1,
			[Description("Loading")]
			Loading = 0,
			[Description("MainMenu")]
			MainMenu = 1,
			[Description("SinglePlayer")]
			SinglePlayer = 2,
			[Description("MultiPlayer")]
			MultiPlayer = 3,
			[Description("Friends")]
			Friends = 4,
			[Description("Settings")]
			Settings = 5,
			[Description("DBG_ADE")]
			DBG_ADE = 255,
		}

		/// <summary>
		/// [ENUM] game MP states
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameMenuMPState {
			[Description("NONE")]
			NONE = -1,
			[Description("LobbyList")]
			LobbyList = 0,
			[Description("Lobby")]
			Lobby = 1,
			[Description("Game")]
			Game = 2,
		}

		public class Game_User {
			public ADE_Client client;
			public GamePlayerState state;

			public Game_User (ADE_Client pClient) {
				client = pClient;
				
				state = GamePlayerState.NONE;
			}
		}

		/*
			[CSCOMM][0000] - Client->Server  ->  GameCommandType.ServerActivateRequest
			[CSCOMM][0001] -   Server<-Client  <-  GameCommandType.ServerActivateRequest
			[CSCOMM][0002] -   Server->Client  ->  GameCommandType.ServerActivateResponse
			[CSCOMM][0003] -     Client<-Server  <-  GameCommandType.ServerActivateResponse

		//	[CSCOMM][0100] - Client->Client  ->  GameCommandType.ServerGetRequest
		//	[CSCOMM][0101] -   Client<-Client  <-  GameCommandType.ServerGetRequest
		//	[CSCOMM][0102] -   Client->Client  ->  GameCommandType.ServerGetResponse
		//	[CSCOMM][0103] -     Client<-Client  <-  GameCommandType.ServerGetResponse
			[CSCOMM][0100] - Client->Server  ->  GameCommandType.ServerGetRequest
			[CSCOMM][0101] -   Server<-Client  <-  GameCommandType.ServerGetRequest
			[CSCOMM][0102] -   Server->Client  ->  GameCommandType.ServerGetResponse
			[CSCOMM][0103] -     Client<-Server  <-  GameCommandType.ServerGetResponse

			[CSCOMM][0200] - Client->Server  ->  GameCommandType.PlayerLoginRequest
			[CSCOMM][0201] -   Server<-Client  <-  GameCommandType.PlayerLoginRequest
			[CSCOMM][0202] -   Server->Client  ->  GameCommandType.PlayerLoginResponse
			[CSCOMM][0203] -     Client<-Server  <-  GameCommandType.PlayerLoginResponse
			[CSCOMM][0204] -   Server->Client  ->  GameCommandType.GameDataUpdate
			[CSCOMM][0205] -     Client<-Server  <-  GameCommandType.GameDataUpdate
			[CSCOMM][0206] -   Server->Client[*]  ->  GameCommandType.PlayerLogin
			[CSCOMM][0207] -     Client{*}<-Server  <-  GameCommandType.PlayerLogin
			[CSCOMM][0208] -     Client{*}->Client  ->  GameCommandType.PlayerLoginAck
			[CSCOMM][0209] -       Client<-Client{*}  <-  GameCommandType.PlayerLoginAck
			[CSCOMM][0210] -       Client->Client{*}  ->  GameCommandType.PlayerStateUpdate
			[CSCOMM][0211] -         Client{*}<-Client  <-  GameCommandType.PlayerStateUpdate
			[CSCOMM][0212] -       Client->Client{*}  ->  GameCommandType.PlayerDataUpdate
			[CSCOMM][0213] -         Client{*}<-Client  <-  GameCommandType.PlayerDataUpdate
			[CSCOMM][0214] -     Client{*}->Client  ->  GameCommandType.PlayerStateUpdate
			[CSCOMM][0215] -       Client<-Client{*}  <-  GameCommandType.PlayerStateUpdate
			[CSCOMM][0216] -     Client{*}->Client  ->  GameCommandType.PlayerDataUpdate
			[CSCOMM][0217] -       Client<-Client{*}  <-  GameCommandType.PlayerDataUpdate

			[CSCOMM][0300] - Client->Server  ->  GameCommandType.GameDataUpdateRequest
			[CSCOMM][0301] -   Server<-Client  <-  GameCommandType.GameDataUpdateRequest
			[CSCOMM][0302] -   Server->Client  ->  GameCommandType.GameDataUpdateResponse
			[CSCOMM][0303] -     Client<-Server  <-  GameCommandType.GameDataUpdateResponse
			[CSCOMM][0304] -   Server->Client[*]  ->  GameCommandType.GameDataUpdate
			[CSCOMM][0305] -     Client{*}<-Server  <-  GameCommandType.GameDataUpdate

			[CSCOMM][0400] - Client->Server  ->  GameCommandType.PlayerStateUpdateRequest
			[CSCOMM][0401] -   Server<-Client  <-  GameCommandType.PlayerStateUpdateRequest
			[CSCOMM][0402] -   Server->Client  ->  GameCommandType.PlayerStateUpdateResponse
			[CSCOMM][0403] -     Client<-Server  <-  GameCommandType.PlayerStateUpdateResponse
			[CSCOMM][0404] -   Server->Client[*]  ->  GameCommandType.PlayerStateUpdate
			[CSCOMM][0405] -     Client{*}<-Server  <-  GameCommandType.PlayerStateUpdate

			[CSCOMM][0500] - Client->Server  ->  GameCommandType.PlayerDataUpdateRequest
			[CSCOMM][0501] -   Server<-Client  <-  GameCommandType.PlayerDataUpdateRequest
			[CSCOMM][0502] -   Server->Client  ->  GameCommandType.PlayerDataUpdateResponse
			[CSCOMM][0503] -     Client<-Server  <-  GameCommandType.PlayerDataUpdateResponse
			[CSCOMM][0504] -   Server->Client[*]  ->  GameCommandType.PlayerDataUpdate
			[CSCOMM][0505] -     Client{*}<-Server  <-  GameCommandType.PlayerDataUpdate

			[CSCOMM][0600] - Client->Server  ->  GameCommandType.TimeSyncRequest
			[CSCOMM][0601] -   Server<-Client  <-  GameCommandType.TimeSyncRequest
			[CSCOMM][0602] -   Server->Client  ->  GameCommandType.TimeSyncResponse
			[CSCOMM][0603] -     Client<-Server  <-  GameCommandType.TimeSyncResponse
			[CSCOMM][0604] - Client->Server  ->  GameCommandType.TimeSync
			[CSCOMM][0605] -   Server<-Client  <-  GameCommandType.TimeSync
			[CSCOMM][0606] -   Server->Client  ->  GameCommandType.TimeSyncAck
			[CSCOMM][0607] -     Client<-Server  <-  GameCommandType.TimeSyncAck

			[CSCOMM][0700] - Client->Server  ->  GameCommandType.PingRequest
			[CSCOMM][0701] -   Server<-Client  <-  GameCommandType.PingRequest
			[CSCOMM][0702] -   Server->Client  ->  GameCommandType.PingResponse
			[CSCOMM][0703] -     Client<-Server  <-  GameCommandType.PingResponse
			[CSCOMM][0710] - Server->Client  ->  GameCommandType.PingRequest
			[CSCOMM][0711] -   Client<-Server  <-  GameCommandType.PingRequest
			[CSCOMM][0712] -   Client->Server  ->  GameCommandType.PingResponse
			[CSCOMM][0713] -     Server<-Client  <-  GameCommandType.PingResponse
			
			[CSCOMM][0800] - Client->Server  ->  GameCommandType.ServerPasswordUpdateRequest
			[CSCOMM][0801] -   Server<-Client  <-  GameCommandType.ServerPasswordUpdateRequest
			[CSCOMM][0802] -   Server->Client  ->  GameCommandType.ServerPasswordUpdateResponse
			[CSCOMM][0803] -     Client<-Server  <-  GameCommandType.ServerPasswordUpdateResponse

			[CSCOMM][0900] - Client->Server  ->  GameCommandType.PlayerGameStartRequest
			[CSCOMM][0901] -   Server<-Client  <-  GameCommandType.PlayerGameStartRequest
			[CSCOMM][0902] -   Server->Client  ->  GameCommandType.PlayerGameStartResponse
			[CSCOMM][0903] -     Client<-Server  <-  GameCommandType.PlayerGameStartResponse
			[CSCOMM][0904] -   Server->Client[*]  ->  GameCommandType.GameLoadRequest
			[CSCOMM][0905] -     Client{*}<-Server  <-  GameCommandType.GameLoadRequest
			[CSCOMM][0906] -     Client{*}->Server  ->  GameCommandType.GameLoadResponse
			[CSCOMM][0907] -       Server<-Client{*}  <-  GameCommandType.GameLoadResponse
			[CSCOMM][0908] -   Server->Client[*]  ->  GameCommandType.GameInitRequest
			[CSCOMM][0909] -     Client{*}<-Server  <-  GameCommandType.GameInitRequest
			[CSCOMM][0910] -     Client{*}->Client[**]  ->  GameCommandType.PlayerInit
			[CSCOMM][0911] -       Client{**}<-Client{*}  <-  GameCommandType.PlayerInit
			[CSCOMM][0912] -     Client{*}->Server  ->  GameCommandType.GameInitResponse
			[CSCOMM][0913] -       Server<-Client{*}  <-  GameCommandType.GameInitResponse
			[CSCOMM][0914] -   Server->Client[*]  ->  GameCommandType.GameStartRequest
			[CSCOMM][0915] -     Client{*}<-Server  <-  GameCommandType.GameStartRequest
			[CSCOMM][0916] -     Client{*}->Server  ->  GameCommandType.GameStartResponse
			[CSCOMM][0917] -       Server<-Client{*}  <-  GameCommandType.GameStartResponse
			[CSCOMM][0918] -   Server->Client[*]  ->  GameCommandType.GameStart
			[CSCOMM][0919] -     Client{*}<-Server  <-  GameCommandType.GameStart
		 */

		public class Game_Manager : MonoBehaviour {
			public ADEG adeg_;
			public ADEG adeg {
				get {
					return adeg_;
				}
			}

			public GameState gameState;
			public GameMode gameMode;
			public GameplayMode gameGPMode;

			private Game_Player player_;
			public Game_Player player {
				get {
					if (player_ == null) {
						player_ = players.Find(x => (ADE.ClientCheckIfLocal(x.client) && (x.location == Location.Local)/* && (x.type == GamePlayerType.Human)*/));
					}

					return player_;
				}
			}
			public List<Game_Player> players;

			public string gameName;
			public int gameMap;

			byte[] gameSettings;
			public int defaultSquads = 0;
			public int friendlyFire = 1;
			public int allowAlliances = 1;
			public int apcType = 1;
			public int damageFactor = 1;
			public int timeLimit = 300;
			public int scoreLimit = 10000;
			public int techLimit = 6;
			public int reinforcements = 1;
			public int reinforceInterval = 30;

			public float timeLobby;
			public float timeGame;

		// lockstep variables
			/// <summary>
			/// last used tick ID
			/// </summary>
			public int tickID;
			public float tickTimeMAX;
			public float tickTime;

			public List<Game_Action> actionsLocal;
			public List<Game_Action> actionsLocalToDistribute;
			public List<Game_Action> actionsReceived;
			public List<Game_Action> actionsToExecute;

			public List<GMObject> gameObjects;
			public List<Game_Unit> gameUnits;
			public List<Game_Building> gameBuildings;

		//	public static bool def_RTT;
		//	public float rtt_time;
		//	public bool rtt_waiting;

			public void Awake_<TADEG>() where TADEG : ADEG {
				var v_obj = (GameObject) GameObject.Find(ADEG.def_ADEG_Name);
				if (v_obj != null) {
					adeg_ = v_obj.GetComponent<TADEG>();

					Reset();
				}
			}

			void Update() {
				Update_();
			}

			void FixedUpdate() {
				FixedUpdate_();
			}

			void OnGUI() {
				OnGUI_();
			}

			public void Update_() {
				//
			}

			public void FixedUpdate_() {
				if (gameState >= GameState.Lobby) {
					timeLobby += Time.deltaTime;
				}
					
				if (gameState == GameState.Playing) {
					//ALog.LogError("FixedUpdate()  ->  Time.deltaTime: " + Time.deltaTime + " Time.fixedDeltaTime: " + Time.fixedDeltaTime, DBGC.DBG);

					timeGame += Time.deltaTime;

					tickTime += Time.deltaTime;
					if (tickTime > tickTimeMAX) {
						tickTime = 0f;
						tickID++;

						// transfer the local actions into the to distribute actions list and distribute them
						actionsLocalToDistribute = actionsLocal;
						adeg.gameClient.ActionsSend(actionsLocalToDistribute);
						actionsLocal = new List<Game_Action>();
						// trasfer the received actions into the executing actions list and execute them
						actionsToExecute = actionsReceived;
						StartCoroutine(ActionsExecute(actionsToExecute));
						actionsReceived = new List<Game_Action>();
					}
				}

				ALog.LogTag("[WIP][000] - client/server ping request/response");
				if (ADEG.dbg_commTimeoutActive && ((gameState == GameState.Lobby) || (gameState == GameState.Playing))) {
					if ((adeg.gameClient.clientType == GameClientType.Secondary)) {
						for (int i=0; i<adeg.gameClient.clients.Count; i++) {
							if (!adeg.gameClient.clients[i].CheckIfSame(adeg.gameUser.client)) {
								adeg.gameClient.clients[i].timeFromLastComm += Time.deltaTime;

								if (!adeg.gameClient.clients[i].pinging && (adeg.gameClient.clients[i].timeFromLastComm > ADEG.def_clientTimePingMAXDefault)) {
									ALog.LogErrorHandling("[GM]  ->  t[" + adeg.gameManager.tickID + "] client: " + adeg.gameClient.clients[i].ToString() + " communication last: " + adeg.gameClient.clients[i].timeFromLastComm);
									ALog.LogDebug("[CSCOMM][0700] - Client->Server  ->  GameCommandType.PingRequest");
									adeg.gameClient.CommandClientToServerSend(adeg.gameClient.clientServer, new Game_Command.PingRequest(-1, ++adeg.gameClient.commandID, ((gameState == GameState.Playing) ? timeGame : timeLobby), adeg.gameUser.client, player.ID, tickID));
									adeg.gameClient.clients[i].pinging = true;

									if (adeg.gameClient.clients[i].timeFromLastComm > ADEG.def_clientTimeTimeoutMAXDefault) {
										ALog.LogTag("[WIP][000] - comm lost");
										ALog.LogError("[GM]  ->  t[" + adeg.gameManager.tickID + "] client: " + adeg.gameClient.clients[i].ToString() + " communication timeout: " + adeg.gameClient.clients[i].timeFromLastComm, DBGC.DBG);

										adeg.gameManager.StartCoroutine(adeg.gameManager.GameStop());
									}
								}
							}
						}
					}
				}
			}

			public void OnGUI_() {
				//
			}


			public IEnumerator TimeSync (Game_Player pPlayer) {
				ALog.LogDebug("[CSCOMM][0600] - Client->Server  ->  GameCommandType.TimeSyncRequest");
				adeg.gameClient.CommandClientToServerSend(adeg.gameClient.clientServer, new Game_Command.TimeSyncRequest(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client, pPlayer.ID, 0));

				// [!!!][003] - time sync
		//		ALog.LogDebug("[CSCOMM][0604] - Client->Server  ->  GameCommandType.TimeSync");
		//		adeg.gameClient.CommandClientToServerSend(adeg.gameClient.clientServer, new Game_Command.TimeSync(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client, pPlayer.ID));

				yield break;
			}

			public IEnumerator GameLoad (int pLevelID) {
				yield return StartCoroutine(GameLoadCustom(pLevelID));
			}

			public virtual IEnumerator GameLoadCustom (int pLevelID) {
				yield break;
			}

			public IEnumerator GameInit() {
				ALog.Log("[GM]  ->  game initializing..");
				gameState = GameState.Initializing;

				yield return StartCoroutine(GameInitCustom());

				for (int i=0; i<players.Count; i++) {
					if (players[i].location == Location.Local) {
						// in singleplayer all local players are set as ready to start
						if (gameMode == GameMode.SinglePlayer) {
							players[i].state = GamePlayerState.LobbyReady;
						}

						if (players[i].state == GamePlayerState.LobbyReady) {
							yield return StartCoroutine(PlayerInitCustom(players[i]));
						}
						else {
							if (gameMode == GameMode.MultiPlayer) {
								ADE.lList.LobbyLeaveRequest(ADE.lobby.aID);

								ADE.lList.ActiveSet(true);
								ADE.lobby.ActiveSet(false);
								ADE.comm.ActiveSet(false);

								players[i].state = GamePlayerState.LookingForGame;
								adeg.stateMenuMP = GameMenuMPState.LobbyList;
							}
						}
					}
				}

				// when all players were initialized
				if (players.Find(x => (x.state != GamePlayerState.Initialized)) == null) {
					gameState = GameState.Initialized;
					ALog.LogDebug("[CSCOMM][0912] -     Client{*}->Server  ->  GameCommandType.GameInitResponse");
					adeg.gameClient.CommandClientToServerSend(adeg.gameClient.clientServer, new Game_Command.GameInitResponse(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client, GameCommandState.ResponseOK));
				}
			}

			public virtual IEnumerator GameInitCustom() {
				yield break;
			}

			public virtual IEnumerator PlayerInitCustom (Game_Player pPlayer) {
				pPlayer.state = GamePlayerState.Initialized;

				for (int i=0; i<players.Count; i++) {
					if (players[i] != pPlayer) {
						ALog.LogDebug("[CSCOMM][0910] -     Client{*}->Client[**]  ->  GameCommandType.PlayerInit");
						ALog.Log("[P[" + pPlayer.ID + "]]  ->  self (" + pPlayer.ToString() + ") sends his new game data to player:  " + players[i].ToString());
						adeg.gameClient.CommandClientToClientSend(players[i].client, new Game_Command.PlayerInit(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client, players[i].ID, pPlayer.ID));
					}
				}

				yield break;
			}

			public IEnumerator GameSPStart (int pLevelID) {
				gameMode = GameMode.SinglePlayer;

				ADE.lobby.ActiveSet(true);
				if (ADE.def_DBG_LocalToP2PLobby) {
					// [!!!][004] - create a SP true lobby
				}
				else {
					adeg.gameLobby.lobby.clients.Add(adeg.gameUser.client);
				}
				if (ADEG.def_DBG_LocalToP2PComm) {
					ADE.comm.ActiveSet(true);

					adeg.gameClient.clientServer = adeg.gameUser.client;
				}

				yield return StartCoroutine(GameSPStartCustom(pLevelID));
			}

			public virtual IEnumerator GameSPStartCustom (int pLevelID) {
				yield break;
			}

			public IEnumerator GameMPCreate (GameplayMode pGameplayMode, string pPassword = "") {
				gameMode = GameMode.MultiPlayer;

				ADE.lobby.ActiveSet(true);
				ADE.comm.ActiveSet(true);

				// set the game lobby data
				adeg.gameLobby.password = pPassword;
				adeg.gameLobby.gameName = adeg.inputGameName;
				adeg.gameLobby.gameMap = adeg.inputGameMap;
				if (adeg.def_clientUnconfirmedLocalEffect) {
					gameName = adeg.inputGameName;
					gameMap = adeg.inputGameMap;
				}
				else {
					adeg.inputGameName = gameName;
					adeg.inputGameMap = gameMap;
				}
				yield return StartCoroutine(ADE.lList.LobbyCreateRequest(ADE.def_ADE_Lobby_ClientsNoMAX, true));

				if (ADE.lobby.aID.valid) {
					ADE.lList.ActiveSet(false);

					//ADE.lobby.Set();
					adeg.stateMenuMP = GameMenuMPState.Lobby;

					var v_clientServer = (ADE_Client) adeg.gameServer.clientServer;

					// activate the game server, the game client and the game manager
					ALog.LogDebug("[CSCOMM][0000] - Client->Server  ->  GameCommandType.ServerActivateRequest");
					var v_command = (Game_Command) new Game_Command.ServerActivateRequest(-1, 0, Time.time, adeg.gameUser.client, GameMode.MultiPlayer, pGameplayMode, pPassword);
					adeg.gameClient.CommandClientToServerSend(v_clientServer, v_command /*, Location.Local*/);

					// add the players
					players.Clear();
					ALog.LogDebug("[CSCOMM][0200] - Client->Server  ->  GameCommandType.PlayerLoginRequest");
					adeg.gameClient.CommandClientToServerSend(v_clientServer, new Game_Command.PlayerLoginRequest(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client, GamePlayerType.Human, adeg.gameUser.client.name, pPassword));}
			}

			public IEnumerator GameMPJoin (ADE_ID pLobbyID, string pPassword) {
				gameMode = GameMode.MultiPlayer;

				ADE.lobby.ActiveSet(true);
				ADE.comm.ActiveSet(true);

				adeg.gameLobby.password = pPassword;

				yield return StartCoroutine(ADE.lList.LobbyJoinRequest(pLobbyID, true));

				if (pLobbyID.valid && ADE.lobby.aID.CheckIfSame(pLobbyID)) {
					ADE.lList.ActiveSet(false);

					adeg.stateMenuMP = GameMenuMPState.Lobby;

					// add the players
					players.Clear();

					var v_clientServerID = (ulong) 0;
					ulong.TryParse(ADE.lList.LobbyDataGet(pLobbyID, "serverID"), out v_clientServerID);
					var v_clientServerPlatform = (int) ADE_Platform.NONE;
					Int32.TryParse(ADE.lList.LobbyDataGet(pLobbyID, "serverPlatform"), out v_clientServerPlatform);
					var v_clientServer = (ADE_Client) ADE.lobby.clients.ClientGet(v_clientServerID, v_clientServerPlatform.ToEnum<ADE_Platform>());
					if (v_clientServer == null) {
						ALog.LogErrorHandling(DBGC.TODO);
					}

		//			ALog.LogDebug("[CSCOMM][0100] - Client->Client  ->  GameCommandType.ServerGetRequest");
		//			adeg.gameClient.CommandClientToClientSend(ADE.lobby.clients.Find(x => !x.CheckIfSame(adeg.gameUser.client)), new Game_Command.ServerGetRequest(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client));
					ALog.LogDebug("[CSCOMM][0100] - Client->Server  ->  GameCommandType.ServerGetRequest");
					adeg.gameClient.CommandClientToServerSend(v_clientServer, new Game_Command.ServerGetRequest(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client), Location.Remote);
				}

				yield break;
			}

			public bool GameMPStartReady() {
				return (adeg.gameLobby.lobby.ClientCheckIfOwnerLocal() && ((player != null) && (player.state == GamePlayerState.LobbyReady)));
			}

			public IEnumerator GameMPStart (int pLevelID) {
				var v_ready = (bool) true;
				for (int i=0; i<players.Count; i++) {
					v_ready = v_ready && ((players[i].state == GamePlayerState.LobbyReady) || (players[i].state == GamePlayerState.LobbySpectating));
				}

				if (v_ready) {
					ALog.LogDebug("[CSCOMM][0900] - Client->Server  ->  GameCommandType.PlayerGameStartRequest");
					adeg.gameClient.CommandClientToServerSend(adeg.gameClient.clientServer, new Game_Command.PlayerGameStartRequest(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client, pLevelID));
				}
				else {
					ADE.lobby.MessageSend("All players must be ready or spectating in order to start the game!", ADE_LobbyMessageType.Info);
				}

				yield break;
			}

			public IEnumerator GameMPLeave() {
				if (adeg.gameClient.clientServer != null) {
				}
				if (adeg.gameClient.CheckIfAdmin()) {
					ALog.LogDebug("[CSCOMM][] - Client->Server  ->  GameCommandType.GameStopRequest");
					adeg.gameClient.CommandClientToServerSend(adeg.gameClient.clientServer, new Game_Command.GameStopRequest(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client));
				}
				else {
					ALog.LogDebug("[CSCOMM][] - Client->Server  ->  GameCommandType.GameLeaveRequest");
					adeg.gameClient.CommandClientToServerSend(adeg.gameClient.clientServer, new Game_Command.PlayerLeaveRequest(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client, player.ID));
				}

				yield break;
			}

			public IEnumerator GameStop (int pLevelID = 0) {
				adeg.gameUser.state = GamePlayerState.LookingForGame;
				switch(gameMode) {
					case GameMode.SinglePlayer : {
						ADE.lobby.ActiveSet(false);
						if (ADEG.def_DBG_LocalToP2PComm) {
							ADE.comm.ActiveSet(false);
						}

						adeg.stateMenu = GameMenuState.MainMenu;
						adeg.stateMenuMP = GameMenuMPState.LobbyList;
					}
						break;
					case GameMode.MultiPlayer: {
						ADE.lList.LobbyLeaveRequest(ADE.lobby.aID);
						
						ADE.lList.ActiveSet(false);
						ADE.lobby.ActiveSet(false);
						ADE.comm.ActiveSet(false);

						// to main menu
						adeg.stateMenu = GameMenuState.MainMenu;
						adeg.stateMenuMP = GameMenuMPState.LobbyList;

						// to multiplayer menu
		//				adeg.stateMenuMP = GameMenuMPState.LobbyList;
						adeg.OnGUI_Menu_ButtonMultiplayer();
					}
						break;
				}

				adeg.gameServer.Reset();
				adeg.gameClient.Reset();
				adeg.gameManager.Reset();

				adeg.inputPasswordCreate = "";
				adeg.inputPasswordJoin = "";

				yield return StartCoroutine(GameStopCustom(pLevelID));
			}

			public virtual IEnumerator GameStopCustom (int pLevelID = 0) {
				yield break;
			}

			public IEnumerator GameEnd (int pLevelID = 0) {
				yield return StartCoroutine(GameEndCustom());

				yield return StartCoroutine(GameStop(pLevelID));
			}

			public virtual IEnumerator GameEndCustom (int pLevelID = 0) {
				yield break;
			}

			public virtual bool CommandDataExecute (Game_Command.Data pCommandData) {
				return false;
			}

			public virtual Game_Player PlayerNew (int pPlayerID, ADE_Client pClient, Location pLocation, GamePlayerType pType, string pName, int pSideID) {
				return (new Game_Player(pPlayerID, pClient, pLocation, pType, pName, pSideID));
			}

			public virtual Game_Player PlayerNewAdd (int pPlayerID, ADE_Client pClient, Location pLocation, GamePlayerType pType, string pName, int pSideID) {
				var v_player = (Game_Player) PlayerNew(pPlayerID, pClient, pLocation, pType, pName, pSideID);
				players.Add(v_player);

				return v_player;
			}

			public IEnumerator PlayerKick (Game_Player pPlayer) {
				ALog.LogDebug("[CSCOMM][] - Client->Server  ->  GameCommandType.PlayerKickRequest");
				adeg.gameClient.CommandClientToServerSend(adeg.gameClient.clientServer, new Game_Command.PlayerKickRequest(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client, pPlayer.ID));

				yield break;
			}

			public GMObject GameObjectGet (int pObjectID) {
				return gameObjects.Find(x => (x.ID == pObjectID));
			}

			public virtual Game_Unit UnitNew (GameObject pGameObject, int pUnitSideID, int pUnitTeamID, bool pActive) {
				return (new Game_Unit(pGameObject, pUnitSideID, pUnitTeamID, pActive));
			}

			public virtual Game_Unit UnitNewAdd (GameObject pGameObject, int pUnitSideID, int pUnitTeamID, bool pActive = true) {
				var v_unit = (Game_Unit) UnitNew(pGameObject, pUnitSideID, pUnitTeamID, ((pGameObject != null) ? pGameObject.activeInHierarchy : false));
				gameUnits.Add(v_unit);
				gameObjects.Add((GMObject)v_unit);

				var v_player = (Game_Player) players.Find(x => (x.ID == v_unit.sideID));
				if (v_player != null) {
					v_unit.player = v_player;
					v_player.units.Add(v_unit);
				}

				return v_unit;
			}

			public Game_Unit GameUnitGet (int pUnitID) {
				return gameUnits.Find(x => (x.ID == pUnitID));
			}

			public void ActionAdd (Game_Action pAction) {
				ALog.Log("[" + tickID + "] - [P[" + pAction.playerID + "]]  ->  self (" + players.PlayerGet(pAction.playerID).ToString() + ") adds new action: " + pAction.ToString());
				actionsLocal.Add(pAction);
			}

			public Game_Action ActionNew (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) {
				var v_action = (Game_Action) null;

				// first byte[] pData's int is for the GameActionCategory, and the second one is for the GameActionType
				var v_category = (GameActionCategory) BitConverter.ToInt32(pData, pOffset).ToEnum<GameActionCategory>();
				//pOffset += sizeof(int);
				var v_type = BitConverter.ToInt32(pData, pOffset +sizeof(int));
				//pOffset += sizeof(int);
				switch (v_category) {
					case GameActionCategory.Base: {
						switch ((GameActionType) v_type) {
							case GameActionType.Data : {
								v_action = new Game_Action.Data(pData, ref pOffset, ref pClientsList);
							}
								break;
						}
					}
						break;
					case GameActionCategory.Extended:
					case GameActionCategory.Custom: {
						v_action = ActionNewCustom(pData, ref pOffset, ref pClientsList);
					}
					break;
				}

				return v_action;
			}

			public virtual Game_Action ActionNewCustom (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) {
				return null;
			}

			public virtual bool ActionDataExecute (Game_Action.Data pActionData) {
				return false;
			}

			public IEnumerator ActionsExecute (List<Game_Action> pListGameActions) {
				if (pListGameActions != null) {
					pListGameActions = pListGameActions.OrderBy(x => x.tickID).OrderBy(x => x.tickTime).ThenBy(x => x.playerID).ToList();

					for (int i=0; i<pListGameActions.Count; i++) {
						if (tickID == pListGameActions[i].tickID +2) {
							while (tickTime < pListGameActions[i].tickTime) {
								yield return new WaitForFixedUpdate();
							}
						}
						else {
							ALog.LogTag("[WIP][000] - comm desync");
							ALog.LogErrorHandling("[GM]  ->  t[" + tickID + "] executing action from t[" + pListGameActions[i].tickID + "] other than the before-previous tick", DBGV.HIDDEN);
						}
						pListGameActions[i].Execute(adeg);
					}
				}
			}

			public void Reset() {
				gameState = GameState.Inactive;
				gameMode = GameMode.NONE;
				gameGPMode = GameplayMode.NONE;

				player_ = null;

				if (players == null) {
					players = new List<Game_Player>();
				}
				else {
					players.Clear();
				}
				gameName = "";
				gameMap = -1;

				timeLobby = 0f;
				timeGame = 0f;

				tickID = 0;
				tickTimeMAX = ADEG.def_tickTimeMAXDefault;
				tickTime = 0f;

				if (actionsLocal == null) {
					actionsLocal = new List<Game_Action>();
				}
				else {
					actionsLocal.Clear();
				}
				if (actionsLocalToDistribute == null) {
					actionsLocalToDistribute = new List<Game_Action>();
				}
				else {
					actionsLocalToDistribute.Clear();
				}
				if (actionsReceived == null) {
					actionsReceived = new List<Game_Action>();
				}
				else {
					actionsReceived.Clear();
				}
				if (actionsToExecute == null) {
					actionsToExecute = new List<Game_Action>();
				}
				else {
					actionsToExecute.Clear();
				}

				GMObject.ResetIDs();
				if (gameObjects == null) {
					gameObjects = new List<GMObject>();
				}
				else {
					gameObjects.Clear();
				}

				Game_Unit.ResetIDs();
				if (gameUnits == null) {
					gameUnits = new List<Game_Unit>();
				}
				else {
					gameUnits.Clear();
				}

				Game_Building.ResetIDs();
				if (gameBuildings == null) {
					gameBuildings = new List<Game_Building>();
				}
				else {
					gameBuildings.Clear();
				}

				Game_Abstract.ResetIDs();
				Game_Projectile.ResetIDs();

				//def_RTT = false;

				ResetCustom();
			}

			public virtual void ResetCustom() {
				//
			}
		}


		/// <summary>
		/// [ENUM] game states
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameState {
			[Description("NONE")]
			NONE = -1,
			[Description("Inactive")]
			Inactive = 0,
			[Description("Active")]
			Active = 1,
			[Description("Lobby")]
			Lobby = 2,
			[Description("Loading")]
			Loading = 3,
			[Description("Loaded")]
			Loaded = 4,
			[Description("Initializing")]
			Initializing = 5,
			[Description("Initialized")]
			Initialized = 6,
			[Description("Ready")]
			Ready = 7,
			[Description("Playing")]
			Playing = 8,
			[Description("Paused")]
			Paused = 9,
			[Description("Stopped")]
			Stopped = 10,
			[Description("Ended")]
			Ended = 11,
		}

		/// <summary>
		/// [ENUM] game modes
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameMode {
			[Description("NONE")]
			NONE = -1,
			[Description("SinglePlayer")]
			SinglePlayer = 0,
			[Description("MultiPlayer")]
			MultiPlayer = 1,
		}

		/// <summary>
		/// [ENUM] gameplay modes
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameplayMode {
			[Description("NONE")]
			NONE = -1,
			[Description("Coop")]
			Coop = 0,
			[Description("Deathmatch")]
			Deathmatch = 1,
			[Description("Score Zone")]
			ScoreZone = 2,
			[Description("CTF")]
			CTF = 3,
		}


		public class Game_Player {
			public int ID;

			public ADE_Client client;
			public Location location;

			public GamePlayerType type;
			public string name;
			public GamePlayerState state;

			public int sideID;
			public List<Game_Building> buildings;
			public List<Game_Unit> units;

			public int number;


			public Game_Player (int pPlayerID, ADE_Client pClient, Location pLocation, GamePlayerType pType, string pName, int pSideID) {
				ID = pPlayerID;

				client = pClient;
				location = pLocation;

				type = pType;
				name = pName;
				state = GamePlayerState.NONE;

				sideID = pSideID;
				buildings = new List<Game_Building>();
				units = new List<Game_Unit>();

				number = 0;
			}

			public override string ToString() {
				return ("[" + ID + "] " + name + " (" + client.ID + ")(" + ((location == Location.Local) ? "L" : (location == Location.Remote) ? "R" : "N") + "," + ((type == GamePlayerType.Human) ? "H" : (type == GamePlayerType.AI) ? "AI" : "N") + ")(" + state + ")(" + sideID +")");
			}
		}

		/// <summary>
		/// game player extension
		/// </summary>
		public static class Game_PlayerExtensions {
			public static Game_Player PlayerGet (this List<Game_Player> pPlayers, int pPlayerID) {
				return pPlayers.Find(x => (x.ID == pPlayerID));
			}

			public static Game_Player PlayerGet (this List<Game_Player> pPlayers, ADE_Client pClient) {
				return pPlayers.Find(x => x.client.CheckIfSame(pClient));
			}

			public static List<Game_Player> PlayersGet (this List<Game_Player> pPlayers, ADE_Client pClient) {
				return pPlayers.FindAll(x => x.client.CheckIfSame(pClient));
			}

			public static Game_Player PlayerGet (this List<Game_Player> pPlayers, CSteamID pCSteamID) {
				return pPlayers.Find(x => x.client.aID.CheckIfSame(pCSteamID));
			}

			public static List<Game_Player> PlayersGet (this List<Game_Player> pPlayers, CSteamID pCSteamID) {
				return pPlayers.FindAll(x => x.client.aID.CheckIfSame(pCSteamID));
			}

			public static Game_Player PlayerGet (this List<Game_Player> pPlayers, GalaxyID pGalaxyID) {
				return pPlayers.Find(x => x.client.aID.CheckIfSame(pGalaxyID));
			}

			public static List<Game_Player> PlayersGet (this List<Game_Player> pPlayers, GalaxyID pGalaxyID) {
				return pPlayers.FindAll(x => x.client.aID.CheckIfSame(pGalaxyID));
			}
		}

		/// <summary>
		/// [ENUM] player locations
		/// </summary>
		[DefaultValue(NONE)]
		public enum Location {
			[Description("NONE")]
			NONE = -1,
			[Description("Local")]
			Local = 0,
			[Description("Remote")]
			Remote = 1,
		}

		/// <summary>
		/// [ENUM] player types
		/// </summary>
		[DefaultValue(NONE)]
		public enum GamePlayerType {
			[Description("NONE")]
			NONE = -1,
			[Description("Human")]
			Human = 0,
			[Description("AI")]
			AI = 1,
		}

		/// <summary>
		/// [ENUM] player states
		/// </summary>
		[DefaultValue(NONE)]
		public enum GamePlayerState {
			[Description("NONE")]
			NONE = -1,
			[Description("LookingForGame")]
			LookingForGame = 0,
			[Description("Lobby Spectating")]
			LobbySpectating = 1,
			[Description("Lobby Seated")]
			LobbySeated = 2,
			[Description("Lobby Ready")]
			LobbyReady = 3,
			[Description("Initialized")]
			Initialized = 4,
			[Description("Playing")]
			Playing = 5,
			[Description("Left")]
			Left = 6,
		}

		public class GMObject {
			private static int objID_;

			public GMObject_Component component;
			public GameObject obj;
			public int objID;
			public GMObjectType objType;
			public GMObjectClass objClass;

			public int ID;
			public Game_Player player;

			public int state_;
			public GMObjectState state {
				get {
					return state_.ToEnum<GMObjectState>();
				}
				set {
					state_ = (int) value;
				}
			}

			public bool active;
			public virtual bool isActive {
				get {
					return (active && (obj != null));
				}
			}

			public Vector3 position {
				get {
					return ((obj != null)? obj.transform.position : Vector3.zero);
				}
				set {
					if (obj != null) {
						obj.transform.position = value;
					}
				}
			}
			public float rotation {
				get {
					return ((obj != null)? obj.transform.rotation.eulerAngles.y : 0f);
				}
				set {
					if (obj != null) {
						obj.transform.rotation = Quaternion.Euler(Vector3.up * value);
					}
				}
			}
			public Vector3 positionNext;
			public float rotationNext;

			public bool selected;


			public GMObject (GameObject pGameObject, GMObjectType pObjectType, bool pActive) {
				obj = pGameObject;
				objID = ++objID_;
				objType = pObjectType;
				switch (pObjectType) {
					case GMObjectType.Ground:
					case GMObjectType.Unit:
					case GMObjectType.Building:
					case GMObjectType.Resource:
					case GMObjectType.Abstract:
						objClass = GMObjectClass.Persistent;
						break;
					case GMObjectType.Projectile:
						objClass = GMObjectClass.Temporary;
						break;
					default:
						objClass = GMObjectClass.NONE;
						break;			
				}

				state_ = (int) GMObjectState.NONE;

				ActiveSet(pActive);
			}

			public bool IsFriendly (Game_Player pPlayer) {
				return (player == pPlayer);
			}

			public void ActiveSet (bool pActive) {
				//ALog.LogDebug("ActiveSet(" + pActive + ")  ->  " + DBG_InfoGet());
				state = (pActive? GMObjectState.Active : GMObjectState.Inactive);
				active = pActive;

				if (obj != null) {
					obj.SetActive(pActive);
				}
			}

			public void SelectedSet (bool pSelected) {
				//ALog.LogDebug("SelectedSet(" + pSelected + ")  ->  " + DBG_InfoGet());
				selected = pSelected;
			}

			public static void ResetIDs() {
				objID_ = -1;
			}

			public T ToGround<T>() where T : Game_Ground {
				if (objType == GMObjectType.Ground) {
					return ((T) this);
				}

				return null;
			}

			public T ToUnit<T>() where T : Game_Unit {
				if (objType == GMObjectType.Unit) {
					return ((T) this);
				}

				return null;
			}

			public T ToBuilding<T>() where T : Game_Building {
				if (objType == GMObjectType.Building) {
					return ((T) this);
				}

				return null;
			}

			public T ToResource<T>() where T : Game_Resource {
				if (objType == GMObjectType.Resource) {
					return ((T) this);
				}

				return null;
			}

			public T ToAbstract<T>() where T : Game_Abstract {
				if (objType == GMObjectType.Projectile) {
					return ((T) this);
				}

				return null;
			}

			public T ToProjectile<T>() where T : Game_Projectile {
				if (objType == GMObjectType.Projectile) {
					return ((T) this);
				}

				return null;
			}


			//	[Conditional("ADEF_ADEG_DBG")]
			public void DBG_ActiveCheck() {
				if (active && (obj == null)) {
					// [ERRH]
					ALog.LogError(DBG_InfoGet() + " has .active = true but has a .obj = <null>");
				}
			}

			//	[Conditional("ADEF_ADEG_DBG")]
			public string DBG_InfoGet (bool pFull = false) {
				return ("GMObject  ->  .objID: " + objID + ((obj == null) ? ", .obj: <null>": ", .obj.name: " + obj.name + ", .obj.ID: " + obj.GetInstanceID()) + ", .objType: " + objType + ", " + (pFull ? ", .objClass: " + objClass + ((player == null) ? ".player: <null>": ".player.ID: " + player.ID) : ""));
			}

			//	[Conditional("ADEF_ADEG_DBG")]
			public void DBG_Info (bool pFull = false) {
				ALog.LogDebug(DBG_InfoGet(pFull));
			}
		}

		public class Game_Ground : GMObject {
			private static int groundID_;

			public Vector3 positionHit;


			public static new void ResetIDs() {
				groundID_ = -1;
			}

			public Game_Ground (GameObject pGameObject, Vector3 pPosition, bool pActive) : base(pGameObject, GMObjectType.Ground, pActive) {
				ID = ++groundID_;

				positionHit = pPosition;
			}
		}

		public delegate void GMObject_ComponentUpdate();
		public class GMObject_Component : MonoBehaviour {
			public GMObject_ComponentUpdate componentUpdate;

			void Update () {
				if (componentUpdate != null) {
					componentUpdate();
				}
			}
		}

		public class Game_Unit : GMObject {
			private static int unitID_;
			public new Game_UnitState state {
				get {
					return state_.ToEnum<Game_UnitState>();
				}
				set {
					state_ = (int) value;
				}
			}

			public int sideID;
			public int teamID;


			public static new void ResetIDs() {
				unitID_ = -1;
			}

			public Game_Unit (GameObject pGameObject, int pUnitSideID, int pUnitTeamID, bool pActive) : base(pGameObject, GMObjectType.Unit, pActive) {
				ID = ++unitID_;

				sideID = pUnitSideID;
				teamID = pUnitTeamID;
			}
		}

		public class Game_Building : GMObject {
			private static int buildingID_;

			public int sideID;
			public int teamID;


			public static new void ResetIDs() {
				buildingID_ = -1;
			}

			public Game_Building (GameObject pGameObject, int pUnitSideID, int pUnitTeamID, bool pActive) : base(pGameObject, GMObjectType.Building, pActive) {
				ID = ++buildingID_;

				sideID = pUnitSideID;
				teamID = pUnitTeamID;
			}
		}

		public class Game_Resource : GMObject {
			private static int resourceID_;


			public static new void ResetIDs() {
				resourceID_ = -1;
			}

			public Game_Resource (GameObject pGameObject, Vector3 pPosition, bool pActive) : base(pGameObject, GMObjectType.Resource, pActive) {
				ID = ++resourceID_;

				position = pPosition;
			}
		}

		public class Game_Abstract : GMObject {
			private static int abstractID_;

			public int sideID;
			public int teamID;


			public static new void ResetIDs() {
				abstractID_ = -1;
			}

			public Game_Abstract (GameObject pGameObject, int pUnitSideID, int pUnitTeamID, bool pActive) : base(pGameObject, GMObjectType.Abstract, pActive) {
				ID = ++abstractID_;

				sideID = pUnitSideID;
				teamID = pUnitTeamID;
			}
		}

		public class Game_Projectile : GMObject {
			private static int projectileID_;


			public static new void ResetIDs() {
				projectileID_ = -1;
			}

			public Game_Projectile (GameObject pGameObject, Vector3 pPosition, bool pActive) : base(pGameObject, GMObjectType.Projectile, pActive) {
				ID = ++projectileID_;

				position = pPosition;
			}
		}

		/// <summary>
		/// [ENUM] game object class
		/// </summary>
		[DefaultValue(NONE)]
		public enum GMObjectClass {
			[Description("NONE")]
			NONE = -1,
			[Description("Persistent")]
			Persistent = 0,
			[Description("Temporary")]
			Temporary = 1,
		}

		/// <summary>
		/// [ENUM] game object types
		/// </summary>
		[DefaultValue(NONE)]
		public enum GMObjectType {
			[Description("NONE")]
			NONE = -1,
			[Description("Ground")]
			Ground = 0,
			[Description("Unit")]
			Unit = 1,	// GetComponent<Unit>();
			[Description("Building")]
			Building = 2,	// GetComponent<Building>();
			[Description("Resource")]
			Resource = 3,	// GetComponent<ResourceSite>();
			[Description("Abstract")]
			Abstract = 4,
			[Description("Projectile")]
			Projectile = 5,
		}

		/// <summary>
		/// [ENUM] game object states
		/// </summary>
		[DefaultValue(NONE)]
		public enum GMObjectState {
			[Description("NONE")]
			NONE = -1,
			[Description("Inactive")]
			Inactive = 0,
			[Description("Active")]
			Active = 1,
		}

		/// <summary>
		/// [ENUM] game unit states
		/// </summary>
		[DefaultValue(NONE)]
		public enum Game_UnitState {
			[Description("NONE")]
			NONE = -1,
			[Description("Inactive")]
			Inactive = 0,
			[Description("Active")]
			Active = 1,
		}

		public class Game_Client : MonoBehaviour {
			public ADEG adeg_;
			public ADEG adeg {
				get {
					return adeg_;
				}
			}

			public GameClientState state;

			public ADE_Client clientServer;
			public List<ADE_Client> clients;

			private GameClientType clientType_;
			public GameClientType clientType {
				get {
					if (clientType_ == GameClientType.NONE) {
						clientType_ = ((clientServer == null) ? GameClientType.NONE : (clientServer.CheckIfSame(adeg.gameUser.client) ? GameClientType.Primary : GameClientType.Secondary));
					}

					return clientType_;
				}
			}

			/// <summary>
			/// last command's ID used
			/// </summary>
			public int commandID;


			public void Awake_<TADEG>() where TADEG : ADEG {
				var v_obj = (GameObject) GameObject.Find(ADEG.def_ADEG_Name);
				if (v_obj != null) {
					adeg_ = v_obj.GetComponent<TADEG>();

					Reset();
				}
			}

			public bool CheckIfAdmin() {
				var v_admin = (bool) false;

				v_admin = (clientType == GameClientType.Primary);
				//v_admin = ADE.lobby.ClientCheckIfOwnerLocal();

				return v_admin;
			}

			public void CommandServerToClientSend (ADE_Client pClientTo, Game_Command pCommand, Location pLocation = Location.NONE) {
				ALog.LogDebug("[GS]  ->  t[" + adeg.gameManager.tickID + "] server self (" + adeg.gameUser.client.ToString() + ") sends to client: " + pClientTo.ToString() + " -> command: " + pCommand.ToString());

				if ( (pLocation == Location.Remote) || ((pLocation == Location.NONE) && ((pClientTo != null) && (!ADE.ClientCheckIfLocal(pClientTo) || ADEG.def_DBG_LocalToP2PComm))) ) {
					GameClientP2PDataSend(pClientTo, pCommand);
				}
				else /*if ((pLocation == Location.Local) || ((pLocation == Location.NONE) && clientServer.CheckIfSame(pClient)))*/ {
					ALog.LogDebug("[GC]  ->  t[" + adeg.gameManager.tickID + "] self (" + adeg.gameUser.client.ToString() + ") locally receives server client: " + pCommand.clientFrom.ToString() + " -> command: " + pCommand.ToString());
					pCommand.Execute(adeg);
				}
			}

			public void CommandClientToClientSend (ADE_Client pClientTo, Game_Command pCommand, Location pLocation = Location.NONE) {
				ALog.LogDebug("[GC]  ->  t[" + adeg.gameManager.tickID + "] self (" + adeg.gameUser.client.ToString() + ") sends to client: " + pClientTo.ToString() + " -> command: " + pCommand.ToString());

				pCommand.sID = -1;
				pCommand.lID = ++commandID;

				if ( (pLocation == Location.Remote) || ((pLocation == Location.NONE) && ((pClientTo != null) && (!ADE.ClientCheckIfLocal(pClientTo) || ADEG.def_DBG_LocalToP2PComm))) ) {
					GameClientP2PDataSend(pClientTo, pCommand);
				}
				else /*if ((pLocation == Location.Local) || ((pLocation == Location.NONE) && clientServer.CheckIfSame(pClientTo)))*/ {
					ALog.LogDebug("[GC]  ->  t[" + adeg.gameManager.tickID + "] self (" + adeg.gameUser.client.ToString() + ") locally receives client: " + pCommand.clientFrom.ToString() + " -> command: " + pCommand.ToString());
					if (pCommand.type.ToEnum<GameCommandType>() == GameCommandType.Action) {
						ALog.LogDebug("[GM]  ->  t[" + adeg.gameManager.tickID + "] self (" + adeg.gameUser.client.ToString() + ") locally receives player: " + adeg.gameManager.players.PlayerGet(((Game_Command.Action) pCommand).action.playerID) + " -> action: " + ((Game_Command.Action) pCommand).action.ToString());
					}
					pCommand.Execute(adeg);
				}
			}

			public void CommandClientToServerSend (ADE_Client pClientTo, Game_Command pCommand, Location pLocation = Location.NONE) {
				ALog.LogDebug("[GC]  ->  t[" + adeg.gameManager.tickID + "] self (" + adeg.gameUser.client.ToString() + ") sends to server client: " + pClientTo.ToString() + " -> command: " + pCommand.ToString());

				pCommand.sID = -1;
				pCommand.lID = ++commandID;

				if ( (pLocation == Location.Remote) || ((pLocation == Location.NONE) && ((clientServer != null) && (!ADE.ClientCheckIfLocal(clientServer) || ADEG.def_DBG_LocalToP2PComm))) ) {
					GameClientP2PDataSend(pClientTo, pCommand);
				}
				else /*if ((pLocation == Location.Local) || ((pLocation == Location.NONE) && clientServer.CheckIfSame(pClientTo)))*/ {
					ALog.LogDebug("[GS]  ->  t[" + adeg.gameManager.tickID + "] self (" + adeg.gameUser.client.ToString() + ") locally receives client: " + pCommand.clientFrom.ToString() + " -> command: " + pCommand.ToString());
					pCommand.Execute(adeg);
				}
			}

			public void ActionsSend (List<Game_Action> pListGameActions, Location pLocation = Location.NONE) {
				if (pListGameActions != null) {
					for (int i=0; i<pListGameActions.Count; i++) {
						for (int j=0; j<clients.Count; j++) {
							ALog.Log("[P[" + pListGameActions[i].playerID + "]]  ->  t[" + adeg.gameManager.tickID + "] self (" + adeg.gameManager.players.PlayerGet(pListGameActions[i].playerID).ToString() + ") distributes his action: " + pListGameActions[i].ToString() + " to client: " + clients[j].ToString());

							ALog.LogDebug("[CSCOMM][] - Client->Client[*]  ->  GameCommandType.Action");
							CommandClientToClientSend(clients[j], new Game_Command.Action(-1, ++adeg.gameClient.commandID, Time.time, adeg.gameUser.client, pListGameActions[i]));
						}
					}
				}
			}

			public void GameClientP2PDataSend (ADE_Client pClientTo, Game_Command pCommand) {
				ALog.LogDebug("P2PDataSend(" + pCommand.clientFrom.name + " ->" + pClientTo.name + ") -> command: " + pCommand.category);
				ADE.comm.P2PDataSend(pClientTo.aID, pCommand.ToData(), ADE_CommDataType.Data);
			}

			public virtual void GameClientP2PDataSendFailure (ADE_ID pClientID) {
				ALog.LogError("[CSCOMM]  ->  outbound communication failure", DBGC.DBG);

				adeg.gameManager.StartCoroutine(adeg.gameManager.GameStop());

				ALog.LogErrorHandling("[INIT] - .. falling back to local");
				ADE.instance.platformNetworking = ADE_Platform.Local;
				ADE.instance.InitPlatformLocal();
			}

			public void GameClientP2PDataReceived (ADE_ID pClientID, byte[] pData) {
				var v_offset = (int) 0;
				var v_command = (Game_Command) CommandNew(pData, ref v_offset, ref adeg.gameLobby.lobby.clients);
				var v_clientFrom = (ADE_Client) clients.ClientGet(pClientID);
				ALog.LogDebug("[GC]  ->  t[" + adeg.gameManager.tickID + "] self (" + adeg.gameUser.client.ToString() + ") remotely receives " + ((v_clientFrom != null) ? "client: " + v_clientFrom.ToString() : "new client: " + pClientID.ID.ToString()) + " -> command: " + v_command.ToString());

				ALog.LogDebug(((v_command.mode == GameCommandMode.GameServer) ? "[GS]  ->  " : "[GM]  ->  ") + "t[" + adeg.gameManager.tickID + "] " + ((v_command.mode == GameCommandMode.GameServer) ? "server" : "self") + " (" + adeg.gameUser.client.ToString() + ") executes command: " + ToString());
				v_command.Execute(adeg);
			}

			public virtual void GameClientP2PDataReceivedSuccess (ADE_ID pClientID) {
				ALog.LogDebug("[CSCOMM]  ->  inbound communication sucess", DBGV.HIDDEN);

				var v_clientFrom = (ADE_Client) clients.ClientGet(pClientID);
				if (v_clientFrom != null) {
					v_clientFrom.timeFromLastComm = 0f;
				}
			}

			public virtual void GameClientP2PDataReceivedFailure() {
				ALog.LogError("[CSCOMM]  ->  inbound communication failure", DBGC.DBG);

				adeg.gameManager.StartCoroutine(adeg.gameManager.GameStop());

				ALog.LogErrorHandling("[INIT] - .. falling back to local");
				ADE.instance.platformNetworking = ADE_Platform.Local;
				ADE.instance.InitPlatformLocal();
			}

			public Game_Command CommandNew (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) {
				var v_command = (Game_Command) null;

				// first byte[] pData's int is for the GameCommandCategory, and the second one is for the GameCommandType
				var v_category = (GameCommandCategory) BitConverter.ToInt32(pData, pOffset).ToEnum<GameCommandCategory>();
				//pOffset += sizeof(int);
				var v_type = BitConverter.ToInt32(pData, pOffset + sizeof(int));
				//pOffset += sizeof(int);
				switch (v_category) {
					case GameCommandCategory.Base: {
						switch ((GameCommandType) v_type) {
							case GameCommandType.ServerActivateRequest: {
								v_command = new Game_Command.ServerActivateRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.ServerActivateResponse: {
								v_command = new Game_Command.ServerActivateResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.ServerGetRequest: {
								v_command = new Game_Command.ServerGetRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.ServerGetResponse: {
								v_command = new Game_Command.ServerGetResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.ServerPasswordUpdateRequest: {
								v_command = new Game_Command.ServerPasswordUpdateRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.ServerPasswordUpdateResponse: {
								v_command = new Game_Command.ServerPasswordUpdateResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameDataUpdateRequest: {
								v_command = new Game_Command.GameDataUpdateRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameDataUpdateResponse: {
								v_command = new Game_Command.GameDataUpdateResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameDataUpdate: {
								v_command = new Game_Command.GameDataUpdate(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameLoadRequest: {
								v_command = new Game_Command.GameLoadRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameLoadResponse: {
								v_command = new Game_Command.GameLoadResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameInitRequest: {
								v_command = new Game_Command.GameInitRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameInitResponse: {
								v_command = new Game_Command.GameInitResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameStartRequest: {
								v_command = new Game_Command.GameStartRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameStartResponse: {
								v_command = new Game_Command.GameStartResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameStart: {
								v_command = new Game_Command.GameStart(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameStopRequest: {
								v_command = new Game_Command.GameStopRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameStopResponse: {
								v_command = new Game_Command.GameStopResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameStop: {
								v_command = new Game_Command.GameStop(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameEndRequest: {
								v_command = new Game_Command.GameEndRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameEndResponse: {
								v_command = new Game_Command.GameEndResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.GameEnd: {
								v_command = new Game_Command.GameEnd(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.TimeSyncRequest: {
								v_command = new Game_Command.TimeSyncRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.TimeSyncResponse: {
								v_command = new Game_Command.TimeSyncResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.TimeSync: {
								v_command = new Game_Command.TimeSync(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.TimeSyncAck: {
								v_command = new Game_Command.TimeSyncAck(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PingRequest: {
								v_command = new Game_Command.PingRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PingResponse: {
								v_command = new Game_Command.PingResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerLoginRequest: {
								v_command = new Game_Command.PlayerLoginRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerLoginResponse: {
								v_command = new Game_Command.PlayerLoginResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerLogin: {
								v_command = new Game_Command.PlayerLogin(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerLoginAck: {
								v_command = new Game_Command.PlayerLoginAck(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerStateUpdateRequest: {
								v_command = new Game_Command.PlayerStateUpdateRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerStateUpdateResponse: {
								v_command = new Game_Command.PlayerStateUpdateResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerStateUpdate: {
								v_command = new Game_Command.PlayerStateUpdate(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerDataUpdateRequest: {
								v_command = new Game_Command.PlayerDataUpdateRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerDataUpdateResponse: {
								v_command = new Game_Command.PlayerDataUpdateResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerDataUpdate: {
								v_command = new Game_Command.PlayerDataUpdate(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerGameStartRequest: {
								v_command = new Game_Command.PlayerGameStartRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerGameStartResponse: {
								v_command = new Game_Command.PlayerGameStartResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerInit: {
								v_command = new Game_Command.PlayerInit(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerLeaveRequest: {
								v_command = new Game_Command.PlayerLeaveRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerLeaveResponse: {
								v_command = new Game_Command.PlayerLeaveResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerLeave: {
								v_command = new Game_Command.PlayerLeave(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerKickRequest: {
								v_command = new Game_Command.PlayerKickRequest(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerKickResponse: {
								v_command = new Game_Command.PlayerKickResponse(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.PlayerKick: {
								v_command = new Game_Command.PlayerKick(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.Data: {
								v_command = new Game_Command.Data(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.DataAck: {
								v_command = new Game_Command.DataAck(pData, ref pOffset, ref pClientsList);
							}
								break;
							case GameCommandType.Action: {
								v_command = new Game_Command.Action(pData, ref pOffset, ref pClientsList, adeg);
							}
								break;
						}
					}
						break;
					case GameCommandCategory.Extended:
					case GameCommandCategory.Custom: {
						v_command = CommandNewCustom(pData, ref pOffset, ref pClientsList);
					}
						break;
				}

				return v_command;
			}

			public virtual Game_Command CommandNewCustom (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) {
				return null;
			}

			public void Reset() {
				state = GameClientState.Inactive;

				clientServer = null;
				if (clients == null) {
					clients = new List<ADE_Client>();
				}
				else {
					clients.Clear();
				}

				clientType_ = GameClientType.NONE;

				commandID = -1;
			}

			#region [WIP] -> Game_Client	
			/// <summary>
			/// [DUMMY]
			/// </summary>
			public void LobbyClientDataInit (ADE_Client pClient) {
				//ALog.Log("LobbyClientDataInit() call");
				return;

				if (pClient != null) {
					var v_players = (List<Game_Player>) adeg.gameManager.players.PlayersGet(pClient);
					for (int i=0; i<v_players.Count; i++) {
						ALog.LogDebug("[ADE]  ->  client: " + pClient.ToString() + " new player: " + v_players[i].ToString() + " data init");

		//				v_players[i].state = (ADE.lobby.ClientCheckIfOwner(v_players[i].client) ? GamePlayerState.Seated : GamePlayerState.Spectating);
		//				SteamMatchmaking.SetLobbyMemberData(adeg.gameLobby.lobby.CSID, "state", ((int) v_players[i].state).ToString());
		//				v_players[i].rand = UnityEngine.Random.Range(1, 101);
		//				SteamMatchmaking.SetLobbyMemberData(adeg.gameLobby.lobby.CSID, "number", v_players[i].rand.ToString());
					}
				}
				else {
					ALog.LogErrorHandling(DBGC.TODO);
				}
			}

			/// <summary>
			/// [DUMMY]
			/// </summary>
			public void LobbyClientDataGet (ADE_Client pClient) {
				//ALog.Log("LobbyClientDataGet() call");
				return;

				if (!adeg.gameLobby.connecting) {
					if (pClient != null) {
						var v_players = (List<Game_Player>) adeg.gameManager.players.PlayersGet(pClient);
						for (int i=0; i<v_players.Count; i++) {
							ALog.LogDebug("[ADE]  ->  client: " + pClient.ToString() + " player: " + v_players[i].ToString() + " data refreshed");

		//					var v_int = (int) 0;
		//					int.TryParse(SteamMatchmaking.GetLobbyMemberData(adeg.gameLobby.lobby.CSID, v_players[i].client.CSID, "state"), out v_int);
		//					v_players[i].state = v_int.ToEnum<GamePlayerState>();
		//					int.TryParse(SteamMatchmaking.GetLobbyMemberData(adeg.gameLobby.lobby.CSID, v_players[i].client.CSID, "number"), out v_int);
		//					v_players[i].rand = v_int;
						}
					}
					else {
						ALog.LogErrorHandling(DBGC.TODO);
					}
				}
			}

			/// <summary>
			/// [DUMMY]
			/// </summary>
			public void LobbyClientJoined (ADE_ID pClientID) {
				//ALog.Log("LobbyClientJoined() call");
				return;

				var v_client = (ADE_Client) adeg.gameLobby.lobby.ClientGet(pClientID);
				if (v_client != null) {
					ALog.LogDebug("[ADE]  ->  client joined: " + v_client.ToString());
				}
			}

			/// <summary>
			/// [DUMMY]
			/// </summary>
			public bool LobbyClientLeft (ADE_ID pClientID) {
				//ALog.LogError("LobbyClientLeft() call");

				var v_client = (ADE_Client) clients.ClientGet(pClientID);
				if (v_client != null) {
					if (adeg.gameUser.client.CheckIfSame(clientServer)) {
						var v_player = (Game_ServerPlayer) adeg.gameServer.gsPlayers.Find(x => x.gsClient.client.CheckIfSame(v_client));
						if (v_player != null) {
							ALog.LogTag("[WIP][000] - client dropped out without leaving");
							ALog.LogError("[ADE]  ->  client left: " + v_client.ToString(), DBGC.DBG);
							adeg.gameLobby.lobby.MessageAdd("[" + Time.time + "] " + (ADE.def_ADE_Lobby_CustomMessageTypesUse ? "[" + ADE_LobbyMessageType.Info + "] " : "") + "[GS] player " + v_player.name + "(" + v_player.gsClient.client.ToString() + ") disconnected!", ADE_MessageCategory.Development);
						}
						else {
							ALog.LogDebug("[ADE]  ->  client left: " + v_client.ToString());
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}
				}

				return (v_client != null);
			}
			#endregion
		}

		/// <summary>
		/// [ENUM] game client types
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameClientType {
			[Description("NONE")]
			NONE = -1,
			[Description("Primary")]
			Primary = 0,
			[Description("Secondary")]
			Secondary = 1,
		}

		/// <summary>
		/// [ENUM] game client states
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameClientState {
			[Description("NONE")]
			NONE = -1,
			[Description("Inactive")]
			Inactive = 0,
			[Description("Active")]
			Active = 1,
		}

		public class Game_Lobby {
			public ADE_Lobby lobby;
			public bool connecting;

			public ulong clientServerID;
			public ADE_Platform clientServerPlatform;
			public ADE_Client clientServer;
			public string password;
			public string gameName;
			public int gameMap;

			public Game_Lobby() {
				lobby = ADE.lobby;

				Reset();
			}


			public void GameLobbyDataInit() {
				//ALog.Log("GameLobbyDataInit() call");

				switch (ADE.instance.platformNetworking) {
					case ADE_Platform.Steam: {
						SteamMatchmaking.SetLobbyOwner(lobby.aID.CSID, ADE.clientLocal.aID.CSID);
						ALog.LogDebug("[Steam] SteamMatchmaking.SetLobbyOwner(" + lobby.aID.ID + ", " + ADE.clientLocal.ID.ToString() + ")");
						SteamMatchmaking.SetLobbyMemberLimit(lobby.aID.CSID, ADE.def_ADE_Lobby_ClientsNoMAX);
						ALog.LogDebug("[Steam] SteamMatchmaking.SetLobbyMemberLimit(" + lobby.aID.ID + ", " + ADE.def_ADE_Lobby_ClientsNoMAX + ")");
					}
						break;
					case ADE_Platform.GoG: {
		//				GalaxyInstance.Matchmaking().SetLobbyOwner(lobby.aID.GID, ADE.clientLocal.ID);
		//				ALog.LogDebug([GoG] "IMatchmaking.SetLobbyOwner(" + lobby.aID.ID + ", " + ADE.clientLocal.ID.ToString() + ")");
						GalaxyInstance.Matchmaking().SetMaxNumLobbyMembers(lobby.aID.GID, (uint) ADE.def_ADE_Lobby_ClientsNoMAX);
						ALog.LogDebug("[GoG] IMatchmaking.SetMaxNumLobbyMembers(" + lobby.aID.ID + ", " + ADE.def_ADE_Lobby_ClientsNoMAX + ")");
					}
						break;
				}

		//		lobby.DataSet("serverID", ADE.clientLocal.ID.ToString());
		//		lobby.DataSet("serverPlatform", ((int) ADE.clientLocal.platform).ToString());
		//		lobby.DataSet("locked", (!string.IsNullOrEmpty(password) ? "1" : "0"));
				lobby.DataSet("ready", "0");
		//		lobby.DataSet("name", gameName);
		//		lobby.DataSet("map", gameMap.ToString("0000"));
			}

			/// <summary>
			/// [DUMMY]
			/// </summary>
			public void GameLobbyJoined (ADE_Lobby pLobby) {
				//ALog.Log("GameLobbyJoined() call");
				return;

				if (lobby == pLobby) {
				}
				else {
					ALog.LogErrorHandling(DBGC.TODO);
				}
			}

			public void GameLobbyDataGet() {
				//ALog.Log("GameLobbyDataGet() call");

				if (!connecting) {
					ulong.TryParse(ADE.lList.LobbyDataGet(ADE.lobby.aID, "serverID"), out clientServerID);
					var v_clientServerPlatform = (int) ADE_Platform.NONE;
					Int32.TryParse(ADE.lList.LobbyDataGet(ADE.lobby.aID, "serverPlatform"), out v_clientServerPlatform);
					clientServerPlatform = v_clientServerPlatform.ToEnum<ADE_Platform>();
					clientServer = ADE.lobby.clients.ClientGet(clientServerID, clientServerPlatform);
					var v_locked = !ADE.lList.LobbyDataGet(ADE.lobby.aID, "locked").Equals("0");
					var v_ready = !ADE.lList.LobbyDataGet(ADE.lobby.aID, "ready").Equals("0");
					gameName = ADE.lList.LobbyDataGet(ADE.lobby.aID, "name");
					int.TryParse(ADE.lList.LobbyDataGet(ADE.lobby.aID, "map"), out gameMap);
				}
			}

			public void Reset() {
				lobby = ADE.lobby;
				connecting = false;

				clientServerID = 0;
				clientServerPlatform = ADE_Platform.NONE;
				clientServer = null;
				password = null;
				gameName = null;
				gameMap = -1;
			}
		}

		public class Game_Server : MonoBehaviour {
			public ADEG adeg_;
			public ADEG adeg {
				get {
					return adeg_;
				}
			}

			public GameServerState state;

			public ADE_Client clientServer;
			public Game_ServerClient gsClientPrimary;
			public List<Game_ServerClient> gsClients;
			public List<Game_ServerPlayer> gsPlayers;

			public GameState gameState;
			public GameMode gameMode;
			public GameplayMode gameGPMode;

			public string password;
			public string gameName;
			public int gameMap;

			/// <summary>
			/// last server command's ID used
			/// </summary>
			public int commandID;
			/// <summary>
			/// last player's ID used
			/// </summary>
			public int playerID;
			/// <summary>
			/// last side's ID used
			/// </summary>
			public int sideID;
			public float timeLobby;
			public float timeLobbyReal;
			public float timeGame;
			public float timeGameReal;

		// lockstep variables
			/// <summary>
			/// last used tick ID
			/// </summary>
			public int tickID;
			public float tickTimeMAX;
			public float tickTime;


			public void Awake_<TADEG>() where TADEG : ADEG {
				var v_obj = (GameObject) GameObject.Find(ADEG.def_ADEG_Name);
				if (v_obj != null) {
					adeg_ = v_obj.GetComponent<TADEG>();

					clientServer = adeg.gameUser.client;

					Reset();
				}
			}

			void FixedUpdate() {
				FixedUpdate_();
			}

			public void FixedUpdate_() {
				timeLobby += Time.deltaTime;

				if (gameState == GameState.Playing) {
					//ALog.LogError("FixedUpdate()  ->  Time.deltaTime: " + Time.deltaTime + " Time.fixedDeltaTime: " + Time.fixedDeltaTime, DBGC.DBG);

					timeGame += Time.deltaTime;

					tickTime += Time.deltaTime;
					if (tickTime > tickTimeMAX) {
						tickTime = 0f;
						tickID++;
					}
				}

				ALog.LogTag("[WIP][000] - client/server ping request/response");
				if (ADEG.dbg_commTimeoutActive && ((gameState == GameState.Lobby) || (gameState == GameState.Playing))) {
					for (int i=0; i<gsClients.Count; i++) {
						if (!gsClients[i].client.CheckIfSame(clientServer)) {
							gsClients[i].client.timeFromLastComm += Time.deltaTime;

							if (gsClients[i].client.timeFromLastComm > ADEG.def_clientTimePingMAXDefault) {
								ALog.LogErrorHandling("[GS]  ->  t[" + adeg.gameManager.tickID + "] client: " + gsClients[i].client.ToString() + " communication last: " + gsClients[i].client.timeFromLastComm);
								ALog.LogDebug("[CSCOMM][0710] - Server->Client  ->  GameCommandType.PingRequest");
								adeg.gameClient.CommandServerToClientSend(gsPlayers[i].gsClient.client, new Game_Command.PingRequest(-1, ++adeg.gameClient.commandID, ((gameState == GameState.Playing) ? timeGame : timeLobby), clientServer, -1, 0));

								if (gsClients[i].client.timeFromLastComm > ADEG.def_clientTimeTimeoutMAXDefault) {
									ALog.LogTag("[WIP][000] - comm lost");
									ALog.LogError("[GS]  ->  t[" + adeg.gameManager.tickID + "] client: " + gsClients[i].client.ToString() + " communication timeout: " + gsClients[i].client.timeFromLastComm, DBGC.DBG);

									adeg.gameManager.StartCoroutine(adeg.gameManager.GameStop());
								}
							}
						}
					}
				}
			}

			public void Reset() {
				state = GameServerState.Inactive;

				clientServer = adeg.gameUser.client;
				gsClientPrimary = null;
				if (gsClients == null) {
					gsClients = new List<Game_ServerClient>();
				}
				else {
					gsClients.Clear();
				}
				if (gsPlayers == null) {
					gsPlayers = new List<Game_ServerPlayer>();
				}
				else {
					gsPlayers.Clear();
				}

				gameState = GameState.NONE;
				gameMode = GameMode.NONE;
				gameGPMode = GameplayMode.NONE;
				password = "";
				gameName = "";
				gameMap = -1;

				commandID = -1;
				playerID = -1;
				sideID = -1;
				timeLobby = 0f;
				timeLobbyReal = 0f;
				timeGame = 0f;
				timeGameReal = 0f;

				tickID = 0;
				tickTimeMAX = ADEG.def_tickTimeMAXDefault;
				tickTime = 0f;
			}

			#region [WIP] -> Game_Server
			/// <summary>
			/// [DUMMY]
			/// </summary>
			public void GameLobbyCreated (ADE_Lobby pLobby) {
				//ALog.Log("GameLobbyCreated() call");
				return;

				if (adeg.gameLobby.lobby == pLobby) {
				}
				else {
					ALog.LogErrorHandling(DBGC.TODO);
				}
			}
			#endregion
		}

		public class Game_ServerClient {
			public ADE_Client client;
			public GameClientType type;
			public GameClientState state;

			public GameState gameState;

			public Game_ServerClient (ADE_Client pClient, GameClientType pClientType) {
				client = pClient;
				type = pClientType;
				state = GameClientState.Active;

				gameState = GameState.Active;
			}
		}

		public class Game_ServerPlayer {
			public int ID;
			public Game_ServerClient gsClient;
			public GamePlayerType type;
			public string name;

			public GamePlayerState playerState;
			public int sideID;
			public int playerNumber;

			public Game_ServerPlayer (int pPlayerID, Game_ServerClient pGSClient, GamePlayerType pPlayerType, string pPlayerName, int pSideID) {
				ID = pPlayerID;
				gsClient = pGSClient;
				type = pPlayerType;
				name = pPlayerName;

				playerState = GamePlayerState.NONE;
				sideID = pSideID;
				playerNumber = -1;
			}
		}

		/// <summary>
		/// [ENUM] game server states
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameServerState {
			[Description("NONE")]
			NONE = -1,
			[Description("Inactive")]
			Inactive = 0,
			[Description("Active")]
			Active = 1,
		}


		public class Game_Command {
			public GameCommandCategory category;
			public int type;
			public GameCommandMode mode;
			/// <summary>
			/// command's server ID
			/// </summary>
			public int sID;
			/// <summary>
			/// command's local ID
			/// </summary>
			public int lID;

			public float timeLobby;
			public GameCommandState state;
			public ulong clientFromID;
			public ADE_Platform clientFromPlatform;
			public ADE_Client clientFrom;
			/// <summary>
			/// command's player ID
			/// </summary>
			//public int pID;

			/// <summary>
			/// command's error ID
			/// </summary>
			public int eID;

			public Game_Command (GameCommandCategory pCategory, int pType, GameCommandMode pMode, int pSDI, int pLID, float pTimeLobby, ADE_Client pClientFrom) {
				category = pCategory;
				type = pType;
				mode = pMode;
				sID = -1;
				lID = -1;
				timeLobby = pTimeLobby;
				state = GameCommandState.NONE;
				clientFrom = pClientFrom;
				//pID = pPlayerID;

				eID = -1;
			}

			public Game_Command (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) {
				//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.Game_Command() - pOffset: " + pOffset);
				category = (GameCommandCategory) BitConverter.ToInt32(pData, pOffset).ToEnum<GameCommandCategory>();
				pOffset += sizeof(int);
				type = BitConverter.ToInt32(pData, pOffset);
				pOffset += sizeof(int);
				mode = (GameCommandMode) BitConverter.ToInt32(pData, pOffset).ToEnum<GameCommandMode>();
				pOffset += sizeof(int);
				sID = BitConverter.ToInt32(pData, pOffset);
				pOffset += sizeof(int);
				lID = BitConverter.ToInt32(pData, pOffset);
				pOffset += sizeof(int);
				timeLobby = BitConverter.ToSingle(pData, pOffset);
				pOffset += sizeof(float);
				state = (GameCommandState) BitConverter.ToInt32(pData, pOffset).ToEnum<GameCommandState>();
				pOffset += sizeof(int);
				clientFromID = (ulong) BitConverter.ToUInt64(pData, pOffset);
				pOffset += sizeof(ulong);
				clientFromPlatform = (ADE_Platform) BitConverter.ToInt32(pData, pOffset).ToEnum<ADE_Platform>();
				pOffset += sizeof(int);
				clientFrom = pClientsList.ClientGet(clientFromID, clientFromPlatform);
				if (clientFrom == null) {
					ALog.LogError("GameClientP2PDataReceived() -> Game_Command.CommandNew()  ->  no lobby client for command: " + ToString());
				}
				eID = BitConverter.ToInt32(pData, pOffset);
				pOffset += sizeof(int);
			}

			public virtual byte[] ToData() {
				return ToDataBase();
			}

			public byte[] ToDataBase() {
				var v_dataCommandCount = (int) 10;
				var v_dataCommand = (byte[][]) new byte[v_dataCommandCount][];
				v_dataCommand[0] = (byte[]) BitConverter.GetBytes((int) category);
				v_dataCommand[1] = (byte[]) BitConverter.GetBytes(type);
				v_dataCommand[2] = (byte[]) BitConverter.GetBytes((int) mode);
				v_dataCommand[3] = (byte[]) BitConverter.GetBytes(sID);
				v_dataCommand[4] = (byte[]) BitConverter.GetBytes(lID);
				v_dataCommand[5] = (byte[]) BitConverter.GetBytes(timeLobby);
				v_dataCommand[6] = (byte[]) BitConverter.GetBytes((int) state);
				v_dataCommand[7] = (byte[]) BitConverter.GetBytes(clientFrom.ID);
				v_dataCommand[8] = (byte[]) BitConverter.GetBytes((int) clientFrom.platform);
				v_dataCommand[9] = (byte[]) BitConverter.GetBytes(eID);

				// computing total data length
				var v_dataLength = (int) 0;
				for (int i=0; i<v_dataCommand.GetLength(0); i++) {
					v_dataLength += v_dataCommand[i].Length;
				}

				// filling up the data
				var v_data = (byte[]) new byte[v_dataLength];
				var v_offset = (int) 0;
				for (int i=0; i<v_dataCommand.GetLength(0); i++) {
					for (int j=0; j<v_dataCommand[i].Length; j++) {
						v_data[v_offset +j] = v_dataCommand[i][j];
					}
					v_offset += v_dataCommand[i].Length;
				}

				return v_data;
			}

			public byte[] ToDataAll (byte[][] pDataCommandAdditional) {
				var v_dataCommand = (byte[]) ToDataBase();

				// computing total data length
				var v_dataLength = (int) v_dataCommand.Length;
				for (int i=0; i<pDataCommandAdditional.GetLength(0); i++) {
					v_dataLength += pDataCommandAdditional[i].Length;
				}

				// filling up the data
				var v_data = (byte[]) new byte[v_dataLength];
				var v_offset = (int) 0;
				//v_dataCommand.CopyTo(v_data, 0);
				for (int i=0; i<v_dataCommand.Length; i++) {
					v_data[v_offset +i] = v_dataCommand[i];
				}
				v_offset += v_dataCommand.Length;
				for (int i=0; i<pDataCommandAdditional.GetLength(0); i++) {
					for (int j=0; j<pDataCommandAdditional[i].Length; j++) {
						v_data[v_offset +j] = pDataCommandAdditional[i][j];
					}
					v_offset += pDataCommandAdditional[i].Length;
				}

				return v_data;
			}

			public virtual bool Execute (ADEG pADEG) {
				ALog.LogDebug(((mode == GameCommandMode.GameServer) ? "[GS]  ->  " : "[GM]  ->  ") + "t[" + pADEG.gameManager.tickID + "] " + ((mode == GameCommandMode.GameServer) ? "server" : "self") + " (" + pADEG.gameUser.client.ToString() + ") executes command: " + ToString());

				return true;
			}

			public virtual void ExecuteCustomResponseSuccess (ADEG pADEG) {
			}

			public virtual void ExecuteCustomResponseFail (ADEG pADEG) {
			}

			public override string ToString() {
				return ("[" + sID + "][" + lID + "](" + timeLobby + ") category: " + category + ", type: " + type + ", mode: " + mode + ", state: " + state + ", sender: " + ((clientFrom != null) ? clientFrom.ToString() : "<null>(" + clientFromID.ToString() + ", " + clientFromPlatform.ToString() + ")") + " (eID: " + eID + ")");
			}


			#region game commands
			public class ServerActivateRequest : Game_Command {
				public GameMode gMode;
				public GameplayMode gpMode;
				public string password;

				public ServerActivateRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameMode pGameMode, GameplayMode pGameplayMode, string pPassword, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.ServerActivateRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					gMode = pGameMode;
					gpMode = pGameplayMode;
					password = pPassword;
					eID = pEID;
				}

				public ServerActivateRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.ServerActivateRequest() - pOffset: " + pOffset);
					gMode = (GameMode) BitConverter.ToInt32(pData, pOffset).ToEnum<GameMode>();
					pOffset += sizeof(int);
					gpMode = (GameplayMode) BitConverter.ToInt32(pData, pOffset).ToEnum<GameplayMode>();
					pOffset += sizeof(int);
					password = G.DataToString(pData, ref pOffset);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 3;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes((int) gMode);
					v_dataCommandAdditional[1] = BitConverter.GetBytes((int) gpMode);
					v_dataCommandAdditional[2] = G.StringToData(password);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0001] -   Server<-Client  <-  GameCommandType.ServerActivateRequest");
					ALog.LogDebug(((mode == GameCommandMode.GameServer) ? "[GS]  ->  " : "[GM]  ->  ") + "t[" + pADEG.gameManager.tickID + "] " + ((mode == GameCommandMode.GameServer) ? "server" : "self") + " (" + pADEG.gameUser.client.ToString() + ") executes command: " + ToString());

					var v_exec = (Game_Server) pADEG.gameServer;
					if (v_exec.state == GameServerState.Inactive) {
						v_exec.state = GameServerState.Active;
						v_exec.gameState = GameState.Active;
						v_exec.gameMode = gMode;
						v_exec.gameGPMode = gpMode;
						v_exec.password = password;
						v_exec.timeLobby = 0f;
						v_exec.timeLobbyReal = Time.time;
						v_exec.timeGame = 0f;
						v_exec.timeGameReal = 0f;

						pADEG.gameLobby.lobby.DataSet("serverID", v_exec.clientServer.ID.ToString());
						pADEG.gameLobby.lobby.DataSet("serverPlatform", ((int) v_exec.clientServer.platform).ToString());
						pADEG.gameLobby.lobby.DataSet("locked", (!string.IsNullOrEmpty(v_exec.password) ? "1" : "0"));

						ExecuteCustomResponseSuccess(pADEG);

						ALog.LogDebug("[CSCOMM][0002] -   Server->Client  ->  GameCommandType.ServerActivateResponse");
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.ServerActivateResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK, this));
					}
					else {
						ExecuteCustomResponseFail(pADEG);

						ALog.LogDebug("[CSCOMM][0002] -   Server->Client  ->  GameCommandType.ServerActivateResponse");
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.ServerActivateResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, this, 1));
					}

					return true;
				}
			}

			public class ServerActivateResponse : Game_Command {
				public GameMode gMode;
				public GameplayMode gpMode;

				public ServerActivateResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, Game_Command.ServerActivateRequest pCommandRequest, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.ServerActivateResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					gMode = pCommandRequest.gMode;
					gpMode = pCommandRequest.gpMode;
					eID = pEID;
				}

				public ServerActivateResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.ServerActivateResponse() - pOffset: " + pOffset);
					gMode = (GameMode) BitConverter.ToInt32(pData, pOffset).ToEnum<GameMode>();
					pOffset += sizeof(int);
					gpMode = (GameplayMode) BitConverter.ToInt32(pData, pOffset).ToEnum<GameplayMode>();
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 2;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes((int) gMode);
					v_dataCommandAdditional[1] = BitConverter.GetBytes((int) gpMode);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0003] -     Client<-Server  <-  GameCommandType.ServerActivateResponse");
					ALog.LogDebug(((mode == GameCommandMode.GameServer) ? "[GS]  ->  " : "[GM]  ->  ") + "t[" + pADEG.gameManager.tickID + "] - " + ((mode == GameCommandMode.GameServer) ? "server" : "self") + " (" + pADEG.gameUser.client.ToString() + ") executes command: " + ToString());

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (pADEG.gameClient.state == GameClientState.Inactive) {
						if (state == GameCommandState.ResponseOK) {
							// activate the game manager
							v_exec.gameState = GameState.Active;
							// activate and set the game client
							pADEG.gameClient.state = GameClientState.Active;
							pADEG.gameClient.clientServer = clientFrom;
							pADEG.gameClient.commandID = 0;

							lID = ++pADEG.gameClient.commandID;
						}
						else /*if (state == GameCommandState.ResponseError)*/ {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}
					else {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class ServerGetRequest : Game_Command {

				public ServerGetRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.ServerGetRequest, GameCommandMode.GameServer/*GameCommandMode.GameManager*/, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					eID = pEID;
				}

				public ServerGetRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					switch (mode) {
						case GameCommandMode.GameServer: {
							ALog.LogDebug("[CSCOMM][0101] -   Server<-Client  <-  GameCommandType.ServerGetRequest");

							var v_exec = (Game_Server) pADEG.gameServer;
							if ((v_exec.state == GameServerState.Active) && (v_exec.gameState == GameState.Lobby)) {
								ALog.LogDebug("[CSCOMM][0102] -   Server->Client  ->  GameCommandType.ServerGetResponse");
								pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.ServerGetResponse(-1, ++pADEG.gameClient.commandID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK, pADEG.gameClient.clientServer, mode));
							}
							else {
								ALog.LogErrorHandling("[CSCOMM][0102] -   Server->Client  ->  GameCommandType.ServerGetResponse", DBGC.DBG);
								pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.ServerGetResponse(-1, ++pADEG.gameClient.commandID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, pADEG.gameClient.clientServer, mode, 1));
							}
						}
							break;
						case GameCommandMode.GameManager: {
							ALog.LogDebug("[CSCOMM][0101] -   Client<-Client  <-  GameCommandType.ServerGetRequest");

							var v_exec = (Game_Manager) pADEG.gameManager;
							if (state == GameCommandState.Request) {
								ALog.LogDebug("[CSCOMM][0102] -   Client->Client  ->  GameCommandType.ServerGetResponse");
								pADEG.gameClient.CommandClientToClientSend(clientFrom, new Game_Command.ServerGetResponse(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, GameCommandState.ResponseOK, pADEG.gameClient.clientServer, mode));
							}
						}
							break;
					}

					return true;
				}
			}

			public class ServerGetResponse : Game_Command {
				public ADE_Client clientServer;
				public GameCommandMode requestCommandMode;

				public ServerGetResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, ADE_Client pClientServer, GameCommandMode pRequestCommandMode, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.ServerGetResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					clientServer = pClientServer;
					requestCommandMode = pRequestCommandMode;
					eID = pEID;
				}

				public ServerGetResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.ServerGetResponse() - pOffset: " + pOffset);
					var v_clientServerID = (ulong) BitConverter.ToUInt64(pData, pOffset);
					pOffset += sizeof(ulong);
					var v_clientServerPlatform = (ADE_Platform) BitConverter.ToInt32(pData, pOffset).ToEnum<ADE_Platform>();
					pOffset += sizeof(int);
					clientServer = pClientsList.ClientGet(v_clientServerID, v_clientServerPlatform);
					requestCommandMode = (GameCommandMode) BitConverter.ToInt32(pData, pOffset).ToEnum<GameCommandMode>();
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 3;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(clientServer.ID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes((int) clientServer.platform);
					v_dataCommandAdditional[2] = BitConverter.GetBytes((int) requestCommandMode);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					switch (requestCommandMode) {
						case GameCommandMode.GameServer: {
							ALog.LogDebug("[CSCOMM][0103] -     Client<-Server  <-  GameCommandType.ServerGetResponse");
						}
							break;
						case GameCommandMode.GameManager: {
							ALog.LogDebug("[CSCOMM][0103] -     Client<-Client  <-  GameCommandType.ServerGetResponse");
						}
							break;
					}

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
						// activate the game manager
						v_exec.gameState = GameState.Active;
						// activate and set the game client
						pADEG.gameClient.state = GameClientState.Active;
						pADEG.gameClient.clientServer = clientServer;
						pADEG.gameClient.commandID = 0;

						lID = ++pADEG.gameClient.commandID;

						ALog.LogDebug("[CSCOMM][0200] - Client->Server  ->  GameCommandType.PlayerLoginRequest");
						pADEG.gameClient.CommandClientToServerSend(pADEG.gameClient.clientServer, new Game_Command.PlayerLoginRequest(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, GamePlayerType.Human, pADEG.gameUser.client.name, pADEG.gameLobby.password));
					}
					else /*if (v_command.state == GameCommandState.ResponseError)*/ {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}
			public class ServerPasswordUpdateRequest : Game_Command {
				public string password;

				public ServerPasswordUpdateRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, string pPassword, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.ServerPasswordUpdateRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					password = pPassword;
					eID = pEID;
				}

				public ServerPasswordUpdateRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.ServerPasswordUpdateRequest() - pOffset: " + pOffset);
					password = G.DataToString(pData, ref pOffset);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = G.StringToData(password);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0801] -   Server<-Client  <-  GameCommandType.ServerPasswordUpdateRequest");
					ALog.LogDebug(((mode == GameCommandMode.GameServer) ? "[GS]  ->  " : "[GM]  ->  ") + "t[" + pADEG.gameManager.tickID + "] " + ((mode == GameCommandMode.GameServer) ? "server" : "self") + " (" + pADEG.gameUser.client.ToString() + ") executes command: " + ToString());

					var v_exec = (Game_Server) pADEG.gameServer;
					if (v_exec.state == GameServerState.Inactive) {
						v_exec.password = password;

						pADEG.gameLobby.lobby.DataSet("locked", (!string.IsNullOrEmpty(v_exec.password) ? "1" : "0"));

						ALog.LogDebug("[CSCOMM][0802] -   Server->Client  ->  GameCommandType.ServerPasswordUpdateRequest");
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.ServerPasswordUpdateResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK, this));
					}
					else {
						// return the old password
						password = v_exec.password;

						ALog.LogErrorHandling("[CSCOMM][0802] -   Server->Client  ->  GameCommandType.ServerPasswordUpdateResponse", DBGC.DBG);
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.ServerPasswordUpdateResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, this, 1));
					}

					return true;
				}
			}

			public class ServerPasswordUpdateResponse : Game_Command {
				public string password;

				public ServerPasswordUpdateResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, Game_Command.ServerPasswordUpdateRequest pCommandRequest, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.ServerPasswordUpdateResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					password = pCommandRequest.password;
					eID = pEID;
				}

				public ServerPasswordUpdateResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.ServerPasswordUpdateResponse() - pOffset: " + pOffset);
					password = G.DataToString(pData, ref pOffset);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = G.StringToData(password);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0803] -     Client<-Server  <-  GameCommandType.ServerPasswordUpdateResponse");
					ALog.LogDebug(((mode == GameCommandMode.GameServer) ? "[GS]  ->  " : "[GM]  ->  ") + "t[" + pADEG.gameManager.tickID + "] - " + ((mode == GameCommandMode.GameServer) ? "server" : "self") + " (" + pADEG.gameUser.client.ToString() + ") executes command: " + ToString());

					var v_exec = (Game_Manager) pADEG.gameManager;

					return true;
				}
			}

			public class GameDataUpdateRequest : Game_Command {
				public string gameName;
				public int gameMap;
				public byte[] gameSettings;

				public GameDataUpdateRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, string pGameName, int pGameMap, byte[] pGameSettings, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameDataUpdateRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					gameName = pGameName;
					gameMap = pGameMap;
					gameSettings = pGameSettings;
					eID = pEID;
				}

				public GameDataUpdateRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.GameDataUpdateRequest() - pOffset: " + pOffset);
					gameName = G.DataToString(pData, ref pOffset);
					gameMap = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					Array.Copy(pData, pOffset, gameSettings, 0, pData.Length - pOffset);
					pOffset += gameSettings.Length;
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 3;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = G.StringToData(gameName);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(gameMap);
					v_dataCommandAdditional[3] = gameSettings;

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0301] -   Server<-Client  <-  GameCommandType.GameDataUpdateRequest");

					var v_exec = (Game_Server) pADEG.gameServer;
					if ((v_exec.state == GameServerState.Active) && (v_exec.gameState == GameState.Lobby)) {
						var v_gsClient = (Game_ServerClient) v_exec.gsClients.Find(x => (x.client.CheckIfSame(clientFrom)));
						if (v_gsClient != null) {
							if (v_gsClient == v_exec.gsClientPrimary) {
		//						pADEG.gameLobby.lobby.DataSet("serverID", v_exec.clientServer.ID.ToString());
		//						pADEG.gameLobby.lobby.DataSet("serverPlatform", ((int) v_exec.clientServer.platform).ToString());
		//						pADEG.gameLobby.lobby.DataSet("locked", (!string.IsNullOrEmpty(v_exec.password) ? "1" : "0"));
								pADEG.gameLobby.lobby.DataSet("ready", "1");
								v_exec.gameName = gameName;
								pADEG.gameLobby.lobby.DataSet("name", gameName);
								v_exec.gameMap = gameMap;
								pADEG.gameLobby.lobby.DataSet("map", gameMap.ToString("0000"));

								if(gameSettings != null && gameSettings.Length > 0)
								{
									int[] gameSettingValues = new int[10];

									int settingsOffset = 0;
									string settingString = "Game Settings: ";
									for(int i=0; i<10; i++)
									{
										gameSettingValues[i] = BitConverter.ToInt32(gameSettings, settingsOffset);
										settingString += gameSettingValues[i].ToString() + " ";
										settingsOffset += sizeof(int);
									}
									ALog.Log(settingString);
								}

								ALog.LogDebug("[CSCOMM][0302] -   Server->Client  ->  GameCommandType.GameDataUpdateResponse");
								pADEG.gameClient.CommandServerToClientSend(v_gsClient.client, new Game_Command.GameDataUpdateResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK, this));

								var v_commandInfoAll = (Game_Command.GameDataUpdate) new Game_Command.GameDataUpdate(++v_exec.commandID, -1, Time.time, v_exec.clientServer, v_exec.gameName, v_exec.gameMap, gameSettings, type);
								for (int i=0; i<v_exec.gsClients.Count; i++) {
									if (v_exec.gsClients[i] != v_gsClient) {
										ALog.LogDebug("[CSCOMM][0304] -   Server->Client[*]  ->  GameCommandType.GameDataUpdate");
										pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandInfoAll);
									}
								}
							}
						}
						else {
							ALog.LogErrorHandling("[CSCOMM][0302] -   Server->Client  ->  GameCommandType.GameDataUpdateResponse", DBGC.DBG);
							pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.GameDataUpdateResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, this, 1));
						}
					}

					return true;
				}
			}

			public class GameDataUpdateResponse : Game_Command {
				public string gameName;
				public int gameMap;
				public byte[] gameSettings;

				public GameDataUpdateResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, Game_Command.GameDataUpdateRequest pCommandRequest, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameDataUpdateResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					gameName = pCommandRequest.gameName;
					gameMap = pCommandRequest.gameMap;
					gameSettings = pCommandRequest.gameSettings;
					eID = pEID;
				}

				public GameDataUpdateResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.GameDataUpdateResponse() - pOffset: " + pOffset);
					gameName = G.DataToString(pData, ref pOffset);
					gameMap = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					Array.Copy(pData, pOffset, gameSettings, 0, pData.Length - pOffset);
					pOffset += gameSettings.Length;
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 3;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = G.StringToData(gameName);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(gameMap);
					v_dataCommandAdditional[2] = gameSettings;

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0303] -     Client<-Server  <-  GameCommandType.GameDataUpdateResponse");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
						if (!pADEG.def_clientUnconfirmedLocalEffect) {
							v_exec.gameName = gameName;
							pADEG.inputGameName = v_exec.gameName;
							v_exec.gameMap = gameMap;
							pADEG.inputGameMap = v_exec.gameMap;
						}
					}

					return true;
				}
			}

			public class GameDataUpdate : Game_Command {
				public string gameName;
				public int gameMap;
				public byte[] gameSettings;
				public int commandRequestType;

				public GameDataUpdate (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, string pGameName, int pGameMap, byte[] pGameSettings, int pCommandRequestType, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameDataUpdate, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					gameName = pGameName;
					gameMap = pGameMap;
					gameSettings = pGameSettings;
					commandRequestType = pCommandRequestType;
					eID = pEID;
				}

				public GameDataUpdate (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.GameDataUpdate() - pOffset: " + pOffset);
					gameName = G.DataToString(pData, ref pOffset);
					gameMap = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					gameSettings = new byte[pData.Length - pOffset - sizeof(int)];
					Array.Copy(pData, pOffset, gameSettings, 0, pData.Length - pOffset - sizeof(int));
					pOffset += gameSettings.Length;
					commandRequestType = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 4;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = G.StringToData(gameName);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(gameMap);
					v_dataCommandAdditional[2] = gameSettings;
					v_dataCommandAdditional[3] = BitConverter.GetBytes(commandRequestType);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					switch ((GameCommandType) commandRequestType) {
						case GameCommandType.PlayerLoginRequest:
							ALog.LogDebug("[CSCOMM][0205] -     Client<-Server  <-  GameCommandType.GameDataUpdate");
							break;
						case GameCommandType.GameDataUpdateRequest:
							ALog.LogDebug("[CSCOMM][0305] -     Client{*}<-Server  <-  GameCommandType.GameDataUpdate");
							break;
					}

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						v_exec.gameName = gameName;
						pADEG.inputGameName = v_exec.gameName;
						v_exec.gameMap = gameMap;
						pADEG.inputGameMap = v_exec.gameMap;

						if(gameSettings != null && gameSettings.Length > 0)
						{
							int[] gameSettingValues = new int[10];

							int settingsOffset = 0;
							string settingString = "Game Settings Received: ";
							for(int i=0; i<10; i++)
							{
								gameSettingValues[i] = BitConverter.ToInt32(gameSettings, settingsOffset);
								settingString += gameSettingValues[i].ToString() + " ";
								settingsOffset += sizeof(int);
							}

							v_exec.defaultSquads = gameSettingValues[0];
							v_exec.friendlyFire = gameSettingValues[1];
							v_exec.allowAlliances = gameSettingValues[2];
							v_exec.apcType = gameSettingValues[3];
							v_exec.damageFactor = gameSettingValues[4];
							v_exec.timeLimit = gameSettingValues[5];
							v_exec.scoreLimit = gameSettingValues[6];
							v_exec.techLimit = gameSettingValues[7];
							v_exec.reinforcements = gameSettingValues[8];
							v_exec.reinforceInterval = gameSettingValues[9];

							ALog.Log(settingString);
						}
					}

					return true;
				}
			}

			public class GameLoadRequest : Game_Command {
				public int levelID;
				
				public GameLoadRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pLevelID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameLoadRequest, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					levelID = pLevelID;
					eID = pEID;
				}

				public GameLoadRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.GameLoadRequest() - pOffset: " + pOffset);
					levelID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(levelID);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0905] -     Client{*}<-Server  <-  GameCommandType.GameLoadRequest");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Request) {
						v_exec.StartCoroutine(v_exec.GameLoad(levelID));
					}
					else /*if (v_commandRequest.state == GameCommandState.ResponseError)*/ {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class GameLoadResponse : Game_Command {

				public GameLoadResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameLoadResponse, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					eID = pEID;
				}

				public GameLoadResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0907] -       Server<-Client{*}  <-  GameCommandType.GameLoadResponse");

					var v_exec = (Game_Server) pADEG.gameServer;
					if (state == GameCommandState.ResponseOK) {
						var v_gsClient = (Game_ServerClient) v_exec.gsClients.Find(x => (x.client.CheckIfSame(clientFrom)));
						if (v_gsClient != null) {
							v_gsClient.gameState = GameState.Loaded;
						}

						// when all clients are loaded
						if (v_exec.gsClients.Find(x => (x.gameState != GameState.Loaded)) == null) {
							v_exec.gameState = GameState.Loaded;

							v_exec.gameState = GameState.Initializing;
							ADE.lobby.MessageSend("The game is initializing..", ADE_LobbyMessageType.Info);

							var v_commandRequestAll = (Game_Command.GameInitRequest) new Game_Command.GameInitRequest(++v_exec.commandID, -1, Time.time, v_exec.clientServer);
							for (int i=0; i<v_exec.gsClients.Count; i++) {
								ALog.LogDebug("[CSCOMM][0908] -   Server->Client[*]  ->  GameCommandType.GameInitRequest");
								pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandRequestAll);
								v_exec.gsClients[i].gameState = GameState.Initializing;
							}
						}
					}
					else {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class GameInitRequest : Game_Command {
				
				public GameInitRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameInitRequest, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					eID = pEID;
				}

				public GameInitRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0909] -     Client{*}<-Server  <-  GameCommandType.GameInitRequest");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Request) {
						v_exec.StartCoroutine(v_exec.GameInit());
					}
					else /*if (v_commandRequest.state == GameCommandState.ResponseError)*/ {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class GameInitResponse : Game_Command {

				public GameInitResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameInitResponse, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					eID = pEID;
				}

				public GameInitResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0913] -       Server<-Client{*}  <-  GameCommandType.GameInitResponse");

					var v_exec = (Game_Server) pADEG.gameServer;
					if (state == GameCommandState.ResponseOK) {
						var v_gsClient = (Game_ServerClient) v_exec.gsClients.Find(x => (x.client.CheckIfSame(clientFrom)));
						if (v_gsClient != null) {
							v_gsClient.gameState = GameState.Initialized;
							// set all client players' state to initialized
							var v_players = (List<Game_ServerPlayer>) v_exec.gsPlayers.FindAll(x => (x.gsClient == v_gsClient));
							for (int i=0; i<v_players.Count; i++) {
								v_players[i].playerState = GamePlayerState.Initialized;
							}
						}

						// when all clients are initialized
						if (v_exec.gsClients.Find(x => (x.gameState != GameState.Initialized)) == null) {
							v_exec.gameState = GameState.Initialized;
							ADE.lobby.MessageSend("The game is initialized..", ADE_LobbyMessageType.Info);

							var v_commandInfoAll = (Game_Command.GameStartRequest) new Game_Command.GameStartRequest(++v_exec.commandID, -1, Time.time, v_exec.clientServer);
							for (int i=0; i<v_exec.gsClients.Count; i++) {
								ALog.LogDebug("[CSCOMM][0914] -   Server->Client[*]  ->  GameCommandType.GameStartRequest");
								pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandInfoAll);
							}
						}
					}
					else {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class GameStartRequest : Game_Command {

				public GameStartRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameStartRequest, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					eID = pEID;
				}

				public GameStartRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0915] -     Client{*}<-Server  <-  GameCommandType.GameStartRequest");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Request) {
						ALog.Log("[GM]  ->  game ready");
						v_exec.gameState = GameState.Ready;

						ALog.LogDebug("[CSCOMM][0916] -     Client{*}->Server  ->  GameCommandType.GameStartResponse");
						pADEG.gameClient.CommandClientToServerSend(pADEG.gameClient.clientServer, new Game_Command.GameStartResponse(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, GameCommandState.ResponseOK));
					}
					else /*if (v_commandRequest.state == GameCommandState.ResponseError)*/ {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class GameStartResponse : Game_Command {

				public GameStartResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameStartResponse, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					eID = pEID;
				}

				public GameStartResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0917] -       Server<-Client{*}  <-  GameCommandType.GameStartResponse");

					var v_exec = (Game_Server) pADEG.gameServer;
					if (state == GameCommandState.ResponseOK) {
						var v_gsClient = (Game_ServerClient) v_exec.gsClients.Find(x => (x.client.CheckIfSame(clientFrom)));
						if (v_gsClient != null) {
							v_gsClient.gameState = GameState.Ready;
						}

						// when all clients are ready
						if (v_exec.gsClients.Find(x => (x.gameState != GameState.Ready)) == null) {
							v_exec.gameState = GameState.Ready;
							ADE.lobby.MessageSend("The game is ready..", ADE_LobbyMessageType.Info);

							v_exec.timeGame = 0f -ADEG.def_tickTimeMAXDefault;
							v_exec.timeGameReal = Time.time -ADEG.def_tickTimeMAXDefault;
							v_exec.gameState = GameState.Playing;
							ADE.lobby.MessageSend("The game is playing..", ADE_LobbyMessageType.Info);
							var v_commandInfoAll = (Game_Command.GameStart) new Game_Command.GameStart(++v_exec.commandID, -1, Time.time, v_exec.clientServer, v_exec.timeLobby +ADEG.def_tickTimeMAXDefault);
							for (int i=0; i<v_exec.gsClients.Count; i++) {
								ALog.LogDebug("[CSCOMM][0918] -   Server->Client[*]  ->  GameCommandType.GameStart");
								pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandInfoAll);
								v_exec.gsClients[i].gameState = GameState.Playing;
							}
							// set all players' state to playing
							for (int i=0; i<v_exec.gsPlayers.Count; i++) {
								v_exec.gsPlayers[i].playerState = GamePlayerState.Playing;
							}
						}
					}
					else {
						ALog.LogErrorHandling(DBGC.TODO);
					}
					return true;
				}
			}

			public class GameStart : Game_Command {
				public float timeLobbyStart;

				public GameStart (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, float pTimeLobbyStart, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameStart, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					timeLobbyStart = pTimeLobbyStart;
					eID = pEID;
				}

				public GameStart (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.GameStart() - pOffset: " + pOffset);
					timeLobbyStart = BitConverter.ToSingle(pData, pOffset);
					pOffset += sizeof(float);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(timeLobbyStart);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0919] -     Client{*}<-Server  <-  GameCommandType.GameStart");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						if (v_exec.gameState == GameState.Ready) {
							if (v_exec.timeLobby <= timeLobbyStart) {
								ALog.LogDebug("Game_Command:GameStart.Execute() received start lobby time: " + timeLobbyStart + ", current lobby time: " + v_exec.timeLobby + " -> game start delay time: " + (v_exec.timeLobby -timeLobbyStart));
								v_exec.timeGame = 0f -(v_exec.timeLobby -timeLobbyStart);
							}
							else {
								ALog.LogErrorHandling("Game_Command:GameStart.Execute() received start lobby time: " + timeLobbyStart + " > current lobby time: " + v_exec.timeLobby);
							}
						}
						v_exec.gameState = GameState.Playing;
						for (int i=0; i<v_exec.players.Count; i++) {
							v_exec.players[i].state = GamePlayerState.Playing;
						}
					}

					return true;
				}
			}

			public class GameStopRequest : Game_Command {

				public GameStopRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameStopRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					eID = pEID;
				}

				public GameStopRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -   Server<-Client  <-  GameCommandType.GameStopRequest");

					var v_exec = (Game_Server) pADEG.gameServer;
					if ((v_exec.state == GameServerState.Active) && ((v_exec.gameState == GameState.Lobby) || (v_exec.gameState == GameState.Playing))) {
						ADE.lobby.MessageSend("The game is stopping..", ADE_LobbyMessageType.Info);

						ALog.LogDebug("[CSCOMM][] -   Server->Client  ->  GameCommandType.GameStopResponse");
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.GameStopResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK));

						v_exec.gameState = GameState.Stopped;

						var v_commandRequestAll = (Game_Command.GameStop) new Game_Command.GameStop(++v_exec.commandID, -1, Time.time, v_exec.clientServer);
						for (int i=0; i<v_exec.gsClients.Count; i++) {
							if (v_exec.gsClients[i] != v_exec.gsClientPrimary) {
								ALog.LogDebug("[CSCOMM][] -   Server->Client[*]  ->  GameCommandType.GameStop");
								v_exec.gsClients[i].gameState = GameState.Stopped;
								pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandRequestAll);
							}
						}

						// stop the primary client last
						ALog.LogDebug("[CSCOMM][] -   Server->Client[*]  ->  GameCommandType.GameStop");
						v_exec.gsClientPrimary.gameState = GameState.Stopped;
						pADEG.gameClient.CommandServerToClientSend(v_exec.gsClientPrimary.client, v_commandRequestAll);
					}
					else {
						ALog.LogErrorHandling("[CSCOMM][] -   Server->Client  ->  GameCommandType.GameStopResponse", DBGC.DBG);
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.GameStopResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, 1));
					}

					return true;
				}
			}

			public class GameStopResponse : Game_Command {

				public GameStopResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameStopResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					eID = pEID;
				}

				public GameStopResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -     Client<-Server  <-  GameCommandType.GameStopResponse");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
						ALog.LogError("[TODO]  Game_Command.GameStopResponse.Execute()  ->  add code");
					}
					else {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class GameStop : Game_Command {

				public GameStop (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameStop, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					eID = pEID;
				}

				public GameStop (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -     Client{*}<-Server  <-  GameCommandType.GameStop");

					var v_exec = (Game_Manager) pADEG.gameManager;
					ADE.lobby.MessageSend("The game has stopped!", ADE_LobbyMessageType.Info);

					if (state == GameCommandState.Info) {
						v_exec.StartCoroutine(v_exec.GameStop());
					}

					return true;
				}
			}

			public class GameEndRequest : Game_Command {

				public GameEndRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameEndRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					eID = pEID;
				}

				public GameEndRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -   Server<-Client  <-  GameCommandType.GameEndRequest");

					var v_exec = (Game_Server) pADEG.gameServer;
					if ((v_exec.state == GameServerState.Active) && ((v_exec.gameState == GameState.Lobby) || (v_exec.gameState == GameState.Playing))) {
						ADE.lobby.MessageSend("The game is ending..", ADE_LobbyMessageType.Info);

						ALog.LogDebug("[CSCOMM][] -   Server->Client  ->  GameCommandType.GameEndResponse");
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.GameEndResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK));

						v_exec.gameState = GameState.Ended;

						var v_commandRequestAll = (Game_Command.GameEnd) new Game_Command.GameEnd(++v_exec.commandID, -1, Time.time, v_exec.clientServer);
						for (int i=0; i<v_exec.gsClients.Count; i++) {
							if (v_exec.gsClients[i] != v_exec.gsClientPrimary) {
								ALog.LogDebug("[CSCOMM][] -   Server->Client[*]  ->  GameCommandType.GameEnd");
								pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandRequestAll);
								v_exec.gsClients[i].gameState = GameState.Ended;
							}
						}

						// stop the primary client last
						ALog.LogDebug("[CSCOMM][] -   Server->Client[*]  ->  GameCommandType.GameStop");
						pADEG.gameClient.CommandServerToClientSend(v_exec.gsClientPrimary.client, v_commandRequestAll);
						v_exec.gsClientPrimary.gameState = GameState.Stopped;
					}
					else {
						ALog.LogErrorHandling("[CSCOMM][] -   Server->Client  ->  GameCommandType.GameEndResponse", DBGC.DBG);
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.GameEndResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, 1));
					}

					return true;
				}
			}

			public class GameEndResponse : Game_Command {

				public GameEndResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameEndResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					eID = pEID;
				}

				public GameEndResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -     Client<-Server  <-  GameCommandType.GameEndResponse");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
						ALog.LogError("[TODO]  Game_Command.GameEndResponse.Execute()  ->  add code");
					}
					else {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class GameEnd : Game_Command {

				public GameEnd (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.GameEnd, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					eID = pEID;
				}

				public GameEnd (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -     Client{*}<-Server  <-  GameCommandType.GameEnd");

					var v_exec = (Game_Manager) pADEG.gameManager;
					ADE.lobby.MessageSend("The game has ended!", ADE_LobbyMessageType.Info);

					if (state == GameCommandState.Info) {
						for (int i=0; i<v_exec.players.Count; i++) {
							if (v_exec.players[i].location == Location.Local) {
								if (v_exec.players[i].state == GamePlayerState.Playing) {
									v_exec.StartCoroutine(v_exec.GameEnd());
									break;
								}
							}
						}
					}

					return true;
				}
			}

			public class TimeSyncRequest : Game_Command {
				public int playerID;
				public int syncRequestID;
				public float timeRequest;

				public TimeSyncRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, int pSyncRequestID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.TimeSyncRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					playerID = pPlayerID;
					syncRequestID = pSyncRequestID;
					timeRequest = pTimestamp;
					eID = pEID;
				}

				public TimeSyncRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.TimeSyncRequest() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					syncRequestID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					timeRequest = BitConverter.ToSingle(pData, pOffset);
					pOffset += sizeof(float);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 3;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(syncRequestID);
					v_dataCommandAdditional[2] = BitConverter.GetBytes(timeRequest);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0601] -   Server<-Client  <-  GameCommandType.TimeSyncRequest");

					var v_exec = (Game_Server) pADEG.gameServer;
					var v_gsPlayer = (Game_ServerPlayer) v_exec.gsPlayers.Find(x => (x.ID == playerID));
					if (v_gsPlayer != null) {
						ALog.LogDebug("[CSCOMM][0602] -   Server->Client  ->  GameCommandType.TimeSyncResponse");
						pADEG.gameClient.CommandServerToClientSend(v_gsPlayer.gsClient.client, new Game_Command.TimeSyncResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK, v_gsPlayer.ID, syncRequestID, timeRequest, v_exec.timeLobby));
					}
					else {
						ALog.LogErrorHandling("[CSCOMM][0602] -   Server->Client  ->  GameCommandType.TimeSyncResponse", DBGC.DBG);
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.TimeSyncResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, playerID, syncRequestID, timeRequest, v_exec.timeLobby, 1));
					}

					return true;
				}
			}

			public class TimeSyncResponse : Game_Command {
				public int playerID;
				public int syncRequestID;
				public float timeRequest;
				public float timeServer;

				public TimeSyncResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pPlayerID, int pSyncRequestID, float pTimeRequest, float pTimeServer, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.TimeSyncResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					playerID = pPlayerID;
					syncRequestID = pSyncRequestID;
					timeRequest = pTimeRequest;
					timeServer = pTimeServer;
					eID = pEID;
				}

				public TimeSyncResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.TimeSyncResponse() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					syncRequestID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					timeRequest = BitConverter.ToSingle(pData, pOffset);
					pOffset += sizeof(float);
					timeServer = BitConverter.ToSingle(pData, pOffset);
					pOffset += sizeof(float);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 4;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(syncRequestID);
					v_dataCommandAdditional[2] = BitConverter.GetBytes(timeRequest);
					v_dataCommandAdditional[3] = BitConverter.GetBytes(timeServer);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0603] -     Client<-Server  <-  GameCommandType.TimeSyncResponse");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
						var v_player = (Game_Player) v_exec.players.PlayerGet(playerID);
						if (v_player != null) {
							var v_timeResponse = (float) Time.time;
							var v_timeTravel = (float) ((v_timeResponse -timeRequest) /2);
							v_exec.timeLobby = (timeServer +v_timeTravel);

		//					ALog.LogError("[TIME][SYNC] -     timeRequest: " + timeRequest, DBGC.DBG);
		//					ALog.LogError("[TIME][SYNC] -     timeResponse: " + v_timeResponse, DBGC.DBG);
		//					ALog.LogError("[TIME][SYNC] -     timeTravel: " + v_timeTravel, DBGC.DBG);
		//					ALog.LogError("[TIME][SYNC] -     timeLobby (timeServer +v_timeTravel) = " + (timeServer +v_timeTravel) + " (" + timeServer + " +" + v_timeTravel + ")", DBGC.DBG);

							// [!!!][003] - time sync - replace Time.time with proper Time.time / gameManager.timeLobby / gameManager.timeGame depending on time sync / game state

							// [!!!][003] - time sync
							ALog.LogDebug("[CSCOMM][0604] - Client->Server  ->  GameCommandType.TimeSync");
							pADEG.gameClient.CommandClientToServerSend(pADEG.gameClient.clientServer, new Game_Command.TimeSync(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, v_player.ID));
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}

					return true;
				}
			}

			public class TimeSync : Game_Command {
				public int playerID;

				public TimeSync (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.TimeSync, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					playerID = pPlayerID;
					eID = pEID;
				}

				public TimeSync (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.TimeSync() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0605] -   Server<-Client  <-  GameCommandType.TimeSync");

					var v_exec = (Game_Server) pADEG.gameServer;
					if (state == GameCommandState.Info) {
						var v_gsPlayer = (Game_ServerPlayer) v_exec.gsPlayers.Find(x => (x.ID == playerID));
						if (v_gsPlayer != null) {
							ALog.LogDebug("[CSCOMM][0606] -   Server->Client  ->  GameCommandType.TimeSyncAck");
							pADEG.gameClient.CommandServerToClientSend(v_gsPlayer.gsClient.client, new Game_Command.TimeSyncAck(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK, v_gsPlayer.ID));
						}
						else {
							ALog.LogErrorHandling("[CSCOMM][0606] -   Server->Client  ->  GameCommandType.TimeSyncAck", DBGC.DBG);
							pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.TimeSyncAck(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, playerID, 1));
						}
					}

					return true;
				}
			}

			public class TimeSyncAck : Game_Command {
				public int playerID;

				public TimeSyncAck (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pPlayerID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.TimeSyncAck, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					playerID = pPlayerID;
					eID = pEID;
				}

				public TimeSyncAck (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.TimeSyncAck() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0607] -     Client<-Server  <-  GameCommandType.TimeSyncAck");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
						var v_player = (Game_Player) v_exec.players.PlayerGet(playerID);
						if (v_player != null) {
							ALog.Log("[P[" + v_player.ID + "]]  ->  time sync complete");
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}

					return true;
				}
			}

			public class PingRequest : Game_Command {
				public int playerID;
				public int tickRequestID;
				public float timeRequest;

				public PingRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, int pTickRequestID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PingRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					playerID = pPlayerID;
					tickRequestID = pTickRequestID;
					timeRequest = pTimestamp;
					eID = pEID;
				}

				public PingRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PingRequest() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					tickRequestID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					timeRequest = BitConverter.ToSingle(pData, pOffset);
					pOffset += sizeof(float);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 3;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(tickRequestID);
					v_dataCommandAdditional[2] = BitConverter.GetBytes(timeRequest);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					if (playerID == -1) {
						ALog.LogDebug("[CSCOMM][0701] -   Server<-Client  <-  GameCommandType.PingRequest");

						var v_exec = (Game_Server) pADEG.gameServer;
						var v_gsPlayer = (Game_ServerPlayer) v_exec.gsPlayers.Find(x => (x.ID == playerID));
						if (v_gsPlayer != null) {
							ALog.LogDebug("[CSCOMM][0702] -   Server->Client  ->  GameCommandType.PingResponse");
							pADEG.gameClient.CommandServerToClientSend(v_gsPlayer.gsClient.client, new Game_Command.PingResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK, -1, tickRequestID, timeRequest, v_exec.tickID, v_exec.timeLobby));
						}
						else {
							ALog.LogErrorHandling("[CSCOMM][7602] -   Server->Client  ->  GameCommandType.PingResponse", DBGC.DBG);
							pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.PingResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, -1, tickRequestID, timeRequest, v_exec.tickID, v_exec.timeLobby, 1));
						}
					}
					else {
						ALog.LogDebug("[CSCOMM][0711] -   Client<-Server  <-  GameCommandType.PingRequest");

						var v_exec = (Game_Manager) pADEG.gameManager;

						ALog.LogDebug("[CSCOMM][0712] -   Client->Server  ->  GameCommandType.PingResponse");
						pADEG.gameClient.CommandClientToServerSend(pADEG.gameClient.clientServer, new Game_Command.PingResponse(sID, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, GameCommandState.ResponseOK, v_exec.player.ID, tickRequestID, timeRequest, v_exec.tickID, v_exec.timeLobby));
					}

					return true;
				}
			}

			public class PingResponse : Game_Command {
				public int playerID;
				public int tickRequestID;
				public float timeRequest;
				public int tickResponseID;
				public float timeResponse;

				public PingResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pPlayerID, int pTickRequestID, float pTimeRequest, int pTickResponseID, float pTimeResponse, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PingResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					playerID = pPlayerID;
					tickRequestID = pTickRequestID;
					timeRequest = pTimeRequest;
					tickResponseID = pTickResponseID;
					timeResponse = pTimeResponse;
					eID = pEID;
				}

				public PingResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PingResponse() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					tickRequestID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					timeRequest = BitConverter.ToSingle(pData, pOffset);
					pOffset += sizeof(float);
					tickResponseID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					timeResponse = BitConverter.ToSingle(pData, pOffset);
					pOffset += sizeof(float);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 5;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(tickRequestID);
					v_dataCommandAdditional[2] = BitConverter.GetBytes(timeRequest);
					v_dataCommandAdditional[3] = BitConverter.GetBytes(tickResponseID);
					v_dataCommandAdditional[4] = BitConverter.GetBytes(timeResponse);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					if (playerID == -1) {
						ALog.LogDebug("[CSCOMM][0703] -     Client<-Server  <-  GameCommandType.PingResponse");

						var v_exec = (Game_Manager) pADEG.gameManager;
						if (state == GameCommandState.ResponseOK) {
							var v_player = (Game_Player) v_exec.players.PlayerGet(playerID);
							if (v_player != null) {
								var v_timeTravelAverage = (float) ((v_exec.timeLobby -timeRequest) /2);

								ALog.LogDebug("[PING] -     timePing: " + timeRequest + " (tick: " + tickRequestID + " , travelTime: " + (timeResponse -timeRequest) + "), timeResponse: " + timeResponse + " (tick: " + tickResponseID + " , travelTime: " + (v_exec.timeLobby -timeResponse) + "), timePong: " + v_exec.timeLobby + " , tick: " + v_exec.tickID + " , v_timeTravelAverage: " + v_timeTravelAverage);

								v_player.client.pinging = false;
							}
							else {
								ALog.LogErrorHandling(DBGC.TODO);
							}
						}
					}
					else {
						ALog.LogDebug("[CSCOMM][0713] -     Server<-Client  <-  GameCommandType.PingResponse");

						var v_exec = (Game_Server) pADEG.gameServer;
						if (state == GameCommandState.ResponseOK) {
							var v_player = (Game_ServerPlayer) v_exec.gsPlayers.Find(x => (x.ID == playerID));
							if (v_player != null) {
								var v_timeTravelAverage = (float) ((v_exec.timeLobby -timeRequest) /2);

								ALog.LogDebug("[PING] -     timePing: " + timeRequest + " (tick: " + tickRequestID + " , travelTime: " + (timeResponse -timeRequest) + "), timeResponse: " + timeResponse + " (tick: " + tickResponseID + " , travelTime: " + (v_exec.timeLobby -timeResponse) + "), timePong: " + v_exec.timeLobby + " , tick: " + v_exec.tickID + " , v_timeTravelAverage: " + v_timeTravelAverage);

								v_player.gsClient.client.pinging = false;
							}
							else {
								ALog.LogErrorHandling(DBGC.TODO);
							}
						}
					}

					return true;
				}
			}

			public class PlayerLoginRequest : Game_Command {
				public GamePlayerType playerType;
				public string playerName;
				public string password;

				public PlayerLoginRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GamePlayerType pPlayerType, string pPlayerName, string pPassword, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerLoginRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					playerType = pPlayerType;
					playerName = pPlayerName;
					password = pPassword;
					eID = pEID;
				}

				public PlayerLoginRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerLoginRequest() - pOffset: " + pOffset);
					playerType = (GamePlayerType) BitConverter.ToInt32(pData, pOffset).ToEnum<GamePlayerType>();
					pOffset += sizeof(int);
					playerName = G.DataToString(pData, ref pOffset);
					password = G.DataToString(pData, ref pOffset);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 3;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes((int) playerType);
					v_dataCommandAdditional[1] = G.StringToData(playerName);
					v_dataCommandAdditional[2] = G.StringToData(password);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0201] -   Server<-Client  <-  GameCommandType.PlayerLoginRequest");

					var v_exec = (Game_Server) pADEG.gameServer;
					if ((v_exec.state == GameServerState.Active) && ((v_exec.gameState == GameState.Active) || (v_exec.gameState == GameState.Lobby)) && (string.IsNullOrEmpty(v_exec.password) || password.Equals(v_exec.password, StringComparison.InvariantCultureIgnoreCase))) {
						// if it's the first player joining
						if ((v_exec.gameState == GameState.Active) && (v_exec.gsPlayers.Count == 0)) {
							v_exec.gameState = GameState.Lobby;
						}

						var v_gsClient = (Game_ServerClient) v_exec.gsClients.Find(x => x.client.CheckIfSame(clientFrom));
						if (v_gsClient == null) {
							v_gsClient = new Game_ServerClient(clientFrom, (clientFrom.CheckIfSame(v_exec.clientServer) ? GameClientType.Primary : GameClientType.Secondary));
							v_exec.gsClients.Add(v_gsClient);
							if (v_gsClient.type == GameClientType.Primary) {
								v_exec.gsClientPrimary = v_gsClient;
							}
						}
						var v_gsPlayer = (Game_ServerPlayer) new Game_ServerPlayer(++v_exec.playerID, v_gsClient, playerType, playerName, ++v_exec.sideID);
						v_exec.gsPlayers.Add(v_gsPlayer);

						ALog.LogDebug("[CSCOMM][0202] -   Server->Client  ->  GameCommandType.PlayerLoginResponse");
						pADEG.gameClient.CommandServerToClientSend(v_gsClient.client, new Game_Command.PlayerLoginResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK, v_exec.gameMode, v_exec.gameGPMode, v_gsPlayer.ID, v_gsPlayer.type, v_gsPlayer.name, v_gsPlayer.sideID));

						// if it's not the first player joining (as the game data are not set yet)
						if ((v_exec.gameState == GameState.Lobby) && ((v_exec.gsPlayers.Count > 1) || (v_gsPlayer.ID != 0))) {
							ALog.LogDebug("[CSCOMM][0204] -   Server->Client  ->  GameCommandType.GameDataUpdate");
							pADEG.gameClient.CommandServerToClientSend(v_gsClient.client, new Game_Command.GameDataUpdate(++v_exec.commandID, lID, Time.time, v_exec.clientServer, v_exec.gameName, v_exec.gameMap, new byte[0], type));
						}

						v_gsPlayer.gsClient.gameState = GameState.Lobby;

						ADE.lobby.MessageSend("[GS]  -> player[" + v_gsPlayer.ID + "] "  + v_gsPlayer.name + " joins the game!", ADE_LobbyMessageType.Info);
						var v_commandInfoAll = (Game_Command.PlayerLogin) new Game_Command.PlayerLogin(++v_exec.commandID, -1, Time.time, v_exec.clientServer, v_gsPlayer.ID, v_gsPlayer.gsClient.client, v_gsPlayer.type, v_gsPlayer.name, v_gsPlayer.sideID);
						for (int i=0; i<v_exec.gsClients.Count; i++) {
							if (v_exec.gsClients[i] != v_gsPlayer.gsClient) {
								ALog.LogDebug("[CSCOMM][0206] -   Server->Client[*]  ->  GameCommandType.PlayerLogin");
								pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandInfoAll);
							}
						}
					}
					else {
						// [ERRH]
						var v_errorID = (int) 1;
						if (!string.IsNullOrEmpty(v_exec.password) && !password.Equals(v_exec.password, StringComparison.InvariantCultureIgnoreCase)) {
							v_errorID = 2;
						}
						ALog.LogErrorHandling("[CSCOMM][0202] -   Server->Client  ->  GameCommandType.PlayerLoginResponse", DBGC.DBG);
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.PlayerLoginResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, v_exec.gameMode, v_exec.gameGPMode, -1, playerType, playerName, v_errorID));
					}

					return true;
				}
			}

			public class PlayerLoginResponse : Game_Command {
				public GameMode gMode;
				public GameplayMode gpMode;
				public int playerID;
				public GamePlayerType playerType;
				public string playerName;
				public int playerSideID;

				public PlayerLoginResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, GameMode pGameMode, GameplayMode pGameplayMode, int pPlayerID, GamePlayerType pPlayerType, string pPlayerName, int pPlayerSideID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerLoginResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					gMode = pGameMode;
					gpMode = pGameplayMode;
					playerID = pPlayerID;
					playerType = pPlayerType;
					playerName = pPlayerName;
					playerSideID = pPlayerSideID;
					eID = pEID;
				}

				public PlayerLoginResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerLoginResponse() - pOffset: " + pOffset);
					gMode = (GameMode) BitConverter.ToInt32(pData, pOffset).ToEnum<GameMode>();
					pOffset += sizeof(int);
					gpMode = (GameplayMode) BitConverter.ToInt32(pData, pOffset).ToEnum<GameplayMode>();
					pOffset += sizeof(int);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					playerType = (GamePlayerType) BitConverter.ToInt32(pData, pOffset).ToEnum<GamePlayerType>();
					pOffset += sizeof(int);
					playerName = G.DataToString(pData, ref pOffset);
					playerSideID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 6;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes((int) gMode);
					v_dataCommandAdditional[1] = BitConverter.GetBytes((int) gpMode);
					v_dataCommandAdditional[2] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[3] = BitConverter.GetBytes((int) playerType);
					v_dataCommandAdditional[4] = G.StringToData(playerName);
					v_dataCommandAdditional[5] = BitConverter.GetBytes(playerSideID);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0203] -     Client<-Server  <-  GameCommandType.PlayerLoginResponse");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
						if (v_exec.gameState == GameState.Active) {
							v_exec.timeLobby = 0f;
						}
						v_exec.gameState = GameState.Lobby;
						v_exec.gameMode = gMode;
						v_exec.gameGPMode = gpMode;

						// add the new (self) player and client
						var v_player = (Game_Player) v_exec.PlayerNewAdd(playerID, pADEG.gameUser.client, Location.Local, playerType, playerName, playerSideID);
						if (pADEG.gameClient.clients.ClientGet(pADEG.gameUser.client) == null) {
							pADEG.gameClient.clients.Add(pADEG.gameUser.client);
						}

						if (v_player.client.CheckIfSame(pADEG.gameClient.clientServer)) {
							v_exec.gameName = (pADEG.def_clientUnconfirmedLocalEffect ? pADEG.inputGameName : v_exec.gameName);
							v_exec.gameMap = (pADEG.def_clientUnconfirmedLocalEffect ? pADEG.inputGameMap : v_exec.gameMap);
							ALog.LogDebug("[CSCOMM][0300] - Client->Server  ->  GameCommandType.GameDataUpdateRequest");
							pADEG.gameClient.CommandClientToServerSend(pADEG.gameClient.clientServer, new Game_Command.GameDataUpdateRequest(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, pADEG.inputGameName, pADEG.inputGameMap, new byte[0]));
						}

						var v_state = (GamePlayerState) GamePlayerState.LobbySeated;
						v_player.state = (pADEG.def_clientUnconfirmedLocalEffect ? v_state : v_player.state);
						ALog.LogDebug("[CSCOMM][0400] - Client->Server  ->  GameCommandType.PlayerStateUpdateRequest");
						pADEG.gameClient.CommandClientToServerSend(pADEG.gameClient.clientServer, new Game_Command.PlayerStateUpdateRequest(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, v_player.ID, v_state));

						var v_number = (int) -1;
						ALog.LogToDo("[GCR][HARDCODED] temporary - to be properly implemented (add a new GCR_Player.color member)");
						if (pADEG.project != ADEG_Project.GCR) {
							v_number = UnityEngine.Random.Range(1, 101);
							v_player.number = (pADEG.def_clientUnconfirmedLocalEffect ? v_number : v_player.number);
						}
						else {
							v_number = (int) -1;
							v_player.number = -1;
						}
						pADEG.inputPlayerNumber = v_player.number;
						ALog.LogDebug("[CSCOMM][0500] - Client->Server  ->  GameCommandType.PlayerDataUpdateRequest");
						pADEG.gameClient.CommandClientToServerSend(pADEG.gameClient.clientServer, new Game_Command.PlayerDataUpdateRequest(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, v_player.ID, v_number));

						// time sync
						if (v_player.client.CheckIfSame(pADEG.gameClient.clientServer)) {
							v_exec.timeLobby = pADEG.gameServer.timeLobby;
						}
						else {
							v_exec.StartCoroutine(v_exec.TimeSync(v_player));
						}
					}
					else {
						// [ERRH]
						ALog.LogToDo("[GCR][HARDCODED] temporary - to be properly implemented (see ADEG_GCR.GCR_Command_PlayerLoginResponse_Extended() method)");
						if (eID == 2) {
							ALog.LogErrorHandling(DBGC.TODO);
							PlayerPrefs.SetInt("SwitchMenu", 6);
						}

						v_exec.StartCoroutine(v_exec.GameStop());
					}

					return true;
				}
			}

			public class PlayerLogin : Game_Command {
				public int playerID;
				public ADE_Client playerClient;
				public GamePlayerType playerType;
				public string playerName;
				public int playerSideID;

				public PlayerLogin (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, ADE_Client pPlayerClient, GamePlayerType pPlayerType, string pPlayerName, int pPlayerSideID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerLogin, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					playerID = pPlayerID;
					playerClient = pPlayerClient;
					playerType = pPlayerType;
					playerName = pPlayerName;
					playerSideID = pPlayerSideID;
					eID = pEID;
				}

				public PlayerLogin (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerLogin() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					var v_playerClientID = (ulong) BitConverter.ToUInt64(pData, pOffset);
					pOffset += sizeof(ulong);
					var v_playerClientPlatform = (ADE_Platform) BitConverter.ToInt32(pData, pOffset).ToEnum<ADE_Platform>();
					pOffset += sizeof(int);
					playerClient = pClientsList.ClientGet(v_playerClientID, v_playerClientPlatform);
					playerType = (GamePlayerType) BitConverter.ToInt32(pData, pOffset).ToEnum<GamePlayerType>();
					pOffset += sizeof(int);
					playerName = G.DataToString(pData, ref pOffset);
					playerSideID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 6;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(playerClient.ID);
					v_dataCommandAdditional[2] = BitConverter.GetBytes((int) playerClient.platform);
					v_dataCommandAdditional[3] = BitConverter.GetBytes((int) playerType);
					v_dataCommandAdditional[4] = G.StringToData(playerName);
					v_dataCommandAdditional[5] = BitConverter.GetBytes(playerSideID);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0207] -     Client{*}<-Server  <-  GameCommandType.PlayerLogin");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						var v_player = (Game_Player) v_exec.players.PlayerGet(playerID);
						if (v_player == null) {
							v_player = v_exec.PlayerNewAdd(playerID, playerClient, Location.Remote, playerType, playerName, playerSideID);

							var v_client = (ADE_Client) pADEG.gameClient.clients.ClientGet(playerClient);
							if (v_client == null) {
								pADEG.gameClient.clients.Add(playerClient);
							}
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
						ALog.Log("[GS]  ->  new player joined:  " + v_player.ToString());

						for (int i=0; i<v_exec.players.Count; i++) {
							if (v_exec.players[i].location == Location.Local) {
								if (v_exec.players[i] != v_player) {
									ALog.LogDebug("[CSCOMM][0208] -     Client{*}->Client  ->  GameCommandType.PlayerLoginAck");
									ALog.Log("[P[" + v_exec.players[i].ID + "]]  ->  self (" + v_exec.players[i].ToString() + ") acknowledges the new player:  " + v_player.ToString());
									pADEG.gameClient.CommandClientToClientSend(v_player.client, new Game_Command.PlayerLoginAck(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, v_player.ID, v_exec.players[i].ID, v_exec.players[i].client, v_exec.players[i].type, v_exec.players[i].name, v_exec.players[i].sideID));

									ALog.LogDebug("[CSCOMM][0214] -     Client{*}->Client  ->  GameCommandType.PlayerStateUpdate");
									pADEG.gameClient.CommandClientToClientSend(v_player.client, new Game_Command.PlayerStateUpdate(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, v_exec.players[i].ID, v_exec.players[i].state, type));

									ALog.LogDebug("[CSCOMM][0216] -     Client{*}->Client  ->  GameCommandType.PlayerDataUpdate");
									pADEG.gameClient.CommandClientToClientSend(v_player.client, new Game_Command.PlayerDataUpdate(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, v_exec.players[i].ID, v_exec.players[i].number, type));
								}
							}
						}
					}

					return true;
				}
			}

			public class PlayerLoginAck : Game_Command {
				public int playerToID;
				public int playerID;
				public ADE_Client playerClient;
				public GamePlayerType playerType;
				public string playerName;
				public int playerSideID;

				public PlayerLoginAck (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerToID, int pPlayerID, ADE_Client pPlayerClient, GamePlayerType pPlayerType, string pPlayerName, int pPlayerSideID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerLoginAck, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					playerToID = pPlayerToID;
					playerID = pPlayerID;
					playerClient = pPlayerClient;
					playerType = pPlayerType;
					playerName = pPlayerName;
					playerSideID = pPlayerSideID;
					eID = pEID;
				}

				public PlayerLoginAck (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerLoginAck() - pOffset: " + pOffset);
					playerToID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					var v_playerClientID = (ulong) BitConverter.ToUInt64(pData, pOffset);
					pOffset += sizeof(ulong);
					var v_playerClientPlatform = (ADE_Platform) BitConverter.ToInt32(pData, pOffset).ToEnum<ADE_Platform>();
					pOffset += sizeof(int);
					playerClient = pClientsList.ClientGet(v_playerClientID, v_playerClientPlatform);
					playerType = (GamePlayerType) BitConverter.ToInt32(pData, pOffset).ToEnum<GamePlayerType>();
					pOffset += sizeof(int);
					playerName = G.DataToString(pData, ref pOffset);
					playerSideID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 7;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerToID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[2] = BitConverter.GetBytes(playerClient.ID);
					v_dataCommandAdditional[3] = BitConverter.GetBytes((int) playerClient.platform);
					v_dataCommandAdditional[4] = BitConverter.GetBytes((int) playerType);
					v_dataCommandAdditional[5] = G.StringToData(playerName);
					v_dataCommandAdditional[6] = BitConverter.GetBytes(playerSideID);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0209] -       Client<-Client{*}  <-  GameCommandType.PlayerLoginAck");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						var v_playerSelf = (Game_Player) v_exec.players.PlayerGet(playerToID);
						if (v_playerSelf != null) {
							var v_player = (Game_Player) v_exec.players.PlayerGet(playerID);
							if (v_player == null) {
								v_player = v_exec.PlayerNewAdd(playerID, playerClient, Location.Remote, playerType, playerName, playerSideID);

								var v_client = (ADE_Client) pADEG.gameClient.clients.ClientGet(playerClient);
								if (v_client == null) {
									pADEG.gameClient.clients.Add(playerClient);
								}
							}

							ALog.Log("[P[" + v_playerSelf.ID + "]]  ->  self (" + v_playerSelf.ToString() + ") acknowledged by " + ((v_player.location == Location.Remote) ? "remote" : "local") + " player:  " + v_player.ToString());

							ALog.LogDebug("[CSCOMM][0210] -       Client->Client{*}  ->  GameCommandType.PlayerStateUpdate");
							pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.PlayerStateUpdate(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, v_playerSelf.ID, v_playerSelf.state, type));

							ALog.LogDebug("[CSCOMM][0212] -       Client->Client{*}  ->  GameCommandType.PlayerDataUpdate");
							pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.PlayerDataUpdate(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, v_playerSelf.ID, v_playerSelf.number, type));
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}

					return true;
				}
			}

			public class PlayerStateUpdateRequest : Game_Command {
				public int playerID;
				public GamePlayerState playerState;

				public PlayerStateUpdateRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, GamePlayerState pPlayerState, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerStateUpdateRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					playerID = pPlayerID;
					playerState = pPlayerState;
					eID = pEID;
				}

				public PlayerStateUpdateRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerStateUpdateRequest() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					playerState = (GamePlayerState) BitConverter.ToInt32(pData, pOffset).ToEnum<GamePlayerState>();
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 2;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes((int) playerState);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0401] -   Server<-Client  <-  GameCommandType.PlayerStateUpdateRequest");

					var v_exec = (Game_Server) pADEG.gameServer;
					var v_gsPlayer = (Game_ServerPlayer) v_exec.gsPlayers.Find(x => (x.ID == playerID));
					if (v_gsPlayer != null) {
						v_gsPlayer.playerState = playerState;
						//SteamMatchmaking.SetLobbyMemberData(pADEG.gameLobby.lobby.CSID, "state", ((int) v_gsPlayer.playerState).ToString());

						ALog.LogDebug("[CSCOMM][0402] -   Server->Client  ->  GameCommandType.PlayerStateUpdateResponse");
						pADEG.gameClient.CommandServerToClientSend(v_gsPlayer.gsClient.client, new Game_Command.PlayerStateUpdateResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK, v_gsPlayer.ID, v_gsPlayer.playerState));

						var v_commandInfoAll = (Game_Command.PlayerStateUpdate) new Game_Command.PlayerStateUpdate(++v_exec.commandID, -1, Time.time, v_exec.clientServer, v_gsPlayer.ID, v_gsPlayer.playerState, type);
						for (int i=0; i<v_exec.gsClients.Count; i++) {
							if (v_exec.gsClients[i] != v_gsPlayer.gsClient) {
								ALog.LogDebug("[CSCOMM][0404] -   Server->Client[*]  ->  GameCommandType.PlayerStateUpdate");
								pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandInfoAll);
							}
						}
					}
					else {
						ALog.LogErrorHandling("[CSCOMM][0402] -   Server->Client  ->  GameCommandType.PlayerStateUpdateResponse", DBGC.DBG);
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.PlayerStateUpdateResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, playerID, playerState, 1));
					}

					return true;
				}
			}

			public class PlayerStateUpdateResponse : Game_Command {
				public int playerID;
				public GamePlayerState playerState;

				public PlayerStateUpdateResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pPlayerID, GamePlayerState pPlayerState, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerStateUpdateResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					playerID = pPlayerID;
					playerState = pPlayerState;
					eID = pEID;
				}

				public PlayerStateUpdateResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerStateUpdateResponse() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					playerState = (GamePlayerState) BitConverter.ToInt32(pData, pOffset).ToEnum<GamePlayerState>();
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 2;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes((int) playerState);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0403] -     Client<-Server  <-  GameCommandType.PlayerStateUpdateResponse");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
						var v_player = (Game_Player) v_exec.players.PlayerGet(playerID);
						if (v_player != null) {
							if (!pADEG.def_clientUnconfirmedLocalEffect) {
								v_player.state = playerState;
							}
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}

					return true;
				}
			}

			public class PlayerStateUpdate : Game_Command {
				public int playerID;
				public GamePlayerState playerState;
				public int commandRequestType;

				public PlayerStateUpdate (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, GamePlayerState pPlayerState, int pCommandRequestType, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerStateUpdate, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					playerID = pPlayerID;
					playerState = pPlayerState;
					commandRequestType = pCommandRequestType;
					eID = pEID;
				}

				public PlayerStateUpdate (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerStateUpdate() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					playerState = (GamePlayerState) BitConverter.ToInt32(pData, pOffset).ToEnum<GamePlayerState>();
					pOffset += sizeof(int);
					commandRequestType = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 3;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes((int) playerState);
					v_dataCommandAdditional[2] = BitConverter.GetBytes(commandRequestType);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					switch ((GameCommandType) commandRequestType) {
						case GameCommandType.PlayerLoginAck:
							ALog.LogDebug("[CSCOMM][0211] -         Client{*}<-Client  <-  GameCommandType.PlayerStateUpdate");
							break;
						case GameCommandType.PlayerLogin:
							ALog.LogDebug("[CSCOMM][0215] -       Client<-Client{*}  <-  GameCommandType.PlayerStateUpdate");
							break;
						case GameCommandType.PlayerStateUpdateRequest:
							ALog.LogDebug("[CSCOMM][0405] -     Client{*}<-Server  <-  GameCommandType.PlayerStateUpdate");
							break;
					}

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						var v_player = (Game_Player) v_exec.players.PlayerGet(playerID);
						if (v_player != null) {
							v_player.state = playerState;
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}

					return true;
				}
			}

			public class PlayerDataUpdateRequest : Game_Command {
				public int playerID;
				public int playerNumber;

				public PlayerDataUpdateRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, int pPlayerNumber, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerDataUpdateRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					playerID = pPlayerID;
					playerNumber = pPlayerNumber;
					eID = pEID;
				}

				public PlayerDataUpdateRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerDataUpdateRequest() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					playerNumber = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 2;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(playerNumber);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0501] -   Server<-Client  <-  GameCommandType.PlayerDataUpdateRequest");

					var v_exec = (Game_Server) pADEG.gameServer;
					var v_gsPlayer = (Game_ServerPlayer) v_exec.gsPlayers.Find(x => (x.ID == playerID));
					if (v_gsPlayer != null) {
						v_gsPlayer.playerNumber = playerNumber;
						//SteamMatchmaking.SetLobbyMemberData(pADEG.gameLobby.lobby.CSID, "number", v_gsPlayer.playerNumber.ToString());

						ALog.LogDebug("[CSCOMM][0502] -   Server->Client  ->  GameCommandType.PlayerDataUpdateResponse");
						pADEG.gameClient.CommandServerToClientSend(v_gsPlayer.gsClient.client, new Game_Command.PlayerDataUpdateResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK, v_gsPlayer.ID, v_gsPlayer.playerNumber));

						var v_commandInfoAll = (Game_Command.PlayerDataUpdate) new Game_Command.PlayerDataUpdate(++v_exec.commandID, -1, Time.time, v_exec.clientServer, v_gsPlayer.ID, v_gsPlayer.playerNumber, type);
						for (int i=0; i<v_exec.gsClients.Count; i++) {
							if (v_exec.gsClients[i] != v_gsPlayer.gsClient) {
								ALog.LogDebug("[CSCOMM][0504] -   Server->Client[*]  ->  GameCommandType.PlayerDataUpdate");
								pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandInfoAll);
							}
						}
					}
					else {
						ALog.LogErrorHandling("[CSCOMM][0502] -   Server->Client  ->  GameCommandType.PlayerDataUpdateResponse", DBGC.DBG);
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.PlayerDataUpdateResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, playerID, playerNumber, 1));
					}

					return true;
				}
			}

			public class PlayerDataUpdateResponse : Game_Command {
				public int playerID;
				public int playerNumber;

				public PlayerDataUpdateResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pPlayerID, int pPlayerNumber, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerDataUpdateResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					playerID = pPlayerID;
					playerNumber = pPlayerNumber;
					eID = pEID;
				}

				public PlayerDataUpdateResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerDataUpdateResponse() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					playerNumber = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 2;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(playerNumber);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0503] -     Client<-Server  <-  GameCommandType.PlayerDataUpdateResponse");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
						var v_player = (Game_Player) v_exec.players.PlayerGet(playerID);
						if (v_player != null) {
							if (!pADEG.def_clientUnconfirmedLocalEffect) {
								v_player.number = playerNumber;
								pADEG.inputPlayerNumber = v_player.number;
							}
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}
					
					return true;
				}
			}

			public class PlayerDataUpdate : Game_Command {
				public int playerID;
				public int playerNumber;
				public int commandRequestType;

				public PlayerDataUpdate (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, int pPlayerNumber, int pCommandRequestType, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerDataUpdate, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					playerID = pPlayerID;
					playerNumber = pPlayerNumber;
					commandRequestType = pCommandRequestType;
					eID = pEID;
				}

				public PlayerDataUpdate (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerDataUpdate() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					playerNumber = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					commandRequestType = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 3;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(playerNumber);
					v_dataCommandAdditional[2] = BitConverter.GetBytes(commandRequestType);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					switch ((GameCommandType) commandRequestType) {
						case GameCommandType.PlayerLoginAck:
							ALog.LogDebug("[CSCOMM][0213] -         Client{*}<-Client  <-  GameCommandType.PlayerDataUpdate");
							break;
						case GameCommandType.PlayerLogin:
							ALog.LogDebug("[CSCOMM][0217] -       Client<-Client{*}  <-  GameCommandType.PlayerDataUpdate");
							break;
						case GameCommandType.PlayerStateUpdateRequest:
							ALog.LogDebug("[CSCOMM][0505] -     Client{*}<-Server  <-  GameCommandType.PlayerDataUpdate");
							break;
					}

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						var v_player = v_exec.players.PlayerGet(playerID);
						if (v_player != null) {
							v_player.number = playerNumber;
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}

					return true;
				}
			}

			public class PlayerGameStartRequest : Game_Command {
				public int levelID;

				public PlayerGameStartRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pLevelID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerGameStartRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					levelID = pLevelID;
					eID = pEID;
				}

				public PlayerGameStartRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerGameStartRequest() - pOffset: " + pOffset);
					levelID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(levelID);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0901] -   Server<-Client  <-  GameCommandType.PlayerGameStartRequest");

					var v_exec = (Game_Server) pADEG.gameServer;
					if ((v_exec.state == GameServerState.Active) && (v_exec.gameState == GameState.Lobby)) {
						ADE.lobby.MessageSend("The game is starting..", ADE_LobbyMessageType.Info);

						ALog.LogDebug("[CSCOMM][0902] -   Server->Client  ->  GameCommandType.PlayerGameStartResponse");
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.PlayerGameStartResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK));

						v_exec.gameState = GameState.Loading;
						ADE.lobby.MessageSend("The game is loading..", ADE_LobbyMessageType.Info);

						var v_commandRequestAll = (Game_Command.GameLoadRequest) new Game_Command.GameLoadRequest(++v_exec.commandID, -1, Time.time, v_exec.clientServer, levelID);
						for (int i=0; i<v_exec.gsClients.Count; i++) {
							ALog.LogDebug("[CSCOMM][0904] -   Server->Client[*]  ->  GameCommandType.GameLoadRequest");
							pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandRequestAll);
							v_exec.gsClients[i].gameState = GameState.Loading;
						}
					}
					else {
						ALog.LogErrorHandling("[CSCOMM][0902] -   Server->Client  ->  GameCommandType.PlayerGameStartResponse", DBGC.DBG);
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.PlayerGameStartResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseError, 1));
					}

					return true;
				}
			}

			public class PlayerGameStartResponse : Game_Command {

				public PlayerGameStartResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerGameStartResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					eID = pEID;
				}

				public PlayerGameStartResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0903] -     Client<-Server  <-  GameCommandType.PlayerGameStartResponse");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
					}
					else /*if (v_commandResponse.state == GameCommandState.ResponseError)*/ {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class PlayerInit : Game_Command {
				public new GameCommandType type;
				public int playerToID;
				public int playerID;

				public PlayerInit (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerToID, int pPlayerID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerInit, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					playerToID = pPlayerToID;
					playerID = pPlayerID;
					eID = pEID;
				}

				public PlayerInit (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					type = base.type.ToEnum<GameCommandType>();
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerInit() - pOffset: " + pOffset);
					playerToID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					return ToDataAll(ToDataExtra());
				}

				public byte[][] ToDataExtra (int pDataCommandExtendedCount = 0) {
					var v_dataCommandAdditionalCount = (int) 2;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount +pDataCommandExtendedCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerToID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(playerID);

					return v_dataCommandAdditional;
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][0911] -       Client{**}<-Client{*}  <-  GameCommandType.PlayerInit");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						var v_player = (Game_Player) v_exec.players.PlayerGet(playerID);
						var v_playerSelf = (Game_Player) v_exec.players.PlayerGet(playerToID);
						if ((v_player != null) && (v_playerSelf != null)) {
							if (v_player.location == Location.Remote) {
								ALog.Log("[P[" + v_playerSelf.ID + "]]  ->  self (" + v_playerSelf.ToString() + ") receives new game data from remote player:  " + v_player.ToString());
								v_player.state = GamePlayerState.Initialized;

							}
							else /*if (v_player.location == Location.Local)*/ {
								ALog.Log("[P[" + v_playerSelf.ID + "]]  ->  self (" + v_playerSelf.ToString() + ") shared new game data from local player:  " + v_player.ToString());
							}
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}

					// when all players were initialized
					if (v_exec.players.Find(x => (x.state != GamePlayerState.Initialized)) == null) {
						v_exec.gameState = GameState.Initialized;
						ALog.LogDebug("[CSCOMM][0912] -     Client{*}->Server  ->  GameCommandType.GameInitResponse");
						pADEG.gameClient.CommandClientToServerSend(pADEG.gameClient.clientServer, new Game_Command.GameInitResponse(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, GameCommandState.ResponseOK));
					}

					return true;
				}
			}

			public class PlayerLeaveRequest : Game_Command {
				public int playerID;

				public PlayerLeaveRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerLeaveRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					playerID = pPlayerID;
					eID = pEID;
				}

				public PlayerLeaveRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerLeaveRequest() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -   Server<-Client  <-  GameCommandType.PlayerLeaveRequest");
					ADE.lobby.MessageSend("[GS]  -> player: " + playerID + " leaves the game!", ADE_LobbyMessageType.Info, ADE_MessageCategory.Development);

					var v_exec = (Game_Server) pADEG.gameServer;
					var v_gsPlayer = (Game_ServerPlayer) v_exec.gsPlayers.Find(x => (x.ID == playerID));
					if ((v_gsPlayer != null) && (v_gsPlayer.gsClient.client.CheckIfSame(clientFrom))) {
						ADE.lobby.MessageSend("[GS]  -> player[" + v_gsPlayer.ID + "] "  + v_gsPlayer.name + " leaves the game!", ADE_LobbyMessageType.Info);

						v_exec.gsPlayers.Remove(v_gsPlayer);
						if (v_exec.gsPlayers.Find(x => (x.gsClient == v_gsPlayer.gsClient)) == null) {
							v_exec.gsClients.Remove(v_gsPlayer.gsClient);
						}
						ALog.LogDebug("[CSCOMM][] -   Server->Client  ->  GameCommandType.PlayerLeaveResponse");
						pADEG.gameClient.CommandServerToClientSend(v_gsPlayer.gsClient.client, new Game_Command.PlayerLeaveResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK));

						var v_commandInfoAll = (Game_Command.PlayerLeave) new Game_Command.PlayerLeave(++v_exec.commandID, -1, Time.time, v_exec.clientServer, v_gsPlayer.ID);
						for (int i=0; i<v_exec.gsClients.Count; i++) {
							ALog.LogDebug("[CSCOMM][] -   Server->Client[*]  ->  GameCommandType.PlayerLeave");
							pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandInfoAll);
						}
					}
					else {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class PlayerLeaveResponse : Game_Command {

				public PlayerLeaveResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerLeaveResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					eID = pEID;
				}

				public PlayerLeaveResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -     Client<-Server  <-  GameCommandType.PlayerLeaveResponse");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
						v_exec.StartCoroutine(v_exec.GameStop());
					}
					else /*if (v_commandResponse.state == GameCommandState.ResponseError)*/ {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class PlayerLeave : Game_Command {
				public int playerID;

				public PlayerLeave (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerLeave, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					playerID = pPlayerID;
					eID = pEID;
				}

				public PlayerLeave (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerLeave() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -     Client{*}<-Server  <-  GameCommandType.PlayerLeave");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						var v_gsPlayer = (Game_Player) v_exec.players.PlayerGet(playerID);
						if (v_gsPlayer != null) {
							v_exec.players.Remove(v_gsPlayer);
							if (v_exec.players.Find(x => (x.client == v_gsPlayer.client)) == null) {
								pADEG.gameClient.clients.Remove(v_gsPlayer.client);
							}
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}

					return true;
				}
			}

			public class PlayerKickRequest : Game_Command {
				public int playerID;

				public PlayerKickRequest (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerKickRequest, GameCommandMode.GameServer, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Request;
					playerID = pPlayerID;
					eID = pEID;
				}

				public PlayerKickRequest (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerKickRequest() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -   Server<-Client  <-  GameCommandType.PlayerKickRequest");

					var v_exec = (Game_Server) pADEG.gameServer;
					var v_gsPlayer = (Game_ServerPlayer) v_exec.gsPlayers.Find(x => (x.ID == playerID));
					if (v_gsPlayer != null) {
						ADE.lobby.MessageSend("[GS]  -> player[" + v_gsPlayer.ID + "] " + v_gsPlayer.name + " kicked from the game!", ADE_LobbyMessageType.Info);

						ALog.LogDebug("[CSCOMM][] -   Server->Client  ->  GameCommandType.PlayerKickResponse");
						pADEG.gameClient.CommandServerToClientSend(clientFrom, new Game_Command.PlayerKickResponse(++v_exec.commandID, lID, Time.time, v_exec.clientServer, GameCommandState.ResponseOK));

						var v_commandInfoAll = (Game_Command.PlayerKick) new Game_Command.PlayerKick(++v_exec.commandID, -1, Time.time, v_exec.clientServer, v_gsPlayer.ID);
						for (int i=0; i<v_exec.gsClients.Count; i++) {
							ALog.LogDebug("[CSCOMM][] -   Server->Client[*]  ->  GameCommandType.PlayerKick");
							pADEG.gameClient.CommandServerToClientSend(v_exec.gsClients[i].client, v_commandInfoAll);
						}

						v_exec.gsPlayers.Remove(v_gsPlayer);
						if (v_exec.gsPlayers.Find(x => (x.gsClient == v_gsPlayer.gsClient)) == null) {
							v_exec.gsClients.Remove(v_gsPlayer.gsClient);
						}
					}
					else {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class PlayerKickResponse : Game_Command {

				public PlayerKickResponse (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, GameCommandState pState, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerKickResponse, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = pState;
					eID = pEID;
				}

				public PlayerKickResponse (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
				}

				public override byte[] ToData() {
					return ToDataBase();
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -     Client<-Server  <-  GameCommandType.PlayerKickResponse");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.ResponseOK) {
					}
					else /*if (v_commandResponse.state == GameCommandState.ResponseError)*/ {
						ALog.LogErrorHandling(DBGC.TODO);
					}

					return true;
				}
			}

			public class PlayerKick : Game_Command {
				public int playerID;

				public PlayerKick (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.PlayerKick, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					playerID = pPlayerID;
					eID = pEID;
				}

				public PlayerKick (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.PlayerKick() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -     Client{*}<-Server  <-  GameCommandType.PlayerKick");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						var v_gsPlayer = (Game_Player) v_exec.players.PlayerGet(playerID);
						if (v_gsPlayer != null) {
							if (v_gsPlayer != v_exec.player) {
								v_exec.players.Remove(v_gsPlayer);
								if (v_exec.players.Find(x => (x.client == v_gsPlayer.client)) == null) {
									pADEG.gameClient.clients.Remove(v_gsPlayer.client);
								}
							}
							else {
								ALog.LogToDo("[GCR][HARDCODED] temporary - to be properly implemented (see ADEG_GCR.GCR_Command_PlayerKick_Extended() method)");
								PlayerPrefs.SetInt("SwitchMenu", 5);

								v_exec.StartCoroutine(v_exec.GameStop());
							}
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}

					return true;
				}
			}

			public class Data : Game_Command {
				public new GameCommandType type;
				public int playerID;
				public int dataType;
				public byte[] data;

				public Data (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, int pDataType, byte[] pData, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.Data, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					type = base.type.ToEnum<GameCommandType>();
					state = GameCommandState.Info;
					playerID = pPlayerID;
					dataType = pDataType;
					data = pData;
					eID = pEID;
				}

				public Data (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					type = base.type.ToEnum<GameCommandType>();
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.Data() - pOffset: " + pOffset);
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					dataType = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					data = new byte[pData.Length -pOffset];
					Array.Copy(pData, pOffset, data, 0, data.Length);
					pOffset += data.Length *sizeof(byte);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 3;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(dataType);
					v_dataCommandAdditional[2] = data;

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -   Client{*}<-Client  <-  GameCommandType.Data");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						v_exec.CommandDataExecute(this);

						ALog.LogDebug("[CSCOMM][] -   Client{*}->Client  ->  GameCommandType.DataAck");
						pADEG.gameClient.CommandClientToClientSend(clientFrom, new Game_Command.DataAck(-1, ++pADEG.gameClient.commandID, Time.time, pADEG.gameUser.client, pADEG.gameManager.player.ID, dataType, data.Length));
					}

					return true;
				}
			}

			public class DataAck : Game_Command {
				public new GameCommandType type;
				public int playerID;
				public int dataType;
				public int dataLength;

				public DataAck (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, int pPlayerID, int pDataType, int pDataLength, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.DataAck, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					type = base.type.ToEnum<GameCommandType>();
					state = GameCommandState.Info;
					playerID = pPlayerID;
					dataType = pDataType;
					dataLength = pDataLength;
					eID = pEID;
				}

				public DataAck (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.DataAck() - pOffset: " + pOffset);
					type = base.type.ToEnum<GameCommandType>();
					playerID = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					dataType = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					dataLength = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 3;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = BitConverter.GetBytes(playerID);
					v_dataCommandAdditional[1] = BitConverter.GetBytes(dataType);
					v_dataCommandAdditional[2] = BitConverter.GetBytes(dataLength);

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -     Client<-Client{*}  <-  GameCommandType.DataAck");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						ALog.LogError("[TODO]  Game_Command.DataAck.Execute()  ->  add code");
					}

					return true;
				}
			}

			public class Action : Game_Command {
				public Game_Action action;

				public Action (int pSID, int pLID, float pTimestamp, ADE_Client pClientFrom, Game_Action pAction, int pEID = -1) : base(GameCommandCategory.Base, (int) GameCommandType.Action, GameCommandMode.GameManager, pSID, pLID, pTimestamp, pClientFrom) {
					state = GameCommandState.Info;
					action = pAction;
					eID = pEID;
				}

				public Action (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList, ADEG pADEG) : base(pData, ref pOffset, ref pClientsList) {
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Game_Command.Action() - pOffset: " + pOffset);
					action = pADEG.gameManager.ActionNew(pData, ref pOffset, ref pClientsList);
				}

				public override byte[] ToData() {
					var v_dataCommandAdditionalCount = (int) 1;
					var v_dataCommandAdditional = (byte[][]) new byte[v_dataCommandAdditionalCount][];

					v_dataCommandAdditional[0] = action.ToData();

					return ToDataAll(v_dataCommandAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					ALog.LogDebug("[CSCOMM][] -   Client{*}<-Client  <-  GameCommandType.Action");

					var v_exec = (Game_Manager) pADEG.gameManager;
					if (state == GameCommandState.Info) {
						ALog.LogDebug("[GM]  ->  t[" + v_exec.tickID + "] self (" + pADEG.gameUser.client.ToString() + ") remotely receives player: " + v_exec.players.PlayerGet(action.playerID) + " -> action: " + action.ToString());

						if (!ADEG.dbg_gameActionTimeoutActive || (v_exec.tickID == (action.tickID +1))) {
							v_exec.actionsReceived.Add(action);
						}
						else {
							ALog.LogTag("[WIP][000] - comm desync");
							ALog.LogError("[GM]  ->  t[" + v_exec.tickID + "] received an out of sync action from t[" + action.tickID + "] other than the previous tick");

							v_exec.StartCoroutine(v_exec.GameStop());
						}
					}

					return true;
				}
			}

			/*public class RTT : Game_Command {
				case GameDataType.SSRTT: {
					var v_dataTimePing = (byte[]) new byte[sizeof(float)];
					for (int i=0; i<v_dataTimePing.Length; i++) {
						v_dataTimePing[i] = pData[v_offset +i];
					}
					v_offset += v_dataTimePing.Length;

					var v_client = (ADE_Client) ADE.lobby.ClientGet(pClientSteamID);
					if (v_client != null) {
						var v_player = (Game_Player) adeg.gameManager.players.PlayerGet(v_client);
						if (v_player != null) {
							var v_rttTimePing = (float) BitConverter.ToSingle(v_dataTimePing, 0);
							var v_rttTimePong = (float) Time.time;
							ALog.LogWarning("P2PDataReceived(" + v_player.name + " ->" + adeg.gameManager.player.name + ") -> SSRTT pong: " + v_rttTimePong + " , RTT: " + (v_rttTimePing -v_rttTimePong) + " ms");

							adeg.gameManager.rtt_time = v_rttTimePong - v_rttTimePing;
							adeg.gameManager.rtt_waiting = false;
						}
						else {
							ALog.LogErrorHandling(DBGC.TODO);
						}
					}
					else {
						ALog.LogErrorHandling(DBGC.TODO);
					}
				}
					break;
			}*/

			#endregion
		}

		/// <summary>
		/// [ENUM] command categories
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameCommandCategory {
			[Description("NONE")]
			NONE = -1,
			[Description("Base")]
			Base = 0,
			[Description("Extended")]
			Extended = 1,
			[Description("Custom")]
			Custom = 2,
		}

		/// <summary>
		/// [ENUM] command types
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameCommandType {
			[Description("NONE")]
			NONE = -1,
			[Description("Server Activate Request")]
			ServerActivateRequest = 0,
			[Description("Server Activate Response")]
			ServerActivateResponse = 1,
			[Description("Server Get Request")]
			ServerGetRequest = 2,
			[Description("Server Get Response")]
			ServerGetResponse = 3,
			[Description("Server Password Update Request")]
			ServerPasswordUpdateRequest = 4,
			[Description("Server Password Update Response")]
			ServerPasswordUpdateResponse = 5,
			[Description("Game Data Update Request")]
			GameDataUpdateRequest = 10,
			[Description("Game Data Update Response")]
			GameDataUpdateResponse = 11,
			[Description("Game Data Update")]
			GameDataUpdate = 12,
			[Description("Game Load Request")]
			GameLoadRequest = 21,
			[Description("Game Load Response")]
			GameLoadResponse = 22,
			[Description("Game Init Request")]
			GameInitRequest = 23,
			[Description("Game Init Response")]
			GameInitResponse = 24,
			[Description("Game Start Request")]
			GameStartRequest = 25,
			[Description("Game Start Response")]
			GameStartResponse = 26,
			[Description("Game Start")]
			GameStart = 27,
			[Description("Game Stop Request")]
			GameStopRequest = 30,
			[Description("Game Stop Response")]
			GameStopResponse = 31,
			[Description("Game Stop")]
			GameStop = 32,
			[Description("Game End Request")]
			GameEndRequest = 33,
			[Description("Game End Response")]
			GameEndResponse = 34,
			[Description("Game End")]
			GameEnd = 35,
			[Description("Time Sync Request")]
			TimeSyncRequest = 40,
			[Description("Time Sync Response")]
			TimeSyncResponse = 41,
			[Description("Time Sync")]
			TimeSync = 42,
			[Description("Time Sync Ack")]
			TimeSyncAck = 43,
			[Description("Ping Request")]
			PingRequest = 45,
			[Description("Ping Response")]
			PingResponse = 46,
			[Description("Player Login Request")]
			PlayerLoginRequest = 50,
			[Description("Player Login Response")]
			PlayerLoginResponse = 51,
			[Description("Player Login")]
			PlayerLogin = 52,
			[Description("Player Login Ack")]
			PlayerLoginAck = 53,
			[Description("Player State Update Request")]
			PlayerStateUpdateRequest = 60,
			[Description("Player State Update Response")]
			PlayerStateUpdateResponse = 61,
			[Description("Player State Update")]
			PlayerStateUpdate = 62,
			[Description("Player Data Update Request")]
			PlayerDataUpdateRequest = 63,
			[Description("Player Data Update Response")]
			PlayerDataUpdateResponse = 64,
			[Description("Player Data Update")]
			PlayerDataUpdate = 65,
			[Description("Player Game Start Request")]
			PlayerGameStartRequest = 66,
			[Description("Player Game Start Response")]
			PlayerGameStartResponse = 67,
			[Description("Player Init")]
			PlayerInit = 68,
			[Description("Player Leave Request")]
			PlayerLeaveRequest = 70,
			[Description("Player Left Response")]
			PlayerLeaveResponse = 71,
			[Description("Player Leave")]
			PlayerLeave = 72,
			[Description("Player Kick Request")]
			PlayerKickRequest = 73,
			[Description("Player Kick Response")]
			PlayerKickResponse = 74,
			[Description("Player Kick")]
			PlayerKick = 75,
			[Description("Data")]
			Data = 80,
			[Description("DataAck")]
			DataAck = 81,
			[Description("Action")]
			Action = 90,
		}

		/// <summary>
		/// [ENUM] command data types
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameCommandDataType {
			[Description("NONE")]
			NONE = -1,
		}

		/// <summary>
		/// [ENUM] command modes
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameCommandMode {
			[Description("NONE")]
			NONE = -1,
			[Description("GameServer")]
			GameServer = 0,
			[Description("GameManager")]
			GameManager = 1,
		}


		/// <summary>
		/// [ENUM] command states
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameCommandState {
			[Description("NONE")]
			NONE = -1,
			[Description("Info")]
			Info = 0,
			[Description("Request")]
			Request = 1,
			[Description("ResponseOK")]
			ResponseOK = 2,
			[Description("ResponseError")]
			ResponseError = 3,
		}

		public class Game_Action {
			public GameActionCategory category;
			public int type;
			public float timeGame;
			public int tickID;
			public float tickTime;
			public int playerID;

			public Game_Action (GameActionCategory pCategory, int pType, float pTimeGame, int pTickID, float pTickTime, int pPlayerID) {
				category = pCategory;
				type = pType;
				timeGame = pTimeGame;
				tickID = pTickID;
				tickTime = pTickTime;
				playerID = pPlayerID;
			}

			public Game_Action (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) {
				category = (GameActionCategory) BitConverter.ToInt32(pData, pOffset).ToEnum<GameActionCategory>();
				pOffset += sizeof(int);
				type = BitConverter.ToInt32(pData, pOffset);
				pOffset += sizeof(int);
				timeGame = BitConverter.ToSingle(pData, pOffset);
				pOffset += sizeof(float);
				tickID = BitConverter.ToInt32(pData, pOffset);
				pOffset += sizeof(int);
				tickTime = BitConverter.ToSingle(pData, pOffset);
				pOffset += sizeof(float);
				playerID = BitConverter.ToInt32(pData, pOffset);
				pOffset += sizeof(int);
			}

			public virtual byte[] ToData() {
				return ToDataBase();
			}

			public byte[] ToDataBase() {
				var v_dataActionCount = (int) 6;
				var v_dataAction = (byte[][]) new byte[v_dataActionCount][];
				v_dataAction[0] = (byte[]) BitConverter.GetBytes((int) category);
				v_dataAction[1] = (byte[]) BitConverter.GetBytes(type);
				v_dataAction[2] = (byte[]) BitConverter.GetBytes(timeGame);
				v_dataAction[3] = (byte[]) BitConverter.GetBytes(tickID);
				v_dataAction[4] = (byte[]) BitConverter.GetBytes(tickTime);
				v_dataAction[5] = (byte[]) BitConverter.GetBytes(playerID);

				// computing total data length
				var v_dataLength = (int) 0;
				for (int i=0; i<v_dataAction.GetLength(0); i++) {
					v_dataLength += v_dataAction[i].Length;
				}

				// filling up the data
				var v_data = (byte[]) new byte[v_dataLength];
				var v_offset = (int) 0;
				for (int i=0; i<v_dataAction.GetLength(0); i++) {
					for (int j=0; j<v_dataAction[i].Length; j++) {
						v_data[v_offset +j] = v_dataAction[i][j];
					}
					v_offset += v_dataAction[i].Length;
				}

				return v_data;
			}

			public byte[] ToDataAll (byte[][] pDataActionAdditional) {
				var v_dataCommand = (byte[]) ToDataBase();

				// computing total data length
				var v_dataLength = (int) v_dataCommand.Length;
				for (int i=0; i<pDataActionAdditional.GetLength(0); i++) {
					v_dataLength += pDataActionAdditional[i].Length;
				}

				// filling up the data
				var v_data = (byte[]) new byte[v_dataLength];
				var v_offset = (int) 0;
				//v_dataCommand.CopyTo(v_data, 0);
				for (int i=0; i<v_dataCommand.Length; i++) {
					v_data[v_offset +i] = v_dataCommand[i];
				}
				v_offset += v_dataCommand.Length;
				for (int i=0; i<pDataActionAdditional.GetLength(0); i++) {
					for (int j=0; j<pDataActionAdditional[i].Length; j++) {
						v_data[v_offset +j] = pDataActionAdditional[i][j];
					}
					v_offset += pDataActionAdditional[i].Length;
				}

				return v_data;
			}

			public virtual bool Execute (ADEG pADEG) {
				var v_player = (Game_Player) pADEG.gameManager.players.PlayerGet(playerID);
				ALog.LogDebug("[GM]  ->  t[" + tickID + "] self (" + pADEG.gameUser.client + ") executes player: " + v_player.ToString() + " -> action: " + ToString());

				return true;
			}


			public override string ToString() {
				return ("[" + tickID + "](" + tickTime + ")(" + timeGame + ") playerID: " + playerID + ", type: " + category);
			}


			#region game actions
			public class Data : Game_Action {
				public new GameActionType type;
				public int dataType;
				public byte[] data;

				public Data (float pTimeGame, int pTickID, float pTickTime, int pPlayerID, int pDataType, byte[] pData) : base(GameActionCategory.Base, (int) GameActionType.Data, pTimeGame, pTickID, pTickTime, pPlayerID) {
					type = base.type.ToEnum<GameActionType>();
					dataType = pDataType;
					data = pData;
				}

				public Data (byte[] pData, ref int pOffset, ref List<ADE_Client> pClientsList) : base(pData, ref pOffset, ref pClientsList) {
					type = base.type.ToEnum<GameActionType>();
					//ALog.LogDebug("[CSCOMM][CONV]  ->  Data() - pOffset: " + pOffset);
					dataType = BitConverter.ToInt32(pData, pOffset);
					pOffset += sizeof(int);
					data = new byte[pData.Length -pOffset];
					Array.Copy(pData, pOffset, data, 0, data.Length);
					pOffset += data.Length *sizeof(byte);
				}

				public override byte[] ToData() {
					var v_dataActionAdditionalCount = (int) 2;
					var v_dataActionAdditional = (byte[][]) new byte[v_dataActionAdditionalCount][];

					v_dataActionAdditional[0] = BitConverter.GetBytes(dataType);
					v_dataActionAdditional[1] = data;

					return ToDataAll(v_dataActionAdditional);
				}

				public override bool Execute (ADEG pADEG) {
					var v_result = (bool) false;

					var v_player = (Game_Player) pADEG.gameManager.players.PlayerGet(playerID);
					ALog.LogDebug("[GM]  ->  t[" + tickID + "] self (" + pADEG.gameUser.client + ") executes player: " + v_player.ToString() + " -> action: " + ToString());

					v_result = pADEG.gameManager.ActionDataExecute(this);

					return v_result;
				}

				public override string ToString() {
					return ("[" + tickID + "](" + tickTime + ")(" + timeGame + ") playerID: " + playerID + ", type: " + type + "  ->  playerID " + playerID + ", dataType: " + ((GameActionDataType) dataType) + ", data.Length: " + data.Length);
				}
			}
			#endregion
		}

		/// <summary>
		/// [ENUM] action categories
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameActionCategory {
			[Description("NONE")]
			NONE = -1,
			[Description("Base")]
			Base = 0,
			[Description("Extended")]
			Extended = 1,
			[Description("Custom")]
			Custom = 2,
		}

		/// <summary>
		/// [ENUM] action types
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameActionType {
			[Description("NONE")]
			NONE = -1,
			[Description("Data")]
			Data = 0,
		}

		/// <summary>
		/// [ENUM] action data types
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameActionDataType {
			[Description("NONE")]
			NONE = -1,
		}


		public class Game_Society : MonoBehaviour {
			public ADEG adeg_;
			public ADEG adeg {
				get {
					return adeg_;
				}
			}

			public List<Game_Person> persons;
			public List<Game_Group> groups;


			public void Awake_<TADEG>() where TADEG : ADEG {
				var v_obj = (GameObject) GameObject.Find(ADEG.def_ADEG_Name);
				if (v_obj != null) {
					adeg_ = v_obj.GetComponent<TADEG>();

					Reset();
				}
			}

			void Update() {
				Update_();
			}

			public void Update_() {
				if (ADE.instance.connected && (SocietyPersonSelfGetState() != GamePersonState.Offline)) {
					var v_friend = (Game_Person) null;
					for (int i=0; i<SocietyPersonsNoGet(); i++) {
						v_friend = (Game_Person) SocietyPersonGetByIndex(i);

						v_friend.client.name = SocietyPersonGetName(v_friend);
						v_friend.state = SocietyPersonGetState(v_friend);
						if (v_friend.state == GamePersonState.Online) {
							SocietyPersonGetGameInfo(v_friend);
						}
					}

					var v_group = (Game_Group) null;
					for (int i=0; i<SocietyGroupsNoGet(); i++) {
						v_group = SocietyGroupGetByIndex(i);
						if (v_group.groupId != -1) {
							v_friend.client.name = SocietyPersonGetName(v_friend);
							v_friend.state = SocietyPersonGetState(v_friend);
						}	
					}
				}
			}


			public GamePersonState SocietyPersonSelfGetState() {
				var v_state = (GamePersonState) GamePersonState.NONE;

				switch (ADE.instance.platformMain) {
					case ADE_Platform.Steam: {
						v_state = SteamPersonStateToGamePersonState(SteamFriends.GetPersonaState());
					}
						break;
					case ADE_Platform.GoG: {
						v_state = GalaxyPersonStateToGamePersonState(GalaxyInstance.Friends().GetPersonaState());
					}
						break;
				}

				return v_state;
			}

			public int SocietyPersonsNoGet() {
				var v_personsNo = (int) 0;

				switch (ADE.instance.platformMain) {
					case ADE_Platform.Steam: {
						v_personsNo = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
					}
						break;
					case ADE_Platform.GoG: {
						v_personsNo = (int) GalaxyInstance.Friends().GetFriendCount();
					}
						break;
				}

				return v_personsNo;
			}

			public Game_Person SocietyPersonGetByIndex (int pIndex) {
				var v_person = (Game_Person) null;

				switch (ADE.instance.platformMain) {
					case ADE_Platform.Steam: {
						var v_CSteamID = (CSteamID) SteamFriends.GetFriendByIndex(pIndex, EFriendFlags.k_EFriendFlagImmediate);
						v_person = persons.Find(x => x.client.aID.CheckIfSame(v_CSteamID));
						if (v_person == null) {
							v_person = new Game_Person(new ADE_Client(v_CSteamID), SteamFriends.GetFriendPersonaName(v_CSteamID));
							persons.Add(v_person);
						}
					}
						break;
					case ADE_Platform.GoG: {
						var v_GalaxyID = (GalaxyID) GalaxyInstance.Friends().GetFriendByIndex((uint) pIndex);
						v_person = persons.Find(x => x.client.aID.CheckIfSame(v_GalaxyID));
						if (v_person == null) {
							v_person = new Game_Person(new ADE_Client(v_GalaxyID), GalaxyInstance.Friends().GetFriendPersonaName(v_GalaxyID));
							persons.Add(v_person);
						}
					}
						break;
				}

				return v_person;
			}

			public string SocietyPersonGetName (Game_Person pPerson) {
				var v_name = (string) "";

				switch (ADE.instance.platformMain) {
					case ADE_Platform.Steam: {
						v_name = SteamFriends.GetFriendPersonaName(pPerson.client.aID.CSID);
					}
						break;
					case ADE_Platform.GoG: {
						v_name = GalaxyInstance.Friends().GetFriendPersonaName(pPerson.client.aID.GID);
					}
						break;
				}

				return v_name;
			}

			public GamePersonState SocietyPersonGetState (Game_Person pPerson) {
				var v_state = (GamePersonState) GamePersonState.NONE;

				switch (ADE.instance.platformMain) {
					case ADE_Platform.Steam: {
						v_state = SteamPersonStateToGamePersonState(SteamFriends.GetFriendPersonaState(pPerson.client.aID.CSID));
					}
						break;
					case ADE_Platform.GoG: {
						v_state = GalaxyPersonStateToGamePersonState(GalaxyInstance.Friends().GetFriendPersonaState(pPerson.client.aID.GID));
					}
						break;
				}

				return v_state;
			}

			public GamePersonState SteamPersonStateToGamePersonState (EPersonaState pState) {
				var v_state = (GamePersonState) GamePersonState.NONE;

				switch (pState) {
					case EPersonaState.k_EPersonaStateOffline:
					case EPersonaState.k_EPersonaStateMax:
						v_state = GamePersonState.Offline;
						break;
					case EPersonaState.k_EPersonaStateSnooze:
					case EPersonaState.k_EPersonaStateAway:
					case EPersonaState.k_EPersonaStateOnline:
					case EPersonaState.k_EPersonaStateBusy:
					case EPersonaState.k_EPersonaStateLookingToPlay:
					case EPersonaState.k_EPersonaStateLookingToTrade:
						v_state = GamePersonState.Online;
						break;
					default:
						v_state = GamePersonState.NONE;
						break;
				}

				return v_state;
			}

			public GamePersonState GalaxyPersonStateToGamePersonState (PersonaState pState) {
				var v_state = (GamePersonState) GamePersonState.NONE;

				switch (pState) {
					case PersonaState.PERSONA_STATE_OFFLINE:
						v_state = GamePersonState.Offline;
						break;
					case PersonaState.PERSONA_STATE_ONLINE:
						v_state = GamePersonState.Online;
						break;
					default:
						v_state = GamePersonState.NONE;
						break;
				}

				return v_state;
			}

			public void SocietyPersonGetGameInfo (Game_Person pPerson) {
				switch (ADE.instance.platformMain) {
					case ADE_Platform.Steam: {
						var v_gameInfo = (FriendGameInfo_t) new FriendGameInfo_t();
						if (SteamFriends.GetFriendGamePlayed(pPerson.client.aID.CSID, out v_gameInfo)) {
							if (pPerson.name.Equals("lord_ender", StringComparison.InvariantCultureIgnoreCase)) {
								var v_state = SteamPersonStateToGamePersonState(SteamFriends.GetFriendPersonaState(pPerson.client.aID.CSID));
							}

							pPerson.gameID = v_gameInfo.m_gameID.m_GameID;
							if (v_gameInfo.m_steamIDLobby.IsValid()) {
								pPerson.lobbyID = v_gameInfo.m_steamIDLobby.m_SteamID;
							}
						}
					}
						break;
					case ADE_Platform.GoG: {
		//				var v_gameInfo = (FriendGameInfo_t) new FriendGameInfo_t();
		//				if (GalaxyInstance.Friends().GetFriendGamePlayed(pPerson.client.aID.GID, out v_gameInfo)) {
		//					pPerson.gameID = v_gameInfo.m_gameID.m_GameID;
		//					if (v_gameInfo.m_steamIDLobby.IsValid()) {
		//						pPerson.lobbyID = v_gameInfo.m_steamIDLobby.m_SteamID;
		//					}
		//				}
					}
						break;
				}
			}

			public int SocietyGroupsNoGet() {
				var v_groupsNo = (int) 0;

				switch (ADE.instance.platformMain) {
					case ADE_Platform.Steam: {
						v_groupsNo = SteamFriends.GetFriendsGroupCount();
					}
						break;
					case ADE_Platform.GoG: {
						//v_groupsNo = (int) GalaxyInstance.Friends().GetFriendsGroupCount();
					}
						break;
				}

				return v_groupsNo;
			}

			public Game_Group SocietyGroupGetByIndex (int pIndex) {
				var v_group = (Game_Group) null;

				switch (ADE.instance.platformMain) {
					case ADE_Platform.Steam: {
						var v_groupId = (FriendsGroupID_t) SteamFriends.GetFriendsGroupIDByIndex(pIndex);
						v_group = groups.Find(x => (x.groupId == v_groupId.m_FriendsGroupID));
						if (v_group == null) {
							v_group = new Game_Group(v_groupId.m_FriendsGroupID, SteamFriends.GetFriendsGroupName(v_groupId), (uint) SteamFriends.GetFriendsGroupMembersCount(v_groupId));
							groups.Add(v_group);
						}
					}
						break;
					case ADE_Platform.GoG: {
		//				var v_groupId = (FriendsGroupID_t) GalaxyInstance.Friends().GetFriendsGroupIDByIndex((uint) pIndex);
		//				v_group = groups.Find(x => (x.groupId == v_groupId.m_FriendsGroupID));
		//				if (v_group == null) {
		//					v_group = new Game_Group(v_groupId.m_FriendsGroupID, GalaxyInstance.Friends().GetFriendsGroupName(v_groupId), GalaxyInstance.Friends().GetFriendsGroupMembersCount(v_groupId));
		//					groups.Add(v_group);
		//				}
					}
						break;
				}

				return v_group;
			}


			public void Reset() {
				if (persons == null) {
					persons = new List<Game_Person>();
				}
				else {
					persons.Clear();
				}
				if (groups == null) {
					groups = new List<Game_Group>();
				}
				else {
					groups.Clear();
				}

				ResetCustom();
			}

			public virtual void ResetCustom() {
				//
			}
		}

		public class Game_Person {
			public ADE_Client client;

			public string name;
			public GamePersonState state;

			public ulong gameID;
			public ulong lobbyID;


			public Game_Person (ADE_Client pClient, string pName) {
				client = pClient;

				name = pName;
				state = GamePersonState.NONE;

				gameID = 0;
				lobbyID = 0;
			}

			public override string ToString() {
				return (name + " (" + client.ID + ")(" + state + ")");
			}
		}

		public class Game_Group {
			public short groupId;

			public string name;
			public uint personsNo;


			public Game_Group (short pGroupId, string pName, uint pPersonsNo) {
				groupId = pGroupId;

				name = pName;
				personsNo = pPersonsNo;
			}

			public override string ToString() {
				return (name + " (" + groupId + ")(" + personsNo + ")");
			}
		}



		/// <summary>
		/// [ENUM] person states
		/// </summary>
		[DefaultValue(NONE)]
		public enum GamePersonState {
			[Description("NONE")]
			NONE = -1,
			[Description("Offline")]
			Offline = 0,
			[Description("Online")]
			Online = 1,
		}

		#if ADEF_ADEG_DBG
		public class DBG_TEst : MonoBehaviour {
			
			void Awake() {
				new Demo_ActionTypeCEnum();

				ALog.LogError("Demo_ActionTypeCEnum   ===>   .count: " + Demo_ActionTypeCEnum.i.count + " (" + Demo_ActionTypeCEnum.i.elements.Count + ")");
				ALog.LogError("Demo_ActionTypeCEnum   ===>   Demo_ActionTypeCEnum.Custom.value: " + Demo_ActionTypeCEnum.Custom.value);
				ALog.LogError("Demo_ActionTypeCEnum   ===>   Demo_ActionTypeCEnum.Movement.value: " + Demo_ActionTypeCEnum.Movement.value);
				var v_element = (EnumClassElement) null;
				v_element = Demo_ActionTypeCEnum.i.GetElement(1);
				ALog.LogError("Demo_ActionTypeCEnum   ===>   Demo_ActionTypeCEnum.i.GetElement(1): " + v_element.name);
				v_element = Demo_ActionTypeCEnum.i.GetElement("Custom");
				ALog.LogError("Demo_ActionTypeCEnum   ===>   Demo_ActionTypeCEnum.i.GetElement(\"Custom\"): " + v_element.name);
				v_element = Demo_ActionTypeCEnum.i.GetElement(2);
				ALog.LogError("Demo_ActionTypeCEnum   ===>   Demo_ActionTypeCEnum.i.GetElement(2): " + v_element.name);
				v_element = Demo_ActionTypeCEnum.i.GetElement("Movement");
				ALog.LogError("Demo_ActionTypeCEnum   ===>   Demo_ActionTypeCEnum.i.GetElement(\"Movement\"): " + v_element.name);
				v_element = Demo_ActionTypeCEnum.i.GetElement(3);
				ALog.LogError("Demo_ActionTypeCEnum   ===>   Demo_ActionTypeCEnum.i.GetElement(3): " + v_element.name);
				v_element = Demo_ActionTypeCEnum.i.GetElement("zzz");
				ALog.LogError("Demo_ActionTypeCEnum   ===>   Demo_ActionTypeCEnum.i.GetElement(\"zzz\"): " + v_element.name);

				v_element = Demo_ActionTypeCEnum.i.GetElement(UnityEngine.Random.Range(-1, Demo_ActionTypeCEnum.i.count +1));
				if (v_element == Demo_ActionTypeCEnum.NONE) {
					ALog.LogError("Demo_ActionTypeCEnum   ===>   v_element randomed Demo_ActionTypeCEnum.NONE");
				}
				else if (v_element == Demo_ActionTypeCEnum.Extended) {
					ALog.LogError("Demo_ActionTypeCEnum   ===>   v_element randomed Demo_ActionTypeCEnum.Extended");
				}
				else if (v_element == Demo_ActionTypeCEnum.Custom) {
					ALog.LogError("Demo_ActionTypeCEnum   ===>   v_element randomed Demo_ActionTypeCEnum.Custom");
				}
				else if (v_element == Demo_ActionTypeCEnum.Movement) {
					ALog.LogError("Demo_ActionTypeCEnum   ===>   v_element randomed Demo_ActionTypeCEnum.Movement");
				}

				v_element = Demo_ActionTypeCEnum.i.GetElement(UnityEngine.Random.Range(-1, Demo_ActionTypeCEnum.i.count +1));
				switch ((Demo_ActionTypeEnum) v_element.value) {
				case Demo_ActionTypeEnum.NONE: {
						ALog.LogError("Demo_ActionTypeCEnum   ===>   v_element randomed Demo_ActionTypeCEnum.NONE");
					}
					break;
				case Demo_ActionTypeEnum.Extended: {
						ALog.LogError("Demo_ActionTypeCEnum   ===>   v_element randomed Demo_ActionTypeCEnum.Extended");
					}
					break;
				case Demo_ActionTypeEnum.Custom: {
						ALog.LogError("Demo_ActionTypeCEnum   ===>   v_element randomed Demo_ActionTypeCEnum.Custom");
					}
					break;
				case Demo_ActionTypeEnum.Movement: {
						ALog.LogError("Demo_ActionTypeCEnum   ===>   v_element randomed Demo_ActionTypeCEnum.Movement");
					}
					break;
				}
			}
		}

		public class GameActionTypeCEnum : EnumClass {
			public static GameActionTypeCEnum i;
			public static int no;

			public static EnumClassElement NONE;
			public static EnumClassElement Extended;
			public static EnumClassElement Custom;

			public GameActionTypeCEnum() {
				i = this;

				NONE = ElementAdd("NONE", (int) GameActionTypeEnum.NONE, "NONE");
				Extended = ElementAdd("Extended", (int) GameActionTypeEnum.Extended, "Extended");
				Custom = ElementAdd("Custom", (int) GameActionTypeEnum.Custom, "Custom");

				Default = NONE;
			}
		}

		/// <summary>
		/// [ENUM] action custom types
		/// </summary>
		[DefaultValue(NONE)]
		public enum GameActionTypeEnum {
			[Description("NONE")]
			NONE = -1,
			[Description("Extended")]
			Extended = 0,
			[Description("Custom")]
			Custom = 1,
		}

		public class Demo_ActionTypeCEnum : GameActionTypeCEnum {
			public static EnumClassElement Movement;

			public Demo_ActionTypeCEnum() {
				i = this;

				Movement = ElementAdd("Movement", (int) Demo_ActionTypeEnum.Movement, "Movement");
				elements.Add(Movement);
			}
		}

		/// <summary>
		/// [ENUM] action custom types
		/// </summary>
		[DefaultValue(NONE)]
		public enum Demo_ActionTypeEnum {
			[Description("NONE")]
			NONE = -1,
			[Description("Extended")]
			Extended = 0,
			[Description("Custom")]
			Custom = 1,
			[Description("Movement")]
			Movement = 2,
		}
		#endif

	}
	
}
