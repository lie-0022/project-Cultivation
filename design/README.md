# 재배 게임 MVP - 기획 문서

## 문서 구조

```
design/
├── README.md                       # 이 문서 (인덱스)
├── decisions.md                    # 모든 결정 사항 요약 (진실 공급원)
├── systems-map.md                  # 시스템 의존 관계 그래프
├── gdd/                            # 시스템별 기획 문서 (8섹션 양식)
│   ├── 00-overview.md              # 게임 컨셉, 코어 루프
│   ├── 01-gacha.md                 # 가챠 시스템
│   ├── 02-farm.md                  # 밭/재배
│   ├── 03-creature-conversion.md   # 작물→크리처 변환
│   ├── 04-barn.md                  # 사육장
│   ├── 05-breeding.md              # 교배
│   ├── 06-economy.md               # 골드/판매/확장 비용
│   ├── 07-inventory.md             # 인벤토리
│   └── 08-player-interaction.md    # 플레이어 이동/상호작용
└── balance/
    └── mvp-dummy-data.md           # MVP 더미 데이터 (씨앗/작물/크리처/레시피/가챠)
```

## 읽는 순서 (Claude Code용)

1. **`decisions.md`** — 모든 결정 사항 한눈에
2. **`gdd/00-overview.md`** — 게임 전체 컨셉 파악
3. **`systems-map.md`** — 시스템 간 의존 관계 이해
4. **시스템별 GDD (01~08)** — 구현할 시스템 GDD 정독
5. **`balance/mvp-dummy-data.md`** — ScriptableObject 에셋 생성 시 참조

## 8섹션 양식

각 GDD는 다음 8섹션을 따름:

1. **Overview** — 시스템 한 줄 요약
2. **Player Fantasy** — 플레이어가 느낄 감정/경험
3. **Detailed Rules** — 동작 규칙 상세
4. **Formulas** — 수식, 의사 코드
5. **Edge Cases** — 예외 상황 처리
6. **Dependencies** — 의존하는 다른 시스템/매니저
7. **Tuning Knobs** — 튜닝 가능한 파라미터
8. **Acceptance Criteria** — 완료 기준 체크리스트

## 개발 진행 권장 순서

### Phase 1: 기반 (1-2일차)
- 폴더/네임스페이스 구조 세팅
- ScriptableObject 클래스 정의 (`SeedData`, `CropData`, `CreatureData`, `BreedingRecipe`, `GachaConfig`, `ExpansionConfig`)
- MVP 더미 데이터 에셋 생성 (`balance/mvp-dummy-data.md` 참조)
- `GameManager`, `InventoryManager`, `EconomyManager` 구현

### Phase 2: 시스템 매니저 (3-5일차)
- `GachaManager` (콘솔 테스트)
- `FarmManager` + `FarmPlot` (실시간 타이머)
- `CreatureManager` (변환)
- `BarnManager` (확장 포함)

### Phase 3: 교배 (6일차)
- `BreedingManager` + `BreedingTask`
- 교배 취소 로직
- 같은 종 교배 + 레시피 우선 처리

### Phase 4: 3D 공간/상호작용 (7-9일차)
- 데모 코드 제거
- 플레이어 컨트롤러 (WASD + 마우스 카메라)
- 시설물 IInteractable 인터페이스
- E키 상호작용 + 거리 체크
- 시설물 배치 (가챠/밭 플롯/사육장/상점)

### Phase 5: 임시 UI + 검증 (10일차)
- 각 시설물 UI 패널 (Button + Text 임시)
- 인벤토리/골드 UI
- 8개 GDD의 Acceptance Criteria 전체 검증

## 주요 컨벤션

- **언어**: C# (Unity)
- **네임스페이스**: `Cultivation.{Data|Runtime|Systems|UI}`
- **필드 노출**: `[SerializeField] private` + 프로퍼티
- **싱글톤 금지**: `GameManager` 통한 매니저 참조만 허용
- **이벤트**: 순수 C# event (UnityEvent 아님)
- **랜덤**: `UnityEngine.Random`
- **저장**: 없음 (메모리 상에서만 동작)

## 변경 이력

- v1.0: 초기 기획 문서 작성
