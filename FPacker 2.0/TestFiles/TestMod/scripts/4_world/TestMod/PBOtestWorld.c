class PBOtestWorld
{
	float m_LastTickTime = GetGame().GetTickTime();
	float m_TickTime;
	
	void PBOtestWorld()
	{
		if (!m_LastTickTime) m_LastTickTime = 0.0;
		if (!m_TickTime) m_TickTime = 0.0;
	}
	
	void TestPrint()
	{
		if (GetGame().GetTickTime()) m_TickTime = GetGame().GetTickTime();
		Print("Loaded PBOtestWorld class m_TickTime: " + m_TickTime + " m_LastTickTime: " + m_LastTickTime);
		if (GetGame().GetTickTime()) m_LastTickTime = GetGame().GetTickTime();
	}
}

static ref PBOtestWorld g_PBOtestWorld;
static PBOtestWorld GetPBOtestWorld()
{
	if (!g_PBOtestWorld) g_PBOtestWorld = new PBOtestWorld();
	return g_PBOtestWorld;
}