# 06. 경제 시스템 (Economy)

## 1. Overview

골드를 중심으로 한 자원 흐름. 가챠 비용, 판매 수익, 확장 비용을 모두 관리.

## 2. Player Fantasy

판매 → 재투자의 점진적 성장 사이클.

## 3. Detailed Rules

### 3.1 골드 변동 케이스

| 케이스 | 방향 | 출처 |
|---|---|---|
| 가챠 1회 뽑기 | -50 | `GachaConfig.pullCost` |
| 작물 판매 | + | `CropData.sellPrice` |
| 크리처 판매 | + | `CreatureData.baseSellPrice` |
| 밭 확장 | - | 공식 계산값 |
| 사육장 확장 | - | 공식 계산값 |

### 3.2 판매 흐름

- 상점 시설물 접근 + E키 → 상점 UI 열림
- UI에서 "작물 판매" / "크리처 판매" 탭 분리
- 작물: 인벤토리에서 1개씩 선택 → 판매 (sellPrice 만큼 골드 획득)
- 크리처: 사육장에서 1마리씩 선택 → 판매 (baseSellPrice 만큼 골드 획득)
- Busy 상태 크리처는 판매 불가

### 3.3 판매 수량

- MVP는 **1개/1마리씩만 판매**
- 다중 선택 판매는 추후 추가

### 3.4 시작 골드

- 게임 시작 시 200 골드 (가챠 4회 분량)

## 4. Formulas

### 4.1 골드 변동

```csharp
// 획득
gold += amount

// 차감 (TrySpend 패턴)
if (gold >= amount) {
    gold -= amount
    return true
} else {
    return false
}
```

### 4.2 확장 비용 (밭/사육장 공통 패턴)

```
nextCost = Mathf.RoundToInt(baseCost * Mathf.Pow(currentCount, costMultiplier))
```

상세는 02 GDD (밭), 04 GDD (사육장) 참조.

## 5. Edge Cases

- **음수 골드 방지**: TrySpend 패턴으로 처리, 부족 시 false 리턴
- **판매 시 인벤토리/사육장에서 정확히 1개 차감**
- **Busy 크리처 판매 시도**: 차단

## 6. Dependencies

- `InventoryManager` — 작물 차감
- `BarnManager` — 크리처 차감
- `CropData.sellPrice`, `CreatureData.baseSellPrice` — 판매가
- `FarmManager`, `BarnManager` — 확장 비용 차감 호출

## 7. Tuning Knobs

| 파라미터 | 기본값 | 설명 |
|---|---|---|
| 시작 골드 | 200 | 게임 시작 시 보유 골드 |
| 작물별 sellPrice | 5~15 | MVP 더미 참조 |
| 크리처별 baseSellPrice | 30~400 | MVP 더미 참조 |

## 8. Acceptance Criteria

- [ ] 시작 시 골드 200으로 시작한다
- [ ] 가챠/확장 시 골드가 정확히 차감된다
- [ ] 골드 부족 시 차감 동작이 차단된다 (TrySpend가 false 리턴)
- [ ] 작물 판매 시 sellPrice 만큼 골드 획득, 작물 1개 차감
- [ ] 크리처 판매 시 baseSellPrice 만큼 골드 획득, 크리처 사육장에서 제거
- [ ] Busy 크리처 판매 시도 차단
- [ ] 골드 변경 시 OnGoldChanged 이벤트 발생 (UI 갱신용)
