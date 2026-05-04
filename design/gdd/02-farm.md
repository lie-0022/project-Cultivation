# 02. 밭/재배 시스템 (Farm)

## 1. Overview

씨앗을 밭 플롯에 심어 실시간으로 자라게 하고, 다 자라면 작물을 수확하는 시스템.

## 2. Player Fantasy

농장을 가꾸며 자원을 키워내는 농부 경험.

## 3. Detailed Rules

### 3.1 플롯 상태

`PlotState` enum:
- `Empty`: 비어있음, 씨앗 심기 가능
- `Growing`: 성장 중, 타이머 진행
- `Ready`: 수확 가능

### 3.2 흐름

1. 빈 플롯에 접근 + E키 → 씨앗 선택 UI
2. 씨앗 선택 → 인벤토리에서 차감 → 플롯 상태 `Growing`, `elapsedTime = 0`
3. 매 프레임 `elapsedTime += Time.deltaTime`
4. `elapsedTime >= seed.growthTime` → 상태 `Ready`
5. `Ready` 플롯에 접근 + E키 → 작물이 인벤토리에 추가, 플롯 상태 `Empty`로 복귀

### 3.3 시간 시스템

- 실시간 (`Time.deltaTime` 기반)
- 게임 종료 시 타이머 정지 (오프라인 진행 없음)
- 게임 종료 시 진행 중인 작물 있으면 알림 표시

### 3.4 확장

- 골드를 사용해 새로운 플롯 추가 구매
- 시작 칸 수: 2
- 상한 없음 (MVP)

## 4. Formulas

### 4.1 확장 비용

```
nextCost = Mathf.RoundToInt(farmBaseCost * Mathf.Pow(currentPlotCount, costMultiplier))
```

- `currentPlotCount`: **현재 보유한 칸 수** (직전 칸 수)
- 예시: 2칸 보유 → 다음 칸 비용 = `100 * (2^1.5)` ≈ 283 골드
- 3칸 보유 → 다음 칸 비용 = `100 * (3^1.5)` ≈ 520 골드

기본값:
- `farmBaseCost = 100`
- `costMultiplier = 1.5`

### 4.2 성장 진행도

```
progress = Mathf.Clamp01(elapsedTime / seed.growthTime)
```

UI 표시용 (0~1).

## 5. Edge Cases

- **이미 차있는 플롯에 심기 시도**: 차단, "이미 작물이 자라고 있음" 메시지
- **씨앗 인벤토리 0개**: 차단, "씨앗이 없음" 메시지
- **확장 시 골드 부족**: 구매 차단
- **게임 종료 시 Growing 플롯 존재**: 알림 표시 후 종료, 다음 게임 시 모두 초기화 (MVP는 저장 없음)

## 6. Dependencies

- `InventoryManager` — 씨앗 차감, 작물 추가
- `EconomyManager` — 확장 비용 차감
- `SeedData` — 성장 시간, 결과 작물 ID
- `CropData` — 수확 시 인벤토리에 추가될 작물
- `ExpansionConfig` — 확장 비용 공식 파라미터

## 7. Tuning Knobs

| 파라미터 | 기본값 | 설명 |
|---|---|---|
| 시작 칸 수 | 2 | 게임 시작 시 밭 플롯 개수 |
| `farmBaseCost` | 100 | 확장 비용 베이스 |
| `costMultiplier` | 1.5 | 확장 비용 지수 |
| `seed.growthTime` | 씨앗별 | 각 씨앗의 성장 시간(초) |

## 8. Acceptance Criteria

- [ ] 빈 플롯 접근 + E키 시 씨앗 선택 UI가 열린다
- [ ] 씨앗 선택 시 인벤토리에서 1개 차감된다
- [ ] 플롯이 `Growing` 상태로 전환되고 시각적으로 변화 (디버그 로그/색 변경 등)
- [ ] `growthTime` 초 경과 후 `Ready` 상태로 자동 전환된다
- [ ] `Ready` 플롯 접근 + E키 시 작물이 인벤토리에 추가되고 플롯이 `Empty`로 돌아간다
- [ ] 골드로 새 플롯 추가 가능, 비용이 공식대로 정확히 계산된다 (`Mathf.RoundToInt`)
- [ ] 골드 부족 시 확장 차단된다
- [ ] 게임 종료 시 진행 중인 작물 있으면 알림 표시된다
