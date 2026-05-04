# Unity MCP 셋업 가이드

> [`justinpbarnett/unity-mcp`](https://github.com/justinpbarnett/unity-mcp) 을 Claude Code와 연결한다.
> Unity Editor에서 Scene/Prefab/Inspector를 Claude가 직접 조작 가능해지므로, B타입 워크플로우의 핵심 약점("AI가 Editor 못 만짐")을 메운다.

---

## 사전 요구사항

- **Unity 6.3 LTS** (6000.3.x) — Unity Hub 설치 완료
- **Python 3.10+** (MCP 서버 실행용)
- **uv** (Python 패키지 매니저, 권장) — `pip install uv` 또는 [공식 설치](https://docs.astral.sh/uv/)
- **Claude Code CLI** 설치 + 본 Cultivation 프로젝트 사용 가능

> ⚠️ Unity MCP 저장소 README가 최신 정보 — 본 가이드 단계와 다르면 README를 우선 참고. 본 문서는 셋업 흐름의 골격만 제공한다.

---

## Step 1 — Unity 프로젝트 생성

> 본 단계는 본인이 Unity Hub에서 직접 수행.

### 위치
- **Project Path**: `C:\Users\jayju\project\Cultivation\src\Cultivation\`

### 작업
| 항목 | 값 |
|---|---|
| Editor Version | Unity 6000.3.x (LTS) |
| Template | Universal 3D (URP) |
| Project Name | `Cultivation` |
| Location | `C:\Users\jayju\project\Cultivation\src\` |

### 검증
`src/Cultivation/Assets/`, `src/Cultivation/ProjectSettings/` 가 생성됨.

---

## Step 2 — Unity MCP 패키지 설치 (Unity Editor)

### 위치
Unity Editor → Window → Package Manager

### 작업
1. Package Manager 열기
2. 좌상단 `+` → `Add package from git URL...`
3. 다음 URL 입력:
   ```
   https://github.com/justinpbarnett/unity-mcp.git?path=/UnityMcpBridge
   ```
4. Add

### 검증
- Project 창 또는 Packages 목록에 `Unity MCP Bridge` 등장
- 메뉴 상단에 `Window > Unity MCP` (또는 비슷한 항목) 추가됨

> 설치 직후 Unity가 백그라운드 Bridge 서버를 띄움. Window 메뉴에서 상태 확인 가능.

---

## Step 3 — MCP 서버 (Python) 셋업

### 위치
터미널 (PowerShell 또는 Git Bash) — 위치 무관

### 작업
1. Unity MCP 저장소 README의 "Server Setup" 또는 "Install" 섹션 따라 Python 서버 설치
   - 보통: `uvx unity-mcp-server` 또는 비슷한 형태
2. Bridge 포트(기본 6400 등) 확인 — README 참조

### 검증
- `unity-mcp-server --help` (또는 그에 상응하는 명령어) 동작
- Unity 측 Bridge가 Listening 상태

---

## Step 4 — Claude Code에 MCP 등록

### 위치
터미널 — Cultivation 프로젝트 루트

### 작업
**옵션 A (권장)** — Claude Code CLI로 등록:
```bash
claude mcp add unity-mcp -- uvx unity-mcp-server
```
> 정확한 실행 명령은 Unity MCP README의 "Claude Code Configuration" 섹션 확인.

**옵션 B** — `~/.claude.json` (또는 프로젝트 `.mcp.json`)에 직접 추가:
```json
{
  "mcpServers": {
    "unity-mcp": {
      "command": "uvx",
      "args": ["unity-mcp-server"]
    }
  }
}
```

### 검증
새 Claude Code 세션 열고:
```
/mcp
```
→ `unity-mcp` 가 connected 상태로 표시됨.

---

## Step 5 — 동작 확인

### 위치
- Unity Editor: `Cultivation` 씬 열어둔 상태
- Claude Code 세션: Cultivation 프로젝트에서 실행

### 작업
Claude에게 간단한 명령:
```
Unity Hierarchy에 Cube를 하나 추가하고 (0, 1, 0) 위치에 놓아줘.
```

### 검증
Unity 씬에 Cube가 실제로 생성되어 위치가 (0,1,0)에 잡힘.

---

## 트러블슈팅

| 증상 | 원인 후보 | 조치 |
|---|---|---|
| `/mcp`에 unity-mcp 없음 | Claude config 등록 실패 | `claude mcp list` 로 확인, 재등록 |
| Connected지만 Unity 응답 없음 | Bridge 서버 비실행 | Unity → Window → Unity MCP 메뉴에서 Start |
| 포트 충돌 | 6400 (또는 지정 포트) 점유 | README의 포트 변경 가이드 참조 |
| Python 못 찾음 | PATH 문제 | `uv` 설치 확인, `where uv` |

---

## 본 가이드와 README 충돌 시

**Unity MCP README가 우선.** 본 문서는 흐름 안내용이며, 정확한 명령어/패키지명은 저장소 최신 README를 본인이 확인.
