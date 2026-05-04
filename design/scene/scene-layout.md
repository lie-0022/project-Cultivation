# 씬 레이아웃 가이드 (Scene Layout)

## 1. 개요

MVP 1차 씬 (`MainScene`)의 오브젝트 배치 가이드. Claude Code가 Unity MCP로 씬을 구성할 때 참조하는 문서.

이 문서는 **사람이 읽기 위한 가이드**이며, 정확한 좌표/회전/스케일은 `scene-data.json`을 참조한다. 시각적 배치는 `scene-diagram.md`(ASCII 맵) 참조.

---

## 2. 좌표 시스템

- **단위**: 1 Unity Unit = 1 미터
- **원점**: 씬 중앙 (0, 0, 0)
- **Y축**: 높이 (위쪽이 +Y)
- **XZ 평면**: 지면
- **방향**:
  - +X = 동쪽 (오른쪽)
  - -X = 서쪽 (왼쪽)
  - +Z = 북쪽 (앞쪽)
  - -Z = 남쪽 (뒤쪽)

---

## 3. 씬 규모

- **전체 씬 크기**: 30 x 30 미터 (XZ 평면)
- **바닥 (Ground)**: 30 x 30 평면, 중심 (0, 0, 0)
- 플레이어 시점에서 모든 시설이 한 화면에 들어오도록 작은 규모 유지

---

## 4. 영역 구분

씬은 4개 논리 영역으로 구분 (실제 분리된 씬 아님, 시각적/논리적 구분만):

| 영역 | 위치 (중심) | 크기 | 용도 | 바닥 색 (구분용) |
|---|---|---|---|---|
| **FarmArea** | (-8, 0, 0) | 10x10 | 밭 플롯 4개 배치 | 갈색 #8B6F47 |
| **BarnArea** | (8, 0, 0) | 10x10 | 사육장 슬롯 4개 배치 | 회색 #A0A0A0 |
| **VillageArea** | (0, 0, 10) | 14x6 | 가챠/상점 NPC 시설물 배치 | 베이지 #D4B896 |
| **SpawnArea** | (0, 0, -8) | 4x4 | 플레이어 스폰 위치 | (없음, 플레이어만) |

영역 바닥은 작은 Cube (Y=0.01, 매우 얇게)로 깔아 시각적 구분.

---

## 5. 배치 의도

### 5.1 동선 설계

플레이어 동선:
```
[스폰 (남쪽)]
    ↓ 북쪽으로 이동
[중앙] ──→ [좌: 밭] [우: 사육장] [북: 마을]
```

- 스폰 위치에서 어느 시설이든 5~15미터 이내 도달 가능
- 밭에서 사육장까지 직선거리 약 16m (걸어서 3~4초)

### 5.2 시설물 배치 원칙

- **밭과 사육장은 좌우 대칭** (시각적 균형)
- **마을(가챠/상점)은 북쪽**에 모여 있음 (NPC 구역)
- **플레이어 스폰은 남쪽** → 자연스럽게 북쪽으로 이동하며 게임 진입

---

## 6. 오브젝트 종류 및 시각 표현

모든 오브젝트는 **Unity Primitive**로만 표현 (MVP 임시).

