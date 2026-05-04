# 01. 가챠 시스템 (Gacha)

## 1. Overview

골드를 소비해 씨앗을 무작위로 획득하는 시스템. MVP는 1회 뽑기만 지원.

## 2. Player Fantasy

"이번엔 어떤 씨앗이 나올까?" — 등급별 확률에 따른 획득의 두근거림.

## 3. Detailed Rules

- 가챠 시설물 접근 → E키 → 가챠 UI 열림
- "1회 뽑기" 버튼 클릭 → 골드 차감 → 가중치 기반 무작위 추첨 → 씨앗 인벤토리에 추가
- 골드 부족 시 버튼 비활성화 (또는 클릭 시 부족 메시지)
- 결과는 즉시 표시 (애니메이션 없음, MVP)

### 3.1 비목표 (MVP 제외)

- 10연차 가챠
- 천장 시스템 (보장 시스템)
- 등급별 결과 연출
- 시즌 한정 풀

## 4. Formulas

### 4.1 가중치 기반 추첨

```
totalWeight = sum(entries[i].weight)
roll = UnityEngine.Random.Range(0, totalWeight)
cumulative = 0
for entry in entries:
    cumulative += entry.weight
    if roll < cumulative:
        return entry.seed
```

### 4.2 비용

- 1회 뽑기 비용: `GachaConfig.pullCost` (기본 50 골드)

## 5. Edge Cases

- **골드 부족**: 뽑기 시도 차단, UI 버튼 비활성화
- **GachaConfig.entries가 비어있음**: 에러 로그 + 뽑기 차단
- **가중치 합이 0**: 에러 로그 + 뽑기 차단

## 6. Dependencies

- `EconomyManager` — 골드 차감
- `InventoryManager` — 씨앗 추가
- `SeedData` — 결과 씨앗 데이터
- `GachaConfig` — 뽑기 풀 정의

## 7. Tuning Knobs

| 파라미터 | 기본값 | 설명 |
|---|---|---|
| `pullCost` | 50 | 1회 뽑기 골드 비용 |
| `entries[].weight` | (씨앗별) | 등장 가중치, 합산 후 비율로 동작 |

MVP 더미 데이터의 가중치 분포는 `../balance/mvp-dummy-data.md` 참조.

## 8. Acceptance Criteria

- [ ] 가챠 시설물에 접근하여 E키 입력 시 가챠 UI가 열린다
- [ ] 골드가 충분할 때 1회 뽑기 버튼이 활성화된다
- [ ] 1회 뽑기 시 골드가 정확히 `pullCost`만큼 차감된다
- [ ] 뽑기 결과 씨앗이 인벤토리에 1개 추가된다
- [ ] 100회 시뮬레이션 시 등장 분포가 가중치 비율과 ±10% 이내로 일치한다
- [ ] 골드 부족 시 뽑기가 차단된다
