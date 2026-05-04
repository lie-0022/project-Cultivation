# 03. 작물→크리처 변환 시스템 (Creature Conversion)

## 1. Overview

수확한 작물을 크리처로 변환해 사육장에 추가하는 시스템. 게임의 핵심 변환 단계.

## 2. Player Fantasy

평범한 작물에 팔다리가 달려 살아 움직이게 되는 신비로운 변환의 순간.

## 3. Detailed Rules

### 3.1 변환 위치

- 사육장 시설물에서 처리 (또는 별도 변환 NPC) — 구현 단계에서 결정
- 사육장 UI 내 "크리처화" 탭에서 작물 선택 → 변환 버튼

### 3.2 흐름

1. 변환 UI 열기
2. 작물 인벤토리 표시
3. 작물 선택 → "크리처화" 버튼 클릭
4. 작물 인벤토리에서 1개 차감
5. `CropData.creatureId`에 해당하는 `CreatureInstance` 신규 생성 (GUID 부여)
6. 사육장에 빈 슬롯 있으면 추가, 없으면 차단

### 3.3 비목표 (MVP 제외)

- 변환 시 형질 유전 (색/크기 변이)
- 변환 비용 (MVP는 무료)
- 변환 시간 (MVP는 즉시)

## 4. Formulas

### 4.1 CreatureInstance 생성

```csharp
new CreatureInstance {
    instanceId = System.Guid.NewGuid().ToString(),
    creatureId = cropData.CreatureId,
    createdAt = System.DateTime.Now,
    isBusy = false
}
```

## 5. Edge Cases

- **사육장 빈 슬롯 없음**: 변환 차단, "사육장이 가득 찼음" 메시지
- **작물 인벤토리 0개**: 변환 차단
- **CropData.creatureId가 비어있음 또는 매칭되는 CreatureData 없음**: 에러 로그 + 변환 차단

## 6. Dependencies

- `InventoryManager` — 작물 차감
- `BarnManager` — 사육장 슬롯 확인 + 크리처 추가
- `CropData` — 변환 결과 크리처 ID
- `CreatureData` — 생성될 크리처 정의

## 7. Tuning Knobs

| 파라미터 | 기본값 | 설명 |
|---|---|---|
| 변환 비용 | 0 | MVP는 무료, 추후 추가 가능 |
| 변환 시간 | 0초 | MVP는 즉시, 추후 추가 가능 |

## 8. Acceptance Criteria

- [ ] 변환 UI에서 작물 인벤토리가 정상 표시된다
- [ ] 작물 선택 후 변환 버튼 클릭 시 작물이 1개 차감된다
- [ ] `CropData.creatureId`에 해당하는 크리처가 생성되어 사육장에 추가된다
- [ ] 생성된 크리처는 고유 `instanceId` (GUID)를 갖는다
- [ ] 사육장이 가득 찼을 때 변환이 차단된다