### 6.1 플레이어
- **Primitive**: Capsule
- **색상**: 파란색 (#4A90E2)
- **크기**: 기본 (높이 2m)
- **위치**: SpawnArea 중심 (0, 1, -8)

### 6.2 밭 플롯 (FarmPlot)
- **Primitive**: Cube
- **색상**: 갈색 (#6B4423, 진한 흙색)
- **크기**: 2 x 0.1 x 2 (얇은 판)
- **개수**: 4개 (2x2 그리드)
- **간격**: 칸 사이 1m 여백
- **상태별 색상 변화** (구현 시):
  - Empty: #6B4423 (기본 갈색)
  - Growing: #8B6F47 (밝은 갈색)
  - Ready: #FFD700 (금색, 수확 가능 시각화)

### 6.3 사육장 슬롯 (BarnSlot)
- **Primitive**: Cube
- **색상**: 연회색 (#C0C0C0, 우리/펜스 느낌)
- **크기**: 2 x 0.5 x 2 (낮은 박스)
- **개수**: 4개 (2x2 그리드)
- **간격**: 칸 사이 1m 여백

### 6.4 크리처 (런타임 생성)
- **Primitive**: Cylinder (몸통) + Sphere (머리) — 단순화하면 Capsule 1개
- **색상**: 크리처 종별로 다름:
  - 당근말: 주황 (#FF8C42)
  - 양배추소: 연두 (#9ACD32)
  - 토마토닭: 빨강 (#E63946)
  - 샐러드말: 진녹색 (#2E8B57)
  - 케첩닭: 진빨강 (#8B0000)
- **크기**: 0.8 x 1 x 0.8
- **위치**: 사육장 슬롯 위 (Y = 1)
- **이동**: 슬롯 내에서 약간 흔들리기/움직임 (선택적, MVP는 정적)

### 6.5 가챠 시설물 (GachaBuilding)
- **Primitive**: Cube
- **색상**: 보라 (#9B59B6, 가챠 느낌)
- **크기**: 2 x 3 x 2 (건물 형태)
- **위치**: VillageArea 좌측 (-4, 1.5, 10)
- **interactionRange**: 2.5m

### 6.6 상점 시설물 (ShopBuilding)
- **Primitive**: Cube
- **색상**: 주황 (#E67E22, 상점 간판 느낌)
- **크기**: 2 x 3 x 2
- **위치**: VillageArea 우측 (4, 1.5, 10)
- **interactionRange**: 2.5m

### 6.7 카메라
- **종류**: 3인칭 카메라 (플레이어 따라다님)
- **초기 위치**: 플레이어 뒤쪽 위 (0, 5, -12)
- **회전**: 약간 아래로 (X = 30도)
- **추후**: Cinemachine 사용 권장 (MVP는 단순 자식 오브젝트로 충분)

### 6.8 조명
- **Directional Light**: 1개 (씬 기본 제공)
- **회전**: (50, -30, 0) — 적당한 그림자
- **색**: 흰색 (#FFFFFF)
- **강도**: 1.0

---

## 7. 영역별 칸 배치 좌표

### 7.1 밭 4칸 (2x2 그리드, FarmArea 내부)

칸 크기 2x2 + 간격 1m → 칸 중심 간격 3m

```
FarmArea 중심: (-8, 0, 0)

farm_plot_01: (-9.5, 0.05, -1.5)   ← 좌하
farm_plot_02: (-6.5, 0.05, -1.5)   ← 우하
farm_plot_03: (-9.5, 0.05, 1.5)    ← 좌상
farm_plot_04: (-6.5, 0.05, 1.5)    ← 우상
```

### 7.2 사육장 4칸 (2x2 그리드, BarnArea 내부)

```
BarnArea 중심: (8, 0, 0)

barn_slot_01: (6.5, 0.25, -1.5)   ← 좌하
barn_slot_02: (9.5, 0.25, -1.5)   ← 우하
barn_slot_03: (6.5, 0.25, 1.5)    ← 좌상
barn_slot_04: (9.5, 0.25, 1.5)    ← 우상
```

---

## 8. 계층 구조 (Hierarchy)

권장 씬 계층:

```
MainScene
├── --- ENVIRONMENT ---
├── Ground (Plane)
├── DirectionalLight
├── --- AREAS ---
├── FarmArea (Empty GameObject)
│   ├── FarmAreaFloor (얇은 Cube, 시각 구분용)
│   ├── farm_plot_01
│   ├── farm_plot_02
│   ├── farm_plot_03
│   └── farm_plot_04
├── BarnArea (Empty GameObject)
│   ├── BarnAreaFloor
│   ├── barn_slot_01
│   ├── barn_slot_02
│   ├── barn_slot_03
│   └── barn_slot_04
├── VillageArea (Empty GameObject)
│   ├── VillageAreaFloor
│   ├── GachaBuilding
│   └── ShopBuilding
├── --- PLAYER ---
├── Player (Capsule, CharacterController, PlayerController, InteractionController)
│   └── MainCamera (자식, PlayerCameraController)
└── --- MANAGERS ---
    └── GameManager (Empty GameObject + Cultivation.Systems.GameManager)
```

> **ADR-0003 적용**: 매니저(Inventory/Economy/Gacha/Barn/Creature/Farm/Breeding/Trade)는 모두 POCO(일반 C# 클래스)이며, GameManager가 Awake에서 `new`로 생성·보유한다. 별도 매니저 GameObject 자식 계층은 없다.

---

## 9. 컴포넌트 부착 가이드

### Player
- `CharacterController`
- `Cultivation.Systems.PlayerController` (WASD 이동, 카메라 yaw 상대)
- `Cultivation.Systems.InteractionController` (E키 + 거리 체크 + 프롬프트 갱신)

### Player/MainCamera (자식)
- `Camera`, `AudioListener`, `UniversalAdditionalCameraData`
- `Cultivation.Systems.PlayerCameraController` (마우스 yaw/pitch)

### 시설물 (밭 플롯, 사육장 슬롯, 가챠 빌딩, 상점 빌딩)
- `BoxCollider` (Trigger)
- `Cultivation.Systems.IInteractable` 구현 컴포넌트:
  - `FarmPlotInteractable` (plotIndex, range 2.0)
  - `BarnSlotInteractable` (slotIndex, range 2.0)
  - `GachaInteractable` (range 2.5)
  - `ShopInteractable` (range 2.5)

### 영역 바닥 (XxxAreaFloor)
- 시각 구분용, Collider 불필요

---

## 10. 확장 시 처리

향후 골드로 밭/사육장 확장 시:
- 코드에서 동적 생성 (Instantiate)
- 위치는 기존 그리드 패턴 연장 (3m 간격 유지)
- 5칸째부터는 한 줄 더 (2x3, 3x2 등)
- **MVP는 4칸까지만 미리 배치, 확장 부분은 추후**

---

## 11. 작업 순서 권장 (Claude Code용)

1. 빈 씬 생성 (`MainScene`)
2. Ground (Plane, 30x30) 생성
3. DirectionalLight 배치
4. 영역 Empty GameObject 4개 생성 (FarmArea, BarnArea, VillageArea, SpawnArea)
5. 영역 바닥 Cube 4개 생성 (XxxAreaFloor, Y=0.01, 색상 구분)
6. 밭 플롯 4개 생성
7. 사육장 슬롯 4개 생성
8. 가챠/상점 빌딩 생성
9. Player 생성 (Capsule + 카메라 자식)
10. GameManager + 모든 매니저 자식으로 생성
11. 컴포넌트 부착

각 단계마다 Editor에서 시각 확인 후 다음 단계로.
