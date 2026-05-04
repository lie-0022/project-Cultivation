# Cultivation

> Unity 6.3 LTS 게임 프로젝트. 컨셉 미정 (TBD).

## Status

🌱 Pre-production. 게임 컨셉 / 핵심 루프 정의 중.

## Tech Stack

- **Engine**: Unity 6.3 LTS (6000.3.x)
- **Language**: C# (.NET Standard 2.1)
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Project Path**: `src/Cultivation/`
- **Version Control**: Git, trunk-based

## Workflow

이 프로젝트는 Claude Code 멀티 에이전트 워크플로우 위에서 운영됩니다 (B타입 — 1인 개발자 + AI 협업).

핵심 문서:
- `CLAUDE.md` — 프로젝트 전역 설정
- `.claude/docs/workflow-b-type.md` — 본인-AI 분담
- `.claude/docs/editor-handoff.md` — Unity Editor 작업 핸드오프 양식
- `.claude/docs/review-workflow.md` — 승인 흐름

## Getting Started

새 세션 시작 시:

1. Claude Code 실행 (`claude`)
2. 작업 컨텍스트 1~3줄 제공:
   ```
   Cultivation Unity 6.3 프로젝트.
   지금 [기능/버그] 작업 중.
   목표: [구체적 목표]
   ```

## License

Internal project.
