# 시스템 의존 관계 (Systems Map)

## 1. 매니저 의존 그래프

```
                    ┌─────────────┐
                    │ GameManager │  (전체 진입점)
                    └──────┬──────┘
                           │
        ┌──────────────────┼──────────────────────────────┐
        ▼                  ▼                              ▼
┌──────────────┐   ┌───────────────┐            ┌──────────────────┐
│ Player &     │   │ EconomyMgr    │            │ InventoryMgr     │
│ Interaction  │   │ (gold, sell)  │            │ (seeds, crops)   │
└──────┬───────┘   └───────┬───────┘            └────────┬─────────┘
       │                   │                              │
       │ E키 입력 시 OnInteract()                          │
       │                   │                              │
       ▼                   │                              │
┌──────────────┐   ┌───────▼────────┐   ┌────────────────▼──────┐
│ GachaMgr     │◄──│                │   │                       │
│ (1회 뽑기)   │   │   사용         │   │                       │
└──────┬───────┘   │                │   │                       │
       │           └────────────────┘   │                       │
       │ AddSeed                                                │
       └────────────────────────────────────►                   │
                                                                │
┌──────────────┐                                                │
│ FarmMgr      │  PlantSeed/Harvest  ──────────────────────────►│
│ (밭, 타이머) │                                                │
└──────┬───────┘                                                │
       │ Harvest → 작물 인벤토리에 추가                         │
       └────────────────────────────────────────────────────────│
                                                                │
┌──────────────┐                                                │
│ CreatureMgr  │  RemoveCrop ──────────────────────────────────►│
│ (변환)       │                                                │
└──────┬───────┘                                                │
       │ AddCreature                                            │
       ▼                                                        │
┌──────────────┐                                                │
│ BarnMgr      │  (크리처 슬롯 관리)                            │
│ (사육장)     │                                                │
└──────┬───────┘                                                │
       │ 두 마리 선택                                           │
       ▼                                                        │
┌──────────────┐                                                │
│ BreedingMgr  │  결과 크리처 → BarnMgr                         │
│ (교배 태스크)│                                                │
└──────────────┘                                                │
                                                                │
┌──────────────┐                                                │
│ EconomyMgr   │  SellCrop / SellCreature 호출                  │
│ (판매 처리)  │  ◄─── 인벤토리/사육장에서 차감 ◄───────────────┘
└──────────────┘
```

---

## 2. 데이터 흐름 (자원 변환)

```
[골드] ─────가챠──────► [씨앗]
                          │
                          │ 심기
                          ▼
                       [밭 플롯]
                          │
                          │ 시간 경과 + 수확
                          ▼
                       [작물]
                          │
                          ├─── 직접 판매 ──► [골드]
                          │
                          │ 크리처화
                          ▼
                       [크리처 (사육장)]
                          │
                          ├─── 판매 ──────► [골드]
                          │
                          │ 교배 (2마리)
                          ▼
                       [새로운 크리처]
                          │
                          └─── 컬렉션 / 판매 / 재교배
```

---

## 3. 이벤트 흐름 (UI 갱신용)

C# event 기반:

| 이벤트 | 발행자 | 구독자 | 트리거 |
|---|---|---|---|
| `OnGoldChanged(int newGold)` | EconomyMgr | UI (골드 표시), 가챠 UI (버튼 활성화) | 골드 변동 시 |
| `OnSeedChanged(string seedId, int newCount)` | InventoryMgr | UI (씨앗 인벤토리) | 씨앗 추가/제거 시 |
| `OnCropChanged(string cropId, int newCount)` | InventoryMgr | UI (작물 인벤토리) | 작물 추가/제거 시 |
| `OnPlotStateChanged(int plotIndex, PlotState newState)` | FarmMgr | UI (밭 표시) | 플롯 상태 변경 시 |
| `OnFarmExpanded(int newCount)` | FarmMgr | UI (밭 표시) | 밭 칸 추가 시 |
| `OnCreatureAdded(CreatureInstance creature)` | BarnMgr | UI (사육장) | 크리처 추가 시 |
| `OnCreatureRemoved(string instanceId)` | BarnMgr | UI (사육장) | 크리처 제거 시 |
| `OnBarnExpanded(int newCount)` | BarnMgr | UI (사육장) | 사육장 칸 추가 시 |
| `OnBreedingStarted(BreedingTask task)` | BreedingMgr | UI (교배 진행 표시) | 교배 시작 시 |
| `OnBreedingCompleted(BreedingTask task, CreatureInstance result)` | BreedingMgr | UI, BarnMgr | 교배 완료 시 |
| `OnBreedingCancelled(BreedingTask task)` | BreedingMgr | UI | 교배 취소 시 |

---

## 4. 의존성 규칙

### 4.1 단방향 의존

- `GameManager`가 모든 매니저를 알고 있음 (전체 진입점)
- 시스템 매니저들은 `GameManager`를 통해 다른 매니저에 접근
- 매니저 → 데이터 (단방향, 데이터는 매니저를 모름)

### 4.2 금지 사항

- 매니저 간 직접 정적 참조 금지 (싱글톤 X)
- 데이터 클래스가 매니저 호출 금지
- ScriptableObject가 런타임 상태를 변경 금지 (읽기 전용)
