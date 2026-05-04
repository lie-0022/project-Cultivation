# ADR-0002: 네임스페이스 `Cultivation.<도메인>` 도입

| 항목 | 값 |
|---|---|
| 상태 | Accepted |
| 결정일 | 2026-05-04 |
| 결정자 | PM (1인 개발자) |
| 관련 문서 | `decisions.md` #12, `.claude/docs/naming-conventions.md`, `.claude/docs/technical-preferences.md` |

## 컨텍스트

초기 컨벤션은 "네임스페이스 미사용"이었음 (`unity-csharp.md` 에이전트 description, `naming-conventions.md`).

Phase 0(데모) 단계에서 `PlayerController`라는 클래스를 네임스페이스 없이 정의 후 Unity MCP의 `manage_components add`로 부착 시도 시 다음 에러 발생:

```
Component type 'PlayerController' not found. Use a fully-qualified name if needed.
```

원인 조사 결과 `Unity.VisualScripting.DocCodeExamples` 패키지에 `VariableExamples+PlayerController` 동명 클래스가 존재하여 type lookup이 모호해짐. 매번 AQN(`PlayerController, Assembly-CSharp`) 명시해야 동작.

흔한 게임 도메인 이름(`PlayerController`, `GameManager`, `EnemyController`, `WeaponSystem` 등)은 다른 Unity 패키지나 향후 추가될 패키지의 동명 클래스와 충돌할 가능성이 항상 존재. 패키지 추가 시점마다 컴파일 에러 또는 Editor 상호작용 실패 위험.

## 결정

모든 게임 코드는 `Cultivation.<도메인>` 네임스페이스 사용. 도메인은 다음 4종(decisions.md #12):

- `Cultivation.Data` — ScriptableObject 정의
- `Cultivation.Runtime` — 런타임 데이터 클래스 (POCO)
- `Cultivation.Systems` — 매니저, 컨트롤러, 서비스
- `Cultivation.UI` — UI 컴포넌트

폴더 구조도 도메인과 일치:
- `Assets/Scripts/Data/`
- `Assets/Scripts/Runtime/`
- `Assets/Scripts/Systems/`
- `Assets/Scripts/UI/`

## 대안 고려

### A. 고유 이름 prefix (예: `CultivationPlayerController`)
- **장점**: 네임스페이스 미사용 정책 유지
- **단점**: 클래스명 길어짐, 도메인 분류 표현 못함, IDE 자동완성에서 검색 시 prefix 외워야 함

### B. 충돌 발견 시 그때그때 AQN 명시
- **장점**: 코드 변경 없음
- **단점**: Unity MCP 호출 시마다 AQN 기억 필요, Editor 작업 시 매번 트러블슈팅

### C. Cultivation.<도메인> (선택됨)
- **장점**: 표준 .NET 관행, 도메인 분류 자연스럽게 표현, 충돌 영구 해결
- **단점**: `using Cultivation.<...>;` 한 줄 추가 필요 (사소함)

## 결과

### 적용 사항
- 모든 신규/기존 게임 코드는 4개 네임스페이스 중 하나에 속함
- 폴더 = 네임스페이스 1:1 매핑
- Unity MCP `manage_components add` 호출 시 `Cultivation.Systems.PlayerController` 형식으로 호출 (AQN 불필요)

### 부수 효과
- `Cultivation.Game.asmdef` 도입으로 어셈블리 분리 (테스트 어셈블리에서 참조 가능)
- 테스트는 `Cultivation.Tests.EditMode.asmdef`에서 게임 어셈블리 참조

### 회귀 위험
- 없음 (첫 도입 단계에서 결정, 기존 데모 코드만 영향)

## 변경 이력

- 2026-05-04: 결정, 적용 (Phase 1 시작 전)
