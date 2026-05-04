# Unity MCP (MCP for Unity) 셋업 가이드

> [`CoplayDev/unity-mcp`](https://github.com/CoplayDev/unity-mcp) 을 Claude Code와 연결한다.
> Unity Editor에서 Scene/Prefab/Inspector를 Claude가 직접 조작 가능해지므로, B타입 워크플로우의 핵심 약점("AI가 Editor 못 만짐")을 메운다.
>
> 패키지 정식 이름은 **MCP for Unity**. (저장소가 `justinpbarnett/unity-mcp` → `CoplayDev/unity-mcp`로 이관됨.)

---

## 사전 요구사항

- **Unity 6.3 LTS** (6000.3.x) 프로젝트가 `src/Cultivation/`에 생성되어 있을 것
- **Python 3.10+** 와 **uv** (`pip install uv` 또는 [공식 설치](https://docs.astral.sh/uv/))
- **Claude Code CLI** 설치 + 본 Cultivation 프로젝트에서 사용 가능
- Unity 2021.3 이상이면 동작하지만 본 프로젝트는 **6.3 LTS** 사용

---

## Step 1 — Unity 프로젝트 (이미 완료)

| 항목 | 값 |
|---|---|
| Project Path | `C:\Users\jayju\project\Cultivation\src\Cultivation\` |
| Editor Version | Unity 6000.3.10f1 (LTS) |
| Template | Universal 3D (URP 17.3.0) |

검증: `src/Cultivation/Assets/`, `src/Cultivation/ProjectSettings/`, `src/Cultivation/Packages/manifest.json` 존재.

---

## Step 2 — MCP for Unity 패키지 설치 (Unity Editor)

### 위치
Unity Editor → **`Window > Package Manager`**

### 작업
| 단계 | 동작 |
|---|---|
| 2.1 | Package Manager 좌상단 **`+`** 버튼 |
| 2.2 | **`Add package from git URL...`** 선택 |
| 2.3 | URL 입력: `https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main` |
| 2.4 | **Add** 버튼 |

> 베타가 필요하면 끝의 `#main`을 `#beta`로 교체. 처음이면 `#main` 권장.

### 이 Step의 검증
- Package Manager 좌측 목록에 **`MCP for Unity`** 항목 등장
- Unity 메뉴에 **`Window > MCP for Unity`** 추가됨
- Console 창에 빨간 에러 없음 (노란 경고는 무시 가능)

---

## Step 3 — MCP for Unity 창 열기 + 서버 시작

### 위치
Unity Editor → **`Window > MCP for Unity`**

### 작업
| 단계 | 동작 |
|---|---|
| 3.1 | MCP for Unity 창 열기 |
| 3.2 | **Start Server** 버튼 클릭 |

### 이 Step의 검증
- 창에 🟢 **`Connected ✓`** 표시
- 서버는 `localhost:8080`에서 동작 (`http://localhost:8080/mcp`)

---

## Step 4 — Claude Code 연결

### 옵션 A: 자동 설정 (권장)

| 단계 | 동작 |
|---|---|
| 4.1 | MCP for Unity 창 내 **클라이언트 드롭다운**에서 `Claude Code` 선택 |
| 4.2 | **Configure** 버튼 클릭 |

→ Claude Code의 MCP 설정 파일에 자동으로 등록됨.

### 옵션 B: 수동 설정 (자동이 실패하면)

`~/.claude.json` 또는 프로젝트 `.mcp.json`에 추가:

```json
{
  "mcpServers": {
    "unityMCP": {
      "url": "http://localhost:8080/mcp"
    }
  }
}
```

### 이 Step의 검증
새 Claude Code 세션 열고:
```
/mcp
```
→ `unityMCP` (또는 `unity-mcp`) 가 **connected** 상태.

---

## Step 5 — 동작 확인

### 위치
- Unity Editor: 어떤 씬이든 열어둔 상태 (서버 Start 상태 유지)
- Claude Code 세션: Cultivation 프로젝트에서 실행

### 작업
Claude에게 명령:
```
Unity Hierarchy에 빨간색, 파란색, 노란색 큐브를 (-2,0,0) (0,0,0) (2,0,0)에 만들어줘.
```

### 이 Step의 검증
Unity 씬에 큐브 3개가 실제로 생성되어 위치/색이 잡힘.

---

## 트러블슈팅

| 증상 | 원인 후보 | 조치 |
|---|---|---|
| `/mcp`에 unityMCP 없음 | Configure 미실행 또는 옵션 B 누락 | MCP for Unity 창에서 Configure 다시 / `~/.claude.json` 확인 |
| Connected 표시지만 Claude가 못 봄 | Claude 세션이 MCP 등록 전에 시작됨 | Claude 세션 재시작 |
| 8080 포트 충돌 | 다른 프로세스가 사용 중 | 그 프로세스 종료, 또는 MCP for Unity 창에서 포트 변경 |
| Python/uv 못 찾음 | PATH 문제 | `where uv` (PowerShell) / `which uv` (bash) 로 확인, 없으면 설치 |
| Unity 패키지 추가 시 에러 | 네트워크/SSL/git 미설치 | git 설치 확인, `git --version` |

---

## 본 가이드와 README 충돌 시

**저장소 README가 우선**: <https://github.com/CoplayDev/unity-mcp>

본 문서는 흐름 안내용. 명령어/패키지명/포트는 저장소 최신 README를 본인이 확인.
