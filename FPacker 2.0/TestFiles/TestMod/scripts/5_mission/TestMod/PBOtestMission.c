class PBOtestMission
{
	float m_LastTickTime = GetGame().GetTickTime();
	float m_TickTime;
	
	void PBOtestMission()
	{
		if (!m_LastTickTime) m_LastTickTime = 0.0;
		if (!m_TickTime) m_TickTime = 0.0;
		#ifdef BULDOZER
		Print("BULDOZER")
		#endif
		#ifdef NO_GUI
		Print("NO_GUI")
		#endif
		#ifdef NO_GUI_INGAME
		Print("NO_GUI_INGAME")
		#endif
		#ifdef SERVER
		Print("SERVER")
		#endif
		#ifdef DIAG
		Print("DIAG")
		#endif
		#ifdef DEVELOPER
		Print("DEVELOPER")
		#endif
		#ifdef RELEASE
		Print("RELEASE")
		#endif
		#ifdef DIAG_DEVELOPER
		Print("DIAG_DEVELOPER")
		#endif
		#ifdef BUILD_EXPERIMENTAL
		Print("BUILD_EXPERIMENTAL")
		#endif
		#ifdef SERVER_FOR_WINDOWS
		Print("SERVER_FOR_WINDOWS")
		#endif
		#ifdef SERVER_FOR_X1
		Print("SERVER_FOR_X1")
		#endif
		#ifdef SERVER_FOR_PS4
		Print("SERVER_FOR_PS4")
		#endif
		#ifdef SERVER_FOR_CONSOLE
		Print("SERVER_FOR_CONSOLE")
		#endif
		#ifdef PLATFORM_LINUX
		Print("PLATFORM_LINUX")
		#endif
		#ifdef PLATFORM_WINDOWS
		Print("PLATFORM_WINDOWS")
		#endif
		#ifdef PLATFORM_XBOX
		Print("PLATFORM_XBOX")
		#endif
		#ifdef PLATFORM_PS4
		Print("PLATFORM_PS4")
		#endif
		#ifdef PLATFORM_CONSOLE
		Print("PLATFORM_CONSOLE")
		#endif
	}
	
	void TestPrint()
	{
		if (GetGame().GetTickTime()) m_TickTime = GetGame().GetTickTime();
		Print("Loaded PBOtestMission class m_TickTime: " + m_TickTime + " m_LastTickTime: " + m_LastTickTime);
		if (GetGame().GetTickTime()) m_LastTickTime = GetGame().GetTickTime();
	}
}

static ref PBOtestMission g_PBOtestMission;
static PBOtestMission GetPBOtestMission()
{
	if (!g_PBOtestMission) g_PBOtestMission = new PBOtestMission();
	return g_PBOtestMission;
}