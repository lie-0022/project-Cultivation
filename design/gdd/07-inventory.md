# 07. 인벤토리 시스템 (Inventory)

## 1. Overview

씨앗과 작물의 수량 기반 인벤토리, 크리처의 개체 기반 리스트.

## 2. Player Fantasy

내가 가진 자원을 한눈에 확인.

## 3. Detailed Rules

### 3.1 데이터 구조

- **씨앗 인벤토리**: `Dictionary<string seedId, int count>`
- **작물 인벤토리**: `Dictionary<string cropId, int count>`
- **크리처 리스트**: 사육장이 관리 (인벤토리에는 포함되지 않음)

### 3.2 인벤토리 용량

- MVP는 무제한 (수량/슬롯 제한 없음)

### 3.3 UI 표시

- 인벤토리 UI는 항상 접근 가능 (예: I키)
- 또는 가챠/밭/상점 UI 안에서 인벤토리 정보 표시

## 4. Formulas

(공식 없음, 단순 카운팅)

### 4.1 추가/제거 패턴

```csharp
// 추가
if (!seeds.ContainsKey(seedId))
    seeds[seedId] = 0
seeds[seedId] += count

// 제거
if (!seeds.ContainsKey(seedId) || seeds[seedId] < count)
    return false
seeds[seedId] -= count
if (seeds[seedId] == 0)
    seeds.Remove(seedId)
return true
```

## 5. Edge Cases

- **존재하지 않는 ID 제거 시도**: false 리턴
- **수량 부족 시 제거 시도**: false 리턴
- **수량 0이 된 항목**: Dictionary에서 제거 (UI 깔끔하게)

## 6. Dependencies

- 다른 시스템에서 호출되는 저수준 매니저
- 호출처: `GachaManager`, `FarmManager`, `CreatureManager`, `EconomyManager`

## 7. Tuning Knobs

(없음 - MVP는 무제한)

## 8. Acceptance Criteria

- [ ] AddSeed 호출 시 정확히 count만큼 증가
- [ ] RemoveSeed 호출 시 정확히 count만큼 감소
- [ ] 수량 부족 시 RemoveSeed가 false 리턴, 인벤토리 변경 없음
- [ ] 수량 0 도달 시 Dictionary에서 자동 제거
- [ ] 작물 인벤토리도 동일 동작
- [ ] 인벤토리 변경 시 이벤트 발생 (OnSeedChanged, OnCropChanged) — UI 갱신용
