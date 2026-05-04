# Lessons Learned — Cultivation 프로젝트

> 세션 중 발생한 오류 패턴 + 교정 내용. 다음 세션에서 같은 실수 방지용.
> 새 교정 시 즉시 추가. 중복 패턴은 갱신(기존 항목 강화).

---

## Unity MCP

### [L-001] Editor 작업은 MCP 직접 — 핸드오프 문서 아님
**상황**: Phase 4 씬 구성 요청에 핸드오프 문서 양식을 먼저 꺼냄  
**교정**: "유니티 MCP로 작업하고 있잖아" — MCP 도구 먼저 쓰는 게 맞다  
**Rule**: Scene/GameObject/Component/Prefab 작업 → `mcp__UnityMCP__*` 직접 사용. 핸드오프는 MCP가 진짜 못 하는 작업(에셋 임포트 .fbx/.png, Play 모드 체감 판정)에만.

### [L-002] `execute_code`에서 `using` 지시문 사용 불가
**상황**: `execute_code` 블록 상단에 `using Cultivation.Data;` 작성 → 컴파일 에러  
**교정**: execute_code는 메서드 바디로 실행됨 — 파일 최상위 `using` 불가  
**Rule**: execute_code에서 외부 타입 참조 시 완전 한정 이름(FQN) 사용: `Cultivation.Data.SeedData`

### [L-003] 컴포넌트 Add 시 네임스페이스 포함 FQN 필수
**상황**: `manage_components add` 호출에 `PlayerController`만 전달 → "Component type not found"  
**교정**: `Cultivation.Systems.PlayerController` 형식으로 전달  
**Rule**: `Cultivation.*` 네임스페이스 클래스는 항상 FQN으로 전달. ADR-0002 참조.

---

## C# / Unity 구조

### [L-004] CharacterController.Move() 분리 호출 → isGrounded 불안정
**상황**: 수평 Move()와 수직 Move()를 별도 호출 → Space 점프 안 됨  
**교정**: 한 프레임에 Move() 단 한 번만, 수평+수직 벡터 합산 후 호출  
**Rule**: `CharacterController.Move()`는 프레임당 1회. `_velocity` 누적 후 단일 호출.

### [L-005] E키 같은 프레임 열기+닫기 race condition
**상황**: E키로 UI 열고 같은 프레임에 닫힘 → UI 즉시 사라짐  
**교정**: `_uiActivatedFrame = Time.frameCount` 저장, 닫기 시 `frameCount > _uiActivatedFrame` 체크  
**Rule**: 같은 키로 열고 닫을 때 frame guard 필수.

### [L-006] POCO 매니저 초기화 순서 — 의존성 역방향 주의
**상황**: GachaManager(Economy, Inventory) 생성 전에 Economy 미초기화 시 NPE  
**교정**: 의존 대상 먼저 `new`. TradeManager는 모든 매니저 준비 후 마지막.  
**Rule**: GameManager.Awake 순서: Inventory → Economy → Farm/Barn → Creature → Gacha → Breeding → Trade.

---

## Git / 파일 시스템

### [L-007] `cp -r` 복사 시 중첩 폴더 생성 위험
**상황**: `cp -r design/` 실행 → `design/design/` 중첩 폴더 생성  
**교정**: 경로 끝 슬래시 + 목적지 폴더 유무 확인 필수  
**Rule**: 폴더 복사 전 `ls 목적지` 확인. 이미 존재하면 내용 파일만 복사.

---

## 문서 / 설계

### [L-008] scene-data.json과 decisions.md 충돌 시 decisions.md 우선
**상황**: scene-data.json이 매니저 자식 GameObject 방식, decisions.md #11이 POCO 방식  
**교정**: 코드는 decisions.md 기준으로 구현, scene-data.json 나중에 수정  
**Rule**: 충돌 시 `design/decisions.md` > `design/scene/*.json`. ADR로 결정 기록 필수.
