class PBOtestGame
{
	void PBOtestGame()
	{
		if (!m_LastTickTime) m_LastTickTime = 0.0;
		if (!m_TickTime) m_TickTime = 0.0;
	}
	
	void TestPrint()
	{
		if (GetGame().GetTickTime()) m_TickTime = GetGame().GetTickTime();
		Print("Loaded PBOtestGame class m_TickTime: " + m_TickTime + " m_LastTickTime: " + m_LastTickTime);
		if (GetGame().GetTickTime()) m_LastTickTime = GetGame().GetTickTime();
	}
}

static ref PBOtestGame g_PBOtestGame;
static PBOtestGame GetPBOtestGame()
{
	if (!g_PBOtestGame) g_PBOtestGame = new PBOtestGame();
	return g_PBOtestGame;
}