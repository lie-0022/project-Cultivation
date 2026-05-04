# ADR-0003: 매니저 POCO + GameManager 단일 진입점

| 항목 | 값 |
|---|---|
| 상태 | Accepted |
| 결정일 | 2026-05-04 |
| 결정자 | PM (1인 개발자) |
| 관련 문서 | `design/decisions.md` #11, `design/systems-map.md`, `design/scene/scene-data.json` |

## 컨텍스트

게임 시스템 매니저(GachaManager, FarmManager, BarnManager, BreedingManager, CreatureManager, InventoryManager, EconomyManager, TradeManager) 8종의 인스턴스화 및 접근 패턴을 정의해야 함.

Unity 관행상 두 가지 주요 옵션 존재:

1. **MonoBehaviour 매니저** — 각 매니저를 GameObject + 컴포넌트로 구성. Scene에 배치되거나 GameManager 자식으로.
2. **POCO 매니저 + 단일 MonoBehaviour 진입점** — 매니저는 일반 C# 클래스, GameManager(MonoBehaviour)가 인스턴스화 보유.

`design/scene/scene-data.json`(2026-05-04 작성된 씬 가이드)은 옵션 1을 가정함 (GameManager 자식으로 7개 매니저 GameObject). 그러나 `design/decisions.md` #11(2026-05-04 합의)은 "GameManager 단일 진입점 + 매니저 참조"로 옵션 2를 명시.

## 결정

**옵션 2 채택**: 매니저는 모두 POCO. GameManager(MonoBehaviour) 단일 GameObject가 Awake에서 매니저들을 `new`로 생성하고 보유.

### 구체적 구조

```csharp
public class GameManager : MonoBehaviour
{
    public InventoryManager Inventory { get; private set; }
    public EconomyManager Economy { get; private set; }
    public GachaManager Gacha { get; private set; }
    public BarnManager Barn { get; private set; }
    public CreatureManager Creature { get; private set; }
    public FarmManager Farm { get; private set; }
    public BreedingManager Breeding { get; private set; }
    public TradeManager Trade { get; private set; }

    private void Awake()
    {
        Inventory = new InventoryManager();
        Economy = new EconomyManager(_startingGold);
        Gacha = new GachaManager(_gachaConfig, Economy, Inventory);
        // ...
    }

    private void Update()
    {
        Farm?.Tick(Time.deltaTime);
        Breeding?.Tick(Time.deltaTime);
    }
}
```

### 의존성 흐름

- 의존성은 **생성자 주입** (constructor injection)
- 한 방향 의존만 허용: `GameManager → Manager → Data SO`
- 매니저 간 의존성도 생성자로만 (예: `BreedingManager(BarnManager, Registry, ...)`)
- **금지**: 매니저 간 정적 참조, `static GachaManager.Instance` 등

### Tick 패턴

- `FarmManager`, `BreedingManager`만 시간 누적 필요 → `Tick(float deltaTime)` 메서드 노출
- `GameManager.Update()`에서 매 프레임 위 매니저들에 `Tick(Time.deltaTime)` 호출
- 다른 매니저는 Update 불필요

## 대안 고려

### A. MonoBehaviour 매니저 + 자식 GameObject
- **장점**:
  - Inspector에서 SerializeField 직접 수정 가능
  - Update/OnEnable/OnDisable 라이프사이클 자동
  - scene-data.json 가이드와 일치
- **단점**:
  - 의존성 주입을 MonoBehaviour reference + GetComponent로 해야 함 (테스트 어려움)
  - 매니저 인스턴스화 순서 제어 어려움 (Script Execution Order 사용 필요)
  - 매니저 자체가 Scene에 의존 → 단위 테스트 힘듦
  - 싱글톤 패턴으로 빠지기 쉬움

### B. POCO 매니저 + GameManager 단일 진입점 (선택됨)
- **장점**:
  - 생성자 주입으로 의존성 명확
  - 단위 테스트 즉시 가능 (NUnit `[Test]`로 매니저 직접 인스턴스화)
  - 인스턴스화 순서 코드로 제어
  - 매니저는 Unity 의존성 최소 (Mathf 등만 사용)
- **단점**:
  - Inspector에서 매니저 내부 상태 직접 수정 불가 (그러나 GameManager 통해 디버그 인터페이스 노출 가능)
  - `Update`/`OnEnable` 자동 호출 안 됨 (GameManager가 명시적으로 Tick 호출)

### C. 정적 싱글톤
- **금지**: decisions.md #11 명시 ("싱글톤 금지")

## 결과

### 적용 사항 (2026-05-04 시점)
- 8개 매니저 모두 POCO 패턴으로 구현 완료
- GameManager가 단일 GameObject로 Scene에 배치
- Phase 1~3 단위 테스트 32개 모두 NUnit 직접 인스턴스화로 작동 (확인됨)

### scene-data.json 충돌
- scene-data.json의 매니저 자식 GameObject 항목은 **무시**
- 추후 scene-data.json 갱신 예정 (해당 항목 제거 또는 metadata로 표시)

### 회귀 위험
- 없음 (첫 결정, 모든 매니저가 처음부터 이 패턴 따름)

### 미래 검토 사항
- 매니저 수가 더 늘어나거나 인스턴스화 그래프가 복잡해지면 가벼운 DI 컨테이너(VContainer 등) 도입 검토
- 현재(8개)는 GameManager.Awake에서 직접 `new`로 충분

## 변경 이력

- 2026-05-04: 결정, 적용 (Phase 1)
- 2026-05-04: TradeManager 추가하면서 패턴 재확인 — 변경 없음
