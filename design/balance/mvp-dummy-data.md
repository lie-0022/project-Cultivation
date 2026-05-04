# MVP 더미 데이터 (Dummy Data)

MVP 테스트용 데이터 정의. Unity Editor에서 ScriptableObject 에셋으로 생성할 항목들.

---

## 1. 씨앗 (SeedData)

| seedId | seedName | growthTime | resultCropId | rarity |
|---|---|---|---|---|
| seed_carrot | 당근 씨앗 | 10초 | crop_carrot | Common |
| seed_cabbage | 양배추 씨앗 | 20초 | crop_cabbage | Common |
| seed_tomato | 토마토 씨앗 | 30초 | crop_tomato | Rare |

---

## 2. 작물 (CropData)

| cropId | cropName | creatureId | sellPrice |
|---|---|---|---|
| crop_carrot | 당근 | creature_carrot_horse | 5 |
| crop_cabbage | 양배추 | creature_cabbage_cow | 8 |
| crop_tomato | 토마토 | creature_tomato_chicken | 15 |

---

## 3. 크리처 (CreatureData)

| creatureId | creatureName | baseSellPrice | category |
|---|---|---|---|
| creature_carrot_horse | 당근말 | 30 | Plant |
| creature_cabbage_cow | 양배추소 | 50 | Plant |
| creature_tomato_chicken | 토마토닭 | 100 | Plant |
| creature_salad_horse | 샐러드말 | 200 | Plant |
| creature_ketchup_chicken | 케첩닭 | 400 | Plant |

---

## 4. 교배 레시피 (BreedingRecipe)

| parentA_Id | parentB_Id | resultCreatureId | breedingTime |
|---|---|---|---|
| creature_carrot_horse | creature_cabbage_cow | creature_salad_horse | 60초 |
| creature_tomato_chicken | creature_tomato_chicken | creature_ketchup_chicken | 90초 |

> 레시피 매칭은 순서 무관 (A+B == B+A).

---

## 5. 가챠 풀 (GachaConfig)

| seed | weight |
|---|---|
| seed_carrot | 60 |
| seed_cabbage | 30 |
| seed_tomato | 10 |

- 1회 뽑기 비용: **50 골드**
- 분포: Common 90%, Rare 10%

---

## 6. 확장 비용 (ExpansionConfig)

| 파라미터 | 값 |
|---|---|
| farmBaseCost | 100 |
| barnBaseCost | 200 |
| costMultiplier | 1.5 |

확장 비용 예시 (Mathf.RoundToInt):

| 현재 칸 수 | 밭 다음 비용 | 사육장 다음 비용 |
|---|---|---|
| 2 | 283 | 566 |
| 3 | 520 | 1039 |
| 4 | 800 | 1600 |
| 5 | 1118 | 2236 |
| 10 | 3162 | 6325 |

---

## 7. 초기 게임 상태

| 항목 | 값 |
|---|---|
| 시작 골드 | 200 (가챠 4회 분량) |
| 시작 밭 칸 | 2 |
| 시작 사육장 칸 | 2 |
| 씨앗 인벤토리 | 비어있음 |
| 작물 인벤토리 | 비어있음 |
| 사육장 | 비어있음 |

---

## 8. 1회전 시뮬레이션 예시

플레이어가 게임 시작부터 첫 교배까지 도달하는 흐름:

1. **시작**: 골드 200
2. **가챠 4회**: 골드 0, 씨앗 4개 획득 (예상: 당근 2-3개, 양배추 1-2개, 토마토 0-1개)
3. **밭 2칸에 심기**: 약 10-30초 대기
4. **수확 → 크리처화**: 사육장에 크리처 2마리
5. **사육장 가득 참** → 사육장 1칸 추가 필요 (566 골드 → 골드 부족)
6. **크리처 1마리 판매**: 골드 30~100 획득
7. **추가 가챠 또는 추가 작물 재배** → 사이클 반복
8. 골드 충분히 모이면 → 사육장 확장 → 교배 시도

이 시뮬레이션이 막히지 않고 진행되면 MVP 밸런스 통과.
