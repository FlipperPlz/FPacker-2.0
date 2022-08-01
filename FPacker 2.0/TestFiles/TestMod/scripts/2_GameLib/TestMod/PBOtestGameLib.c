class PBOtestGameLib
{
	float m_LastTickTime, m_TickTime;
	
	void PBOtestGameLib()
	{
		if (!m_LastTickTime) m_LastTickTime = 0.0;
		if (!m_TickTime) m_TickTime = 0.0;
	}
	
	void TestPrint()
	{
		if (GetGame().GetTickTime()) m_TickTime = GetGame().GetTickTime();
		Print("Loaded PBOtestGameLib class m_TickTime: " + m_TickTime + " m_LastTickTime: " + m_LastTickTime);
		if (GetGame().GetTickTime()) m_LastTickTime = GetGame().GetTickTime();
	}
}

static ref PBOtestGameLib g_PBOtestGameLib;
static PBOtestGameLib GetPBOtestGameLib()
{
	if (!g_PBOtestGameLib) g_PBOtestGameLib = new PBOtestGameLib();
	return g_PBOtestGameLib;
}