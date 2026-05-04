# 08. 플레이어 이동 및 상호작용 (Player & Interaction)

## 1. Overview

3D 공간을 자유 이동하며 시설물에 접근해 E키로 상호작용하는 시스템. 스타듀밸리 방식.

## 2. Player Fantasy

내 캐릭터가 농장을 직접 걸어 다니며 가꾸는 물리적 존재감.

## 3. Detailed Rules

### 3.1 이동

- WASD 키로 이동
- Space로 점프 (선택적, MVP 필수 아님)
- 마우스로 카메라 회전 (3인칭 또는 1인칭, MVP는 3인칭 권장)
- 데모 코드는 제거하고 본 게임 구조에 맞춰 새로 구현

### 3.2 상호작용

- 플레이어와 시설물 사이 거리 측정
- 시설물의 `interactionRange` (기본 2.0m) 이내 진입 시:
  - 화면에 "E키로 상호작용" 프롬프트 표시
  - 가장 가까운 시설물 1개만 활성화 (여러 개가 범위 내일 경우)
- E키 입력 시 → 해당 시설물의 OnInteract() 호출
- OnInteract() 결과는 시설물 종류별로 다름:
  - 가챠 시설물 → 가챠 UI 열림
  - 밭 플롯 → 상태에 따라 (Empty: 씨앗 선택 UI, Ready: 즉시 수확)
  - 사육장 → 사육장 UI 열림
  - 상점 → 상점 UI 열림

### 3.3 시설물 종류 (MVP)

| 종류 | 설명 | 위치 |
|---|---|---|
| 가챠 시설물 (Gacha NPC/건물) | 씨앗 뽑기 | 마을 구역 |
| 밭 플롯 (Farm Plot) | 씨앗 심기/수확 | 농장 구역 |
| 사육장 시설물 (Barn) | 크리처 관리/교배 | 사육장 구역 |
| 상점 (Shop NPC/건물) | 작물/크리처 판매 | 마을 구역 |

### 3.4 공간 구조

- **하나의 넓은 씬**에 모든 시설 배치
- 영역 구분은 시각적으로만 (펜스, 바닥 색 등) — 실제 분리된 씬은 아님
- MVP는 작은 영역 (대략 20x20m)에 모든 요소 배치

### 3.5 UI 모드

- 시설물 UI 열려 있을 때:
  - 플레이어 이동/카메라 회전 차단 (또는 일시정지)
  - ESC 또는 닫기 버튼으로 UI 종료 → 이동 재개

## 4. Formulas

### 4.1 상호작용 거리 체크

```csharp
foreach (interactable in allInteractables) {
    distance = Vector3.Distance(player.position, interactable.position)
    if (distance <= interactable.interactionRange) {
        candidates.Add(interactable, distance)
    }
}
nearest = candidates.OrderBy(distance).FirstOrDefault()
```

매 프레임 또는 일정 주기(0.1초)로 갱신.

## 5. Edge Cases

- **여러 시설물 동시 범위 내**: 가장 가까운 1개만 프롬프트 표시
- **상호작용 중 다른 시설물 진입**: 현재 UI 닫히기 전까지 다른 상호작용 무시
- **E키 누른 채 이동**: 한 번의 입력으로만 처리 (GetKeyDown)

## 6. Dependencies

- Unity Input System 또는 레거시 Input 매니저
- `GameManager` — UI 모드 전환 신호
- 각 시설물 클래스 — IInteractable 인터페이스 구현

## 7. Tuning Knobs

| 파라미터 | 기본값 | 설명 |
|---|---|---|
| 플레이어 이동 속도 | 5 m/s | WASD 이동 속도 |
| 점프 높이 | 1.5m | (선택적) |
| 마우스 감도 | 2.0 | 카메라 회전 감도 |
| `interactionRange` (시설물별) | 2.0m | 상호작용 가능 거리 |
| 상호작용 갱신 주기 | 0.1초 | 거리 체크 빈도 |

## 8. Acceptance Criteria

- [ ] WASD로 자유 이동 가능
- [ ] 마우스로 카메라 회전 가능
- [ ] 시설물 2m 이내 접근 시 "E키로 상호작용" 프롬프트 표시
- [ ] 여러 시설물 범위 내일 때 가장 가까운 것만 활성화
- [ ] E키 입력 시 해당 시설물의 UI가 열린다
- [ ] UI 열린 동안 플레이어 이동 차단
- [ ] ESC 또는 닫기 버튼으로 UI 종료 가능
- [ ] 모든 시설물 종류 (가챠/밭/사육장/상점)에서 상호작용 정상 동작
