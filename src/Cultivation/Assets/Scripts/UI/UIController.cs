using UnityEngine;

namespace Cultivation.UI
{
    /// <summary>
    /// 모든 UI 패널의 루트 조율자. GameManager가 단일 참조를 보유한다.
    /// 패널 Open/Close 진입점을 제공하며, 한 번에 하나의 패널만 열린다.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [SerializeField] private HUDController _hud;
        [SerializeField] private GachaPanelController _gachaPanel;
        [SerializeField] private FarmPlotPanelController _farmPlotPanel;
        [SerializeField] private BarnPanelController _barnPanel;
        [SerializeField] private ShopPanelController _shopPanel;

        public HUDController HUD => _hud;

        private Systems.GameManager _gm;

        public void Initialize(Systems.GameManager gm)
        {
            _gm = gm;
            _hud?.Initialize(gm);
            _gachaPanel?.Initialize(gm);
            _farmPlotPanel?.Initialize(gm);
            _barnPanel?.Initialize(gm);
            _shopPanel?.Initialize(gm);

            // 패널 시작 시 모두 닫힘 상태
            _gachaPanel?.Close();
            _farmPlotPanel?.Close();
            _barnPanel?.Close();
            _shopPanel?.Close();
        }

        public void OpenGachaPanel()
        {
            CloseAll();
            _gachaPanel?.Open();
            _gm?.SetUIModeActive(true);
        }

        public void OpenFarmPlotPanel(int plotIndex)
        {
            CloseAll();
            _farmPlotPanel?.Open(plotIndex);
            _gm?.SetUIModeActive(true);
        }

        public void OpenBarnPanel()
        {
            CloseAll();
            _barnPanel?.Open();
            _gm?.SetUIModeActive(true);
        }

        public void OpenShopPanel()
        {
            CloseAll();
            _shopPanel?.Open();
            _gm?.SetUIModeActive(true);
        }

        /// <summary>GameManager의 ESC/E 핸들러에서 호출. 열린 패널 전체 닫기.</summary>
        public void CloseAll()
        {
            _gachaPanel?.Close();
            _farmPlotPanel?.Close();
            _barnPanel?.Close();
            _shopPanel?.Close();
        }
    }
}
